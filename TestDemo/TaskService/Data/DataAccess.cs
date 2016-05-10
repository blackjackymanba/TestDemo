using GPMGateway.Common.DataStructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace GPMTaskService.Data
{
    public class DataAccess
    {
        private DbTransContext dbTrans;
        public DataAccess()
        {
            dbTrans = new DbTransContext();
        }
        public DataAccess(DbTransContext _dbTrans)
        {
            dbTrans = _dbTrans;
        }

        public string GetTransitionID(int tradeOrderId)
        {
            var sql = "SELECT CHANNELORDERID FROM foundin.payment_order where TRADE_ORDER_ID=@toid;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@toid",MySqlDbType.Int32)
            };
            paras[0].Value = tradeOrderId;
            var chanelorderid = dbTrans.ExecuteScalar(sql, paras);

            var tid = string.Empty;
            if (chanelorderid != null)
                tid = chanelorderid.ToString();
            return tid;
        }
        public string GetOrgNo(BMember bm)
        {
            var sql = "SELECT SGRCVORGNO FROM STATEGRID.ORGTERMINAL WHERE BMEMBERID = @bmid;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@bmid",MySqlDbType.Int32)
            };
            paras[0].Value = bm.ID;
            var orgno = dbTrans.ExecuteScalar(sql, paras);

            var tid = string.Empty;
            if (orgno != null)
                tid = orgno.ToString();
            return tid;
        }
        public string GetSettingForPlatform(Platform p, string settingName)
        {
            var sql = "SELECT SETTINGSVALUE FROM INFRASTRUCTURE.PLATFORM_SETTINGS WHERE PLATFORMID=@pid AND SETTINGSNAME=@sName;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@pid",MySqlDbType.VarChar,40),
                new MySqlParameter("@sName",MySqlDbType.VarChar,40)
            };
            paras[0].Value = p.PlatformId;
            paras[1].Value = settingName;
            var sValue = dbTrans.ExecuteScalar(sql, paras);

            var sv = string.Empty;
            if (sValue != null)
                sv = sValue.ToString();
            return sv;
        }
        public SGTrade GetSGPaymentOrder(int pid)
        {
            SGTrade sg = null;
            var sql = @"SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER 
                            where STATUS=21 and ORDER_ID=@pid FOR UPDATE";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@pid",MySqlDbType.Int32)
            };
            paras[0].Value = pid;
            var dr = dbTrans.ExecuteReader(sql, paras);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    sg = new SGTrade();
                    sg.ID = dr.GetInt32(0);
                    sg.Status = dr.GetInt16(4);
                    sg.Type = dr.GetInt16(6);
                    sg.SGUserAccount = new SGAccount(dr.GetString(1));
                    sg.ChargeOrganization = dr.GetString(3);
                    sg.SourcePlatform = new Platform();
                    sg.SourcePlatform.PlatformId = dr.GetString(5);
                    sg.FoundInOrder = new FIOrder();
                    sg.FoundInOrder.ID = dr.GetInt32(2);
                    sg.Amount = dr.IsDBNull(7) ? 0 : dr.GetInt32(7);
                    sg.Penalty = dr.IsDBNull(8) ? 0 : dr.GetInt32(8);
                }
            }

            dr.Close();

            return sg;
        }

        public FIPaymentOrder GetFIPaymentOrder(int payment_order_id)
        {
            FIPaymentOrder fi = null;
            var sql = @"SELECT PAYMENT_ORDER_ID,STATUS,TRADE_ORDER_ID FROM FOUNDIN.PAYMENT_ORDER where PAYMENT_ORDER_ID=@payment_order_id FOR UPDATE";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@payment_order_id",MySqlDbType.Int32)
            };
            paras[0].Value = payment_order_id;
            var dr = dbTrans.ExecuteReader(sql, paras);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    fi = new FIPaymentOrder();
                    fi.ID = dr.GetInt32(0);
                    fi.Status = dr.GetInt16(1);
                }
            }

            dr.Close();

            return fi;
        }
        public FIPaymentOrder GetFIPaymentOrderByTradeOrderID(int tradeOrderId)
        {
            FIPaymentOrder fi = null;
            var sql = @"SELECT PAYMENT_ORDER_ID,STATUS,AMOUNT,TRADE_ORDER_ID FROM FOUNDIN.PAYMENT_ORDER where TRADE_ORDER_ID=@tradeOrderId FOR UPDATE";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@tradeOrderId",MySqlDbType.Int32)
            };
            paras[0].Value = tradeOrderId;
            var dr = dbTrans.ExecuteReader(sql, paras);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    fi = new FIPaymentOrder();
                    fi.ID = dr.GetInt32(0);
                    fi.Status = dr.GetInt16(1);
                    fi.Amount = dr.GetInt32(2);
                }
            }

            dr.Close();

            return fi;
        }
        public SGTrade GetSGTrade(int fi_order_id)
        {
            var sql = "SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER WHERE FOUNDINORDERID=@fi_order_id;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@fi_order_id",MySqlDbType.Int32)
            };
            paras[0].Value = fi_order_id;

            var dr = dbTrans.ExecuteReader(sql, paras);

            SGTrade model = null;
            if (dr.HasRows && dr.Read())
            {
                model = new SGTrade();
                model.ID = dr.GetInt32(0);
                model.Status = dr.GetInt16(4);
                model.Type = dr.GetInt16(6);
                model.SGUserAccount = new SGAccount(dr.GetString(1));
                model.FoundInOrder = new FIOrder();
                model.FoundInOrder.ID = dr.GetInt32(2);
                model.ChargeOrganization = dr.GetString(3);
                model.SourcePlatform = new Platform();
                model.SourcePlatform.PlatformId = dr.GetString(5);
                model.Amount = dr.IsDBNull(7) ? 0 : dr.GetInt32(7);
                model.Penalty = dr.IsDBNull(8) ? 0 : dr.GetInt32(8);
            }

            dr.Close();

            if (model != null)
            {
                model.FoundInOrder = GetFIOrder(model.FoundInOrder.ID);
                model.SourcePlatform = GetPlatform(model.SourcePlatform.PlatformId);
            }

            return model;
        }

        public SGTrade GetSGTrade(Guid tKey)
        {
            var sql = "SELECT ORDER_ID,PAYSGACCOUNT,FOUNDINORDERID,CHARGEORGNO,STATUS,SOURCEPLATFORM,TYPE,AMOUNT,PENALTY FROM STATEGRID.PAYMENT_ORDER WHERE TRADEORDERKEY=@tkey;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@tkey",MySqlDbType.Binary,16)
            };
            paras[0].Value = tKey.ToByteArray();

            var dr = dbTrans.ExecuteReader(sql, paras);

            SGTrade model = null;
            if (dr.HasRows && dr.Read())
            {
                model = new SGTrade();
                model.ID = dr.GetInt32(0);
                model.Key = tKey;
                model.Status = dr.GetInt16(4);
                model.Type = dr.GetInt16(6);
                model.SGUserAccount = new SGAccount(dr.GetString(1));
                model.FoundInOrder = new FIOrder();
                model.FoundInOrder.ID = dr.GetInt32(2);
                model.ChargeOrganization = dr.GetString(3);
                model.SourcePlatform = new Platform();
                model.SourcePlatform.PlatformId = dr.GetString(5);
                model.Amount = dr.IsDBNull(7) ? 0 : dr.GetInt32(7);
                model.Penalty = dr.IsDBNull(8) ? 0 : dr.GetInt32(8);
            }

            dr.Close();

            if (model != null)
            {
                model.FoundInOrder = GetFIOrder(model.FoundInOrder.ID);
                model.SourcePlatform = GetPlatform(model.SourcePlatform.PlatformId);
            }

            return model;
        }
        public FIOrder GetFIOrder(int oID)
        {
            var sql = "SELECT ORDER_ID,OUTTRADEID,PAYMENTCOMPLETIONDATE,RESQUESTKEY,STATUS FROM FOUNDIN.TRADE_ORDER WHERE ORDER_ID=@oid FOR UPDATE;";
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



        public Platform GetPlatform(string pID)
        {
            var sql = "SELECT PLATFORM_ID,CHINESENAME,WEIXINPAYMCHID,WEIXINAPPID,WEIXINAPPKEY,CLIENTCERT,APPKEY,LINK1,LINK2,LINK3,LINK4,LINK5 FROM INFRASTRUCTURE.PLATFORM WHERE PLATFORM_ID=@pid;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@pid",MySqlDbType.VarChar,40)
            };
            paras[0].Value = pID;
            var dr = dbTrans.ExecuteReader(sql, paras);
            Platform model = null;
            if (dr.HasRows && dr.Read())
            {
                model = new Platform();
                model.PlatformId = dr.GetString(0);
                model.Name = dr.GetString(1);
                model.WechatPayMerchantId = dr.GetString(2);
                model.WechatPayAppId = dr.GetString(3);
                model.WechatPayAppKey = dr.GetString(4);
                model.AppKey = dr.GetString(6);
                long len = dr.GetBytes(5, 0, null, 0, 0);
                var buffer = new byte[len];
                len = dr.GetBytes(5, 0, buffer, 0, (int)len);
                model.ClientCertificateRaw = buffer;
                model.SGGetAmount = dr.GetString(7);
                model.SGPayBill = dr.GetString(8);
                model.SGPaybackBill = dr.GetString(9);
                model.SGReconcilation = dr.GetString(10);
                model.SGFTPRec = dr.GetString(11);
            }
            dr.Close();
            return model;
        }

        public List<BMember> GetBMembers()
        {
            var sql = "SELECT TB.MEMBER_ID,TB.MEMBERCODE,TB.SOURCEPLATFORM,TB.STATUS,TB.CREATEDATE,TB.MODIFYDATE,TB.MEMBERNAME,TB.WEIXINAPPID,TB.WEIXINPAYMCHID FROM STATEGRID.ORGTERMINAL TA INNER JOIN MEMBERACCOUNT.BUSINESS_MEMBER TB ON TB.MEMBER_ID=TA.BMEMBERID WHERE TA.STATUS=1 AND TB.STATUS=1";
            var models = new List<BMember>();
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.MAConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                            while (dr.Read())
                            {
                                var model = new BMember(dr.GetInt32(0));
                                model.MemberCode = dr.GetDecimal(1);
                                model.SourcePlatform = new Platform();
                                model.SourcePlatform.PlatformId = dr.GetString(2);
                                model.Status = dr.GetInt16(3);
                                model.CreateDate = dr.GetDateTime(4);
                                model.ModifyDate = dr.GetDateTime(5);
                                model.Name = dr.GetString(6);
                                model.WxAppID = dr.GetString(7);
                                model.WxPayMerchantID = dr.GetString(8);
                                model.SourcePlatform = GetPlatform(model.SourcePlatform.PlatformId);
                            }
                        dr.Close();
                    }
                    conn.Close();
                }
            }
            return models;
        }

        public BMember GetBMember(string chargeOrgNo)
        {
            var sql = "SELECT TB.MEMBER_ID,TB.MEMBERCODE,TB.SOURCEPLATFORM,TB.STATUS,TB.CREATEDATE,TB.MODIFYDATE,TB.MEMBERNAME,TB.WEIXINAPPID,TB.WEIXINPAYMCHID,TB.WEIXINCERTIFICATION,TB.WEIXINAPPSECRET FROM STATEGRID.ORGTERMINAL TA INNER JOIN MEMBERACCOUNT.BUSINESS_MEMBER TB ON TB.MEMBER_ID=TA.BMEMBERID WHERE TB.STATUS=1 AND TA.STATUS=1 AND TA.SGRCVORGNO=@rcvorg;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@rcvorg",MySqlDbType.VarChar,40)
            };
            paras[0].Value = chargeOrgNo;
            var dr = dbTrans.ExecuteReader(sql, paras);
            BMember model = null;
            if (dr.HasRows && dr.Read())
            {
                model = new BMember(dr.GetInt32(0));
                model.MemberCode = dr.GetDecimal(1);
                model.SourcePlatform = new Platform();
                model.SourcePlatform.PlatformId = dr.GetString(2);
                model.Status = dr.GetInt16(3);
                model.CreateDate = dr.GetDateTime(4);
                model.ModifyDate = dr.GetDateTime(5);
                model.Name = dr.GetString(6);
                model.WxAppID = dr.GetString(7);
                model.WxPayMerchantID = dr.GetString(8);
                long len = dr.GetBytes(9, 0, null, 0, 0);
                var buffer = new byte[len];
                len = dr.GetBytes(9, 0, buffer, 0, (int)len);
                model.ClientCertificateRaw = buffer;
                model.WxPayAppKey = dr.GetString(10);
            }
            dr.Close();
            if (model != null)
                model.SourcePlatform = GetPlatform(model.SourcePlatform.PlatformId);
            return model;
        }

        public int SetSGTradeStatus(int id, int status)
        {
            var sql = "UPDATE STATEGRID.PAYMENT_ORDER SET STATUS = @stat,MODIFYDATE=@date WHERE ORDER_ID = @oid;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@stat",MySqlDbType.Int32),
                new MySqlParameter("@oid",MySqlDbType.Int32),
                new MySqlParameter("@date",MySqlDbType.DateTime)
            };
            paras[0].Value = status;
            paras[1].Value = id;
            paras[2].Value = DateTime.Now;

            var rows = dbTrans.ExecuteNonQuery(sql, paras);
            return rows;
        }
        public int SetRefundStatus(int refund_order_id, int status, string refund_id)
        {
            var sql = "UPDATE FOUNDIN.REFUND_ORDER SET STATUS = @stat,CHANNELORDERID=@refund_id,MODIFYDATE=@date WHERE REFUND_ORDER_ID = @refund_order_id;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@stat",MySqlDbType.Int32),
                new MySqlParameter("@refund_order_id",MySqlDbType.Int32),
                new MySqlParameter("@refund_id",MySqlDbType.VarChar,128),
                new MySqlParameter("@date",MySqlDbType.DateTime)
            };
            paras[0].Value = status;
            paras[1].Value = refund_order_id;
            paras[2].Value = refund_id;
            paras[3].Value = DateTime.Now;

            var rows = dbTrans.ExecuteNonQuery(sql, paras);
            return rows;
        }
        public int SetRefundStatus(int refund_order_id, int status)
        {
            var sql = "UPDATE FOUNDIN.REFUND_ORDER SET STATUS = @stat,MODIFYDATE=@date WHERE REFUND_ORDER_ID = @refund_order_id;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@stat",MySqlDbType.Int32),
                new MySqlParameter("@refund_order_id",MySqlDbType.Int32),
                new MySqlParameter("@date",MySqlDbType.DateTime)
            };
            paras[0].Value = status;
            paras[1].Value = refund_order_id;
            paras[2].Value = DateTime.Now;

            var rows = dbTrans.ExecuteNonQuery(sql, paras);
            return rows;
        }
        public int SetTradeOrderStatus(int orderId, int status)
        {
            var sql = "UPDATE FOUNDIN.TRADE_ORDER SET STATUS = @stat,MODIFYDATE=@date WHERE ORDER_ID = @orderId;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@stat",MySqlDbType.Int32),
                new MySqlParameter("@orderId",MySqlDbType.Int32),
                new MySqlParameter("@date",MySqlDbType.DateTime)
            };
            paras[0].Value = status;
            paras[1].Value = orderId;
            paras[2].Value = DateTime.Now;

            var rows = dbTrans.ExecuteNonQuery(sql, paras);
            return rows;
        }
        public int SetPaymentOrderStatus(int pid, int status)
        {
            var sql = "UPDATE FOUNDIN.PAYMENT_ORDER SET STATUS = @stat,MODIFYDATE=@date WHERE PAYMENT_ORDER_ID = @pid;";
            var paras = new MySqlParameter[] {
                new MySqlParameter("@stat",MySqlDbType.Int32),
                new MySqlParameter("@pid",MySqlDbType.Int32),
                new MySqlParameter("@date",MySqlDbType.DateTime)
            };
            paras[0].Value = status;
            paras[1].Value = pid;
            paras[2].Value = DateTime.Now;

            var rows = dbTrans.ExecuteNonQuery(sql, paras);
            return rows;
        }
    }
}
