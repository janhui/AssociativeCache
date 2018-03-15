using System;

namespace AssociativeCache
{
    public class LRUEvictionPolicy<K, V> : IEvictionPolicy<K,V> 
    {
        public int Evict(CacheItem<K, V>[] cacheItems, int startIndex, int entries)
        {
            var oldestCacheUsedTime = DateTime.MaxValue;
            var indexOfItemToEvict = -1;
            for (var i = startIndex; i < startIndex +entries; i++)
            {
                if (cacheItems[i].UpdatedTime < oldestCacheUsedTime)
                {
                    indexOfItemToEvict = i;
                    oldestCacheUsedTime = cacheItems[i].UpdatedTime;
                }
            }

            return indexOfItemToEvict;
        }
    }

    public class MRUEvictionPolicy<K, V> : IEvictionPolicy<K, V> 
    {
        public int Evict(CacheItem<K, V>[] cacheItems, int startIndex, int entries)
        {
            var newestItem = DateTime.MinValue;
            var indexOfItemToEvict = -1;
            for (int i = startIndex; i < startIndex +entries; i++)
            {
                if (cacheItems[i].UpdatedTime > newestItem)
                {
                    indexOfItemToEvict = i;
                    newestItem = cacheItems[i].UpdatedTime;
                }
            }
            
            return indexOfItemToEvict;
        }
    }
}