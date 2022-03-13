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
    public async Task<IActionResult> AddSecret(string key, string secret)
    {
        var response = await _distributedSecretSerivce.SaveSecret(key, secret);

        if (response.SuccessCount < _clusterConfig.RequiredNodesCount)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetSecret(string key)
    {
        var response = await _distributedSecretSerivce.GetSecret(key);

        if (_clusterConfig.NodesCount - response.UnWorkedNodes.Count < _clusterConfig.RequiredNodesCount)
        {
            return StatusCode(500,new {response.UnWorkedNodes, response.NodesWithNotExistentKey});
        }

        if (_clusterConfig.NodesCount - response.NodesWithNotExistentKey.Count <
            _clusterConfig.RequiredNodesCount)
        {
            return BadRequest(new {response.UnWorkedNodes, response.NodesWithNotExistentKey});
        }

        return Ok(new{response.Key, response.Secret});
    }
}