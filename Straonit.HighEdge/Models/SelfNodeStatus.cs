namespace hackation_high_edge.Models;

public class SelfNodeStatus
{
    public ServiceStatus RedisStatus { get; set; }
    public IEnumerable<Disk> Disks { get; set; }
    public long SecretsSize { get; set; }
    public Ram Ram { get; set; }
}