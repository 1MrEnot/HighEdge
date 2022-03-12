namespace Straonit.HighEdge.Core.SplitSecret;

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