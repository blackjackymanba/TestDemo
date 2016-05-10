using System.ComponentModel.DataAnnotations;
using GPMGateway.Common.IOObjectType;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public abstract class InputObjectType : ObjectType
    {
        [Required]
        [MaxLength(32)]
        [JsonProperty("platform_id")]
        public string PlatformId;
    }
}