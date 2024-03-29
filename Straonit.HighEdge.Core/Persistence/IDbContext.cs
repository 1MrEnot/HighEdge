﻿using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Core.Persistence;

public interface IDbContext
{
    Task<GetSecretPartResponse> GetSecretPart(GetSecretRequest request);

    Task<bool> CreateSecretPart(CreateSecretRequest request);

    Task DeleteSecretPart(DeleteSecretRequest request);

    Task<bool> UpdateSecretPart(UpdateSecretRequest request);
}