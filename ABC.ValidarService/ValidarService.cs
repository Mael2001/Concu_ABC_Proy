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
using System.Net.Http;
using System.Collections.Concurrent;

namespace ABC.ValidarService
{
    public class ValidarService : BackgroundService
    {
        private readonly ILogger<ValidarService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public ValidarService(ILogger<ValidarService> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("recollection-queue", false, false, false, null);
            _consumer = new EventingBasicConsumer(_channel);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var res = new List<SaleDto>();
            Console.WriteLine("Before Consuming...");
            _consumer.Received += async (model, content) =>
            {
                Console.WriteLine("Inside Consuming...");
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var results = new List<string>();
                var validarSales = JsonConvert.DeserializeObject<List<SaleDto>>(json);
                //Console.WriteLine(json);
                /*foreach (var item in validarSales)
                {
                    Console.WriteLine($"{item.username}, {item.car_id}, {item.division_id}");
                }*/
                results = await SendHttpRequest(validarSales);
                //var recolectorInformation = JsonConvert.DeserializeObject<SaleDto>(json);
                SendResults(results);
            };

            _channel.BasicConsume("recollection-queue", true, _consumer);
            

            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task<List<string>> SendHttpRequest(List<SaleDto> sales)
        {
            ConcurrentBag<SaleDto> results = new ConcurrentBag<SaleDto>();
            ConcurrentBag<string> errores = new ConcurrentBag<string>();
            SaleDto error = new SaleDto();
            Console.WriteLine("Before HttpRequest");
            Parallel.ForEach(sales, async (tmp) =>
            {

                using (var httpClient = new HttpClient())
                {
                    var responseEmployee = await httpClient.GetAsync($"https://localhost:44346/employee/{tmp.username}");
                    var responseCar = await httpClient.GetAsync($"https://localhost:44346/car/{tmp.car_id}");
                    var responseSucursal = await httpClient.GetAsync($"https://localhost:44346/sucursal/{tmp.division_id}");
                    if (responseSucursal.IsSuccessStatusCode && responseCar.IsSuccessStatusCode && responseEmployee.IsSuccessStatusCode)
                    {
                        EmployeeDto employee = JsonConvert.DeserializeObject<EmployeeDto>(await responseEmployee.Content.ReadAsStringAsync());
                        CarsDto car = JsonConvert.DeserializeObject<CarsDto>(await responseCar.Content.ReadAsStringAsync());
                        SurcursalDto sucursale = JsonConvert.DeserializeObject<SurcursalDto>(await responseSucursal.Content.ReadAsStringAsync());
                        Console.WriteLine("Exito!");
                        if (tmp.buyer_id == null)
                        {
                            //Console.WriteLine($"Ocurrio un error con el buyer_id de {error.username}");
                            error = tmp;
                            results.Add(tmp);
                            errores.Add($"Ocurrio un error con el buyer_id de {error.username}");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"Ocurrio un error con {tmp.username}");
                        error = tmp;
                        //Console.WriteLine($"Ocurrio un error con {error.username}");
                        results.Add(tmp);
                        errores.Add($"Ocurrio un error con {error.username}");
                    }
                    
                }
            });
            await Task.Delay(10000);
            /*foreach (var item in re)
            {
                Console.WriteLine(item.username);
            }*/
            return errores.ToList();
            /*if (error.buyer_first_name == null)
            {
                return this.BadRequest($"There is an error on sale: {error.ToString()}");
            }
            return this.Ok(sales.ToList());*/
            //return error!= null
            //?this.Ok(sales.ToList()) 
            //:this.BadRequest($"There is an error on sale: {error.ToString()}");
        }
        private void SendResults(List<string> results)
        {
            //var json = JsonConvert.SerializeObject(results);
            string errors = string.Join("\n",results);
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("send-queue", false, false, false, null);
                    var body = Encoding.UTF8.GetBytes(errors);
                    Console.WriteLine("The errors Are: ");
                    Console.WriteLine(errors);
                    channel.BasicPublish(string.Empty, "send-queue", null, body);
                }
            }
        }
    
    }
}
