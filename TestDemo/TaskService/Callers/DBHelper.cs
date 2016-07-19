using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Callers
{
    public class DBHelper: testhelper
    {
        internal readonly string connStrName;
        public DBHelper(string conntectionStringName)
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
            var conn = new SqlConnection("33333");
            conn.Open();
            return conn.BeginTransaction();
        }

    }
}
