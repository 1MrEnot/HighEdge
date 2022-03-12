namespace Straonit.HighEdge.Core.Secret;

public class SplittingRequest
{
    public SplittingRequest(SecretWithKey secretWithKey, int totalPartCount, int minimumPartCount)
    {
        SecretWithKey = secretWithKey;
        TotalPartCount = totalPartCount;
        MinimumPartCount = minimumPartCount;
    }

    public SecretWithKey SecretWithKey { get; }

    public int TotalPartCount { get; }

    public int MinimumPartCount { get; }
}