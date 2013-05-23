using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections;

namespace Encryptor
{
    public class EncryptorAgent
    {
        public static byte[] Lock(string str, out byte[] iv)
        {
            return Lock(_key, str, out iv);
        }

        public static string Unlock(byte[] code, byte[] iv)
        {
            return Unlock(_key, code, iv);
        }

        public static byte[] Lock(byte[] key, string str, out byte[] iv)
        {
            var aes = new RijndaelManaged();
            aes.GenerateIV();
            iv = aes.IV;
            var memstrm = new MemoryStream();
            var csw = new CryptoStream(memstrm, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            csw.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
            csw.FlushFinalBlock();
            var cryptdata = memstrm.ToArray();
            csw.Close();
            memstrm.Close();
            return cryptdata;
        }

        public static string Unlock(byte[] key, byte[] code, byte[] iv)
        {
            var aes = new RijndaelManaged();
            var memstrm = new MemoryStream(code) {Position = 0};

            var csr = new CryptoStream(memstrm, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);

            var dataFragments = new ArrayList();
            var recv = 0;
            while (true)
            {
                var dataFragment = new byte[1024];
                var recvdThisFragment = csr.Read(dataFragment, 0, dataFragment.Length);
                if (recvdThisFragment == 0) break;
                dataFragments.Add(dataFragment);
                recv = recv + recvdThisFragment;
            }

            var data = new byte[dataFragments.Count * 1024];
            var idx = 0;

            //now let's build the entire data
            foreach (var t in dataFragments)
            {
                for (var j = 0; j < 1024; j++) data[idx++] = ((byte[])t)[j];
            }

            var newphrase = Encoding.ASCII.GetString(data, 0, recv);
            csr.Close();
            memstrm.Close();
            return newphrase;
        }

        // ReSharper disable InconsistentNaming
        private readonly static byte[] _key = { 0xA6, 0x46, 0x10, 0xF1, 0xEA, 0x16, 0x51, 0xA0, 0xB2, 0x41, 0x27, 0x5C, 0x23, 0x9C, 0xF0, 0xDD };
        // ReSharper restore InconsistentNaming
    }
}

