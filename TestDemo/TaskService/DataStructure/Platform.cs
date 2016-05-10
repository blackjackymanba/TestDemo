using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace GPMGateway.Common.DataStructure
{
    public class Platform
    {
        public int ID;

        public string PlatformId;

        public string Name;

        public string AppKey;

        public byte[] ClientCertificateRaw;

        public string WechatPayMerchantId;

        public string WechatPayAppId;

        public string WechatPayAppKey;

        public string SGGetAmount;

        public string SGFTPRec;

        public string SGPayBill;

        public string SGReconcilation;

        public string SGPaybackBill;

        [JsonIgnore]
        public X509Certificate2 ClientCertificate
        {
            get
            {
                return new X509Certificate2(ClientCertificateRaw);
            }
        }
    }
}