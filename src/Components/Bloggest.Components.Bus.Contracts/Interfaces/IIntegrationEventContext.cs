using Bloggest.Components.Bus.Contracts.Entities;

namespace Bloggest.Components.Bus.Contracts.Interfaces;

public interface IIntegrationEventContext
{
    IQueryable<IntegrationEventEntry> IntegrationEventEntries { get; set; }
}