using Straonit.HighEdge.Core.SplitSecret;

namespace Straonit.HighEdge.Core.Distribution;

public class GetSecretResponse
{
    public List<PartOfSecret> PartOfSecrets { get; set; }
}