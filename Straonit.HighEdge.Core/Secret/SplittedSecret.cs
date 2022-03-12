namespace Straonit.HighEdge.Core.Secret;

public class SplittedSecret
{
    public SplittedSecret(string key, IEnumerable<PartOfSecret> valueParts)
    {
        Key = key;
        ValueParts = valueParts.ToArray();
    }

    public string Key { get; }

    public PartOfSecret[] ValueParts { get; }
}