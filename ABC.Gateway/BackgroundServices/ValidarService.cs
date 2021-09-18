using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ABC.Gateway.BackgroundServices
{
    public class ValidarService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public ValidarService()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("send-queue", false, false, false, null);
            _consumer = new EventingBasicConsumer(_channel);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            string js = "";
            _consumer.Received += async (model, content) =>
            {
                var body = content.Body.ToArray();
                var result = Encoding.UTF8.GetString(body);
                js = result;
                //await System.IO.File.WriteAllTextAsync("WriteText.txt", js);
                await System.IO.File.AppendAllTextAsync("errores.txt", $"{js}\n");
            };

            _channel.BasicConsume("send-queue", true, _consumer);
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
