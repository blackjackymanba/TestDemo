using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class PayBillInput : InputObjectType
    {
        //网关交易流水号
        [Required]
        [MaxLength(10)]
        [JsonProperty("order_no")]
        public string OrderNo;

        //微信支付订单号
        [MaxLength(32)]
        [JsonProperty("transaction_id")]
        public string TransactionID;
    }
}