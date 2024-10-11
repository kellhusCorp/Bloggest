using Bloggest.Components.Bus.Contracts.Interfaces;
using Bloggest.Components.Bus.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Bloggest.Common.Extensions;

public static class CommonExtensions
{
    public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBusSection = configuration.GetSection("EventBus");
        if (!eventBusSection.Exists())
        {
            return;
        }

        var providerName = eventBusSection["Provider"];
        if (string.Equals(providerName, "RabbitMQ", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IRabbitConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitConnection>>();
                var factory = new ConnectionFactory
                {
                    HostName = eventBusSection["HostName"],
                    Port = int.Parse(eventBusSection["Port"]),
                    UserName = eventBusSection["Username"],
                    Password = eventBusSection["Password"],
                    DispatchConsumersAsync = true
                };

                var retryCount = int.Parse(eventBusSection["RetryCount"]);
                return new RabbitConnection(factory, logger, retries: retryCount);
            });

            services.AddSingleton<IEventBus, RabbitBus>(sp =>
            {
                var subscriptionClientName = eventBusSection["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitBus>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IBusSubscriptionManager>();
                var retryCount = eventBusSection["RetryCount"];

                return new RabbitBus(rabbitMQPersistentConnection, logger, eventBusSubscriptionsManager, sp, subscriptionClientName,
                    retryCount);
            });
            
            services.AddSingleton<IBusSubscriptionManager, DefaultBusSubscriptionManager>();
        }
    }
}