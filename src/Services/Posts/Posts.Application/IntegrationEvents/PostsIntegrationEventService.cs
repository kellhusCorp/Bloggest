using System.Data;
using System.Data.Common;
using Bloggest.Components.Bus.Contracts.Interfaces;
using Bloggest.Components.Bus.Contracts.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Posts.Infrastructure.Contexts;

namespace Posts.Application.IntegrationEvents;

public class PostsIntegrationEventService
{
    private readonly Func<DbConnection, IIntegrationEventService> _integrationEventServiceFactory;
    private readonly IEventBus _eventBus;
    private readonly PostsContext _postsContext;
    private readonly IIntegrationEventService _integrationEventService;
    private readonly ILogger<PostsIntegrationEventService> _logger;

    public PostsIntegrationEventService(
        IEventBus eventBus,
        PostsContext postsContext,
        ILogger<PostsIntegrationEventService> logger,
        Func<DbConnection, IIntegrationEventService> integrationEventServiceFactory)
    {
        _eventBus = eventBus;
        _postsContext = postsContext;
        _integrationEventService = integrationEventServiceFactory(_postsContext.Database.GetDbConnection());
        _logger = logger;
        _integrationEventServiceFactory = integrationEventServiceFactory;
    }
    
    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        var pendingEvents = await _integrationEventService.GetIntegrationEventsPendingToPublishAsync(transactionId);

        foreach (var logEvt in pendingEvents)
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", logEvt.EventId, logEvt.IntegrationEvent);

            try
            {
                await _integrationEventService.MarkEventAsInProgressAsync(logEvt.EventId);
                _eventBus.Publish(logEvt.IntegrationEvent);
                await _integrationEventService.MarkEventAsPublishedAsync(logEvt.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event: {IntegrationEventId}", logEvt.EventId);

                await _integrationEventService.MarkEventAsFailedAsync(logEvt.EventId);
            }
        }
    }

    public async Task AddAndSaveEventAsync(IntegrationEvent evt)
    {
        _logger.LogInformation("Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

        await _integrationEventService.SaveEventAsync(evt, _postsContext.Database.CurrentTransaction);
    }
}