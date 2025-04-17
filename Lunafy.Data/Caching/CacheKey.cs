namespace Lunafy.Data.Caching;

public class CacheKey
{
    public string Prefix { get; private set; }
    public string Key { get; private set; }

    public CacheKey(string key, string prefix)
    {
        Key = key;
        Prefix = prefix;
    }
}