using CsvHelper.Configuration.Attributes;

namespace ABC.Sucursales.Models
{
    public class CarsDto
    {
        [Name("id")]
        public string id { get; set; }
        [Name("make")]
        public string country { get; set; }
        [Name("model")]
        public string state { get; set; }
        [Name("year")]
        public string year { get; set; }
    }
}