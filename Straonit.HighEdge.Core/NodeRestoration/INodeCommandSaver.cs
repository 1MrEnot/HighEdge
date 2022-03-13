namespace Straonit.HighEdge.Core.NodeRestoration;

using SplitSecret;

public interface INodeCommandSaver
{
    public Task WriteCreateCommand(string key, PartOfSecret partOfSecret, string nodeUrl);

    public Task WriteDeleteCommand(string key, string nodeUrl);
}