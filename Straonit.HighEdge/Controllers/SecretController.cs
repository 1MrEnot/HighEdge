namespace Straonit.HighEdge.Controllers;

using Core;
using Microsoft.AspNetCore.Mvc;
using Response = Core.Distribution.Response;

[ApiController]
[Route("[controller]")]
public class SecretController
{
    private readonly DistributedSecretSerivce _distributedSecretSerivce;

    public SecretController(DistributedSecretSerivce distributedSecretSerivce)
    {
        _distributedSecretSerivce = distributedSecretSerivce;
    }

    [HttpPost("{key}/{secret}")]
    public Task<Response> AddSecret(string key, string secret)
    {
        return _distributedSecretSerivce.SaveSecret(key, secret);
    }

    [HttpGet("{key}")]
    public async Task<string> GetSecret(string key)
    {
        return (await _distributedSecretSerivce.GetSecret(key)).Secret;
    }
}