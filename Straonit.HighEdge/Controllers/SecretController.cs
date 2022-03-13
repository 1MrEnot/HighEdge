using Straonit.HighEdge.Core.Configuration;

namespace Straonit.HighEdge.Controllers;

using Core;
using Microsoft.AspNetCore.Mvc;
using Response = Core.Distribution.Response;

[ApiController]
[Route("[controller]")]
public class SecretController:ControllerBase
{
    private readonly DistributedSecretSerivce _distributedSecretSerivce;
    private readonly ClusterConfig _clusterConfig;

    public SecretController(DistributedSecretSerivce distributedSecretSerivce,ClusterConfig clusterConfig)
    {
        _distributedSecretSerivce = distributedSecretSerivce;
        _clusterConfig = clusterConfig;
    }

    [HttpPost("{key}/{secret}")]
    public Task<Response> AddSecret(string key, string secret)
    {
        return _distributedSecretSerivce.SaveSecret(key, secret);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetSecret(string key)
    {
        var response = await _distributedSecretSerivce.GetSecret(key);

        if (_clusterConfig.NodesCount - response.Response.UnWorkedNodes.Count < _clusterConfig.RequiredNodesCount)
        {
            return StatusCode(500,response);
        }

        if (_clusterConfig.NodesCount - response.Response.NodesWithNotExistentKey.Count <
            _clusterConfig.RequiredNodesCount)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}