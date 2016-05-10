using System.ComponentModel.DataAnnotations;
using GPMGateway.Common.IOObjectType;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public enum GWReturnCodes
    {
        SUCCESS,
        FAIL
    }

    public enum GWResultCodes
    {
        SUCCESS,
        TIMEOUT,
        FAIL,
        NODATA,
        BADINPUT
    }

    public enum GWErrorCodes
    {
        TIMEOUTABORT,
        SYSTEMERROR,
        GRIDTIMEOUT,
        GRIDERROR,
        ALREADYPAID,
        BADAMOUNT,
        BADDATA,
        ORDERNOTFOUND,
        CANNOTCANCEL,
        NOTPAID,
        WRONGOPENIDSAMEPAYMENT,
        WXERROR
    }

    public abstract class OutputObjectType : ObjectType
    {
        [Required]
        [EnumDataType(typeof(GWReturnCodes))]
        [JsonProperty("return_code")]
        public string ReturnCode = GWReturnCodes.SUCCESS.ToString();

        [MaxLength(128)]
        [JsonProperty("return_msg")]
        public string ReturnMessage;

        [EnumDataType(typeof(GWResultCodes))]
        [JsonProperty("result_code")]
        public string ResultCode = GWResultCodes.SUCCESS.ToString();

        [EnumDataType(typeof(GWErrorCodes))]
        [JsonProperty("err_code")]
        public string ErrorCode;

        [MaxLength(128)]
        [JsonProperty("err_desc")]
        public string ErrorDescription;

        [Required]
        [MaxLength(32)]
        [JsonProperty("platform_id")]
        public string PlatformId;
    }
}