namespace AssociativeCache
{
    public interface IEvictionPolicy
    {
        int Evict(CacheItem[] cacheItems, int startIndex, int numberOfEntries);
    }
}