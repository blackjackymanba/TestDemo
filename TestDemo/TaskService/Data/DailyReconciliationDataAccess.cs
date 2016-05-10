using GPMGateway.Common.DataStructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace GPMTaskService.Data
{
    public class DailyReconciliationDataAccess
    {
        private DbTransContext dbTrans;
        public DailyReconciliationDataAccess()
        {
            dbTrans = new DbTransContext();
        }
        public DailyReconciliationDataAccess(DbTransContext _dbTrans)
        {
            dbTrans = _dbTrans;
        }

        public int? ImportReconciliationFile(string filename, int ChannelId, DateTime businessDate, string fileContent, BMember bMember)
        {
            var sql = "INSERT INTO RECONCILE_IMPORT_FILE(FILENAME,CHANNELID,STATUS,BUSINESSDATE,UPLOADDATE,FILECONTENT,BMEMBERCODE) VALUE(@filename,@channelid,@status,@buisDate,@uploadDate,@fileContent,@bMemberCode);SELECT LAST_INSERT_ID();";
            int? file_id = null;
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.FOConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@channelid", ChannelId);
                    cmd.Parameters.AddWithValue("@status", 1);
                    cmd.Parameters.AddWithValue("@buisDate", businessDate);
                    cmd.Parameters.AddWithValue("@uploadDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@fileContent", fileContent);
                    cmd.Parameters.AddWithValue("@bMemberCode", bMember.MemberCode);
                    conn.Open();
                    file_id = cmd.ExecuteScalar() as int?;
                    conn.Close();
                }
            }
            return file_id;
        }

        public int? ImportReconciliationResultFile(string filename, int count, int amount, string fileContent, BMember bMember)
        {
            var sql = "INSERT INTO RECONCILE_IMPORT_FILE(FILENAME,FILETYPE,ALLCOUNT,ALLAMOUNT,CHANNELID,MODIFYDATE,GENERATE_TIME,IMPORT_TIME,FILECONTENT,BMEMBERCODE,BUSITYPE) VALUE(@filename,1,@count,@amount,1,@uploadDate,@uploadDate,@uploadDate,@fileContent,@bMemberCode,1);SELECT LAST_INSERT_ID();";
            int? file_id = null;
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.FOConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@count", count);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@uploadDate", DateTime.Now.AddDays(-1));
                    cmd.Parameters.AddWithValue("@fileContent", fileContent);
                    cmd.Parameters.AddWithValue("@bMemberCode", bMember.MemberCode);
                    conn.Open();
                    file_id = cmd.ExecuteScalar() as int?;
                    conn.Close();
                }
            }
            return file_id;
        }

        public bool IsRCRFileExist(string filename)
        {
            var sql = "SELECT * FROM FOUNDOUT.RECONCILE_RESULT_FILE WHERE FILENAME=@filename;";
            var exists = false;
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.FOConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        exists = dr.HasRows;
                    }
                    conn.Close();
                }
            }
            return exists;
        }

        public bool ImportReconciliationRecord(DateTime tradeTime, string channeSeqNo, int amount, string tradeSeqNo, byte channelBusiType, int fileId)
        {
            var sql = "INSERT INTO RECONCILE_IMPORT_RECORD(TRADETIME,CHANNELSEQ,CHANNELAMOUNT,TRADESEQ,CHANNELID,CHANNELBUSITYPE,FILE_ID) VALUE(@TRADETIME,@CHANNELSEQ,@CHANNELAMOUNT,@TRADESEQ,@CHANNELID,@CHANNELBUSITYPE,@FILE_ID);";
            var noErr = true;
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.FOConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TRADETIME", tradeTime);
                    cmd.Parameters.AddWithValue("@CHANNELSEQ", channeSeqNo);
                    cmd.Parameters.AddWithValue("@CHANNELAMOUNT", amount);
                    cmd.Parameters.AddWithValue("@TRADESEQ", tradeSeqNo);
                    cmd.Parameters.AddWithValue("@CHANNELID", 1);
                    cmd.Parameters.AddWithValue("@CHANNELBUSITYPE", channelBusiType);
                    cmd.Parameters.AddWithValue("@FILE_ID", fileId);
                    conn.Open();
                    noErr = cmd.ExecuteNonQuery() == 1;
                    conn.Close();
                }
            }
            return noErr;
        }

        public FIPaymentOrder GetFIPaymentOrder(string wx_trans_id)
        {
            FIPaymentOrder fi = null;
            var sql = @"SELECT PAYMENT_ORDER_ID,STATUS,TRADE_ORDER_ID FROM FOUNDIN.PAYMENT_ORDER where CHANNELORDERID=@wx_trans_id FOR UPDATE";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@wx_trans_id",MySqlDbType.VarChar,128)
            };
            paras[0].Value = wx_trans_id;
            var dr = dbTrans.ExecuteReader(sql, paras);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    fi = new FIPaymentOrder();
                    fi.ID = dr.GetInt32(0);
                    fi.Status = dr.GetInt16(1);
                    fi.TradeOrder = new FIOrder();
                    fi.TradeOrder.ID = dr.GetInt32(2);
                }
            }

            dr.Close();

            return fi;
        }
    }
}
