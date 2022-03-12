using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Distribution;
using GetSecretResponse = Straonit.HighEdge.Core.Distribution.GetSecretResponse;
using Response = Straonit.HighEdge.Core.Distribution.Response;

namespace Straonit.HighEdge.Infrastructure.Service;

using Core.SplitSecret;

public class SecretService:ISecretService
{
    public async Task<Response> CreateSecret(SplittedSecret splittedSecret)
    {

        using var channel = GrpcChannel.ForAddress("adress");

        var client = new SecretsService.SecretsServiceClient(channel);

        var reply = await client.CreateSecretAsync(new CreateSecretMessage()
        {
        
        });

        return new Response()
        {
            Message = reply.Message
        };
        
    }

    public Task<Response> DeleteSecret(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UpdateSecret(SplittedSecret splittedSecret)
    {
        throw new NotImplementedException();
    }

    public Task<GetSecretResponse> GetSecret(string id)
    {
        throw new NotImplementedException();
    }
}