namespace Straonit.HighEdge.Core.SplitSecret;

public class SecretWithKey
{
    public SecretWithKey(string key, string secret)
    {
        Key = key;
        Secret = secret;
    }

    public string Key { get; }

    public string Secret { get; }
}