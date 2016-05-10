using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public abstract class ObjectType : IObjectType
    {
        [Required]
        [MaxLength(32)]
        [JsonProperty("nonce_str")]
        public string NonceString;

        [Required]
        [MaxLength(32)]
        [JsonProperty("sign")]
        public string Sign;

        public virtual string ToNoSignJson()// 将当前对象的sign字符串清空
        {
            var nosignObj = this;
            nosignObj.Sign = string.Empty;
            return JsonConvert.SerializeObject(nosignObj);//返回一个没有sign的对象
        }

        public virtual string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N").ToLowerInvariant();
        }

        public virtual string MakeSign(string ApiKey)//sign构建函数
        {
            string str = ToNoSignJson() + (string.IsNullOrEmpty(ApiKey) ? string.Empty : ApiKey);// 拼接验证字符串
            var bs = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str));//MD5处理拼接后的字符串
            return Convert.ToBase64String(bs).ToLowerInvariant();
        }

        public virtual bool CheckSign(string ApiKey)// 判断sign字符串是否为空函数
        {
            if (!string.IsNullOrEmpty(Sign))//如果没有sign
                return Sign == MakeSign(ApiKey);//调用sign构建函数
            return false;
        }
    }
}