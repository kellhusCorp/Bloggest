using RabbitMQ.Client;

namespace Bloggest.Components.Bus.Interfaces;

public interface IRabbitConnection : IDisposable
{
    bool IsConnected { get; }
    
    bool TryConnect();
    
    IModel CreateModel();
}