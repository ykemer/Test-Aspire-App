using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace Library.AsyncMessages;

public class AsyncMessenger<T> where T : class
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<T> _logger;
  private readonly IModel channel;

  private readonly IConnection connection;

  public AsyncMessenger(ILogger<T> logger, IConfiguration configuration)
  {
    _logger = logger;
    _configuration = configuration;

    string? connectionString = _configuration.GetConnectionString("messaging");

    ConnectionFactory? factory = new() { Uri = new Uri(connectionString) };

    try
    {
      connection = factory.CreateConnection();
      channel = connection.CreateModel();
      channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
      _logger.LogTrace("Connected to message bus");

      connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Could not connect to message bus");
    }
  }

  protected void SendMessage(string message)
  {
    byte[]? body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("trigger",
      "",
      null,
      body);
  }

  private void CloseConnection()
  {
    Console.WriteLine("--> Message bus disposed");
    if (channel.IsOpen)
    {
      channel.Close();
      connection.Close();
    }
  }

  private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
  {
    CloseConnection();
    _logger.LogWarning("RabbitMQ connection shutdown");
  }
}
