using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class GetBillHistoryOutput : OutputObjectType
    {
        //户号
        [JsonProperty("acct_no")]
        public string AccountNo;

        //缴费单位
        [JsonProperty("org_no")]
        public string ChargeOrganization;

        //户名
        [JsonProperty("acct_name")]
        public string AccountName;

        //地址
        [JsonProperty("address")]
        public string Address;

        //总金额
        [JsonProperty("amount")]
        public int Amount;

        //总欠电费金额
        [JsonProperty("owe_amount")]
        public int OweAmount;

        //总违约金
        [JsonProperty("penalty")]
        public int Penalty;

        //明细
        [JsonProperty("detail")]
        public List<BillDetail> Detail;
    }

    public class BillDetail
    {
        //缴费时间
        [JsonProperty("pay_date")]
        public string PayDate;

        //金额
        [JsonProperty("amount")]
        public int Amount;

        //总欠电费金额
        [JsonProperty("owe_amount")]
        public int OweAmount;

        //违约金
        [JsonProperty("penalty")]
        public int Penalty;

        //交易类
        [EnumDataType(typeof(OrderTypeCode))]
        [JsonProperty("order_type")]
        public string OrderType;

        //说明
        [JsonProperty("desc")]
        public string Description;

        //支付状态
        [EnumDataType(typeof(OrderStatusCode))]
        [JsonProperty("order_state")]
        public string PaymentStatus;

        //商户订单号
        [JsonProperty("trade_order_id")]
        public string TradeOrderId;

        //交易订单号
        [JsonProperty("order_no")]
        public string OrderNo;
    }
}