using System;

namespace GPMGateway.Common.DataStructure
{
    public class SGTrade
    {
        public int ID;

        public Guid Key;

        public SGAccount SGUserAccount;

        public string ChargeOrganization;

        public Int16 Status;

        public DateTime CreateDate;

        public DateTime ModifyDate;

        public Platform SourcePlatform;

        public string PayMonth;

        public FIOrder FoundInOrder;

        public FOOrder FoundOutOrder;

        public Int16 Type;

        public int Amount;

        public int Penalty;

        public int OweAmount;

        public string PayBillSeqNo;

        public string CancelBillSeqNo;

        public CMember Payer;
    }
}