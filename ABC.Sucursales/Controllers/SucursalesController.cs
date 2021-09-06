using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABC.Sucursales.Models;

namespace ABC.Sucursales.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SucursalesController : ControllerBase
    {
        [HttpGet("/employee/{username}")]
        public ActionResult<EmployeeDto> GetEmployeeByUsername(string username)
        {
            CSVReader reader = new CSVReader();
            var employee = reader.ProcessEmployee().Result.FirstOrDefault(x => x.username == username);
            if (employee==null)
            {
                return this.BadRequest($"Employee with username: {username} doesn't exit");
            }

            return this.Ok(employee);
        }
        [HttpGet("/car/{id}")]
        public ActionResult<CarsDto> GetCarById(string id)
        {
            CSVReader reader = new CSVReader();
            var car = reader.ProcessCar().Result.FirstOrDefault(x => x.id == id);
            if (car == null)
            {
                return this.BadRequest($"Car with id: {id} doesn't exit");
            }

            return this.Ok(car);
        }
        [HttpGet("/sucursal/{id}")]
        public ActionResult<SurcursalDto> GetSucursalById(string id)
        {
            CSVReader reader = new CSVReader();
            var sucursal = reader.ProcessSucursal().Result.FirstOrDefault(x => x.id == id);
            if (sucursal == null)
            {
                return this.BadRequest($"Sucursal with id: {id} doesn't exit");
            }

            return this.Ok(sucursal);
        }
    }
}
