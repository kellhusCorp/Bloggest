using Bloggest.Components.Bus.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace Bloggest.Components.Tests;

public class RabbitConnectionTests
{
    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithHostname("localhost")
        .WithUsername("guest")
        .WithPassword("guest")
        .WithPortBinding(54614, 5672)
        .Build();

    private IConnectionFactory _rabbitConnectionFactory;
    
    [SetUp]
    public async Task SetUp()
    {
        await _rabbitMq.StartAsync();

        _rabbitConnectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMq.Hostname,
            UserName = "guest",
            Password = "guest",
            Port = 54614
        };
    }

    [Test]
    public void Test_RabbitConnection_TryConnect_Returns_True_When_Connection_Is_Established()
    {
        var logger = new Mock<ILogger<RabbitConnection>>();

        using RabbitConnection rabbitConnection = new RabbitConnection(_rabbitConnectionFactory, logger.Object);
        rabbitConnection.TryConnect();
        
        Assert.That(rabbitConnection.IsConnected, Is.True);
    }
    
    [Test]
    public async Task Test_RabbitConnection_IsConnected_Returns_False_When_Connection_IsShutdown()
    {
        var logger = new Mock<ILogger<RabbitConnection>>();

        using RabbitConnection rabbitConnection = new RabbitConnection(_rabbitConnectionFactory, logger.Object);
        rabbitConnection.TryConnect();
        
        Assert.That(rabbitConnection.IsConnected, Is.True);

        await _rabbitMq.StopAsync();
        
        Assert.That(rabbitConnection.IsConnected, Is.False);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await _rabbitMq.StopAsync();
        await _rabbitMq.DisposeAsync();
    }
}