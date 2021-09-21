using System.Threading.Tasks;

namespace System.X.Cryptography
{
    public sealed class CryptoHelper
    {
        private CryptoHelper() { }
        internal static readonly CryptoHelper Instance = new CryptoHelper();

        readonly byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        readonly byte[] aesIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public byte[] DES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Encrypt("DES", value, rgbKey, rgbIV);
        }
        public string DES(string value, string rgbKey, string rgbIV)
        {
            return Encrypt("DES", value, rgbKey, rgbIV);
        }
        public byte[] DESDecrypt(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Decrypt("DES", value, rgbKey, rgbIV);
        }
        public string DESDecrypt(string value, string rgbKey, string rgbIV)
        {
            return Decrypt("DES", value, rgbKey, rgbIV);
        }

        public byte[] TripleDES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Encrypt("TripleDES", value, rgbKey, rgbIV);
        }
        public string TripleDES(string value, string rgbKey, string rgbIV)
        {
            return Encrypt("TripleDES", value, rgbKey, rgbIV);
        }
        public byte[] TripleDESDecrypt(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Decrypt("TripleDES", value, rgbKey, rgbIV);
        }
        public string TripleDESDecrypt(string value, string rgbKey, string rgbIV)
        {
            return Decrypt("TripleDES", value, rgbKey, rgbIV);
        }

        public byte[] AES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Encrypt("Aes", value, rgbKey, rgbIV);
        }
        public string AES(string value, string rgbKey, string rgbIV)
        {
            return Encrypt("Aes", value, rgbKey, rgbIV);
        }
        public byte[] AESDecrypt(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Decrypt("Aes", value, rgbKey, rgbIV);
        }
        public string AESDecrypt(string value, string rgbKey, string rgbIV)
        {
            return Decrypt("Aes", value, rgbKey, rgbIV);
        }

        byte[] Encrypt(string algName, byte[] bytes, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = Security.Cryptography.SymmetricAlgorithm.Create(algName))
            using (var transform = provider.CreateEncryptor(rgbKey, rgbIV))
            using (var ms = new global::System.IO.MemoryStream())
            using (var cs = new global::System.Security.Cryptography.CryptoStream(ms, transform, global::System.Security.Cryptography.CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
        byte[] Decrypt(string algName, byte[] bytes, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = Security.Cryptography.SymmetricAlgorithm.Create(algName))
            using (var transform = provider.CreateDecryptor(rgbKey, rgbIV))
            using (var ms = new global::System.IO.MemoryStream())
            using (var cs = new global::System.Security.Cryptography.CryptoStream(ms, transform, global::System.Security.Cryptography.CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
        string Encrypt(string algName, string value, string rgbKey, string rgbIV)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] rgbKeyArray = System.Text.Encoding.UTF8.GetBytes(rgbKey);
            byte[] rgbIVArray = System.Text.Encoding.UTF8.GetBytes(rgbIV);
            return global::System.Convert.ToBase64String(Encrypt(algName, buffer, rgbKeyArray, rgbIVArray));
        }
        string Decrypt(string algName, string value, string rgbKey, string rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            byte[] rgbKeyArray = System.Text.Encoding.UTF8.GetBytes(rgbKey);
            byte[] rgbIVArray = System.Text.Encoding.UTF8.GetBytes(rgbIV);
            return System.Text.Encoding.UTF8.GetString(Decrypt(algName, buffer, rgbKeyArray, rgbIVArray));
        }

        public string MD5(string value)
        {
            return Hash("MD5", value);
        }
        public string SHA1(string value)
        {
            return Hash("SHA1", value);
        }
        public string SHA1(byte[] bytes)
        {
            return Hash("SHA1", bytes);
        }
        public string SHA1(global::System.IO.Stream inputStream)
        {
            return Hash("SHA1", inputStream);
        }
        public string SHA256(string value)
        {
            return Hash("SHA256", value);
        }
        public string SHA256(byte[] bytes)
        {
            return Hash("SHA256", bytes);
        }
        public string SHA256(global::System.IO.Stream inputStream)
        {
            return Hash("SHA512", inputStream);
        }
        public string SHA512(string value)
        {
            return Hash("SHA512", value);
        }
        string Hash(string hashName, byte[] bytes)
        {
            using (var provider = Security.Cryptography.HashAlgorithm.Create(hashName))
                return ToBitString(provider.ComputeHash(bytes));
        }
        string Hash(string hashName, string value)
        {
            using (var provider = Security.Cryptography.HashAlgorithm.Create(hashName))
            {
                char[] chars = value.ToCharArray();
                byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(chars, 0, chars.Length);
                return ToBitString(provider.ComputeHash(buffer, 0, buffer.Length));
            }
        }
        string Hash(string hashName, global::System.IO.Stream inputStream)
        {
            using (var provider = Security.Cryptography.HashAlgorithm.Create(hashName))
                return ToBitString(provider.ComputeHash(inputStream));
        }


        public string RSA(string value, string publicKey)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                return Convert.ToBase64String(provider.Encrypt(global::System.Text.Encoding.UTF8.GetBytes(value), false));
            }
        }
        public string RSADecrypt(string value, string privateKey)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return global::System.Text.Encoding.UTF8.GetString(provider.Decrypt(Convert.FromBase64String(value), false));
            }
        }

        public string RSASign(string value, string privateKey, string hashName)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return Convert.ToBase64String(provider.SignData(global::System.Text.Encoding.UTF8.GetBytes(value), global::System.Security.Cryptography.HashAlgorithm.Create(hashName)));
            }
        }
        public bool RSAVerifySign(string value, string sign, string publicKey, string hashName)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                return provider.VerifyData(global::System.Text.Encoding.UTF8.GetBytes(value), global::System.Security.Cryptography.HashAlgorithm.Create(hashName), Convert.FromBase64String(sign));
            }
        }

        public string DSASign(string value, string privateKey)
        {
            using (var provider = new global::System.Security.Cryptography.DSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return global::System.Text.Encoding.UTF8.GetString(provider.CreateSignature(Convert.FromBase64String(value)));
            }
        }
        public bool DSAVerifySign(string value, string sign, string publicKey)
        {
            using (var provider = new global::System.Security.Cryptography.DSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                return provider.VerifyData(global::System.Text.Encoding.UTF8.GetBytes(value), Convert.FromBase64String(sign));
            }
        }

        internal string ToBitString(byte[] value)
        {
            int length = value.Length * 2;
            char[] chars = new char[length];
            for (int i = 0, j = 0; i < length; i += 2, j++)
            {
                byte tmp = value[j];
                chars[i] = ToHexValue(tmp / 0x10);
                chars[i + 1] = ToHexValue(tmp % 0x10);
            }
            return new string(chars, 0, length);
        }
        internal static char ToHexValue(int value)
        {
            if (value < 10)
            {
                return (char)(value + 0x30);
            }
            return (char)((value - 10) + 0x41);
        }
    }
}