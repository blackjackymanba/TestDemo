using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GPMGateway.Common.Lib;
using MySql.Data.MySqlClient;
using System.Configuration;
using WxPayAPI;

namespace GPMTaskService.Callers
{
    public class WechatPayCaller : ConnectorCaller
    {
        private static readonly WechatPayCaller _caller;
        private static int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["timeout"]);

        static WechatPayCaller()
        {
            _caller = new WechatPayCaller();
        }

        private WechatPayCaller() { }

        public static WechatPayCaller Caller { get { return _caller; } }

        //公众账号ID appid 是 String(32) wx8888888888888888 微信分配的公众账号ID（企业号corpid即为此appId） 
        //商户号 mch_id 是 String(32) 1900000109 微信支付分配的商户号
        //设备号 device_info 否 String(32) 013467007045764 终端设备号
        //随机字符串 nonce_str 是 String(32) 5K8264ILTKCH16CQ2502SI8ZNMTM67VS 随机字符串，不长于32位。推荐随机数生成算法
        //签名 sign 是 String(32) C380BEC2BFD727A4B6845133519F3AD6 签名，详见签名生成算法
        //微信订单号 transaction_id 二选一 String(28) 1217752501201407033233368018 微信生成的订单号，在支付通知中有返回
        //商户订单号 out_trade_no String(32) 1217752501201407033233368018 商户侧传给微信的订单号
        //商户退款单号 out_refund_no 是 String(32) 1217752501201407033233368018 商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔
        //总金额 total_fee 是 Int 100 订单总金额，单位为分，只能为整数，详见支付金额
        //退款金额 refund_fee 是 Int 100 退款总金额，订单总金额，单位为分，只能为整数，详见支付金额
        //货币种类 refund_fee_type 否 String(8) CNY 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        //操作员 op_user_id 是 String(32) 1900000109 操作员帐号, 默认为商户号
        public SortedDictionary<string, object> RefundOrder(Guid gwRequestKey, string apiKey, string appid, string mch_id, string transaction_id, string out_trade_no, string out_refund_no, int amount, int refund_amount, byte[] certRawData)
        {
            Uri apiUrl = new Uri(@"https://api.mch.weixin.qq.com/secapi/pay/refund");
            var prepay_id = string.Empty;
            if (string.IsNullOrEmpty(appid))
                throw new Exception("缺少接口必填参数appid！");
            if (string.IsNullOrEmpty(mch_id))
                throw new Exception("缺少接口必填参数mch_id！");
            if (string.IsNullOrEmpty(out_refund_no))
                throw new Exception("缺少接口必填参数out_refund_no！");
            if (string.IsNullOrEmpty(out_trade_no) && string.IsNullOrEmpty(transaction_id))
                throw new Exception("缺少接口必填参数,微信订单号transaction_id、商户订单号out_trade_no二选一必传！");

            var nonce_str = Guid.NewGuid().ToString("N");

            var obj = new SortedDictionary<string, object>();
            obj.Add("appid", appid);
            obj.Add("mch_id", mch_id);
            obj.Add("out_refund_no", out_refund_no);
            obj.Add("total_fee", amount);
            obj.Add("refund_fee", refund_amount);
            obj.Add("refund_fee_type", "CNY");
            obj.Add("nonce_str", nonce_str);
            if (!string.IsNullOrEmpty(transaction_id))
                obj.Add("transaction_id", transaction_id);
            if (!string.IsNullOrEmpty(out_trade_no))
                obj.Add("out_trade_no", out_trade_no);
            obj.Add("sign", WxApi.MakeSign(obj, apiKey));
            obj.Add("op_user_id", mch_id);

            var xml = WxApi.ToXml(obj);
            X509Certificate2 clientCert = new X509Certificate2(certRawData, mch_id);
            var outRes = WxPayAPI.HttpService.Post(xml, apiUrl.AbsoluteUri, clientCert, timeout);

            RecordTraffic(gwRequestKey, apiUrl.AbsoluteUri, outRes, xml);

            SortedDictionary<string, object> data = null;
            if (!string.IsNullOrEmpty(outRes))
                data = WxApi.FromXml(outRes, apiKey);
            return data;
        }
        /// <summary>
        /// 微信通道走主商户(服务商模式)
        /// </summary>
        public SortedDictionary<string, object> RefundOrder(Guid gwRequestKey, string apiKey, string appid, string mch_id, string sub_appid, string sub_mch_id, string transaction_id, string out_trade_no, string out_refund_no, int amount, int refund_amount, byte[] certRawData)
        {
            Uri apiUrl = new Uri(@"https://api.mch.weixin.qq.com/secapi/pay/refund");
            var prepay_id = string.Empty;
            if (string.IsNullOrEmpty(appid))
                throw new Exception("缺少接口必填参数appid！");
            if (string.IsNullOrEmpty(mch_id))
                throw new Exception("缺少接口必填参数mch_id！");
            if (string.IsNullOrEmpty(sub_mch_id))
                throw new Exception("缺少接口必填参数sub_mch_id！");
            if (string.IsNullOrEmpty(out_refund_no))
                throw new Exception("缺少接口必填参数out_refund_no！");
            if (string.IsNullOrEmpty(out_trade_no) && string.IsNullOrEmpty(transaction_id))
                throw new Exception("缺少接口必填参数,微信订单号transaction_id、商户订单号out_trade_no二选一必传！");

            var nonce_str = Guid.NewGuid().ToString("N");

            var obj = new SortedDictionary<string, object>();
            obj.Add("appid", appid);
            obj.Add("mch_id", mch_id);
            if (!string.IsNullOrEmpty(sub_appid))
                obj.Add("sub_appid", sub_appid);
            obj.Add("sub_mch_id", sub_mch_id);
            obj.Add("out_refund_no", out_refund_no);
            obj.Add("total_fee", amount);
            obj.Add("refund_fee", refund_amount);
            obj.Add("refund_fee_type", "CNY");
            obj.Add("nonce_str", nonce_str);
            if (!string.IsNullOrEmpty(transaction_id))
                obj.Add("transaction_id", transaction_id);
            if (!string.IsNullOrEmpty(out_trade_no))
                obj.Add("out_trade_no", out_trade_no);
            obj.Add("sign", WxApi.MakeSign(obj, apiKey));
            obj.Add("op_user_id", mch_id);

            var xml = WxApi.ToXml(obj);
            X509Certificate2 clientCert = new X509Certificate2(certRawData, mch_id);
            var outRes = WxPayAPI.HttpService.Post(xml, apiUrl.AbsoluteUri, clientCert, timeout);

            RecordTraffic(gwRequestKey, apiUrl.AbsoluteUri, outRes, xml);

            SortedDictionary<string, object> data = null;
            if (!string.IsNullOrEmpty(outRes))
                data = WxApi.FromXml(outRes, apiKey);
            return data;
        }
        //公众账号ID appid 是 String(32) wx8888888888888888 微信分配的公众账号ID（企业号corpid即为此appId） 
        //商户号 mch_id 是 String(32) 1900000109 微信支付分配的商户号
        //设备号 device_info 否 String(32) 013467007045764 终端设备号
        //随机字符串 nonce_str 是 String(32) 5K8264ILTKCH16CQ2502SI8ZNMTM67VS 随机字符串，不长于32位。推荐随机数生成算法
        //签名 sign 是 String(32) C380BEC2BFD727A4B6845133519F3AD6 签名，详见签名生成算法
        //微信订单号 transaction_id 二选一 String(28) 1217752501201407033233368018 微信生成的订单号，在支付通知中有返回
        //商户订单号 out_trade_no String(32) 1217752501201407033233368018 商户侧传给微信的订单号
        //商户退款单号 out_refund_no 是 String(32) 1217752501201407033233368018 商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔
        //微信退款单号 refund_id String(28) 1217752501201407033233368018 微信生成的退款单号，在申请退款接口有返回 
        public SortedDictionary<string, object> RefundQuery(Guid gwRequestKey, string apiKey, string appid, string mch_id, string transaction_id, string out_trade_no, string out_refund_no, string refund_id)
        {
            Uri apiUrl = new Uri(@"https://api.mch.weixin.qq.com/pay/refundquery");
            var prepay_id = string.Empty;
            if (string.IsNullOrEmpty(appid))
                throw new Exception("缺少查询订单接口必填参数appid！");
            if (string.IsNullOrEmpty(mch_id))
                throw new Exception("缺少查询订单接口必填参数mch_id！");
            if (string.IsNullOrEmpty(out_trade_no) && string.IsNullOrEmpty(transaction_id) && string.IsNullOrEmpty(out_refund_no) && string.IsNullOrEmpty(refund_id))
                throw new Exception("缺少接口必填参数,微信订单号transaction_id、商户订单号out_trade_no、微信退款单号refund_id、商户退款单号out_refund_no四个参数必填一个！");

            var nonce_str = Guid.NewGuid().ToString("N");

            var obj = new SortedDictionary<string, object>();
            obj.Add("appid", appid);
            obj.Add("mch_id", mch_id);
            obj.Add("out_refund_no", out_refund_no);
            obj.Add("nonce_str", nonce_str);
            if (!string.IsNullOrEmpty(transaction_id))
                obj.Add("transaction_id", transaction_id);
            if (!string.IsNullOrEmpty(out_trade_no))
                obj.Add("out_trade_no", out_trade_no);
            if (!string.IsNullOrEmpty(out_refund_no))
                obj.Add("out_refund_no", out_refund_no);
            if (!string.IsNullOrEmpty(refund_id))
                obj.Add("refund_id", refund_id);
            obj.Add("sign", WxApi.MakeSign(obj, apiKey));

            var xml = WxApi.ToXml(obj);
            var outRes = WxPayAPI.HttpService.Post(xml, apiUrl.AbsoluteUri, null, timeout);
            RecordTraffic(gwRequestKey, apiUrl.AbsoluteUri, outRes, xml);
            SortedDictionary<string, object> data = null;
            if (!string.IsNullOrEmpty(outRes))
                data = WxApi.FromXml(outRes, apiKey);
            return data;
        }

