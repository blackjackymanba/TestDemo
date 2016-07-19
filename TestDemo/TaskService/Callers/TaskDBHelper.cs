using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Callers
{
    public class TaskDBHelper:testhelper
    {
        internal readonly string connStrName;
        public TaskDBHelper(string conntectionStringName)
        {
            connStrName = conntectionStringName;
        }
        public FundInHelper FundIn { get { return new FundInHelper("FIConnection"); } }
        public override SqlTransaction GetNewTrans(string connStrName)
        {
            return NewSqlServerTransaction(connStrName);
        }
        public override SqlTransaction NewSqlServerTransaction(string connectionName)
        {
            var conn = new SqlConnection("2222");
            conn.Open();
            return conn.BeginTransaction();
        }
    }
}
