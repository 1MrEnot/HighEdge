namespace Straonit.HighEdge.Core;

using Configuration;
using Distribution;
using SplitSecret;

public class DistributedSecretSerivce
{
    private readonly ISecretSplitter _secretSplitter;
    private readonly ISecretCombiner _secretCombiner;
    private readonly ISecretService _secretService;
    private readonly ClusterConfig _clusterConfig;

    public DistributedSecretSerivce(ISecretSplitter secretSplitter, ISecretCombiner secretCombiner,
        ISecretService secretService, ClusterConfig clusterConfig)
    {
        _secretSplitter = secretSplitter;
        _secretCombiner = secretCombiner;
        _secretService = secretService;
        _clusterConfig = clusterConfig;
    }

    public async Task<Response> SaveSecret(string key, string secret)
    {
        var secretWithKey = new SecretWithKey(key, secret);
        var splittingRequest = new SplittingRequest(secretWithKey,
            _clusterConfig.Nodes.Count, _clusterConfig.RequiredNodesCount);
        var splittedSecret = _secretSplitter.SplitSecret(splittingRequest);
        var res = await _secretService.CreateSecret(splittedSecret);
        return res;
    }

    public async Task<SecretWithKey> GetSecret(string key)
    {
        var response = await _secretService.GetSecret(key);

        if (response.PartOfSecrets.Count < _clusterConfig.RequiredNodesCount)
        {
            return new SecretWithKey()
            {
                UnWorkedNodes = response.UnWorkedNodes,
                NodesWithNotExistentKey = response.NodesWithNotExistentKey
            };
        }

        var combinedSecret = _secretCombiner.CombineSecret(new SplittedSecret(key, response.PartOfSecrets));

        return combinedSecret;
    }
}