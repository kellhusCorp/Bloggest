using Bloggest.Components.Bus.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace Bloggest.Components.Tests;

public class RabbitConnectionTests
{
    private RabbitMqContainer _rabbitMq;

    private ILogger<RabbitConnection> _logger;

    private IConnectionFactory _rabbitConnectionFactory;
    
    [SetUp]
    public async Task SetUp()
    {
        _rabbitMq = new RabbitMqBuilder()
            .WithHostname("localhost")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(54614, 5672)
            .Build();
        await _rabbitMq.StartAsync();

        _rabbitConnectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMq.Hostname,
            UserName = "guest",
            Password = "guest",
            Port = 54614
        };
        
        _logger = Mock.Of<ILogger<RabbitConnection>>();
    }

    [Test]
    public void Test_RabbitConnection_TryConnect_Returns_True_When_Connection_Is_Established()
    {
        using RabbitConnection rabbitConnection = new RabbitConnection(_rabbitConnectionFactory, _logger);
        rabbitConnection.TryConnect();
        
        Assert.That(rabbitConnection.IsConnected, Is.True);
    }
    
    [Test]
    public async Task Test_RabbitConnection_IsConnected_Returns_False_When_Connection_IsShutdown()
    {
        using RabbitConnection rabbitConnection = new RabbitConnection(_rabbitConnectionFactory, _logger);
        rabbitConnection.TryConnect();
        
        Assert.That(rabbitConnection.IsConnected, Is.True);

        await _rabbitMq.StopAsync();
        
        Assert.That(rabbitConnection.IsConnected, Is.False);
    }
    
    [Test]
    public void Test_RabbitConnection_CreateModel_Returns_Model_When_Connection_Is_Established()
    {
        using RabbitConnection rabbitConnection = new RabbitConnection(_rabbitConnectionFactory, _logger);
        rabbitConnection.TryConnect();
        
        var model = rabbitConnection.CreateModel();
        Assert.Multiple(() =>
        {
            Assert.That(model, Is.Not.Null);
            Assert.That(model.IsOpen, Is.True);
        });
    }
    
    [Test]
    public void Test_RabbitConnection_CreateModel_Throws_NullReferenceException_When_Connection_Is_Null()
    {
        using RabbitConnection rabbitConnection = new RabbitConnection(_rabbitConnectionFactory, _logger);
        
        Assert.Throws<NullReferenceException>(() => rabbitConnection.CreateModel());
    }

    [TearDown]
    public async Task TearDown()
    {
        await _rabbitMq.StopAsync();
    }
}