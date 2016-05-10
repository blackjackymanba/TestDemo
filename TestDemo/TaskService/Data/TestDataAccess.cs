using GPMGateway.Common;
using GPMGateway.Common.DataStructure;
using GPMTaskService.Callers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPMTaskService.Data
{
    public class TestDataAccess
    {
        private DbTransContext dbTrans;
        private Nlog nlog;
        public TestDataAccess()
        {
            dbTrans = new DbTransContext();
        }
        public TestDataAccess(DbTransContext _dbTrans)
        {
            dbTrans = _dbTrans;
        }

        public void Test()
        {

            nlog = new Nlog("PayBillRetry");
            nlog.Error("sssss");
            //nlog.Error("test");

            var fiOrder = GetFIOrder(520);
            var pdate = fiOrder.PayDate.ToString();

            //RefundOrderAccess rdal = new RefundOrderAccess(dbTrans);

            //var rows1 = rdal.CreateRefundOrder(600, 601, 1, 0, 1, 1);
            ////dbTrans.Commit();
            //var ss = "";

            //refundorder();

           // refund();
        }

        public void refundorder()
        {
            nlog = new Nlog("RefundOrder");

            var second = Convert.ToInt32(ConfigurationManager.AppSettings["second"]);
            var today = DateTime.Now.ToString("yyyy-MM-dd");


            var list = new RefundOrderAccess().GetOrderList(second);
            if (list != null)
            {
                foreach (var item in list)
                {
                    DbTransContext dbTrans = new DbTransContext();
                    DataAccess dal = new DataAccess(dbTrans);
                    RefundOrderAccess rdal = new RefundOrderAccess(dbTrans);
                    try
                    {
                        var FIOrder = dal.GetFIOrder(item.FoundInOrder.ID);  //锁定当前更新行
                        if (FIOrder == null || FIOrder.status != 2) //交易订单为已付款
                            continue;

                        var fipaydate = FIOrder.PayDate.ToLocalTime().ToString("yyyy-MM-dd");
                        if (fipaydate != today) //只处理当天发生的
                            continue;

                        var sgPayOrder = dal.GetSGPaymentOrder(item.ID); //锁定当前更新行
                        if (sgPayOrder == null || item.Status != 21)
                            continue;

                        var fiPayOrder = dal.GetFIPaymentOrderByTradeOrderID(item.FoundInOrder.ID);
                        if (fiPayOrder == null || fiPayOrder.Status != 1) //fi payment_order 必须满足状态为1
                            continue;

                        var rows1 = rdal.CreateRefundOrder(FIOrder.ID, fiPayOrder.ID, fiPayOrder.Amount, 0, 1, 1);
                        var rows2 = dal.SetSGTradeStatus(item.ID, 23); //sg payment_order 更新状态为 已支付退款中
                        var rows3 = dal.SetTradeOrderStatus(FIOrder.ID, 3); //fi trade_order 更新状态为 交易退款中

                        if (rows1 > 0 && rows2 > 0 && rows3 > 0)
                            dbTrans.Commit();
                        else
                            dbTrans.Rollback();

                    }
                    catch (Exception ex)
                    {
                        nlog.Error(string.Format("GPMTaskService RefundOrder Job Error:SG paymentid :{0}, error message :{1}", item.ID, ex.ToString()));
                        dbTrans.Rollback();
                    }
                    finally
                    {
                        if (dbTrans != null) dbTrans.Close();
                    }
                }
            }
        }

        public void refund()
        {
            nlog = new Nlog("Refund");
            var stoptime = Convert.ToInt32(ConfigurationManager.AppSettings["stoptime"]);

            var list = new RefundDataAccess().GetRefundList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    DbTransContext dbTrans = new DbTransContext();
                    DataAccess dal = new DataAccess(dbTrans);
                    RefundDataAccess rdal = new RefundDataAccess(dbTrans);
                    try
                    {
                        var refund = rdal.GetRefund(item.FoundInOrder.ID); //查询锁定行
                        if (refund == null || refund.status != 11)
                            continue;

                        var FIOrder = dal.GetFIOrder(item.FoundInOrder.ID);
                        if (FIOrder == null || FIOrder.status != 3) //fi tradeorder表状3表示退款中
                            continue;

                        var fiPayOrder = dal.GetFIPaymentOrderByTradeOrderID(item.FoundInOrder.ID);
                        if (fiPayOrder == null || fiPayOrder.Status != 1) //fi payment_order 必须满足状态为1
                            continue;

                        var platform = dal.GetPlatform(item.SourcePlatform.PlatformId);
                        if (platform == null)
                            continue;

                        var bMember = dal.GetBMember(item.ChargeOrganization);
                        if (bMember == null)
                            continue;

                        var transationID = dal.GetTransitionID(FIOrder.ID);
                        if (string.IsNullOrEmpty(transationID))
                            continue;

                        var rows = dal.SetRefundStatus(refund.refund_order_id, 19);

                        var refundSettingValue = dal.GetSettingForPlatform(platform, "RefundEnabled");
                        // platsetting中设置为false不做退款处理时，状态变更成15
                        if (refundSettingValue == "false")
                        {
                            var rows0 = dal.SetRefundStatus(refund.refund_order_id, 15);
                            if (rows0 > 0)
                            {
                                dbTrans.Commit();
                                continue;
                            }
                        }
                        //当前时间超过配置时间，状态变更成15
                        if (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, stoptime, 0, 0, 0))
                        {
                            var rows1 = dal.SetRefundStatus(refund.refund_order_id, 15);
                            if (rows1 > 0)
                            {
                                dbTrans.Commit();
                                continue;
                            }
                        }

                        var refund_amount = refund.refund_amount;
                        var out_refund_no = refund.refund_order_id;

                        var settingValue = dal.GetSettingForPlatform(platform, "UnifiedOrderUseSub");
                        SortedDictionary<string, object> wxTrans = null;

                        if (settingValue == "1") //微信通道走主商户（服务商模式）
                        {
                            wxTrans = WechatPayCaller.Caller.RefundOrder
                            (
                               FIOrder.gwRequestkey,
                               bMember.SourcePlatform.WechatPayAppKey,
                               bMember.SourcePlatform.WechatPayAppId,
                               bMember.SourcePlatform.WechatPayMerchantId,
                               bMember.WxAppID,
                               bMember.WxPayMerchantID,
                               transationID,
                               FIOrder.OutTradeID,
                               out_refund_no.ToString(),
                               item.Amount,
                               refund_amount,
                               platform.ClientCertificateRaw
                            );
                        }
                        else //微信通道走子商户(普通商户)
                        {
                            wxTrans = WechatPayCaller.Caller.RefundOrder
                            (
                               FIOrder.gwRequestkey,
                               bMember.WxPayAppKey,
                               bMember.WxAppID,
                               bMember.WxPayMerchantID,
                               transationID,
                               FIOrder.OutTradeID,
                               out_refund_no.ToString(),
                               item.Amount,
                               refund_amount,
                               bMember.ClientCertificateRaw
                            );
                        }
                        if (wxTrans != null && wxTrans.ContainsKey("return_code") && wxTrans["return_code"].ToString() == "SUCCESS")
                        {
                            //更新状态
                            var rows2 = dal.SetRefundStatus(refund.refund_order_id, 4, wxTrans["refund_id"].ToString());

                            if (rows2 > 0)
                                dbTrans.Commit();
                            else
                                dbTrans.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        nlog.Error(string.Format("GPMTaskService Refund Job Error:SG paymentid :{0}, error message :{1}", item.ID, ex.ToString()));
                        dbTrans.Rollback();
                    }
                    finally
                    {
                        if (dbTrans != null) dbTrans.Close();
                    }
                }
            }
        }


        public FIOrder GetFIOrder(int oID)
        {
            var sql = "SELECT ORDER_ID,OUTTRADEID,PAYMENTCOMPLETIONDATE,RESQUESTKEY,STATUS FROM FOUNDIN.TRADE_ORDER WHERE ORDER_ID=@oid;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@oid",MySqlDbType.Int32)
            };
            paras[0].Value = oID;
            var dr = dbTrans.ExecuteReader(sql, paras);
            FIOrder model = null;
            if (dr.HasRows && dr.Read())
            {
                model = new FIOrder();
                model.ID = dr.GetInt32(0);
                model.OutTradeID = dr.GetString(1);
                if (!dr.IsDBNull(2))
                    model.PayDate = dr.GetDateTime(2);
                model.gwRequestkey = dr.GetGuid(3);
                model.status = dr.GetInt32(4);
            }
            dr.Close();
            return model;
        }
    }
}
