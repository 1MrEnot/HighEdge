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
using Straonit.HighEdge.Services;

[ApiController]
[Route("[controller]")]
public class AdminController
{
    private readonly ClusterConfig _clusterConfig;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly StatusChecker _checker;
    private TaskService _taskService;

    public AdminController(ClusterConfig clusterConfig,
        IHttpClientFactory httpClientFactory, StatusChecker checker, TaskService taskService)
    {
        _clusterConfig = clusterConfig;
        _httpClientFactory = httpClientFactory;
        _checker = checker;
        _taskService = taskService;
    }

    [HttpGet("node/status/all")]
    public async Task<ClusterStatus> GetStatuses()
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(1);
        var statuses = new List<NodeStatus>(_clusterConfig.Nodes.Count());
        var clusterStatus = new ClusterStatus();
        foreach (var node in _clusterConfig.Nodes)
        {
            try
            {
                var response = await client.GetAsync($"http://{node}:80/admin/node/status");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var selfStatus = await response.GetObjectAsync<SelfNodeStatus>();
                    statuses.Add(new NodeStatus(node, selfStatus));
                    clusterStatus.Tasks.AddRange(selfStatus?.Tasks??Array.Empty<TaskData>());
                }
                else
                {
                    statuses.Add(NodeStatus.CreateFailedStatus(node));
                }
            }
            catch (Exception ex)
            {
                statuses.Add(NodeStatus.CreateFailedStatus(node));
            }
        }
        clusterStatus.NodesStatuses = statuses;
        clusterStatus.SecretsCount = await _checker.GetSecretsCountAsync();
        clusterStatus.ClusterConfig = _clusterConfig;
        return clusterStatus;
    }

    [HttpGet("node/status")]
    public async Task<SelfNodeStatus> GetStatus()
    {
        var status = await _checker.GetNodeStatusAsync();
        return status;
    }
}