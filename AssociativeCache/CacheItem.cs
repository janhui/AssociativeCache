using System;

namespace AssociativeCache
{
    public class CacheItem<K, V> 
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public DateTime UpdatedTime { get; set; }

        public CacheItem(K key, V value)
        {
            Key = key;
            Value = value;
            UpdatedTime = DateTime.Now;
        }
    }
}