        public SortedDictionary<string, object> RefundQuery(Guid gwRequestKey, string apiKey, string appid, string mch_id, string sub_appid, string sub_mch_id, string transaction_id, string out_trade_no, string out_refund_no, string refund_id)
        {
            Uri apiUrl = new Uri(@"https://api.mch.weixin.qq.com/pay/refundquery");
            var prepay_id = string.Empty;
            if (string.IsNullOrEmpty(appid))
                throw new Exception("缺少查询订单接口必填参数appid！");
            if (string.IsNullOrEmpty(mch_id))
                throw new Exception("缺少查询订单接口必填参数mch_id！");
            if (string.IsNullOrEmpty(sub_mch_id))
                throw new Exception("缺少接口必填参数sub_mch_id！");
            if (string.IsNullOrEmpty(out_trade_no) && string.IsNullOrEmpty(transaction_id) && string.IsNullOrEmpty(out_refund_no) && string.IsNullOrEmpty(refund_id))
                throw new Exception("缺少接口必填参数,微信订单号transaction_id、商户订单号out_trade_no、微信退款单号refund_id、商户退款单号out_refund_no四个参数必填一个！");

            var nonce_str = Guid.NewGuid().ToString("N");

            var obj = new SortedDictionary<string, object>();
            obj.Add("appid", appid);
            obj.Add("mch_id", mch_id);
            if (!string.IsNullOrEmpty(sub_appid))
                obj.Add("sub_appid", sub_appid);
            obj.Add("sub_mch_id", sub_mch_id);
            obj.Add("out_refund_no", out_refund_no);
            obj.Add("nonce_str", nonce_str);
            if (!string.IsNullOrEmpty(transaction_id))
                obj.Add("transaction_id", transaction_id);
            if (!string.IsNullOrEmpty(out_trade_no))
                obj.Add("out_trade_no", out_trade_no);
            if (!string.IsNullOrEmpty(out_refund_no))
                obj.Add("out_refund_no", out_refund_no);
            if (!string.IsNullOrEmpty(refund_id))
                obj.Add("refund_id", refund_id);
            obj.Add("sign", WxApi.MakeSign(obj, apiKey));

            var xml = WxApi.ToXml(obj);
            var outRes = WxPayAPI.HttpService.Post(xml, apiUrl.AbsoluteUri, null, timeout);
            RecordTraffic(gwRequestKey, apiUrl.AbsoluteUri, outRes, xml);
            SortedDictionary<string, object> data = null;
            if (!string.IsNullOrEmpty(outRes))
                data = WxApi.FromXml(outRes, apiKey);
            return data;
        }
        public SortedDictionary<string, object> OrderQuery(Guid gwRequestKey, string apiKey, string appid, string mch_id, string sub_appid, string sub_mch_id, string transaction_id, string out_trade_no)
        {
            Uri apiUrl = new Uri(@"https://api.mch.weixin.qq.com/pay/orderquery");
            var prepay_id = string.Empty;
            if (string.IsNullOrEmpty(appid))
                throw new Exception("缺少查询订单接口必填参数appid！");
            if (string.IsNullOrEmpty(mch_id))
                throw new Exception("缺少查询订单接口必填参数mch_id！");
            if (string.IsNullOrEmpty(sub_mch_id))
                throw new Exception("缺少查询订单接口必填参数sub_mch_id！");
            if (string.IsNullOrEmpty(out_trade_no) && string.IsNullOrEmpty(transaction_id))
                throw new Exception("缺少查询订单接口必填参数,微信订单号transaction_id、商户订单号out_trade_no二选一必传！");

            var nonce_str = Guid.NewGuid().ToString("N");

            var obj = new SortedDictionary<string, object>();
            obj.Add("appid", appid);
            obj.Add("mch_id", mch_id);
            if (!string.IsNullOrEmpty(sub_appid))
                obj.Add("sub_appid", sub_appid);
            obj.Add("sub_mch_id", sub_mch_id);
            obj.Add("nonce_str", nonce_str);
            if (!string.IsNullOrEmpty(transaction_id))
                obj.Add("transaction_id", transaction_id);
            if (!string.IsNullOrEmpty(out_trade_no))
                obj.Add("out_trade_no", out_trade_no);
            obj.Add("sign", WxApi.MakeSign(obj, apiKey));

            var xml = WxApi.ToXml(obj);
            var outRes = WxPayAPI.HttpService.Post(xml, apiUrl.AbsoluteUri, null, timeout);
            RecordTraffic(gwRequestKey, apiUrl.AbsoluteUri, outRes, xml);
            SortedDictionary<string, object> data = null;
            if (!string.IsNullOrEmpty(outRes))
                data = WxApi.FromXml(outRes, apiKey);
            return data;
        }

