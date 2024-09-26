using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Bloggest.Components.Bus.Contracts.Interfaces;
using Bloggest.Components.Bus.Contracts.Types;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Bloggest.Components.Bus.Implementations;

public class RabbitBus : IEventBus
{
    private const string DefaultExchangeName = "posts_bus";
    private readonly IRabbitConnection _rabbitConnection;
    private readonly ILogger<RabbitBus> _logger;
    private readonly IBusSubscriptionManager _subscriptionManager;
    private readonly IServiceProvider _serviceProvider;
    private IModel? _channel;
    private readonly string _exchangeName;
    private string? _queueName;
    private readonly Dictionary<Type, MethodInfo> _cachedHandlerMethods = new();
    private readonly RetryPolicy _policy;
    private const int DefaultRetryCount = 5;
    private int _retries;

    public RabbitBus(
        IRabbitConnection rabbitConnection,
        ILogger<RabbitBus> logger,
        IBusSubscriptionManager subscriptionManager,
        IServiceProvider serviceProvider,
        string exchangeName = DefaultExchangeName,
        string? queueName = null,
        int retries = DefaultRetryCount,
        RetryPolicy? policy = null)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
        _subscriptionManager = subscriptionManager;
        _serviceProvider = serviceProvider;
        _exchangeName = exchangeName;
        _queueName = queueName ?? string.Empty;
        _retries = retries;
        _policy = policy ?? CreateDefaultPolicy();
        _channel = CreateConsumerChannel();
    }

    private RetryPolicy CreateDefaultPolicy()
    {
        return Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Не удалось отправить сообщение в шину. Повторная попытка через {Time}", time);
            });
    }
    
    private IModel CreateConsumerChannel()
    {
        if (!_rabbitConnection.IsConnected)
        {
            _rabbitConnection.TryConnect();
        }

        var channel = _rabbitConnection.CreateModel();

        channel.ExchangeDeclare(_exchangeName, "direct");

        _queueName = channel.QueueDeclare(_queueName, true, false, false).QueueName;

        channel.CallbackException += OnCallbackException;
        
        return channel;
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Ошибка во время обработки сообщения");
        _channel?.Dispose();
        RecreateConsumerChannel();
    }

    private void RecreateConsumerChannel()
    {
        _channel = CreateConsumerChannel();
        StartBasicConsume();
    }

    private void StartBasicConsume()
    {
        if (_channel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += OnMessageReceived;

            _channel.BasicConsume(_queueName, false, consumer);
        }
        else
        {
            _logger.LogError("Не удалось привязать обработчик сообщений");
        }
    }

    private async Task OnMessageReceived(object? sender, BasicDeliverEventArgs @event)
    {
        var eventName = @event.RoutingKey;
        var message = Encoding.UTF8.GetString(@event.Body.Span);
        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка во время обработки сообщения");
        }

        _channel?.BasicAck(@event.DeliveryTag, false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        var exists = _subscriptionManager.HasSubscriptionsForEvent(eventName);
        if (!exists)
        {
            _logger.LogWarning("Не найдено обработчиков для события {EventName}", eventName);
            return;
        }
        
        var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            var handler = _serviceProvider.GetService(subscription.HandlerType);
            if (handler == null)
            {
                _logger.LogWarning("Обработчик {HandlerType} для события {EventName} не зарегистрирован", subscription.HandlerType, eventName);
                continue;
            }
            
            var eventType = _subscriptionManager.GetEventTypeByName(eventName);
            var integrationEvent = (IntegrationEvent)JsonSerializer.Deserialize(message, eventType);
            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
            await Task.Yield();

            if (!_cachedHandlerMethods.ContainsKey(concreteType))
            {
                _cachedHandlerMethods[concreteType] = concreteType.GetMethod("Handle");
            }
            
            await (Task)_cachedHandlerMethods[concreteType].Invoke(handler, [integrationEvent]);
        }
    }

    public void Publish(IntegrationEvent @event)
    {
        if (!_rabbitConnection.IsConnected)
        {
            _rabbitConnection.TryConnect();
        }
        
        var eventName = @event.GetType().Name;

        using (var channel = _rabbitConnection.CreateModel())
        {
            channel.ExchangeDeclare(_exchangeName, "direct");
            var bodyMessage = JsonSerializer.Serialize(@event, @event.GetType());
            _policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                var body = Encoding.UTF8.GetBytes(bodyMessage);
                channel.BasicPublish(_exchangeName, eventName, true, properties, body);
            });
        }
    }

    public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        InternalSubscribe(eventName);
        
        _subscriptionManager.AddSubscription<TEvent, THandler>();
        StartBasicConsume();
    }

    private void InternalSubscribe(string eventName)
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            if (!_rabbitConnection.IsConnected)
            {
                _rabbitConnection.TryConnect();
            }
            
            _channel.QueueBind(_queueName, _exchangeName, eventName);
        }
    }

    public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
    {
        _subscriptionManager.RemoveSubscription<TEvent, THandler>();
    }
    
    public void Dispose()
    {
        if (_channel != null)
        {
            _channel.Dispose();
        }

        _subscriptionManager.Clear();
    }
}