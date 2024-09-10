using Bloggest.Components.Bus.Interfaces;
using Bloggest.Components.Bus.Types;

namespace Bloggest.Components.Bus.Implementations;

public class DefaultBusSubscriptionManager : IBusSubscriptionManager
{
    private readonly Dictionary<string, List<HandlerSubscription>> _eventHandlers = new();
    private readonly List<Type> _eventTypes = new();
    
    public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
    {
        return _eventHandlers.ContainsKey(typeof(T).Name);
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        return _eventHandlers.ContainsKey(eventName);
    }

    public void AddSubscription<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        
        if (!_eventTypes.Contains(typeof(TEvent)))
        {
            _eventTypes.Add(typeof(TEvent));
        }

        var exists = HasSubscriptionsForEvent<TEvent>();

        if (!exists)
        {
            _eventHandlers.Add(eventName, []);
        }
        
        // todo check that TEventHandler is not in different places.
        
        _eventHandlers[eventName].Add(new HandlerSubscription(typeof(TEventHandler)));
    }

    public void RemoveSubscription<TEvent, TEventHandler>() where TEvent : IntegrationEvent where TEventHandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;

        if (!HasSubscriptionsForEvent<TEvent>())
        {
            return;
        }

        var subscription = _eventHandlers[eventName].FirstOrDefault(x => x.HandlerType == typeof(TEventHandler));

        if (subscription is not null)
        {
            _eventHandlers[eventName].Remove(subscription);
            var eventType = _eventTypes.FirstOrDefault(x => x.Name == eventName);
            if (eventType is not null)
            {
                _eventTypes.Remove(eventType);
            }
        }
    }

    public IEnumerable<HandlerSubscription> GetHandlersForEvent(string eventName)
    {
        return _eventHandlers[eventName];
    }

    public Type GetEventTypeByName(string eventName)
    {
        return _eventTypes.FirstOrDefault(x => x.Name == eventName);
    }

    public void Clear()
    {
        _eventHandlers.Clear();
    }

    public IEnumerable<HandlerSubscription> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        var eventName = typeof(T).Name;
        return GetHandlersForEvent(eventName);
    }
}