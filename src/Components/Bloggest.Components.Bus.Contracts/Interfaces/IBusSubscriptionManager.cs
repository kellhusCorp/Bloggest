using Bloggest.Components.Bus.Contracts.Types;

namespace Bloggest.Components.Bus.Contracts.Interfaces;

public interface IBusSubscriptionManager
{
    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
    
    bool HasSubscriptionsForEvent(string eventName);
    
    void AddSubscription<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>;
    
    void RemoveSubscription<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>;
    
    IEnumerable<HandlerSubscription> GetHandlersForEvent<T>() where T : IntegrationEvent;
    
    IEnumerable<HandlerSubscription> GetHandlersForEvent(string eventName);
    
    Type GetEventTypeByName(string eventName);
    
    void Clear();
}