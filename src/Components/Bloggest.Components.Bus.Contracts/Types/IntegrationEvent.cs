namespace Bloggest.Components.Bus.Contracts.Types;

public class IntegrationEvent
{
    public Guid Id { get; }
    
    public DateTimeOffset CreatedAt { get; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    public IntegrationEvent(Guid id, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }
}