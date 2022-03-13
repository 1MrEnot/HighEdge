using Google.Protobuf;
using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Infrastructure.Service;

public class RollBackService : IRollBack
{
    private readonly RollBackConfig _rollBackConfig;

    public RollBackService(RollBackConfig rollBackConfig) =>
        (_rollBackConfig) = (rollBackConfig);

    public async Task RollBackUpdate(List<OldValue> oldValues)
    {
        foreach (var oldValue in oldValues)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("http://" + oldValue.Node + ":82");

                var client = new SecretsService.SecretsServiceClient(channel);

                await client.PutSecretAsync(new PutSecretMessage()
                {
                    Id = _rollBackConfig.Key,
                    X = ByteString.CopyFrom(oldValue.PartOfSecret.X.ToByteArray()),
                    Y = ByteString.CopyFrom(oldValue.PartOfSecret.Y.ToByteArray()),
                }, deadline: DateTime.UtcNow.AddSeconds(0.5));
            }
            catch
            {
                //ignore
            }
        }
    }

    public async Task RollBackCreate(List<string> nodes, string key)
    {
        foreach (var node in nodes)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("http://" + node + ":82");

                var client = new SecretsService.SecretsServiceClient(channel);

                await client.DeleteSecretAsync(new DeleteSecretMessage()
                {
                    Id = key
                }, deadline: DateTime.UtcNow.AddSeconds(0.5));
            }
            catch (Exception e)
            {
                //ignore
            }
        }
    }
}