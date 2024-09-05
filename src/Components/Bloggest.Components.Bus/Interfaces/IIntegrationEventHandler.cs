using Bloggest.Components.Bus.Types;

namespace Bloggest.Components.Bus.Interfaces;

public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event);
}