namespace Straonit.HighEdge.Secret;

using System.Numerics;
using SecretSharingDotNet.Cryptography;
using SecretSharingDotNet.Math;

public class Class1
{
    void Foo()
    {
        var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();

        var split = new ShamirsSecretSharing<BigInteger>(gcd);

        string password = "Hello World!!";

        var shares = split.MakeShares(3, 7, password);

        var combine = new ShamirsSecretSharing<BigInteger>(gcd);
        var subSet = shares.Where(p => p.X.IsEven).ToList();
        var recoveredSecret = combine.Reconstruction(subSet.ToArray());



    }

}