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
            var cache = new AssociativeCache<string, string>(1);
            cache.Add(new CacheItem<string, string>("key", "value"));
            
            //Assert
            Assert.AreSame("value", cache.Get("key"));
        }

        [Test]
        public void WhenItemNotInCache()
        {
            //Setup
            var cache = new AssociativeCache<string, int>(1);
           
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get("anykey"));
        }
        
        
        [Test]
        public void AddTwoObjectToCache()
        {
            //Setup
            var cache = new AssociativeCache<string, string>(2);
           
            //Action
            cache.Add(new CacheItem<string, string>("key1", "value1"));
            cache.Add(new CacheItem<string, string>("asdf", "value2"));

            //Assert
            Assert.AreSame("value1", cache.Get("key1"));
            Assert.AreSame("value2", cache.Get("asdf"));
        }

        [Test]
        public void LRUEvictionTest()
        {
            //Setup
            var cache = new AssociativeCache<int, string>(2, new LRUEvictionPolicy<int, string>(), new MyMd5HashAlgorithm<int>());
            cache.Add(new CacheItem<int, string>(2, "value1"));
            cache.Add(new CacheItem<int, string>(3, "value2"));
            
            //Action
            cache.Add(new CacheItem<int, string>(4, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get(2));
        }
        
        [Test]
        public void MRUEvictionTest()
        {
            //Setup
            var cache = new AssociativeCache<string, string>(2, new MRUEvictionPolicy<string, string>(), new MyMd5HashAlgorithm<string>());
            cache.Add(new CacheItem<string, string>("key1", "value1"));
            cache.Add(new CacheItem<string, string>("key2", "value2"));
            
            //Action
            cache.Add(new CacheItem<string, string>("key3", "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get("key2"));
        }

        
        // i am going to use int as key so that i can ensure it goes into the right bucket
        [Test]
        public void WhenUsingTwoSetCacheAndLRUEviction()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm<int>();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            var cache = new AssociativeCache<int, string>(2, 2, new LRUEvictionPolicy<int, string>(), fakeHashAlgorithm);
            cache.Add(new CacheItem<int, string>(1, "value1"));
            cache.Add(new CacheItem<int, string>(2, "value2"));
            
            //Action
            cache.Add(new CacheItem<int, string>(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get(1));
            Assert.AreSame("value2", cache.Get(2));
            Assert.AreSame("value3", cache.Get(3));
        }
        
        [Test]
        public void WhenUsingTwoSetCacheAndMRUEviction()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm<int>();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            var cache = new AssociativeCache<int, string>(2, 2, new MRUEvictionPolicy<int, string>(), fakeHashAlgorithm);
            cache.Add(new CacheItem<int, string>(1, "value1"));
            cache.Add(new CacheItem<int, string>(2, "value2"));
            
            //Action
            cache.Add(new CacheItem<int, string>(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get(2));
            Assert.AreSame("value3", cache.Get(3));
            Assert.AreSame("value1", cache.Get(1));
        }
        
        [Test]
        public void WhenUsingTwoSetCacheAndMRUEvictionAfterCallingOldUsed()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm<int>();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            var cache = new AssociativeCache<int, string>(2, 2, new MRUEvictionPolicy<int, string>(), fakeHashAlgorithm);
            cache.Add(new CacheItem<int, string>(1, "value1"));
            cache.Add(new CacheItem<int, string>(2, "value2"));
            var value = cache.Get(1);
            
            //Action
            cache.Add(new CacheItem<int, string>(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get(1));
            Assert.AreSame("value3", cache.Get(3));
            Assert.AreSame("value1", value);
            Assert.AreSame("value2", cache.Get(2));
        }
        
        [Test]
        public void WhenUsingTwoSetCacheAndMRUEvictionAfterCallingOldestAgaing()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm<int>();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 1);
            fakeHashAlgorithm.ObjectToHash(3, 1);
            
            var cache = new AssociativeCache<int, string>(2, 2, new LRUEvictionPolicy<int, string>(), fakeHashAlgorithm);
            cache.Add(new CacheItem<int, string>(1, "value1"));
            cache.Add(new CacheItem<int, string>(2, "value2"));
            var value = cache.Get(1);
            
            //Action
            cache.Add(new CacheItem<int, string>(3, "value3"));
            
            //Assert
            Assert.Throws<CacheMissException>(() => cache.Get(2));
            Assert.AreSame("value3", cache.Get(3));
            Assert.AreSame("value1", value);
            Assert.AreSame("value1", cache.Get(1));
        }
        
        [Test]
        public void TestAFullCache()
        {
            //Setup
            var fakeHashAlgorithm = new FakeHashAlgorithm<int>();
            fakeHashAlgorithm.ObjectToHash(1, 1);
            fakeHashAlgorithm.ObjectToHash(2, 2);
            fakeHashAlgorithm.ObjectToHash(3, 3);
            fakeHashAlgorithm.ObjectToHash(4, 4);
            var cache = new AssociativeCache<int, string>(2, 2, new MRUEvictionPolicy<int, string>(), fakeHashAlgorithm);
            
            //Action
            cache.Add(new CacheItem<int, string>(1, "value1"));
            cache.Add(new CacheItem<int, string>(2, "value2"));
            cache.Add(new CacheItem<int, string>(3, "value3"));
            cache.Add(new CacheItem<int, string>(4, "value4"));
            
            //Assert
            Assert.AreSame("value1", cache.Get(1));
            Assert.AreSame("value2", cache.Get(2));
            Assert.AreSame("value3", cache.Get(3));
            Assert.AreSame("value4", cache.Get(4));
        }
    }
}