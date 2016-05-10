using System;
namespace GPMGateway.Common.DataStructure
{
    public class FIOrder
    {
        public int ID;

        public string OutTradeID;

        public DateTime PayDate;

        public Guid gwRequestkey;

        public int status;
    }
}