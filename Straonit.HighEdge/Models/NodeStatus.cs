namespace Straonit.HighEdge.Models
{
    public class NodeStatus
    {        
        public string IpAddress { get; set; }
        public string RedisStatus { get; set; }        
        public Ram Ram { get; set; }        
        public bool IsApiAvailable { get; set; }

        public NodeStatus()
        {

        }

        public NodeStatus(string ipAddress, SelfNodeStatus status)
        {            
            IpAddress = ipAddress;
            RedisStatus = status.RedisStatus.ToString();              
            IsApiAvailable = true;
            Ram = status.Ram;
        }


        public static NodeStatus CreateFailedStatus(string ipAddress)
        {
            return new NodeStatus()
            {
                IpAddress = ipAddress,
                IsApiAvailable = false,
                RedisStatus = ServiceStatus.Unknown.ToString()                         
            };
        }
    }
}