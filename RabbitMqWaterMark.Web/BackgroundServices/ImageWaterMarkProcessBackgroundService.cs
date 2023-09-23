using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqWaterMark.Web.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMqWaterMark.Web.BackgroundServices
{
    public class ImageWaterMarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMqClientService _rabbitMqClientService;
        private readonly ILogger<ImageWaterMarkProcessBackgroundService> _logger;
        private IModel _channel;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ImageWaterMarkProcessBackgroundService(RabbitMqClientService rabbitMqClientService, ILogger<ImageWaterMarkProcessBackgroundService> logger)

        {
            _rabbitMqClientService = rabbitMqClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMqClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMqClientService.QueueName, false, consumer);
            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", productImageCreatedEvent!.ImageName!);
                var siteName = "www.erhangokceoglu.com";
                using var image = Image.FromFile(path);
                using var graphic = Graphics.FromImage(image);
                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString(siteName, font);
                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);
                var position = new Point(image.Width - ((int)textSize.Width + 30), image.Height - (int)textSize.Height);
                graphic.DrawString(siteName, font, brush, position);
                image.Save("wwwroot/images/watermarks/" + productImageCreatedEvent.ImageName);
                image.Dispose();
                graphic.Dispose();
                _channel.BasicAck(@event.DeliveryTag, false);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
