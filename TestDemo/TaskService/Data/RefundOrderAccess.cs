using GPMGateway.Common.DataStructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPMTaskService.Data
{
    public class RefundOrderAccess
    {
        private DbTransContext dbTrans;
        public RefundOrderAccess()
        {
            dbTrans = new DbTransContext();
        }
        public RefundOrderAccess(DbTransContext _dbTrans)
        {
            dbTrans = _dbTrans;
        }
        public List<SGTrade> GetOrderList(int second)
        {
            List<SGTrade> list = null;
            var sql = @"SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER 
                            where STATUS=21 and TIMESTAMPDIFF(second,CREATEDATE,date_add(now(), interval -8 hour))>@second order by CREATEDATE desc";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@second",MySqlDbType.Int32),
            };
            paras[0].Value = second;
            var dr = dbTrans.ExecuteReader(sql, paras);
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

        public int CreateRefundOrder(int tradeOrderID, int paymentOrderID, int amount, int type, int status, int payChannelId)
        {
            var sql = "INSERT INTO FOUNDIN.REFUND_ORDER(TRADE_ORDER_ID,PAYMENT_ORDER_ID,REFUNDAMOUNT,TYPE,STATUS,CREATEDATE,MODIFYDATE,PAYCHANNELID) VALUES(@toid, @poid,@amount,@type,@status,@createdate,@modifydate,@paychannelid);";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@toid",MySqlDbType.Int32),
                new MySqlParameter("@poid",MySqlDbType.Int32),
                new MySqlParameter("@amount",MySqlDbType.Int32),
                new MySqlParameter("@type",MySqlDbType.Byte),
                new MySqlParameter("@status",MySqlDbType.Byte),
                new MySqlParameter("@createdate",MySqlDbType.DateTime),
                new MySqlParameter("@modifydate",MySqlDbType.DateTime),
                new MySqlParameter("@paychannelid",MySqlDbType.Int32)
            };
            paras[0].Value = tradeOrderID;
            paras[1].Value = paymentOrderID;
            paras[2].Value = amount;
            paras[3].Value = type; 
            paras[4].Value = status;
            paras[5].Value = DateTime.Now;
            paras[6].Value = DateTime.Now;
            paras[7].Value = payChannelId;

            var rows = dbTrans.ExecuteNonQuery(sql, paras);

            return rows;
        }

    }
}
