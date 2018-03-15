using System;

namespace AssociativeCache
{
    public class CacheItem
    {
        public object Key { get; set; }
        public object Value { get; set; }
        public DateTime UpdatedTime { get; set; }

        public CacheItem(object key, object value)
        {
            Key = key;
            Value = value;
            UpdatedTime = DateTime.Now;
        }
    }
}