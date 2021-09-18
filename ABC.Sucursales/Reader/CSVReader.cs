using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ABC.Sucursales.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration;

namespace ABC.Sucursales
{
    public class CSVReader
    {
        public async Task<List<SurcursalDto>> ProcessSucursal()
        {
            using (var reader = new StreamReader("C:/ABCFiles/sucursales.csv"))
            {
                using (var csvReader = new CsvReader(reader, new
                    CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null
                    }))
                {
                    return csvReader.GetRecords<SurcursalDto>().ToList();
                }
            }
        }
        public async Task<List<EmployeeDto>> ProcessEmployee()
        {
            using (var reader = new StreamReader("C:/ABCFiles/employees.csv"))
            {
                using (var csvReader = new CsvReader(reader, new
                    CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null
                    }))
                {
                    return csvReader.GetRecords<EmployeeDto>().ToList();
                }
            }
        }
        public async Task<List<CarsDto>> ProcessCar()
        {
            using (var reader = new StreamReader("C:/ABCFiles/cars.csv"))
            {
                using (var csvReader = new CsvReader(reader, new
                    CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null
                    }))
                {
                    return csvReader.GetRecords<CarsDto>().ToList();
                }
            }
        }
    }
}