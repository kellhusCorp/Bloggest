using System.Net.Sockets;
using Bloggest.Components.Bus.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Bloggest.Components.Bus.Implementations;

public class RabbitConnection : IRabbitConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitConnection> _logger;
    private IConnection _connection;
    private bool _disposed;
    private readonly object _lock = new();
    private readonly int _retries;
    private readonly Policy _policy;

    public RabbitConnection(
        IConnectionFactory connectionFactory,
        ILogger<RabbitConnection> logger,
        Policy? retryPolicy = null, int retries = 3)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _retries = retries;
        _policy = retryPolicy ?? CreateDefaultPolicy();
    }

    private RetryPolicy CreateDefaultPolicy()
    {
        return Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(_retries, attempt => TimeSpan.FromSeconds(attempt * 2),
                (exception, _, attempt, _) =>
                {
                    _logger.LogWarning(exception, $"Не удалось установить соединение с RabbitMQ. Попытка {attempt}");
                });
    }

    public bool IsConnected => _connection.IsOpen && !_disposed;

    public bool TryConnect()
    {
        lock (_lock)
        {
            _policy.Execute(() => _connection = _connectionFactory.CreateConnection());

            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                return true;
            }

            _logger.LogWarning("Не удалось установить соединение с RabbitMQ");
            return false;
        }
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("Соединение с RabbitMQ заблокировано. Повторное подключение.");

        TryConnect();
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("Произошла ошибка в RabbitMQ. Повторное подключение.");

        TryConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("Соединение с RabbitMQ было закрыто. Повторное подключение.");

        TryConnect();
    }

    public IModel CreateModel()
    {
        if (!IsConnected) throw new InvalidOperationException("Соединение с RabbitMQ недоступно");

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;
            _connection.CallbackException -= OnCallbackException;
            _connection.ConnectionBlocked -= OnConnectionBlocked;
            _connection.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при закрытии соединения с RabbitMQ");
        }
    }
}