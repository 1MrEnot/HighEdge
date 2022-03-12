namespace Straonit.HighEdge.Secret;

using System.Numerics;
using Core.Secret;
using SecretSharingDotNet.Cryptography;
using SecretSharingDotNet.Math;

public class ShamirSecretSplitter : ISecretSplitter
{
    private readonly ShamirsSecretSharing<BigInteger> _splitter;

    public ShamirSecretSplitter()
    {
        var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();
        _splitter = new ShamirsSecretSharing<BigInteger>(gcd);
    }

    public SplittedSecret SplitSecret(SplittingRequest splittingRequest)
    {
        var shares = _splitter.MakeShares(splittingRequest.MinimumPartCount,
            splittingRequest.TotalPartCount,
            splittingRequest.SecretWithKey.Secret);
        var keyParts = shares.Select(ShareToSecret);
        var result = new SplittedSecret(splittingRequest.SecretWithKey.Key, keyParts);

        return result;
    }

    private static PartOfSecret ShareToSecret(FinitePoint<BigInteger> point)
    {
        return new PartOfSecret(point.X.Value, point.Y.Value);
    }
}