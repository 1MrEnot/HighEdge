using System.Numerics;
using Google.Protobuf;
using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using GetSecretResponse = Straonit.HighEdge.Core.Distribution.GetSecretResponse;
using Response = Straonit.HighEdge.Core.Distribution.Response;

namespace Straonit.HighEdge.Infrastructure.Service;

using Core.SplitSecret;

public class SecretService:ISecretService
{
    private readonly ClusterConfig _config;
    private readonly GrpcChannelOptions _dangerousChannelOptions;
    private readonly RollBackConfig _rollBackConfig;
    private readonly IRollBack _rollBackService;
    
    public SecretService(ClusterConfig config,RollBackConfig rollBackConfig,IRollBack rollBackService)
    {
        _config = config;
        var dangerousHandler = new HttpClientHandler();
        dangerousHandler.ServerCertificateCustomValidationCallback
            = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        _dangerousChannelOptions = new GrpcChannelOptions
        {
            HttpHandler = dangerousHandler
        };
        (_config,_rollBackConfig,_rollBackService)=(config,rollBackConfig,rollBackService);
    }

    public async Task<Response> CreateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        var nodes = new List<string>();

        for (var i=0; i < _config.Nodes.Count; i++)
        {
            try
            {
                using var channel =
                    GrpcChannel.ForAddress("http://" + _config.Nodes[i] + ":82", _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var reply = await client.CreateSecretAsync(new CreateSecretMessage()
                {
                    Id = splittedSecret.Key,
                    X = ByteString.CopyFrom(splittedSecret.ValueParts[i].X.ToByteArray()),
                    Y = ByteString.CopyFrom(splittedSecret.ValueParts[i].Y.ToByteArray())
                },deadline:DateTime.UtcNow.AddSeconds(0.5));

                if (!reply.IsSuccess) continue;

                nodes.Add(_config.Nodes[i]);
                successNodesCount++;
            }
            catch (Exception ex)
            {
            }
        }

        if (successNodesCount < _config.RequiredNodesCount)
        {
            await _rollBackService.RollBackCreate(nodes,splittedSecret.Key);
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
            try
            {
                using var channel = GrpcChannel.ForAddress("http://" + node + ":82", _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var reply = await client.DeleteSecretAsync(new DeleteSecretMessage()
                {
                    Id = id
                },deadline:DateTime.UtcNow.AddSeconds(0.5));

                if (reply.IsSuccess)
                    successNodesCount++;
            }
            catch
            {
            }
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
            try
            {
                using var channel =
                    GrpcChannel.ForAddress("http://" + _config.Nodes[i] + ":82", _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var reply = await client.PutSecretAsync(new PutSecretMessage()
                {
                    Id = splittedSecret.Key,
                    X = ByteString.CopyFrom(splittedSecret.ValueParts[i].X.ToByteArray()),
                    Y = ByteString.CopyFrom(splittedSecret.ValueParts[i].Y.ToByteArray()),
                },deadline:DateTime.UtcNow.AddSeconds(0.5));

                if (!reply.IsSuccess) continue;

                successNodesCount++;
                oldValueParts.Add(new OldValue()
                {
                    PartOfSecret = splittedSecret.ValueParts[i],
                    Node = _config.Nodes[i]
                });
            }
            catch
            {
            }


        }
        
        if (successNodesCount < _config.RequiredNodesCount)
        {
            // await _rollBackService.RollBackUpdate(oldValueParts);
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
            try
            {
                using var channel = GrpcChannel.ForAddress("http://" + node + ":82", _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var reply = await client.GetSecretAsync(new GetSecretMessage()
                {
                    Id = id
                }, deadline: DateTime.UtcNow.AddSeconds(0.5));

                response.PartOfSecrets.Add(new PartOfSecret(new BigInteger(reply.X.ToByteArray()),
                    new BigInteger(reply.Y.ToByteArray())));
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                response.NodesWithNotExistentKey.Add($"Узел {node}: {keyNotFoundException.Message}");
            }
            catch (Exception ex)
            {
                 response.UnWorkedNodes.Add($"Узел {node} не работает");
            }
        }

        return response;
    }
}