namespace Straonit.HighEdge.Infrastructure;

public class RedisPointModel
{
    public RedisPointModel(byte[] x, byte[] y)
    {
        X = x;
        Y = y;
    }

    public byte[] X { get; }
    public byte[] Y { get; }
}