using Bloggest.Components.Bus.Contracts.Entities;
using Bloggest.Components.Bus.Contracts.Types;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bloggest.Components.Bus.Contracts.Interfaces;

public interface IIntegrationEventService
{
    Task<IEnumerable<IntegrationEventEntry>> GetIntegrationEventsPendingToPublishAsync(Guid transactionId);
    
    Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
    
    Task MarkEventAsPublishedAsync(Guid eventId);
    
    Task MarkEventAsInProgressAsync(Guid eventId);
    
    Task MarkEventAsFailedAsync(Guid eventId);
}