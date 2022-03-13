using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Core.SplitSecret;

public class SecretWithKey
{
    public SecretWithKey()
    {
    }

    public SecretWithKey(string key, string secret)
    {
        Key = key;
        Secret = secret;
    }

    public List<string> NodesWithNotExistentKey { get; set; } = new List<string>();
    public List<string> UnWorkedNodes { get; set; } = new List<string>();

    public string Key { get; }

    public string Secret { get; }
}
