namespace Straonit.HighEdge.Secret;

using System.Numerics;
using Core.SplitSecret;
using SecretSharingDotNet.Cryptography;
using SecretSharingDotNet.Math;

public class ShamirSecretCombiner : ISecretCombiner
{
    private readonly ShamirsSecretSharing<BigInteger> _combiner;

    public ShamirSecretCombiner()
    {
        var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();
        _combiner = new ShamirsSecretSharing<BigInteger>(gcd);
    }

    public SecretWithKey CombineSecret(SplittedSecret splittedSecret)
    {
        var pointForRecreating = splittedSecret.ValueParts
            .Select(PointFromSecretPart);

        var recoveredSecret = _combiner.Reconstruction(pointForRecreating.ToArray());

        return new SecretWithKey(splittedSecret.Key, recoveredSecret.ToString());
    }

    private static FinitePoint<BigInteger> PointFromSecretPart(PartOfSecret partOfSecret)
    {
        return new FinitePoint<BigInteger>(
            new BigIntCalculator(partOfSecret.X),
            new BigIntCalculator(partOfSecret.Y));
    }
}