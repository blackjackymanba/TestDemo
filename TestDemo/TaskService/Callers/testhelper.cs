using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Callers
{
    public class testhelper
    {
        public virtual SqlTransaction GetNewTrans(string connStrName)
        {
            return NewSqlServerTransaction(connStrName);
        }

        public virtual SqlTransaction NewSqlServerTransaction(string connectionName)
        {
            var conn = new SqlConnection("1234344");
            conn.Open();
            return conn.BeginTransaction();
        }
    }
}
