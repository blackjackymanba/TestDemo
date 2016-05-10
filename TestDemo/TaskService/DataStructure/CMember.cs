using System;

namespace GPMGateway.Common.DataStructure
{
    public class CMember
    {
        public CMember()
        {
        }

        public CMember(decimal cMemberCode)
        {
            MemberCode = cMemberCode;
        }

        public int ID;

        public decimal MemberCode;

        public Platform SourcePlatform;

        public int Status;

        public DateTime CreateDate;

        public DateTime ModifyDate;

        public string WxOpenID;

        public string UserName;
    }
}