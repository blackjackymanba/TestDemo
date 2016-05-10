using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class ReconciliationInput : InputObjectType
    {
        [Required]
        [MaxLength(8)]
        [JsonProperty("date")]
        public string Date;
    }
}