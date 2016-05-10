using GPMGateway.Common.DataStructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace GPMTaskService.Data
{
    public class PayBillRetryDataAccess
    {
        private DbTransContext dbTrans;
        public PayBillRetryDataAccess()
        {
            dbTrans = new DbTransContext();
        }
        public PayBillRetryDataAccess(DbTransContext _dbTrans)
        {
            dbTrans = _dbTrans;
        }

        public List<SGTrade> GetPayBillList(int count,string orderby)
        {
            List<SGTrade> list = null;
            var sql = @"SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER 
                            where STATUS=21 order by CREATEDATE @orderby LIMIT 0,@count";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@orderby",MySqlDbType.VarChar,20),
                new MySqlParameter("@count",MySqlDbType.Int32)
            };
            paras[0].Value = orderby;
            paras[1].Value = count;
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

    }
}
