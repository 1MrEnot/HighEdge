namespace Straonit.HighEdge.Core;

using Distribution;
using SplitSecret;

public class DistributedSecretSerivce
{
    private readonly ISecretSplitter _secretSplitter;
    private readonly ISecretCombiner _secretCombiner;
    private readonly ISecretService _secretService;

    public DistributedSecretSerivce(ISecretSplitter secretSplitter, ISecretCombiner secretCombiner,
        ISecretService secretService)
    {
        _secretSplitter = secretSplitter;
        _secretCombiner = secretCombiner;
        _secretService = secretService;
    }

    public async Task<Response> SaveSecret(string key, string secret)
    {
        var secretWithKey = new SecretWithKey(key, secret);
        var splittingRequest = new SplittingRequest(secretWithKey, 0, 0);
        var splittedSecret = _secretSplitter.SplitSecret(splittingRequest);
        var res = await _secretService.CreateSecret(splittedSecret);
        return res;
    }

    public async Task<SecretWithKey> GetSecret(string key)
    {
        var combinedSecret = _secretCombiner.CombineSecret(null);
        return combinedSecret;
    }

    public async Task UpdateSecret(string key, string secret)
    {
        throw new NotImplementedException();
    }



}