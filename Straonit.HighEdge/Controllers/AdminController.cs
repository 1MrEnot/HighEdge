using System.Net;
using hackation_high_edge.Extensions;
using hackation_high_edge.Models;
using hackation_high_edge.Service;
using Microsoft.AspNetCore.Mvc;
using Straonit.HighEdge.Core.Configuration;

namespace hackation_high_edge.Controllers;

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

    // [HttpGet("node/status/all")]
    // public async Task<IEnumerable<NodeStatus>> GetStatuses()
    // {
    //     var client = _httpClientFactory.CreateClient();
    //     var statuses = new List<NodeStatus>(_clusterConfig.Nodes.Count());
    //     foreach (var node in _clusterConfig.Nodes)
    //     {
    //         var response = await client.GetAsync($"http://{node}/admin/node/status");
    //         if (response.StatusCode == HttpStatusCode.OK)
    //         {
    //             statuses.Add(new NodeStatus(node, await response.GetObjectAsync<SelfNodeStatus>()));
    //         }
    //         else
    //         {
    //             statuses.Add(NodeStatus.CreateFailedStatus(node));
    //         }
    //     }
    //     return statuses;
    // }

    [HttpGet("node/status")]
    public async Task<SelfNodeStatus> GetStatus()
    {
        System.Console.WriteLine("Requested status");
        var status = await _checker.GetNodeStatusAsync();
        return status;        
    }
}