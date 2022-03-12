using Straonit.HighEdge.Models;
using Straonit.HighEdge.Services.Implementations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Straonit.HighEdge.Core.Configuration;


namespace Straonit.HighEdge.Controllers;


using System.Net;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Implementations;
using Straonit.HighEdge.Extensions;

[ApiController]
[Route("[controller]")]
public class AdminController
{
    private ClusterConfig _clusterConfig;
    private IHttpClientFactory _httpClientFactory;
    private StatusChecker _checker;

    public AdminController(ClusterConfig clusterConfig, IHttpClientFactory httpClientFactory, StatusChecker checker)
    {        
        _clusterConfig = clusterConfig;
        _httpClientFactory = httpClientFactory;
        _checker = checker;
    }

    [HttpGet("node/status/all")]
    public async Task<ClusterStatus> GetStatuses()
    {
        var client = _httpClientFactory.CreateClient();
        var statuses = new List<NodeStatus>(_clusterConfig.Nodes.Count());
        foreach (var node in _clusterConfig.Nodes)
        {
            var response = await client.GetAsync($"http://{node}:5016/admin/node/status");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                statuses.Add(new NodeStatus(node, await response.GetObjectAsync<SelfNodeStatus>()));
            }
            else
            {
                statuses.Add(NodeStatus.CreateFailedStatus(node));
            }
        }
        var clusterStatus = new ClusterStatus(){
            NodesStatuses = statuses,
            SecretsCount = await _checker.GetSecretsCountAsync()
        };
        return clusterStatus;
    }

    [HttpGet("node/status")]
    public async Task<SelfNodeStatus> GetStatus()
    {        
        var status = await _checker.GetNodeStatusAsync();   
        System.Console.WriteLine(status.Ram.TotalSize);     
        return status;        
    }
}