using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public abstract class SGOutputObjectType : ObjectType
    {
        [Required]
        [JsonProperty("return_code")]
        public string ReturnCode;

        [MaxLength(128)]
        [JsonProperty("return_msg")]
        public string ReturnMessage;
    }
}
