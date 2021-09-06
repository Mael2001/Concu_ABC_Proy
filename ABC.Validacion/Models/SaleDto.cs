using System.ComponentModel.DataAnnotations;

namespace ABC.Validacion.Models
{
    public class SaleDto
    {
        
        public string username { get; set; }
        public string car_id { get; set; }
        public string price { get; set; }
        [RegularExpression("[A-HJ-NPR-Z0-9]{13}[0-9]{4}", ErrorMessage = "Invalid Vehicle Identification Number Format.")]
        public string vin { get; set; }
        public string buyer_first_name { get; set; }
        public string buyer_last_name { get; set; }
        public string buyer_id { get; set; }
        public string division_id { get; set; }

        public override string ToString()
        {
            return $"Username: [{username}], Car id: [{car_id}], Price: [{price}], Vin: [{vin}]" +
                   $", Buyer's Name [{buyer_first_name} {buyer_last_name}], Buyer id: [{buyer_id}], Division Id: [{division_id}]\n";
        }
    }
}