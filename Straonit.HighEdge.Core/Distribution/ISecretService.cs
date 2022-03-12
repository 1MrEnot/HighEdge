using Straonit.HighEdge.Core.Secret;

namespace Straonit.HighEdge.Core.Distribution;

public interface ISecretService
{
    Task<Response> CreateSecret(SplittedSecret splittedSecret);
    Task<Response> DeleteSecret(string id);
    Task<Response> UpdateSecret(SplittedSecret splittedSecret);
    Task<GetSecretResponse> GetSecret(string id);
}