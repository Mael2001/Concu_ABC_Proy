using CsvHelper.Configuration.Attributes;

namespace ABC.Sucursales.Models
{
    public class EmployeeDto
    {
        [Name("username")]
        public string username { get; set; }
        [Name("first_name")]
        public string first_name { get; set; }
        [Name("last_name")]
        public string last_name { get; set; }
        [Name("ID")]
        public string ID { get; set; }
    }
}