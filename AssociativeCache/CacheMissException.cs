using System;

namespace AssociativeCache
{
    public class CacheMissException : Exception
    {
        private readonly object _key;

        public CacheMissException(object key)
        {
            _key = key;
        }

        public override string Message => $"Cache Miss exception thrown for object {_key}";
    }
}