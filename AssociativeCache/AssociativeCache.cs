using System;

namespace AssociativeCache
{
    public class AssociativeCache<K, V> 
    {
        private readonly int _sets;
        private readonly int _entries;
        private readonly IEvictionPolicy<K, V> _evictionPolicy;
        private readonly IHashAlgorithm<K> _hashAlgorithm;
        private readonly CacheItem<K, V>[] _cache;

        public AssociativeCache(int sets) : this(sets, 1, new LRUEvictionPolicy<K, V>(), new MyMd5HashAlgorithm<K>())
        {
        }
        
        public AssociativeCache(int sets, IEvictionPolicy<K, V> evictionPolicy, IHashAlgorithm<K> hashAlgorithm)
        : this(sets, 1, evictionPolicy, hashAlgorithm)
        {
        }
        
        public AssociativeCache(int sets, int entries, IEvictionPolicy<K, V> evictionPolicy, IHashAlgorithm<K> hashAlgorithm)
        {
            _sets = sets;
            _entries = entries;
            _evictionPolicy = evictionPolicy;
            _hashAlgorithm = hashAlgorithm;
            _cache = new CacheItem<K, V>[sets * entries];
        }

        public void Add(CacheItem<K, V> cacheItem)
        {
            var startIndex = (_hashAlgorithm.Hash(cacheItem.Key) % _sets) * _entries;
            var added = false;
            for (var slot = startIndex; slot < startIndex + _entries; slot++)
            {
                if (_cache[slot] == null)
                {
                    _cache[slot] = cacheItem;
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                var slot =_evictionPolicy.Evict(_cache, startIndex, _entries);
                _cache[slot] = cacheItem;
            }
        }

        public V Get(K key)
        {
            try
            {
                var startIndex = (_hashAlgorithm.Hash(key) % _sets) * _entries;
                for (var slot = startIndex; slot < startIndex + _entries; slot++)
                {
                    if (_cache[slot].Key.Equals(key))
                    {
                        var item = _cache[slot];
                        item.UpdatedTime = DateTime.Now;
                        return item.Value;
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }

            throw new CacheMissException(key);
        }
    }
}