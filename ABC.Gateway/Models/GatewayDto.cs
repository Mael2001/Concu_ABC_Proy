using System.Collections.Generic;

namespace ABC.Gateway.Models
{
    public class GatewayDto
    {
        public int id { get; set; }
        public List<string> ErrorList { get; set; }
    }
}