        public string DownloadBill(Guid gwRequestKey, string apiKey, string appid, string mch_id, string bill_date)
        {
            Uri apiUrl = new Uri(@"https://api.mch.weixin.qq.com/pay/downloadbill");
            var prepay_id = string.Empty;
            if (string.IsNullOrEmpty(bill_date))
                throw new Exception("对账单接口中，缺少必填参数bill_date！");

            var nonce_str = Guid.NewGuid().ToString("N");

            var obj = new SortedDictionary<string, object>();
            obj.Add("appid", appid);
            obj.Add("mch_id", mch_id);
            obj.Add("nonce_str", nonce_str);
            if (!string.IsNullOrEmpty(bill_date))
                obj.Add("bill_date", bill_date);
            obj.Add("sign", WxApi.MakeSign(obj, apiKey));

            var xml = WxApi.ToXml(obj);
            var outRes = WxPayAPI.HttpService.Post(xml, apiUrl.AbsoluteUri, null, timeout);
            RecordTraffic(gwRequestKey, apiUrl.AbsoluteUri, outRes, xml);
            string data = string.Empty;
            if (!string.IsNullOrEmpty(outRes))
                data = outRes;
            return data;
        }

        protected override Guid RecordRequest(string url, string requestData, Guid gwRequestKey)
        {
            var key = Guid.NewGuid();
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.FIConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(@"INSERT INTO FOUNDIN.CHANNEL_REQUEST (REQUESTURL,REQUESTCONTENT,GATEWAYREQUESTKEY,REQUESTDATE,REQUEST_KEY) VALUES (@url,@content,@gwKey,@creation,@key);", conn))
                {
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@content", requestData);
                    cmd.Parameters.AddWithValue("@gwKey", gwRequestKey.ToByteArray());
                    cmd.Parameters.AddWithValue("@creation", DateTime.Now);
                    cmd.Parameters.AddWithValue("@key", key.ToByteArray());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            return key;
        }

        protected override void RecordResponse(string responseData, Guid sgcRequestKey)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.FIConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(@"INSERT INTO FOUNDIN.CHANNEL_RESPONSE (RESPONSECONTENT,RESPONSEDATE,REQUESTKEY) VALUES (@content,@creation,@key);", conn))
                {
                    cmd.Parameters.AddWithValue("@content", responseData);
                    cmd.Parameters.AddWithValue("@creation", DateTime.Now);
                    cmd.Parameters.AddWithValue("@key", sgcRequestKey.ToByteArray());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
        }
    }
}