using AssociativeCache;
using NUnit.Framework;

namespace AssociativeCacheTests
{
    public class CacheTests
    {
        [Test]
        public void AddOneObjectToCache()
        {
            //Setup
            var cache = new AssociativeCache.AssociativeCache(1);
            cache.Add(new CacheItem("key", "value"));
            
            //Assert
            Assert.AreSame("value", (string)cache.GetKey("key"));
        }

        [Test]
        public void WhenItemNotInCache()
        {
            //Setup
            var cache = new AssociativeCache.AssociativeCache(1);
           
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey("anykey"));
        }
        
        
        [Test]
        public void AddTwoObjectToCache()
        {
            //Setup
            var cache = new AssociativeCache.AssociativeCache(2);
           
            //Action
            cache.Add(new CacheItem("key1", "value1"));
            cache.Add(new CacheItem("asdf", "value2"));

            //Assert
            Assert.AreSame("value1", (string)cache.GetKey("key1"));
            Assert.AreSame("value2", (string)cache.GetKey("asdf"));
        }

        [Test]
        public void LRUEvictionTest()
        {
            //Setup
            var cache = new AssociativeCache.AssociativeCache(2, new LRUEvictionPolicy(), new MyMd5HashAlgorithm());
            cache.Add(new CacheItem(2, "value1"));
            cache.Add(new CacheItem(3, "value2"));
            
            //Action
            cache.Add(new CacheItem(4, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey("2"));
        }
        
        [Test]
        public void MRUEvictionTest()
        {
            //Setup
            var cache = new AssociativeCache.AssociativeCache(2, new MRUEvictionPolicy(), new MyMd5HashAlgorithm());
            cache.Add(new CacheItem("key1", "value1"));
            cache.Add(new CacheItem("key2", "value2"));
            
            //Action
            cache.Add(new CacheItem("key3", "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey("key2"));
        }

        
        // i am going to use int as key so that i can ensure it goes into the right bucket
        [Test]
        public void WhenUsingTwoSetCacheAndLRUEviction()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            var cache = new AssociativeCache.AssociativeCache(2, 2, new LRUEvictionPolicy(), fakeHashAlgorithm);
            cache.Add(new CacheItem(1, "value1"));
            cache.Add(new CacheItem(2, "value2"));
            
            //Action
            cache.Add(new CacheItem(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey(1));
            Assert.AreSame("value2", cache.GetKey(2));
            Assert.AreSame("value3", cache.GetKey(3));
        }
        
        [Test]
        public void WhenUsingTwoSetCacheAndMRUEviction()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            var cache = new AssociativeCache.AssociativeCache(2, 2, new MRUEvictionPolicy(), fakeHashAlgorithm);
            cache.Add(new CacheItem(1, "value1"));
            cache.Add(new CacheItem(2, "value2"));
            
            //Action
            cache.Add(new CacheItem(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey(2));
            Assert.AreSame("value3", cache.GetKey(3));
            Assert.AreSame("value1", cache.GetKey(1));
        }
        
        [Test]
        public void WhenUsingTwoSetCacheAndMRUEvictionAfterCallingOldUsed()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            var cache = new AssociativeCache.AssociativeCache(2, 2, new MRUEvictionPolicy(), fakeHashAlgorithm);
            cache.Add(new CacheItem(1, "value1"));
            cache.Add(new CacheItem(2, "value2"));
            var value = cache.GetKey(1);
            
            //Action
            cache.Add(new CacheItem(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey(1));
            Assert.AreSame("value3", cache.GetKey(3));
            Assert.AreSame("value1", value);
            Assert.AreSame("value2", cache.GetKey(2));
        }
        
        [Test]
        public void WhenUsingTwoSetCacheAndMRUEvictionAfterCallingOldestAgaing()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            
            var cache = new AssociativeCache.AssociativeCache(2, 2, new LRUEvictionPolicy(), fakeHashAlgorithm);
            cache.Add(new CacheItem(1, "value1"));
            cache.Add(new CacheItem(2, "value2"));
            var value = cache.GetKey(1);
            
            //Action
            cache.Add(new CacheItem(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.GetKey(2));
            Assert.AreSame("value3", cache.GetKey(3));
            Assert.AreSame("value1", value);
            Assert.AreSame("value1", cache.GetKey(1));
        }
        
        [Test]
        public void TestAFullCache()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 2);
            fakeHashAlgorithm.ObjectToHash(3, 3);
            fakeHashAlgorithm.ObjectToHash(4, 4);
            var cache = new AssociativeCache.AssociativeCache(2, 2, new MRUEvictionPolicy(), fakeHashAlgorithm);
            
            //Action
            cache.Add(new CacheItem(1, "value1"));
            cache.Add(new CacheItem(2, "value2"));
            cache.Add(new CacheItem(3, "value3"));
            cache.Add(new CacheItem(4, "value4"));
            
            //Assert
            Assert.AreSame("value1", cache.GetKey(1));
            Assert.AreSame("value2", cache.GetKey(2));
            Assert.AreSame("value3", cache.GetKey(3));
            Assert.AreSame("value4", cache.GetKey(4));
        }
    }
}