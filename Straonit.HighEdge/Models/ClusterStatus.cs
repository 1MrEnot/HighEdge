using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Models;

namespace Straonit.HighEdge.Models;

public class ClusterStatus
{
    public IEnumerable<NodeStatus> NodesStatuses { get; set; }
    public long SecretsCount { get; set; }
    public ClusterConfig ClusterConfig { get; set; }
    public List<TaskData> Tasks { get; set; }

    public ClusterStatus()
    {
        Tasks = new List<TaskData>();
    }
}