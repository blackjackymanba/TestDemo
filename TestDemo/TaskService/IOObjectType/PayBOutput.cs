using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class PayBOutput : SGOutputObjectType
    {
        //户号
        [JsonProperty("acct_no")]
        public string AccountNo;

        //缴费单位
        [JsonProperty("org_no")]
        public string ChargeOrganization;

        //总金额
        [JsonProperty("amount")]
        public int Amount;

        [JsonProperty("org_name")]
        public string ChargeOrgName;

        [JsonProperty("acct_name")]
        public string AccountName;

        [JsonProperty("address")]
        public string Address;

        [JsonProperty("serial_no")]
        public string SGSerialNo;

        [JsonProperty("balance")]
        public int Balance;

    }
}
