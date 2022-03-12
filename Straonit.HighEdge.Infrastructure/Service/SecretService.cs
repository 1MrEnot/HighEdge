using Google.Protobuf;
using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.Secret;
using GetSecretResponse = Straonit.HighEdge.Core.Distribution.GetSecretResponse;
using Response = Straonit.HighEdge.Core.Distribution.Response;

namespace Straonit.HighEdge.Infrastructure.Service;


public class SecretService:ISecretService
{
    public SecretService()
    {
        
    }
    
    public async Task<Response> CreateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        
        foreach (var vp in splittedSecret.ValueParts)
        {
            using var channel = GrpcChannel.ForAddress("adress");

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.CreateSecretAsync(new CreateSecretMessage()
            {
                Id = splittedSecret.Key,
                X = ByteString.CopyFrom(vp.X.ToByteArray()),
                Y= ByteString.CopyFrom(vp.Y.ToByteArray())
            });

            if (reply.IsSuccess) successNodesCount++;
        }
        
        return new Response()
        {
            
        };
        
    }

    public async Task<Response> DeleteSecret(string id)
    {
        
        using var channel = GrpcChannel.ForAddress("adress");

        var client = new SecretsService.SecretsServiceClient(channel);

        var reply = await client.DeleteSecretAsync(new DeleteSecretMessage()
        {
            Id = id
        });

        return new Response()
        {
            Message = reply.Message
        };
    }

    public async Task<Response> UpdateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        
        foreach (var vp in splittedSecret.ValueParts)
        {
            using var channel = GrpcChannel.ForAddress("adress");

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.PutSecretAsync(new PutSecretMessage()
            {
                Id = splittedSecret.Key,
                X = ByteString.CopyFrom(vp.X.ToByteArray()),
                Y = ByteString.CopyFrom(vp.Y.ToByteArray()),
            });
            
            if (reply.IsSuccess) successNodesCount++;
        }
        
        
        return new Response()
        {
            
        };
    }

    public async Task<GetSecretResponse> GetSecret(string id)
    {
        
        using var channel = GrpcChannel.ForAddress("adress");

        var client = new SecretsService.SecretsServiceClient(channel);

        var reply = await client.GetSecretAsync(new GetSecretMessage()
        {
            Id = id
        });

        return new GetSecretResponse()
        {
            
        };
    }
}