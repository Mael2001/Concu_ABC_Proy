using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ABC.RecolectorService
{
    class RecolectorService : BackgroundService
    {
        private readonly ILogger<RecolectorService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public RecolectorService(ILogger<RecolectorService> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("validation-queue", false, false, false, null);
            _consumer = new EventingBasicConsumer(_channel);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            

            _consumer.Received += async (model, content) =>
            {
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                //Console.WriteLine(json);
                //var recolectorInformation = JsonConvert.DeserializeObject<SaleDto>(json);
                var csvReader = new CSVReader();
                var result = csvReader.ProcessSales().Result;
                sendHttpRequest(result);
            };

            _channel.BasicConsume("validation-queue", true, _consumer);

            
            return Task.CompletedTask;
        }

        private async void sendHttpRequest(List<SaleDto> results)
        {
            
            if (results.Count<=50)
            {
                var json = JsonConvert.SerializeObject(results);
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672//Port = 42578,
                    //VirtualHost = "/validate"
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("recollection-queue", false, false, false, null);
                        var body = Encoding.UTF8.GetBytes(json);
                        Console.WriteLine(json);
                        channel.BasicPublish(string.Empty, "recollection-queue", null, body);
                    }
                }
                //Cambiar Por RabbitMq
                /*using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(results);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("https://localhost:44374/validate", data);
                }*/
            }
            else
            {
                var tmpList1 = results.GetRange(0, 50);
                var tmpList2 = results.GetRange(50, results.Count-tmpList1.Count);
                sendHttpRequest(tmpList1);
                sendHttpRequest(tmpList2);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
