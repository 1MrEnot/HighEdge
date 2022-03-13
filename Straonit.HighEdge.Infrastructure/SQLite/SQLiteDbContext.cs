using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.Persistence;

namespace Straonit.HighEdge.Infrastructure;

public class SQLiteDbContext : IDbContext
{
    public Task<bool> CreateSecretPart(CreateSecretRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteSecretPart(DeleteSecretRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetSecretPartResponse> GetSecretPart(GetSecretRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSecretPart(UpdateSecretRequest request)
    {
        throw new NotImplementedException();
    }
}