using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetUnpaidBillInput : InputObjectType
    {
        //户号
        [Required]
        [MaxLength(10)]
        [JsonProperty("acct_no")]
        public string AccountNo;

        //收费单位
        [Required]
        [MaxLength(16)]
        [JsonProperty("rcv_org_no")]
        public string ChargeOrganizationNo;
    }
}