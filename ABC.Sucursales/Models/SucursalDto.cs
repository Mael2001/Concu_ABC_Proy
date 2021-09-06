using CsvHelper.Configuration.Attributes;

namespace ABC.Sucursales.Models
{
    public class SurcursalDto
    {
        [Name("id")]
        public string id { get; set; }
        [Name("country")]
        public string country { get; set; }
        [Name("state")]
        public string state { get; set; }
    }
}