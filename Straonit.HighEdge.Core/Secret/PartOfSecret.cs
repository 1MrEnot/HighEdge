namespace Straonit.HighEdge.Core.Secret;

using System.Numerics;

public class PartOfSecret
{
    public PartOfSecret(int x, BigInteger y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }

    public BigInteger Y { get; }
}