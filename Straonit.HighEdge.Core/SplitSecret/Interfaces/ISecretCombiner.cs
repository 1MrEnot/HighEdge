namespace Straonit.HighEdge.Core.SplitSecret;

public interface ISecretCombiner
{
    SecretWithKey CombineSecret(SplittedSecret splittedSecret);
}