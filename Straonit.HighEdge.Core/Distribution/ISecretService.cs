namespace Straonit.HighEdge.Core.Distribution;

public interface ISecretService
{
    Task CreateSecret(CreateSecretRequest request);
}