namespace Straonit.HighEdge.Core.Secret;

public interface ISecretSplitter
{
    SplittedSecret SplitSecret(SplittingRequest splittingRequest);
}