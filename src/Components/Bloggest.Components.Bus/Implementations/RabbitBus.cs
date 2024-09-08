using System.Text;
using Bloggest.Components.Bus.Interfaces;
using Bloggest.Components.Bus.Types;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Bloggest.Components.Bus.Implementations;

public class RabbitBus : IEventBus
{
    private const string DefaultExchangeName = "posts_bus";
    private readonly IRabbitConnection _rabbitConnection;
    private readonly ILogger<RabbitBus> _logger;
    private IModel? _channel;
    private readonly string _exchangeName;
    private readonly string? _queueName;

    public RabbitBus(
        IRabbitConnection rabbitConnection,
        ILogger<RabbitBus> logger,
        string exchangeName = DefaultExchangeName,
        string? queueName = null)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
        _exchangeName = exchangeName;
        _channel = CreateConsumerChannel();
        _queueName = queueName;
    }

    private IModel CreateConsumerChannel()
    {
        if (!_rabbitConnection.IsConnected) _rabbitConnection.TryConnect();

        var channel = _rabbitConnection.CreateModel();

        channel.ExchangeDeclare(_exchangeName, "direct");

        channel.QueueDeclare(_queueName, true, false, false);

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

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs @event)
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
        throw new NotImplementedException();
    }

    public void Publish(IntegrationEvent @event)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}