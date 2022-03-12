namespace Straonit.HighEdge.Core.Secret;

public interface ISecretMerger
{
    SecretWithKey MergeSecret(SplittedSecret splittedSecret);
}