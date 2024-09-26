using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Bloggest.Components.Bus.Contracts.Enums;
using Bloggest.Components.Bus.Contracts.Types;

namespace Bloggest.Components.Bus.Contracts.Entities;

public class IntegrationEventEntry
{
    public IntegrationEventEntry()
    {
        
    }

    public IntegrationEventEntry(IntegrationEvent @event, Guid tid)
    {
        EventId = @event.Id;
        CreatedAt = @event.CreatedAt;
        EventTypeName = @event.GetType().Name;
        Content = JsonSerializer.Serialize(@event, @event.GetType());
        State = IntegrationEventState.IsNotPublished;
        TimesSent = 0;
        TransactionId = tid.ToString();
    }
    
    public Guid EventId { get; set; }
    
    public string EventTypeName { get; set; }
    
    public string Content { get; set; }
    
    public IntegrationEventState State { get; set; }
    
    public int TimesSent { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public string TransactionId { get; set; }
    
    [NotMapped]
    public IntegrationEvent IntegrationEvent { get; private set; }
    
    public IntegrationEventEntry DeserializeJsonContent(Type type)
    {
        IntegrationEvent = (JsonSerializer.Deserialize(Content, type) as IntegrationEvent)!;
        return this;
    }
}