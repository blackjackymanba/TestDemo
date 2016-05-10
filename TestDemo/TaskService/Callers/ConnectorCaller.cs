using System;
using System.Threading.Tasks;

namespace GPMTaskService.Callers
{
    public abstract class ConnectorCaller : ICaller
    {
        public virtual void RecordTraffic(Guid gwRequestKey, string url, string responseData, string requestData)
        {
            Task.Run(() =>
            {
                try
                {
                    var reqKey = RecordRequest(url, requestData, gwRequestKey);
                    RecordResponse(responseData, reqKey);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        protected abstract Guid RecordRequest(string url, string requestData, Guid gwRequestKey);

        protected abstract void RecordResponse(string responseData, Guid sgcRequestKey);
    }
}