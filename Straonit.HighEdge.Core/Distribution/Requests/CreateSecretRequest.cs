namespace Straonit.HighEdge.Core.Distribution;

public class CreateSecretRequest
{
    public string Id { get; set; }
    public byte[] X { get; set; }
    public byte[] Y { get; set; }
}