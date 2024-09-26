using Bloggest.Components.Bus.Contracts.Types;

namespace Bloggest.Components.Bus.Contracts.Interfaces;

public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event);
}