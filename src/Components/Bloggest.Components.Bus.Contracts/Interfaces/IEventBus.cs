using Bloggest.Components.Bus.Contracts.Types;

namespace Bloggest.Components.Bus.Contracts.Interfaces;

public interface IEventBus : IDisposable
{
    void Publish(IntegrationEvent @event);

    void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>;
    
    void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>;
}