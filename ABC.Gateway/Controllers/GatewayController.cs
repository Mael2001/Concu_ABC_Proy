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

namespace ABC.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        [HttpPost("/Gateway")]
        public ActionResult Initialize()
        {
            GatewayDto transaction = new GatewayDto();
            var json = JsonConvert.SerializeObject(transaction);
            var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 42422,
                    VirtualHost = "/sales"
            };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("validation-queue", false, false, false, null);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(string.Empty, "validation-queue", null, body);
                    }
                }
            return this.Ok();
        }
    }
}
