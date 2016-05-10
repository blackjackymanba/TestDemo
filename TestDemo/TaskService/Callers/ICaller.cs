using System;
using System.Net.Http;

namespace GPMTaskService.Callers
{
    public interface ICaller
    {
        void RecordTraffic(Guid gwRequestKey, string url, string responseData, string requestData);
    }
}