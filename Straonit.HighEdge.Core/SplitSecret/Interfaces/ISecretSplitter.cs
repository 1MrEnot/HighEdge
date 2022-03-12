namespace Straonit.HighEdge.Core.SplitSecret;

public interface ISecretSplitter
{
    SplittedSecret SplitSecret(SplittingRequest splittingRequest);
}