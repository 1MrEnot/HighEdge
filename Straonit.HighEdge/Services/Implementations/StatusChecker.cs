using System.Diagnostics;
using System.Text.RegularExpressions;
using hackation_high_edge.Extensions;
using hackation_high_edge.Models;
using StackExchange.Redis;

namespace hackation_high_edge.Service;

public class StatusChecker
{

    private IConnectionMultiplexer _connection;
    private IHttpClientFactory _httpClientFactory;

    public StatusChecker(ClusterConfig clusterConfig, IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public IEnumerable<Disk> GetDisks()
    {
        return new[]{ new Disk(){
           FreeSpace = 50,
           TotalSize = 200
       }};
    }

    public Ram GetRamInfo()
    {
        var memoryData = File.ReadAllLines("/proc/meminfo");
        var total = Int32.Parse(Regex.Replace(memoryData[0], @"\s+", " ").Split(' ')[1]);
        var free = Int32.Parse(Regex.Replace(memoryData[1], @"\s+", " ").Split(' ')[1]);
        var swapTotal = Int32.Parse(Regex.Replace(memoryData[14], @"\s+", " ").Split(' ')[1]);
        var swapFree = Int32.Parse(Regex.Replace(memoryData[15], @"\s+", " ").Split(' ')[1]);
        return new Ram()
        {
            TotalSize = total,
            Free = free,
            SwapTotalSize = swapTotal,
            SwappFree = swapFree
        };
    }

    public async Task<ServiceStatus> GetRedisStatusAsync()
    {
        ServiceStatus status;
        if (_connection.IsConnected)
        {
            var pong = await _connection.GetDatabase().PingAsync();
            if (pong.Milliseconds < 100)
            {
                status = ServiceStatus.Ok;
            }
            else
            {
                status = ServiceStatus.HighPing;
            }
        }
        else if (_connection.IsConnecting)
        {
            status = ServiceStatus.Connecting;
        }
        else
        {
            status = ServiceStatus.Unavailable;
        }
        return status;
    }

    public async Task<long> GetSecretsCountAsync()
    {        
        return await _connection.GetServer(_connection.GetEndPoints().First()).DatabaseSizeAsync();
    }

    public async Task<SelfNodeStatus> GetNodeStatusAsync()
    {
        return new SelfNodeStatus()
        {
            Disks = GetDisks(),            
            RedisStatus = await GetRedisStatusAsync(),
            Ram = GetRamInfo()
        };
    }
}