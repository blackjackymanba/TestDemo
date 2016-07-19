using GPMGateway.Common.DataStructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace GPMTaskService.Data
{
    public class RefundDataAccess
    {
        private DbTransContext dbTrans;
        public RefundDataAccess()
        {
            dbTrans = new DbTransContext();
        }
        public RefundDataAccess(DbTransContext _dbTrans)
        {
            dbTrans = _dbTrans;
        }

        public List<SGTrade> GetRefundList()
        {
            List<SGTrade> list = null;
            var sql = @"SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER 
                             order by CREATEDATE desc";
           
            var dr = dbTrans.ExecuteReader(sql);
            if (dr.HasRows)
            {
                list = new List<SGTrade>();
                while (dr.Read())
                {
                    var p = new SGTrade();
                    p.ID = dr.GetInt32(0);
                    p.Status = dr.GetInt16(4);
                    p.Type = dr.GetInt16(6);
                    p.SGUserAccount = new SGAccount(dr.GetString(1));
                    p.ChargeOrganization = dr.GetString(3);
                    p.SourcePlatform = new Platform();
                    p.SourcePlatform.PlatformId = dr.GetString(5);
                    p.FoundInOrder = new FIOrder();
                    p.FoundInOrder.ID = dr.GetInt32(2);
                    p.Amount = dr.IsDBNull(7) ? 0 : dr.GetInt32(7);
                    p.Penalty = dr.IsDBNull(8) ? 0 : dr.GetInt32(8);
                    list.Add(p);
                }
            }

            dr.Close();
            dbTrans.Close();

            return list;
        }
        public List<SGTrade> GetRefundQueryList()
        {
            List<SGTrade> list = null;
            var sql = @"SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER 
                            where STATUS=23  order by CREATEDATE desc";

            var dr = dbTrans.ExecuteReader(sql);
            if (dr.HasRows)
            {
                list = new List<SGTrade>();
                while (dr.Read())
                {
                    var p = new SGTrade();
                    p.ID = dr.GetInt32(0);
                    p.Status = dr.GetInt16(4);
                    p.Type = dr.GetInt16(6);
                    p.SGUserAccount = new SGAccount(dr.GetString(1));
                    p.ChargeOrganization = dr.GetString(3);
                    p.SourcePlatform = new Platform();
                    p.SourcePlatform.PlatformId = dr.GetString(5);
                    p.FoundInOrder = new FIOrder();
                    p.FoundInOrder.ID = dr.GetInt32(2);
                    p.Amount = dr.IsDBNull(7) ? 0 : dr.GetInt32(7);
                    p.Penalty = dr.IsDBNull(8) ? 0 : dr.GetInt32(8);
                    list.Add(p);
                }
            }

            dr.Close();
            dbTrans.Close();

            return list;
        }
        public Refund GetRefund(int tradeOrderId)
        {
                var sql = "SELECT REFUND_ORDER_ID, STATUS, REFUNDAMOUNT,CHANNELORDERID,PAYMENT_ORDER_ID FROM FOUNDIN.REFUND_ORDER where TRADE_ORDER_ID=@tradeOrderId FOR UPDATE;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@tradeOrderId",MySqlDbType.Int32)
            };
            paras[0].Value = tradeOrderId;
            var dr = dbTrans.ExecuteReader(sql, paras);
            Refund model = null;
            if (dr.HasRows && dr.Read())
            {
                model = new Refund();
                model.refund_order_id = dr.GetInt32(0);
                model.status = dr.GetInt32(1);
                model.refund_amount = dr.GetInt32(2);
                model.refund_id = dr.IsDBNull(3) ? "" : dr.GetString(3);
                model.payment_order_id = dr.GetInt32(4);
            }
            dr.Close();
            return model;
        }
    }
}
