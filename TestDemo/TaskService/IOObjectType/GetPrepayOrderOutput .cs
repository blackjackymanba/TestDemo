using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetPrepayOrderOutput : OutputObjectType
    {
        //预支付交易会话标识
        [JsonProperty("prepay_id")]
        public string PrepayID;

        //网关订单号
        [JsonProperty("order_no")]
        public string OrderNo;
    }
}