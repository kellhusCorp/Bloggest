using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;
using Posts.Domain.Entities;
using Posts.Infrastructure.Contexts;

namespace Posts.Infrastructure;

public class PostsContextSeed
{
    private const int DefaultRetryForAvailability = 5;
    
    public async Task SeedAsync(PostsContext context, ILogger<PostsContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(PostsContextSeed), DefaultRetryForAvailability);
        
        await policy.ExecuteAsync(async () =>
        {
            if (!context.Posts.Any())
            {
                await context.Posts.AddRangeAsync(GetDefaultPosts());
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<PostDbo> GetDefaultPosts()
    {
        return new List<PostDbo>
        {
            new PostDbo
            {
                Id = Guid.NewGuid(),
                Header = "Post 1",
                Content = "Content 1",
                PublishedAt = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                Link = "what-is-that-1"
            },
            new PostDbo
            {
                Id = Guid.NewGuid(),
                Header = "Post 2",
                Content = "Content 2",
                PublishedAt = DateTimeOffset.UtcNow - TimeSpan.FromDays(2),
                Link = "what-is-that-2"
            },
            new PostDbo
            {
                Id = Guid.NewGuid(),
                Header = "Post 3",
                Content = "Content 3",
                PublishedAt = DateTimeOffset.UtcNow - TimeSpan.FromDays(3),
                Link = "what-is-that-3"
            }
        };
    }
    
    private AsyncRetryPolicy CreatePolicy(ILogger<PostsContextSeed> logger, string prefix, int retries)
    {
        return Policy.Handle<PostgresException>()
            .WaitAndRetryAsync(retryCount: retries,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(10),
                onRetry: (ex, ts, retry, ctx) =>
                {
                    logger.LogWarning(ex, "[{prefix}] Ошибка при инициализации данных в БД (попытка {retry} из {retries})", prefix, retry, retries);
                });
    }
}