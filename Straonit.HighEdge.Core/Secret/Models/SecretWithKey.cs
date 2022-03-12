namespace Straonit.HighEdge.Core.Secret;

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