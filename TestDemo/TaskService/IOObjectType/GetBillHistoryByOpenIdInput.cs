
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetBillHistoryByOpenIdInput : InputObjectType
    {
        [Required]
        [MaxLength(64)]
        [JsonProperty("wx_openid")]
        public string OpenId;
    }
}