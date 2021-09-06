using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ABC.Validacion.Models;
using Newtonsoft.Json;

namespace ABC.Validacion.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidarController : ControllerBase
    {

        [HttpPost("/validate")]
        public ActionResult<List<SaleDto>> Get([FromBody] List<SaleDto> sales)
        {
            SaleDto error = new SaleDto();
            Parallel.ForEach(sales, async (tmp) =>
            {

                using (var httpClient = new HttpClient())
                {
                    var responseEmployee = await httpClient.GetAsync($"https://localhost:44374/employee/{tmp.username}");
                    var responseCar = await httpClient.GetAsync($"https://localhost:44374/car/{tmp.car_id}");
                    var responseSucursal = await httpClient.GetAsync($"https://localhost:44374/sucursal/{tmp.division_id}");
                    if (responseSucursal.IsSuccessStatusCode && responseCar.IsSuccessStatusCode && responseEmployee.IsSuccessStatusCode)
                    {
                        EmployeeDto employee = JsonConvert.DeserializeObject<EmployeeDto>(await responseEmployee.Content.ReadAsStringAsync());
                        CarsDto car = JsonConvert.DeserializeObject<CarsDto>(await responseCar.Content.ReadAsStringAsync());
                        SurcursalDto sucursale = JsonConvert.DeserializeObject<SurcursalDto>(await responseSucursal.Content.ReadAsStringAsync());
                        if (tmp.buyer_id== null)
                        {
                            error = tmp;
                        }
                    }
                    else
                    {
                        error = tmp;
                    }
                }
            });
            if (error.buyer_first_name==null)
            {
                return this.BadRequest($"There is an error on sale: {error.ToString()}");
            }
            return this.Ok(sales.ToList());
            //return error!= null
                //?this.Ok(sales.ToList()) 
                //:this.BadRequest($"There is an error on sale: {error.ToString()}");
        }
    }
}
