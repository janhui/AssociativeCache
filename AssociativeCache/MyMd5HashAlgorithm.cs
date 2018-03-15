using System;
using System.Security.Cryptography;
using System.Text;

namespace AssociativeCache
{
    public class MyMd5HashAlgorithm<T> : IHashAlgorithm<T> 
    {
        public int Hash(object itemToHash)
        {
            var md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(itemToHash.ToString()));
            return Math.Abs(BitConverter.ToInt32(hashed, 0));
        }
    }
    
    public interface IHashAlgorithm<T> 
    {
        int Hash(object itemToHash);
    }
}