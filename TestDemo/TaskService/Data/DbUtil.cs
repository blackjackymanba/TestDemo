
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;

namespace GPMTaskService.Data
{
    public class DbUtil
    {
        public static string DefaultConnStr = ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.SGConnectionConnectionString"].ConnectionString;
        public static CommandType DefaultCmdType = CommandType.StoredProcedure;

        private static void PrepareCommand(MySqlConnection conn, MySqlCommand cmd, CommandType cmdType, params MySqlParameter[] paras)
        {
            cmd.CommandType = cmdType;
            cmd.Connection = conn;

            if (paras != null && paras.Length > 0)
            {
                foreach (var m in paras)
                {
                    cmd.Parameters.Add(m);
                }
            }
        }
        private static T ExecuteCommand<T>(string connStr, MySqlCommand cmd, CommandType cmdType, Func<T> func, params MySqlParameter[] paras)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                PrepareCommand(conn, cmd, cmdType, paras);
                return func();
            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                if (func.Target.ToString() != "ExecuteReader") conn.Close();
            }
        }
        private T ExecuteReader<T>(string connStr, MySqlCommand cmd, CommandType cmdType, Func<MySqlDataReader, T> func, params MySqlParameter[] paras)
        {
            try
            {
                var dr = ExecuteCommand(connStr, cmd, cmdType, cmd.ExecuteReader, paras);
                return func(dr);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public static object ExecuteScalar(string connStr, string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(connStr, cmd, cmdType, cmd.ExecuteScalar, paras);
        }
        public static object ExecuteScalar(string cmdText, params MySqlParameter[] paras)
        {
            return ExecuteScalar(DefaultConnStr, cmdText, DefaultCmdType, paras);
        }

        public static int ExecuteNonQuery(string connStr, string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(connStr, cmd, cmdType, cmd.ExecuteNonQuery, paras);
        }
        public static int ExecuteNonQuery(string cmdText, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(DefaultConnStr, cmd, DefaultCmdType, cmd.ExecuteNonQuery, paras);
        }
    }

    public class DbTransContext
    {
        public static string DefaultConnStr = ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.SGConnectionConnectionString"].ConnectionString;
        public static CommandType DefaultCmdType = CommandType.Text;

        private MySqlConnection conn;
        private bool isTransBegin = false;
        //private MySqlTransaction trans = null;

        public DbTransContext(string _conn, bool isBeginTrans = true)
        {
            conn = new MySqlConnection(_conn);
            if (isBeginTrans) Begin();
        }
        public DbTransContext(bool isBeginTrans = true) : this(DefaultConnStr, isBeginTrans)
        {

        }

        #region 事务执行方法
        public void Begin()
        {
            conn.Open();
            if (conn.State != ConnectionState.Open)
                throw new InvalidOperationException();
            if (isTransBegin)
                throw new InvalidOperationException();
            MySqlCommand cmd = new MySqlCommand("BEGIN WORK", conn);
            cmd.ExecuteNonQuery();
            isTransBegin = true;
        }
        public void Close()
        {
            if ((conn != null && conn.State == ConnectionState.Open) && isTransBegin)
                Rollback();
            if (conn != null) conn.Close();
        }
        public void Commit()
        {
            if (conn == null || (conn.State != ConnectionState.Open))
                throw new InvalidOperationException("Connection must be valid and open to commit transaction");
            if (!isTransBegin)
                throw new InvalidOperationException("Transaction has already been committed or is not pending");
            MySqlCommand cmd = new MySqlCommand("COMMIT WORK", conn);
            cmd.ExecuteNonQuery();
            isTransBegin = false;
        }
        public void Rollback()
        {
            if (conn == null || (conn.State != ConnectionState.Open))
                throw new InvalidOperationException("Connection must be valid and open to rollback transaction");
            if (!isTransBegin)
                throw new InvalidOperationException("Transaction has already been rolled back or is not pending");
            MySqlCommand cmd = new MySqlCommand("ROLLBACK WORK", conn);
            cmd.ExecuteNonQuery();
            isTransBegin = false;
        }
        #endregion

        #region MySqlCommand 公共执行方法

        private void PrepareCommand(MySqlCommand cmd, CommandType cmdType, params MySqlParameter[] paras)
        {
            cmd.Connection =conn;
            cmd.CommandType = cmdType;
            //cmd.Transaction = trans;

            if (paras != null && paras.Length > 0)
            {
                foreach (var m in paras)
                {
                    cmd.Parameters.Add(m);
                }
            }
        }
        private T ExecuteCommand<T>(MySqlCommand cmd, CommandType cmdType, Func<T> func, params MySqlParameter[] paras)
        {
            try
            {
                PrepareCommand(cmd, cmdType, paras);
                return func();
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        private T ExecuteReader<T>(Func<MySqlDataReader, T> func, MySqlDataReader dr)
        {
            try
            {
                return func(dr);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dr.Close();
            }
        }
        #endregion

        #region DataReader 反射转换为 List<T>

        private static bool IsDBNull(object obj)
        {
            return obj == null;
        }
        private static object HackType(object value, Type conversionType)
        {
            if (value == null) return null;
            if (value.ToString() == string.Empty) return null;

            if (conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null) return null;
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            return Convert.ChangeType(value, conversionType);
        }
        private static IList<T> GetList<T>(MySqlDataReader reader, bool isOne)
        {
            var list = new List<T>();
            while (reader.Read())
            {
                T RowInstance = Activator.CreateInstance<T>();//动态创建数据实体对象

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = typeof(T).GetProperty(reader.GetName(i));
                    if (property == null)
                    {
                        throw new Exception(string.Format("类({0})不存在属性({1})", typeof(T).Name, reader.GetName(i)));
                    }
                    if (!property.CanWrite)
                    {
                        continue;
                    }
                    try
                    {
                        if (!IsDBNull(reader.GetValue(i)))
                        {
                            property.SetValue(RowInstance, HackType(reader.GetValue(i), property.PropertyType), null);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(RowInstance);

                if (isOne) break;
            }
            return list;
        }
        private static IList<T> GetList<T>(MySqlDataReader reader)
        {
            return GetList<T>(reader, false);
        }
        private static T GetEntity<T>(MySqlDataReader reader)
        {
            return GetList<T>(reader, true).FirstOrDefault();
        }

        #endregion

        public object ExecuteScalar(string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(cmd, cmdType, cmd.ExecuteScalar, paras);
        }
        public object ExecuteScalar(string cmdText, params MySqlParameter[] paras)
        {
            return ExecuteScalar(cmdText, DefaultCmdType, paras);
        }

        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(cmd, cmdType, cmd.ExecuteNonQuery, paras);
        }
        public int ExecuteNonQuery(string cmdText, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(cmd, DefaultCmdType, cmd.ExecuteNonQuery, paras);
        }

        public MySqlDataReader ExecuteReader(string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var cmd = new MySqlCommand(cmdText);
            return ExecuteCommand(cmd, cmdType, cmd.ExecuteReader, paras);
        }
        public MySqlDataReader ExecuteReader(string cmdText, params MySqlParameter[] paras)
        {
            return ExecuteReader(cmdText, DefaultCmdType, paras);
        }

        public T ExcuteToEntity<T>(string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var dr = ExecuteReader(cmdText, cmdType, paras);
            return ExecuteReader(GetEntity<T>, dr);
        }
        public T ExcuteToEntity<T>(string cmdText, params MySqlParameter[] paras)
        {
            return ExcuteToEntity<T>(cmdText, DefaultCmdType, paras);
        }

        public IList<T> ExcuteToEnumerable<T>(string cmdText, CommandType cmdType, params MySqlParameter[] paras)
        {
            var dr = ExecuteReader(cmdText, cmdType, paras);
            return ExecuteReader(GetList<T>, dr);
        }
        public IList<T> ExcuteToEnumerable<T>(string cmdText, params MySqlParameter[] paras)
        {
            return ExcuteToEnumerable<T>(cmdText, DefaultCmdType, paras);
        }
    }
}
