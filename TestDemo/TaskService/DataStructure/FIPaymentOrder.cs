using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPMGateway.Common.DataStructure
{
    public class FIPaymentOrder
    {
        public int ID;

        public int Status;

        public FIOrder TradeOrder;

        public string PaymentChannelOrderID;

        public DateTime PaymentCompletionDate;

        public Guid ChannelRequestKey;

        public DateTime CreateDate;

        public DateTime ModifyDate;

        public string PaymentChannelPrepayID;

        public int Amount;
    }
}
