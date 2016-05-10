using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetBillHistoryInput : InputObjectType
    {
        [Required]
        [MaxLength(10)]
        [JsonProperty("acct_no")]
        public string AccountNo;
    }
}