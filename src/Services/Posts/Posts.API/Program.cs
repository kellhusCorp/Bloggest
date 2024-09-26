using Bloggest.Common.Extensions;
using Bloggest.Components.IntegrationEventContext.Contexts;
using Microsoft.EntityFrameworkCore;
using Posts.API.Extensions;
using Posts.Application;
using Posts.Infrastructure;
using Posts.Infrastructure.Contexts;

namespace Posts.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContexts(builder.Configuration);
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplicationServices();
        builder.Services.AddEventBus(builder.Configuration);

        var app = builder.Build();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.UseAuthorization();
        
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostsContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PostsContextSeed>>();
            await context.Database.MigrateAsync();
            await new PostsContextSeed().SeedAsync(context, logger);
            
            var integrationEventContext = scope.ServiceProvider.GetRequiredService<EFIntegrationEventContext>();
            await integrationEventContext.Database.MigrateAsync();
        }
        
        await app.RunAsync();
    }
}