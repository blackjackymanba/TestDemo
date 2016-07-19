using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Callers
{
    public class FundInHelper :DBHelper
    {
        public FundInHelper(string conntectionStringName) : base(conntectionStringName)
        {
        }

        public List<string> GetFIPaymentOrder()
        {
            using (var t = NewSqlServerTransaction(""))
            {
                return null;
            }
        }
    }
}
