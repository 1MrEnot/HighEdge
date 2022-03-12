using System.Numerics;
using Google.Protobuf;
using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.SplitSecret;
using GetSecretResponse = Straonit.HighEdge.Core.Distribution.GetSecretResponse;
using Response = Straonit.HighEdge.Core.Distribution.Response;

namespace Straonit.HighEdge.Infrastructure.Service;

public class SecretService:ISecretService
{
    private readonly ClusterConfig _config;
    private readonly RollBackConfig _rollBackConfig;
    private readonly IRollBack _rollBackService;
    public SecretService(ClusterConfig config,RollBackConfig rollBackConfig,IRollBack rollBackService) => 
        (_config,_rollBackConfig,_rollBackService)=(config,rollBackConfig,rollBackService);


    public async Task<Response> CreateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        _rollBackConfig.Key = splittedSecret.Key;

        for (var i=0;i< _config.NodesCount;i++)
        {
            using var channel = GrpcChannel.ForAddress(_config.Nodes[i]);

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.CreateSecretAsync(new CreateSecretMessage()
            {
                Id = splittedSecret.Key,
                X = ByteString.CopyFrom(splittedSecret.ValueParts[i].X.ToByteArray()),
                Y= ByteString.CopyFrom(splittedSecret.ValueParts[i].Y.ToByteArray())
            });

            if (!reply.IsSuccess) continue;
            
            _rollBackConfig.RollBackNodes.Add(_config.Nodes[i]);
            successNodesCount++;
        }

        if (successNodesCount < _config.RequiredNodesCount)
        {
            await _rollBackService.RollBackCreate();
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
        _rollBackConfig.Key = splittedSecret.Key;
        var oldValueParts = new List<OldValue>();

        for(var i=0;i< _config.NodesCount;i++)
        {
            using var channel = GrpcChannel.ForAddress(_config.Nodes[i]);

            var client = new SecretsService.SecretsServiceClient(channel);

            var reply = await client.PutSecretAsync(new PutSecretMessage()
            {
                Id = splittedSecret.Key,
                X = ByteString.CopyFrom(splittedSecret.ValueParts[i].X.ToByteArray()),
                Y = ByteString.CopyFrom(splittedSecret.ValueParts[i].Y.ToByteArray()),
            });

            if (!reply.IsSuccess) continue; 
            
            successNodesCount++;
            oldValueParts.Add(new OldValue()
            {
                PartOfSecret = splittedSecret.ValueParts[i],
                Node = _config.Nodes[i]
            });
        }
        
        if (successNodesCount < _config.RequiredNodesCount)
        {
            await _rollBackService.RollBackUpdate(oldValueParts);
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