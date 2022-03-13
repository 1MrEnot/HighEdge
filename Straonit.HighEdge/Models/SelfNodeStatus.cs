namespace Straonit.HighEdge.Models;

public class SelfNodeStatus
{
    public ServiceStatus RedisStatus { get; set; }
    public IEnumerable<Disk> Disks { get; set; }
    public Ram Ram { get; set; }
    public IEnumerable<TaskData> Tasks { get; set; }
}