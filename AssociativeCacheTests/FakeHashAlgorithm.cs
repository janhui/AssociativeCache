using System.Collections.Generic;
using AssociativeCache;

namespace AssociativeCacheTests
{
    public class FakeHashAlgorithm : IHashAlgorithm
    {
        private Dictionary<object, int> fakeValues;

        public FakeHashAlgorithm()
        {
            fakeValues = new Dictionary<object, int>();
        }
        
        public int Hash(object itemToHash)
        {
            if (fakeValues.ContainsKey(itemToHash))
            {
                return fakeValues[itemToHash];
            }
            throw new CacheMissException(-1);
        }

        public void ObjectToHash(object itemToHash, int hashValues)
        {
            fakeValues.Add(itemToHash, hashValues);
        }
    }
}