namespace Straonit.HighEdge.Core.Distribution;

public interface IRollBack
{
    Task RollBackUpdate(List<OldValue> oldValues,string key);
    Task RollBackCreate(List<string> nodes, string key);
    Task RollBackDelete(List<OldValue> oldValues, string key);
}