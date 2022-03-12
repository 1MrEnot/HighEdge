namespace hackation_high_edge.Models;

public class ClusterConfig
{
    public int NodesCount { get; set; }
    public int RequiredNodesCount { get; set; }
    public IEnumerable<string> Nodes { get; set; }
}