using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class PayBillOutput : OutputObjectType
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

        //计算年月
        [JsonProperty("month")]
        public string Month;

        //商户交易流水号
        [JsonProperty("trade_order_id")]
        public string TradeOrderId;

        //网关交易流水号
        [JsonProperty("order_no")]
        public string OrderNo;

        //交易类型
        [JsonProperty("order_type")]
        public string OrderType;

        //交易状态
        [JsonProperty("order_state")]
        public string OrderStatus;
    }
}