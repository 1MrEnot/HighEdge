namespace Straonit.HighEdge.Core.Distribution;

public class GetSecretResponse
{
    public GetSecretResponse(string id, byte[] x, byte[] y)
    {
        Id = id;
        X = x;
        Y = y;
    }

    public string Id { get; }

    public byte[] X { get; }

    public byte[] Y { get; }
}