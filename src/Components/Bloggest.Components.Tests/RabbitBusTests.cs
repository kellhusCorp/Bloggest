using Bloggest.Components.Bus.Implementations;
using Bloggest.Components.Bus.Interfaces;
using Bloggest.Components.Tests.Types;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace Bloggest.Components.Tests;

public class RabbitBusTests
{
    private RabbitMqContainer _rabbitMq;

    private ILogger<RabbitConnection> _connectionLogger;

    private ILogger<RabbitBus> _eventBusLogger;

    private IConnectionFactory _rabbitConnectionFactory;

    private Mock<IServiceProvider> _serviceProvider;
    
    private FakeIntegrationEventHandler fakeHandler = new();

    [SetUp]
    public async Task SetUp()
    {
        _rabbitMq = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.13-management")
            .WithHostname("localhost")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(54614, 5672)
            .WithPortBinding(54615, 15672)
            .Build();

        await _rabbitMq.StartAsync();

        _rabbitConnectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMq.Hostname,
            UserName = "guest",
            Password = "guest",
            Port = 54614,
            DispatchConsumersAsync = true
        };

        _connectionLogger = Mock.Of<ILogger<RabbitConnection>>();
        _eventBusLogger = Mock.Of<ILogger<RabbitBus>>();
        _serviceProvider = new Mock<IServiceProvider>();
            
        _serviceProvider
        .Setup(x => x.GetService(It.Is<Type>(y => y == typeof(FakeIntegrationEventHandler))))
        .Returns(fakeHandler);
    }

    [Test]
    public async Task Test_RabbitBus()
    {
        using IRabbitConnection rabbitConnection = new RabbitConnection(
            _rabbitConnectionFactory,
            _connectionLogger);
        rabbitConnection.TryConnect();

        IEventBus rabbitBus = new RabbitBus(
            rabbitConnection,
            _eventBusLogger,
            new DefaultBusSubscriptionManager(),
            _serviceProvider.Object,
            queueName: "testqueue");
            
        rabbitBus.Subscribe<FakeIntegrationEvent, FakeIntegrationEventHandler>();

        rabbitBus.Publish(new FakeIntegrationEvent("this is payload"));

        await Task.Delay(10000);

        Assert.That(fakeHandler.Succeeded, Is.True);
        
        rabbitBus.Dispose();
    }
}