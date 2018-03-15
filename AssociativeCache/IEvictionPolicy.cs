namespace AssociativeCache
{
    public interface IEvictionPolicy<K, V> 
    {
        int Evict(CacheItem<K, V>[] cacheItems, int startIndex, int numberOfEntries);
    }
}