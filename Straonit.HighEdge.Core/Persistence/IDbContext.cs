using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Core.Persistence;

public interface IDbContext
{
    Task<GetSecretResponse> GetSecretPart(GetSecretRerquest request);
    Task<Response> CreateSecretPart(CreateSecretRequest request);
    Task<Response> DeleteSecretPart(DeleteSecretRequest request);
    Task<Response> UpdateSecretPart(UpdateSecretRequest request);
}