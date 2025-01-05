using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.AsyncMessages;

public class AsyncMessageHandler<T>: BackgroundService where T: class
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<T> _logger;

    private IConnection connection;
    private IModel channel;
    private string queueName;

    public AsyncMessageHandler(IConfiguration configuration, IEventProcessor eventProcessor,
        ILogger<T> logger)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        _logger = logger;
        InitialiseRabbitMq();
    }

    private void InitialiseRabbitMq()
    {
        var connectionString = _configuration.GetConnectionString("messaging");

        var factory = new ConnectionFactory()
        {
            Uri = new Uri(connectionString)
        };

        try
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(
                queue: queueName,
                exchange: "trigger",
                routingKey: string.Empty
            );
            _logger.LogTrace($"--> Listening on the message bus");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not connect to message bus: ${Message}", e.Message);
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection shutdown");
        Dispose();
    }

    private void Dispose()
    {
        _logger.LogInformation($"Message bus disposed");
        if (channel.IsOpen)
        {
            channel.Close();
            connection.Close();
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            _eventProcessor.ProcessEvent(notificationMessage);
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}