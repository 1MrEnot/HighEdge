using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Core.SplitSecret;

public class SecretWithKey
{
    public SecretWithKey(string key, string secret)
    {
        Key = key;
        Secret = secret;
    }

    public GetSecretResponse Response { get; set; }

    public string Key { get; }

    public string Secret { get; }
}