namespace Straonit.HighEdge.Core.Distribution;

public interface IPingService
{
    Task<int> PingNodes(List<string> nodes);
}