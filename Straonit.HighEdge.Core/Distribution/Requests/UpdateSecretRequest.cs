namespace Straonit.HighEdge.Core.Distribution;

public class UpdateSecretRequest
{
    public string Id { get; set; }
    public byte[] X { get; set; }
    public byte[] Y { get; set; }
}