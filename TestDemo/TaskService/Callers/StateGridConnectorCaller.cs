using System;
using System.Net.Http;
using GPMGateway.Common.IOObjectType;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace GPMTaskService.Callers
{
    public class SGConnectorResponse
    {
        public int flg;

        public string result;
    }

    public class StateGridConnectorCaller : ConnectorCaller
    {
        private static readonly StateGridConnectorCaller _caller;

        static StateGridConnectorCaller()
        {
            _caller = new StateGridConnectorCaller();
        }

        private StateGridConnectorCaller()
        {
        }

        public static StateGridConnectorCaller Caller { get { return _caller; } }

        private TOutput CallSGConnector<TInput, TOutput>(TInput input, string requestUrl, Guid gwRequestKey)
            where TInput : ObjectType, new()
            where TOutput : SGOutputObjectType, new()
        {
            var output = new TOutput();
            var httpClient = new HttpClient();
            try
            {
                var t = JsonConvert.SerializeObject(input);
                var httpResT = httpClient.PostAsync(requestUrl, new StringContent(t));
                httpResT.Wait();
                var httpRes = httpResT.Result;
                var o = httpRes.Content.ReadAsStringAsync();
                o.Wait();
                RecordTraffic(gwRequestKey, requestUrl, o.Result, t);
                output = JsonConvert.DeserializeObject<TOutput>(o.Result);
            }
            catch (Exception e)
            {
                output.ReturnCode = GWReturnCodes.FAIL.ToString();
                output.ReturnMessage = e.Message;
            }
            return output;
        }

        public GetAmountOutput GetAmount(GetAmountInput input, string requestUrl, Guid gwRequestKey)
        {
            return CallSGConnector<GetAmountInput, GetAmountOutput>(input, requestUrl, gwRequestKey);
        }

        public RecFileOutput Reconciliation(RecFileInput input, string requestUrl, Guid gwRequestKey)
        {
            return CallSGConnector<RecFileInput, RecFileOutput>(input, requestUrl, gwRequestKey);
        }

        public PayBOutput PayBill(PayBInput input, string requestUrl, Guid gwRequestKey)
        {
            return CallSGConnector<PayBInput, PayBOutput>(input, requestUrl, gwRequestKey);
        }

        public PaybackOutput PaybackBill(PaybackInput input, string requestUrl, Guid gwRequestKey)
        {
            return CallSGConnector<PaybackInput, PaybackOutput>(input, requestUrl, gwRequestKey);
        }
        protected override Guid RecordRequest(string url, string requestData, Guid gwRequestKey)
        {
            var key = Guid.NewGuid();
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.SGConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(@"INSERT INTO STATEGRID.CONNECTOR_REQUEST (REQUESTURL,REQUESTCONTENT,GATEWAYREQUESTKEY,REQUESTDATE,REQUEST_KEY) VALUES (@url,@rData,@gwKey,@creation,@key);", conn))
                {
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@rData", requestData);
                    cmd.Parameters.AddWithValue("@gwKey", gwRequestKey.ToByteArray());
                    cmd.Parameters.AddWithValue("@creation", DateTime.Now);
                    cmd.Parameters.AddWithValue("@key", key.ToByteArray());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            return key;
        }

        protected override void RecordResponse(string responseData, Guid sgcRequestKey)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GPMTaskService.Properties.Settings.SGConnectionConnectionString"].ConnectionString))
            {
                using (var cmd = new MySqlCommand(@"INSERT INTO STATEGRID.CONNECTOR_RESPONSE (RESPONSECONTENT,RESPONSEDATE,REQUESTKEY) VALUES (@content,@creation,@key);", conn))
                {
                    cmd.Parameters.AddWithValue("@content", responseData);
                    cmd.Parameters.AddWithValue("@creation", DateTime.Now);
                    cmd.Parameters.AddWithValue("@key", sgcRequestKey.ToByteArray());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
        }
    }
}