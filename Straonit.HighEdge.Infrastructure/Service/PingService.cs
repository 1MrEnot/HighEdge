using Grpc.Net.Client;
using Secrets.Lib;
using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Infrastructure.Service;

public class PingService:IPingService
{
    public Task<int> PingNodes(List<string> nodes)
    {
        var workedNodesCount = 0;

        foreach (var node in nodes)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("http://" + node + ":82");
                var client = new SecretsService.SecretsServiceClient(channel);
                workedNodesCount++;
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        return Task.FromResult(workedNodesCount);
    }
}