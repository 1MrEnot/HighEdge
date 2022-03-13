namespace Straonit.HighEdge.Core.Distribution;

public interface IRollBack
{
    Task RollBackUpdate(IEnumerable<OldValue> oldValues,string key);
    Task RollBackCreate(IEnumerable<string> nodes, string key);
    Task RollBackDelete(IEnumerable<OldValue> oldValues, string key);
}