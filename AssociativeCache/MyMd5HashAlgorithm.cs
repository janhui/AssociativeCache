using System;
using System.Security.Cryptography;
using System.Text;

namespace AssociativeCache
{
    public class MyMd5HashAlgorithm : IHashAlgorithm
    {
        public int Hash(object itemToHash)
        {
            var md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(itemToHash.ToString()));
            return Math.Abs(BitConverter.ToInt32(hashed, 0));
        }
    }
    
    public interface IHashAlgorithm
    {
        int Hash(object itemToHash);
    }
}