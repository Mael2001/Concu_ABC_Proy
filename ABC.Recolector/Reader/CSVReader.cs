using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABC.Recolector.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace ABC.Recolector.Reader
{
    public class CSVReader
    {
        public async Task<List<SaleDto>> ProcessSales()
        {
            using (var reader = new StreamReader("./sales.csv"))
            {
                using (var csvReader = new CsvReader(reader, new
                    CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null
                    }))
                {
                    return csvReader.GetRecords<SaleDto>().ToList();
                }
            }
        }
    }
}