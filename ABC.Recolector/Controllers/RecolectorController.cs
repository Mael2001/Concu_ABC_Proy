using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ABC.Recolector.Models;
using ABC.Recolector.Reader;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ABC.Recolector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecolectorController : ControllerBase
    {
        [HttpPost("/sales")]
        public ActionResult<IEnumerable<SaleDto>> GetSales()
        {
            var csvReader = new CSVReader();
            var result = csvReader.ProcessSales().Result;
            sendHttpRequest(result);
            return this.Ok(result);
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
                        channel.QueueDeclare("validation-queue", false, false, false, null);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(string.Empty, "validation-queue", null, body);
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
    }
}
