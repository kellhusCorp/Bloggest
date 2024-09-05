using Bloggest.Components.Bus.Types;

namespace Bloggest.Components.Bus.Interfaces;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);

    void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>;
    
    void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>;
}