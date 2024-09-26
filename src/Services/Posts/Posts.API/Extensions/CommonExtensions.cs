using Bloggest.Components.IntegrationEventContext.Contexts;
using Microsoft.EntityFrameworkCore;
using Posts.Infrastructure.Contexts;

namespace Posts.API.Extensions;

public static class CommonExtensions
{
    public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostsContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddDbContext<EFIntegrationEventContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"), builder =>
            {
                builder.MigrationsHistoryTable("__IntegrationEventMigrationsHistory");
            });
        });
    }
}