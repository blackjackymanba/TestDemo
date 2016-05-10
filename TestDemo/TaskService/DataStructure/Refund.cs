using System;
using System.Collections.Generic;
using System.Text;

namespace GPMGateway.Common.DataStructure
{
     public class Refund
    {
        public int refund_order_id;
        public int trade_oredr_id;
        public int payment_order_id;
        public int status;
        public int refund_amount;
        public string refund_id;
    }
}
