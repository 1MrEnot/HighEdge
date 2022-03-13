namespace Straonit.HighEdge.Core.NodeRestoration;

using SplitSecret;

public interface INodeCommandSaver
{
    public Task WriteCreateCommand(PartOfSecret partOfSecret, string nodeUrl);

    public Task WriteUpdateCommand(PartOfSecret partOfSecret, string nodeUrl);

    public Task WriteDeleteCommand(string key, string nodeUrl);
}