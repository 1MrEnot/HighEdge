using Straonit.HighEdge.Core.SplitSecret;

namespace Straonit.HighEdge.Core.Distribution;

public class GetSecretResponse
{
    public List<string> NodesWithNotExistentKey { get; set; } = new List<string>();
    public List<string> UnWorkedNodes { get; set; } = new List<string>();
    public List<PartOfSecret> PartOfSecrets { get; set; }
}