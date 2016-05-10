namespace GPMGateway.Common.IOObjectType
{
    public interface IObjectType
    {
        string ToNoSignJson();

        string GenerateNonce();

        string MakeSign(string ApiKey);

        bool CheckSign(string ApiKey);
    }
}