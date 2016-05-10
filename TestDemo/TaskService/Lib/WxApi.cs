using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace GPMGateway.Common.Lib
{
    public class WxApi
    {
        public static string MakeSign(SortedDictionary<string, object> m_values, string apiKey)
        {
            //转url格式
            string str = ToUrl(m_values);
            //在string后加入API KEY
            str += "&key=" + apiKey;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
                sb.Append(b.ToString("x2"));
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        public static bool CheckSign(SortedDictionary<string, object> m_values, string apiKey)
        {
            //如果没有设置签名，则跳过检测
            if (!m_values.ContainsKey("sign"))
                throw new Exception("WxPayData签名存在但不合法!");
            //如果设置了签名但是签名为空，则抛异常
            else if (m_values["sign"] == null || m_values["sign"].ToString() == "")
                throw new Exception("WxPayData签名存在但不合法!");
            //获取接收到的签名
            string return_sign = m_values["sign"].ToString();
            //在本地计算新的签名
            string cal_sign = MakeSign(m_values, apiKey);
            if (cal_sign == return_sign)
                return true;
            throw new Exception("WxPayData签名验证错误!");
        }

        public static SortedDictionary<string, object> FromXml(string xml, string apiKey)
        {
            var m_values = new SortedDictionary<string, object>();
            if (string.IsNullOrEmpty(xml))
                throw new Exception("将空的xml串转换为WxPayData不合法!");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }
            try
            {
                //2015-06-29 错误是没有签名
                if (m_values["return_code"].ToString() != "SUCCESS")
                    return m_values;
                CheckSign(m_values, apiKey);//验证签名,不通过会抛异常
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return m_values;
        }

        public static string ToXml(SortedDictionary<string, object> m_values)
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
                throw new Exception("WxPayData数据为空!");

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                    m_values[pair.Key] = string.Empty;
                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + XmlTextEncoder.Encode(pair.Key) + ">"
                        + XmlTextEncoder.Encode(pair.Value.ToString())
                        + "</" + XmlTextEncoder.Encode(pair.Key) + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + XmlTextEncoder.Encode(pair.Key) + ">"
                        + XmlTextEncoder.Encode(pair.Value.ToString())
                        + "</" + XmlTextEncoder.Encode(pair.Key) + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData字段数据类型错误!");
                    throw new Exception("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        public static string ToUrl(SortedDictionary<string, object> m_values)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                    m_values[pair.Key] = string.Empty;
                if (pair.Key != "sign" && !string.IsNullOrEmpty(pair.Value.ToString()))
                    buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }
    }
}