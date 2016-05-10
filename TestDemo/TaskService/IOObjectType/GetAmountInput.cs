using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetAmountInput : ObjectType
    {
        [Required]
        [MaxLength(10)]
        [JsonProperty("acct_no")]
        public string AccountNo;

        [Required]
        [MaxLength(16)]
        [JsonProperty("rcv_org_no")]
        public string ChargeOrganizationNo;

    }
}
