using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{    
    public class GetPrepayOrderInput : InputObjectType
    {
        //户号
        [Required]
        [MaxLength(10)]
        [JsonProperty("acct_no")]
        public string AccountNo;

        //收款单位
        [Required]
        [MaxLength(16)]
        [JsonProperty("rcv_org_no")]
        public string ChargeOrganizationNo;

        //订单类型
        [Required]
        [JsonProperty("order_type")]
        [EnumDataType(typeof(OrderTypeCode))]
        public string OrderType;

        //订单金额                
        [JsonProperty("amount")]
        public int? Amount;

        //计算年月
        [MaxLength(6)]
        [JsonProperty("month")]
        public string Month;

        //商户订单号
        [Required]
        [MaxLength(32)]
        [JsonProperty("trade_order_id")]
        public string TradeOrder;

        //微信用户标识
        [Required]
        [MaxLength(128)]
        [JsonProperty("open_id")]
        public string OpenId;

        //回调地址
        [Required]
        [MaxLength(256)]
        [JsonProperty("notify_url")]
        public string NotifyUrl;

        //终端IP
        [Required]
        [MaxLength(16)]
        [JsonProperty("spbill_create_ip")]
        public string ClientIP;

        //微信昵称
        [MaxLength(128)]
        [JsonProperty("nick_name")]
        public string WechatNickName;
    }

    public enum OrderTypeCode
    {
        PAYBILL,
        CHARGE
    }
}