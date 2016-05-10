using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetUnpaidBillOutput : OutputObjectType
    {
        //户号
        [JsonProperty("acct_no")]
        public string AccountNo;

        //供电单位
        [JsonProperty("org_no")]
        public string ChargeOrganizationNo;

        //户名
        [JsonProperty("acct_name")]
        public string AccountName;

        //地址
        [JsonProperty("address")]
        public string Address;

        //总待缴金额
        [JsonProperty("amount")]
        public int Amount;

        //总欠费金额
        [JsonProperty("owe_amount")]
        public int OwnAmount;

        //总违约金
        [JsonProperty("penalty")]
        public int Penalty;

        //预缴费余额
        [JsonProperty("balance")]
        public int Balance;

        //[JsonProperty("detail")]
        //public List<UnpaidBillDetail> Detail;
    }

    public class UnpaidBillDetail
    {
        [JsonProperty("month")]
        public string Month;

        [JsonProperty("amount")]
        public decimal Amount;

        [JsonProperty("owe_amount")]
        public int OwnAmount;

        [JsonProperty("penalty")]
        public decimal Penalty;

        [JsonProperty("expiry_date")]
        public string ExpiryDate;

        [JsonProperty("desc")]
        public string Description;
    }
}