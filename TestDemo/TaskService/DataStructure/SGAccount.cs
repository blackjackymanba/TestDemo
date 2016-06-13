using System;

namespace GPMGateway.Common.DataStructure
{
    public class SGAccount
    {
        public SGAccount()
        {
        }

        public SGAccount(string accountNo)
        {
            ID = accountNo;
            AccountID = accountNo;
        }
        public string AccountID;
        public string ID;

        public string Name;

        public string Address;

        public string ChargeOrganization;

        public DateTime CreateDate;

        public DateTime ModifyDate;
    }
}