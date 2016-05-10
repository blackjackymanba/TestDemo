using System;

namespace GPMGateway.Common.DataStructure
{
    public class BMember
    {
        public BMember()
        {
        }

        public BMember(int bMemberID)
        {
            ID = bMemberID;
        }

        //MEMBER_ID int(11) AI PK
        //MEMBERCODE decimal(16,0)
        //SOURCEPLATFORM int(11)
        //TYPE tinyint(4)
        //STATUS tinyint(4)
        //CREATEDATE datetime
        //MODIFYDATE datetime
        //CREATEUSER varchar(40)
        //MODIFYUSER varchar(40)
        //ACCOUNTCARD varchar(40)
        //AREAFAMILY varchar(40)
        //AREAFAMILYNAME varchar(200)
        //MEMBERNAME varchar(80)
        //WEIXINAPPID varchar(32)
        //WEIXINPAYMCHID varchar(32)
        public int ID;

        public decimal MemberCode;

        public Platform SourcePlatform;

        public int Status;

        public DateTime CreateDate;

        public DateTime ModifyDate;

        public string Name;

        public string WxAppID;

        public string WxPayMerchantID;

        public string WxPayAppKey;

        public byte[] ClientCertificateRaw;
    }
}