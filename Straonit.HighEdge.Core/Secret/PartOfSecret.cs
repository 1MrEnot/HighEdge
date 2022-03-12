﻿namespace Straonit.HighEdge.Core.Secret;

using System.Numerics;

public class PartOfSecret
{
    public PartOfSecret(int x, BigInteger y)
    {
        X = x;
        Y = y;
    }

    public BigInteger X { get; }

    public BigInteger Y { get; }
}