namespace Apps.Magento.Utils;

public readonly record struct Ucid(string ContentType, string Key)
{
    public override string ToString() => $"{ContentType}-{Key}";
}