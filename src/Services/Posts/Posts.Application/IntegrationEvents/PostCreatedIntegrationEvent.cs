using Bloggest.Components.Bus.Contracts.Types;

namespace Posts.Application.IntegrationEvents;

public class PostCreatedIntegrationEvent : IntegrationEvent
{
    public Guid PostId { get; init; }
    
    public string Header { get; init; }
    
    public string? AuthorId { get; init; }
    
    public IEnumerable<Guid> TagIds { get; init; }
}