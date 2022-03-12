namespace Straonit.HighEdge.Core.SplitSecret;

using System.Numerics;

public class PartOfSecret
{
    public PartOfSecret(BigInteger x, BigInteger y)
    {
        X = x;
        Y = y;
    }

    public BigInteger X { get; }

    public BigInteger Y { get; }
}