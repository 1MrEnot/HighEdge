using System.Numerics;
using Google.Protobuf;
using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using GetSecretResponse = Straonit.HighEdge.Core.Distribution.GetSecretResponse;
using Response = Straonit.HighEdge.Core.Distribution.Response;

namespace Straonit.HighEdge.Infrastructure.Service;

using Core.NodeRestoration;
using Core.SplitSecret;

public class SecretService : ISecretService
{
    private readonly ClusterConfig _config;
    private readonly GrpcChannelOptions _dangerousChannelOptions;
    private readonly RollBackConfig _rollBackConfig;
    private readonly IRollBack _rollBackService;
    private readonly INodeCommandSaver _nodeCommandSaver;

    public SecretService(ClusterConfig config, RollBackConfig rollBackConfig, IRollBack rollBackService,
        INodeCommandSaver nodeCommandSaver)
    {
        _config = config;
        _nodeCommandSaver = nodeCommandSaver;
        var dangerousHandler = new HttpClientHandler();
        dangerousHandler.ServerCertificateCustomValidationCallback
            = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        _dangerousChannelOptions = new GrpcChannelOptions
        {
            HttpHandler = dangerousHandler
        };
        (_config, _rollBackConfig, _rollBackService) = (config, rollBackConfig, rollBackService);
    }

    public async Task<Response> CreateSecret(SplittedSecret splittedSecret)
    {
        var nodes = new List<string>();
        var failed = new Dictionary<string, (string, PartOfSecret)>();

        for (var i = 0; i < _config.Nodes.Count; i++)
        {
            try
            {
                var nodeUrl = "http://" + _config.Nodes[i] + ":82";
                using var channel = GrpcChannel.ForAddress(nodeUrl, _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var x = splittedSecret.ValueParts[i].X;
                var y = splittedSecret.ValueParts[i].Y;

                var reply = await client.CreateSecretAsync(new CreateSecretMessage()
                {
                    Id = splittedSecret.Key,
                    X = ByteString.CopyFrom(x.ToByteArray()),
                    Y = ByteString.CopyFrom(y.ToByteArray())
                }, deadline: DateTime.UtcNow.AddSeconds(0.5));

                if (!reply.IsSuccess)
                    failed.Add(nodeUrl, (splittedSecret.Key, new PartOfSecret(x, y)));

                nodes.Add(_config.Nodes[i]);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        if (nodes.Count < _config.RequiredNodesCount)
        {
            await _rollBackService.RollBackCreate(nodes, splittedSecret.Key);
        }
        else
        {
            var saveTask = Task.WhenAll(failed
                .Select(pair => _nodeCommandSaver.WriteCreateCommand(pair.Key, pair.Value.Item2, pair.Value.Item1)));

            await saveTask;
        }

        return new Response { SuccessCount = nodes.Count };
    }

    public async Task<Response> DeleteSecret(string id)
    {
        var successNodesCount = 0;
        var failed = new Dictionary<string, string>();
        foreach (var node in _config.Nodes)
        {
            try
            {
                var nodeUrl = "http://" + node + ":82";
                using var channel = GrpcChannel.ForAddress(nodeUrl, _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var reply = await client.DeleteSecretAsync(new DeleteSecretMessage()
                {
                    Id = id
                }, deadline: DateTime.UtcNow.AddSeconds(0.5));

                successNodesCount++;
            }
            catch
            {
            }
        }

        if (successNodesCount > _config.RequiredNodesCount)
        {
            var saveTask = Task.WhenAll(failed
                .Select(pair => _nodeCommandSaver.WriteDeleteCommand(pair.Key, pair.Value)));

            await saveTask;
        }

        return new Response()
        {
            SuccessCount = successNodesCount
        };
    }

    public async Task<Response> UpdateSecret(SplittedSecret splittedSecret)
    {
        var successNodesCount = 0;
        var oldValueParts = new List<OldValue>();
        var failed = new Dictionary<string, (string, PartOfSecret)>();

        for (var i = 0; i < _config.NodesCount; i++)
        {
            try
            {
                var nodeUrl = "http://" + _config.Nodes[i] + ":82";
                using var channel = GrpcChannel.ForAddress(nodeUrl, _dangerousChannelOptions);

                var client = new SecretsService.SecretsServiceClient(channel);

                var x = splittedSecret.ValueParts[i].X;
                var y = splittedSecret.ValueParts[i].Y;

                var reply = await client.PutSecretAsync(new PutSecretMessage()
                {
                    Id = splittedSecret.Key,
                    X = ByteString.CopyFrom(x.ToByteArray()),
                    Y = ByteString.CopyFrom(y.ToByteArray()),
                }, deadline: DateTime.UtcNow.AddSeconds(0.5));

                if (!reply.IsSuccess)
                    failed.Add(nodeUrl, (splittedSecret.Key, new PartOfSecret(x, y)));

                successNodesCount++;
                oldValueParts.Add(new OldValue()
                {
                    PartOfSecret = splittedSecret.ValueParts[i],
                    Node = _config.Nodes[i]
                });
            }
            catch
            {
                //ignore
            }
        }

        if (successNodesCount < _config.RequiredNodesCount)
        {
            await _rollBackService.RollBackUpdate(oldValueParts, splittedSecret.Key);
        }
        else
        {
            foreach (var (nodeUrl, (key, partOfSecret)) in failed)
            {
                await _nodeCommandSaver.WriteDeleteCommand(key, nodeUrl);
                await _nodeCommandSaver.WriteCreateCommand(key, partOfSecret, nodeUrl);
            }
        }

        return new Response()
        {
            SuccessCount = successNodesCount
        };
    }

    public async Task<GetSecretResponse> GetSecret(string id)
    {
        var response = new GetSecretResponse() { PartOfSecrets = new List<PartOfSecret>() };

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

                if (!reply.IsFound)
                {
                    response.NodesWithNotExistentKey.Add($"Узел {node} не содержит ключа");
                }
                else
                {
                    response.PartOfSecrets.Add(new PartOfSecret(new BigInteger(reply.X.ToByteArray()),
                                        new BigInteger(reply.Y.ToByteArray())));
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.GetType() +": "+ ex.Message+ ": "+ ex.StackTrace);
                response.UnWorkedNodes.Add($"Узел {node} не работает");
            }
        }

        return response;
    }
}