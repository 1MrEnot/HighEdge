namespace hackation_high_edge.Models;

public class ClusterStatus
{
    public IEnumerable<NodeStatus> NodesStatuses { get; set; }
    public long SecretsCount { get; set; }
}