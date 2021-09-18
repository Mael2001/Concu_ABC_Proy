using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace ABC.RecolectorService
{
    public class SaleDto
    {
        [Name("username")]
        public string username { get; set; }
        [Name("car_id")]
        public string car_id { get; set; }
        [Name("price")]
        public string price { get; set; }
        [Name("vin")]
        public string vin { get; set; }
        [Name("buyer_first_name")]
        public string buyer_first_name { get; set; }
        [Name("buyer_last_name")]
        public string buyer_last_name { get; set; }
        [Name("buyer_id")]
        public string buyer_id { get; set; }
        [Name("division_id")]
        public string division_id { get; set; }
    }
}
