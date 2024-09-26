namespace Bloggest.Components.Bus.Contracts.Enums;

public enum IntegrationEventState
{
    IsNotPublished = 0,
    IsInProgress = 1,
    IsPublished = 2,
    IsPublishedFailed = 3
}