using Bloggest.Components.Bus.Interfaces;
using Bloggest.Components.Bus.Types;
using RabbitMQ.Client;

namespace Bloggest.Components.Bus.Implementations;

public class RabbitBus : IEventBus
{
    private readonly IRabbitConnection _rabbitConnection;
    private IModel _modelChannel;

    public RabbitBus(IRabbitConnection rabbitConnection)
    {
        _rabbitConnection = rabbitConnection;
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