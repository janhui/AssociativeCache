using System;

namespace AssociativeCache
{
    public class AssociativeCache
    {
        private readonly int _sets;
        private readonly int _entries;
        private readonly IEvictionPolicy _evictionPolicy;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly CacheItem[] _cache;

        public AssociativeCache(int sets) : this(sets, 1, new LRUEvictionPolicy(), new MyMd5HashAlgorithm())
        {
        }
        
        public AssociativeCache(int sets, IEvictionPolicy evictionPolicy, IHashAlgorithm hashAlgorithm)
        : this(sets, 1, evictionPolicy, hashAlgorithm)
        {
        }
        
        public AssociativeCache(int sets, int entries, IEvictionPolicy evictionPolicy, IHashAlgorithm hashAlgorithm)
        {
            _sets = sets;
            _entries = entries;
            _evictionPolicy = evictionPolicy;
            _hashAlgorithm = hashAlgorithm;
            _cache = new CacheItem[sets * entries];
        }

        public void Add(CacheItem cacheItem)
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

        public object GetKey(object key)
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