namespace Straonit.HighEdge.Core.Distribution;

public interface IRollBack
{
    Task RollBackUpdate(List<OldValue> oldValues);
    Task RollBackCreate(List<string> nodes, string key);
}