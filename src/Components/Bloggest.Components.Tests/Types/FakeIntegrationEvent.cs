using Bloggest.Components.Bus.Contracts.Types;

namespace Bloggest.Components.Tests.Types;

public class FakeIntegrationEvent : IntegrationEvent
{
    public string Payload { get; }

    public FakeIntegrationEvent(string payload)
    {
        Payload = payload;
    }
}