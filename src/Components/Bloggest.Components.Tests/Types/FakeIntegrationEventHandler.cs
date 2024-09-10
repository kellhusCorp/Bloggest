using Bloggest.Components.Bus.Interfaces;

namespace Bloggest.Components.Tests.Types;

public class FakeIntegrationEventHandler : IIntegrationEventHandler<FakeIntegrationEvent>
{
    public bool Succeeded { get; private set; }
    
    public Task Handle(FakeIntegrationEvent @event)
    {
        Succeeded = true;
        return Task.CompletedTask;
    }
}