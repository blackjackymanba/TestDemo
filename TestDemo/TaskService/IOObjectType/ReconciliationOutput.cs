using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class ReconciliationOutput : OutputObjectType
    {
        [JsonProperty("acct_no")]
        public string AccountNo;

        [JsonProperty("org_no")]
        public string ChargeOrganization;

        [JsonProperty("acct_name")]
        public string AccountName;

        [JsonProperty("address")]
        public string Address;

        [JsonProperty("amount")]
        public decimal Amount;

        [JsonProperty("penalty")]
        public string Penalty;
    }
}