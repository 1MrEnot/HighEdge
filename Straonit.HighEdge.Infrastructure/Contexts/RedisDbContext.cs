using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.Persistence;

namespace Straonit.HighEdge.Infrastructure.Contexts;

public class RedisDbContext:IDbContext
{
    public Task<GetSecretResponse> GetSecretPart(GetSecretRerquest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response> CreateSecretPart(CreateSecretRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response> DeleteSecretPart(DeleteSecretRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UpdateSecretPart(UpdateSecretRequest request)
    {
        throw new NotImplementedException();
    }
}