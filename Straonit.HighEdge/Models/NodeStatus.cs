namespace Straonit.HighEdge.Models
{
    public class NodeStatus
    {
        //public string NodeId { get; set; }
        public string IpAddress { get; set; }
        public ServiceStatus RedisStatus { get; set; }
        public IEnumerable<Disk> Disks { get; set; }
        public long SecretsSize { get; set; }
        public bool IsApiAvailable { get; set; }

        public NodeStatus()
        {
            
        }

        public NodeStatus(string ipAddress,SelfNodeStatus status)
        {
            IpAddress = ipAddress;
            RedisStatus = status.RedisStatus;
            Disks = status.Disks;
            SecretsSize = status.SecretsSize;
            IsApiAvailable = true;
        }


        public static NodeStatus CreateFailedStatus(string ipAddress)
        {
            return new NodeStatus()
            {
                IpAddress = ipAddress,
                IsApiAvailable = false,
                RedisStatus = ServiceStatus.Unknown,
                Disks = null,
                SecretsSize = 0
            };
        }
    }
}