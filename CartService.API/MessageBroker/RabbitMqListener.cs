
using CartService.API.MessageBroker.Messages;
using CartService.BLL.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CartService.API.MessageBroker
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;

        public RabbitMqListener(IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"],
                UserName = config["RabbitMQ:Username"],
                Password = config["RabbitMQ:Password"]
            };
        }
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            var connection = await _factory.CreateConnectionAsync(ct);
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                "catalog-updates",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<CatalogItemUpdatedEvent>(json);
                if (message != null)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var cartService = scope.ServiceProvider
                                       .GetRequiredService<ICartService>();
                        await cartService.UpdateItemAsync(message.ProductId, message.Name, message.Price, ct);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error, sending to retry… " + ex.Message);
                        await channel.BasicRejectAsync(args.DeliveryTag, false);
                    }
                }
            };

            await channel.BasicConsumeAsync(
                queue: "catalog-updates",
                autoAck: true,
                consumer: consumer);

        }
    }
}