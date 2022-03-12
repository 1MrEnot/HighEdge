using System.Numerics;
using Google.Protobuf;
using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.Secret;
using GetSecretResponse = Straonit.HighEdge.Core.Distribution.GetSecretResponse;
using Response = Straonit.HighEdge.Core.Distribution.Response;

namespace Straonit.HighEdge.Infrastructure.Service;

public class SecretService:ISecretService
{
    private readonly ClusterConfig _config;
    public SecretService(ClusterConfig config) => (_config)=(config);
  
    
    public async Task<Response> CreateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        var nodes = _config.Nodes.ToArray();
        
        for (var i=0;i< _config.NodesCount;i++)
        {
            using var channel = GrpcChannel.ForAddress(nodes[i]);

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.CreateSecretAsync(new CreateSecretMessage()
            {
                Id = splittedSecret.Key,
                X = ByteString.CopyFrom(splittedSecret.ValueParts[i].X.ToByteArray()),
                Y= ByteString.CopyFrom(splittedSecret.ValueParts[i].Y.ToByteArray())
            });

            if (reply.IsSuccess) successNodesCount++;
        }
        
        return new Response()
        {
            SuccessCount = successNodesCount
        };
    }

    public async Task<Response> DeleteSecret(string id)
    {
        var successNodesCount = 0;
        foreach (var node in _config.Nodes)
        {
            using var channel = GrpcChannel.ForAddress(node);

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.DeleteSecretAsync(new DeleteSecretMessage()
            {
                Id = id
            });
        }
        
        return new Response()
        {
            SuccessCount = successNodesCount
        };
    }

    public async Task<Response> UpdateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        var nodes = _config.Nodes.ToArray();
        
        for(var i=0;i< _config.NodesCount;i++)
        {
            using var channel = GrpcChannel.ForAddress(nodes[i]);

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.PutSecretAsync(new PutSecretMessage()
            {
                Id = splittedSecret.Key,
                X = ByteString.CopyFrom(splittedSecret.ValueParts[i].X.ToByteArray()),
                Y = ByteString.CopyFrom(splittedSecret.ValueParts[i].Y.ToByteArray()),
            });
            
            if (reply.IsSuccess) successNodesCount++;
        }
        
        return new Response()
        {
            SuccessCount = successNodesCount
        };
    }

    public async Task<GetSecretResponse> GetSecret(string id)
    {
        var response = new GetSecretResponse(){PartOfSecrets = new List<PartOfSecret>()};
        
        foreach (var node in _config.Nodes)
        {
            using var channel = GrpcChannel.ForAddress(node);

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.GetSecretAsync(new GetSecretMessage()
            {
                Id = id
            });
            
            response.PartOfSecrets.Add(new PartOfSecret(new BigInteger(reply.X.ToByteArray()),new BigInteger(reply.Y.ToByteArray())));
        }
        
        return response;
    }
}