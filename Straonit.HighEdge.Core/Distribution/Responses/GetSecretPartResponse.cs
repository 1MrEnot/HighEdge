namespace Straonit.HighEdge.Core.Distribution;

public class GetSecretPartResponse
{
    public GetSecretPartResponse(string id, byte[] x, byte[] y)
    {
        Id = id;
        X = x;
        Y = y;
    }
    public string Id { get; set; }
    public byte[] X { get; set; }
    public byte[] Y { get; set; }
}