using System;
using System.Security.Cryptography;
using System.Text;

namespace Encryptor
{
    public static class HashAgent
    {
        public static string GenerateGuidSalt()
        {
            return Guid.NewGuid().ToString();
        }
        public static string HashPasswordSHA1(string password, string salt)
        {
            return HashPassword(password, salt, new SHA1Cng());
        }
        public static string HashPasswordMD5(string password, string salt)
        {
            return HashPassword(password, salt, new MD5Cng());
        }

        private static string HashPassword(string password, string salt, HashAlgorithm hashAlgorithm)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (salt == null) throw new ArgumentNullException("salt");

            using (hashAlgorithm)
            {
                return BinaryToHex(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt)));
            }
        }
        private static string BinaryToHex(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            char[] hex = new char[checked(data.Length * 2)];

            for (int i = 0; i < data.Length; i++)
            {
                byte thisByte = data[i];
                hex[2 * i] = NibbleToHex((byte)(thisByte >> 4)); // high nibble
                hex[2 * i + 1] = NibbleToHex((byte)(thisByte & 0xf)); // low nibble
            }

            return new string(hex);
        }
        private static char NibbleToHex(byte nibble)
        {
            return (char)((nibble < 10) ? (nibble + '0') : (nibble - 10 + 'A'));
        }

        
        //public static string HashPasswordSHA1(string password, string salt)
        //{
        //    #pragma warning disable 612,618
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, FormsAuthPasswordFormat.SHA1.ToString()); 
        //    #pragma warning restore 612,618
        //}

        //public static string HashPasswordMD5(string password, string salt)
        //{
        //    #pragma warning disable 612,618
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, FormsAuthPasswordFormat.MD5.ToString());
        //    #pragma warning restore 612,618
        //}
    }
}
