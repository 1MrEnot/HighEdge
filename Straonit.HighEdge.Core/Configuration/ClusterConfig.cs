namespace Straonit.HighEdge.Core.Configuration;

public class ClusterConfig
{
    public int NodesCount => Nodes.Count;
    public int RequiredNodesCount { get; set; }
    public List<string> Nodes { get; set; }
}