using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Gateway.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ABC.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        [HttpPost("/Gateway")]
        public ActionResult InitializeAsync()
        {
            System.IO.File.Delete("errores.txt");
            GatewayDto transaction = new GatewayDto();
            var json = JsonConvert.SerializeObject(transaction);
            var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672//Port = 42422,
                    //VirtualHost = "/sales"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("validation-queue", false, false, false, null);
                    var body = Encoding.UTF8.GetBytes(json);
                    Console.WriteLine(body);
                    channel.BasicPublish(string.Empty, "validation-queue", null, body);
                    /*channel.BasicPublish(exchange: "",
                                         routingKey: "validation-queue",
                                         basicProperties: null,
                                         body: body);*/

                }
            }
            
            return this.Ok();
        }
        
    }
}
