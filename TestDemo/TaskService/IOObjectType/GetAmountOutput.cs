using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetAmountOutput : SGOutputObjectType
    {
        [JsonProperty("acct_no")]
        public string AccountNo;

        [JsonProperty("org_no")]
        public string ChargeOrganizationNo;

        [JsonProperty("acct_name")]
        public string AccountName;

        [JsonProperty("address")]
        public string Address;

        [JsonProperty("amount")]
        public int Amount;

        [JsonProperty("owe_amount")]
        public int OweAmount;

        [JsonProperty("penalty")]
        public int Penalty;

        [JsonProperty("balance")]
        public int Balance;
    }
}
