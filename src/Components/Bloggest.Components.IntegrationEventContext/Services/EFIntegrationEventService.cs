using System.Data;
using System.Data.Common;
using System.Reflection;
using Bloggest.Components.Bus.Contracts.Entities;
using Bloggest.Components.Bus.Contracts.Enums;
using Bloggest.Components.Bus.Contracts.Interfaces;
using Bloggest.Components.Bus.Contracts.Types;
using Bloggest.Components.IntegrationEventContext.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bloggest.Components.IntegrationEventContext.Services;

public class EfIntegrationEventService : IIntegrationEventService, IDisposable
{
    private readonly EFIntegrationEventContext _context;
    private readonly IDbConnection _dbConnection;
    private readonly Dictionary<string, Type> _eventTypesMapping;
    private volatile bool _disposedValue;

    public EfIntegrationEventService(DbConnection dbConnection, params Assembly[] mappingIntegrationEventAssemblies)
    {
        _dbConnection = dbConnection;
        _context = new EFIntegrationEventContext(new DbContextOptionsBuilder<EFIntegrationEventContext>()
            .UseNpgsql(dbConnection).Options);

        _eventTypesMapping = mappingIntegrationEventAssemblies.SelectMany(x => x.GetTypes())
            .Where(x => x.IsSubclassOf(typeof(IntegrationEvent)))
            .ToDictionary(x => x.Name.Split('.').First(), x => x);
    }

    public async Task<IEnumerable<IntegrationEventEntry>> GetIntegrationEventsPendingToPublishAsync(Guid transactionId)
    {
        var tid = transactionId.ToString();
        var events = await _context.IntegrationEvents
            .Where(e => e.TransactionId == tid && e.State == IntegrationEventState.IsNotPublished)
            .ToListAsync();

        if (events.Count != 0)
            return events.OrderBy(o => o.CreatedAt).Select(e => e.DeserializeJsonContent(_eventTypesMapping[e.EventTypeName]));

        return Enumerable.Empty<IntegrationEventEntry>();
    }

    public async Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        var eventLogEntry = new IntegrationEventEntry(@event, transaction.TransactionId);

        await _context.Database.UseTransactionAsync(transaction.GetDbTransaction());
        await _context.IntegrationEvents.AddAsync(eventLogEntry);

        await _context.SaveChangesAsync();
    }

    public Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventState.IsPublished);
    }

    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventState.IsInProgress);
    }

    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventState.IsPublishedFailed);
    }

    private Task UpdateEventStatus(Guid eventId, IntegrationEventState status)
    {
        var eventLogEntry = _context.IntegrationEvents.Single(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == IntegrationEventState.IsInProgress)
            eventLogEntry.TimesSent++;

        _context.IntegrationEvents.Update(eventLogEntry);

        return _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing) _context?.Dispose();

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}