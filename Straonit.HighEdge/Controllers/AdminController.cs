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
    private readonly ClusterConfig _clusterConfig;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly StatusChecker _checker;
    private readonly BackgroundWorkerQueue _backgroundWorkerQueue;

    public AdminController(ClusterConfig clusterConfig,
        IHttpClientFactory httpClientFactory, StatusChecker checker, BackgroundWorkerQueue backgroundWorkerQueue)
    {
        _clusterConfig = clusterConfig;
        _httpClientFactory = httpClientFactory;
        _checker = checker;
        _backgroundWorkerQueue = backgroundWorkerQueue;
    }

    [HttpGet("node/status/all")]
    public async Task<ClusterStatus> GetStatuses()
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(1);
        var statuses = new List<NodeStatus>(_clusterConfig.Nodes.Count());
        foreach (var node in _clusterConfig.Nodes)
        {
            try
            {
                var response = await client.GetAsync($"http://{node}:80/admin/node/status");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    statuses.Add(new NodeStatus(node, await response.GetObjectAsync<SelfNodeStatus>()));
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
        var clusterStatus = new ClusterStatus()
        {
            NodesStatuses = statuses,
            SecretsCount = await _checker.GetSecretsCountAsync(),
            ClusterConfig = _clusterConfig
        };
        return clusterStatus;
    }

    [HttpGet("node/status")]
    public async Task<SelfNodeStatus> GetStatus()
    {
        var status = await _checker.GetNodeStatusAsync();
        return status;
    }

    [HttpGet("Test")]
    public async Task Test()
    {
        _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
        {
            await Task.Delay(10000);
            System.Console.WriteLine("Poker");
        });
    }
}