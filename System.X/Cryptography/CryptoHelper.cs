using System.Threading.Tasks;

namespace System.X.Cryptography
{
    public sealed class CryptoHelper
    {
        private CryptoHelper() { }
        internal static readonly CryptoHelper Instance = new CryptoHelper();

        readonly byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        readonly byte[] aesIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        byte[] GetTransformBytes(byte[] buffer, global::System.Security.Cryptography.ICryptoTransform transform)
        {
            using (transform)
            using (var ms = new global::System.IO.MemoryStream())
            using (var cs = new global::System.Security.Cryptography.CryptoStream(ms, transform, global::System.Security.Cryptography.CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
        byte[] Encrypt(byte[] value, byte[] rgbKey, byte[] rgbIV, Security.Cryptography.SymmetricAlgorithm provider)
        {
            return GetTransformBytes(value, provider.CreateEncryptor(rgbKey, rgbIV));
        }
        byte[] Decrypt(byte[] value, byte[] rgbKey, byte[] rgbIV, Security.Cryptography.SymmetricAlgorithm provider)
        {
            return GetTransformBytes(value, provider.CreateDecryptor(rgbKey, rgbIV));
        }

        //对称加解密 DES、IDEA、RC2、RC4、SKIPJACK、RC5、AES
        public byte[] DES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Encrypt(value, rgbKey, rgbIV, new global::System.Security.Cryptography.DESCryptoServiceProvider());
        }
        public string DES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            return global::System.Convert.ToBase64String(DES(global::System.Text.Encoding.UTF8.GetBytes(value), rgbKey, rgbIV));
        }
        public byte[] DESDecrypt(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Decrypt(value, rgbKey, rgbIV, new global::System.Security.Cryptography.DESCryptoServiceProvider());
        }
        public string DESDecrypt(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(DESDecrypt(buffer, rgbKey, rgbIV));
        }

        public byte[] TripleDES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Encrypt(value, rgbKey, rgbIV, new global::System.Security.Cryptography.TripleDESCryptoServiceProvider());
        }
        public string TripleDES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            return global::System.Convert.ToBase64String(TripleDES(global::System.Text.Encoding.UTF8.GetBytes(value), rgbKey, rgbIV));
        }
        public byte[] TripleDESDecrypt(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Decrypt(value, rgbKey, rgbIV, new global::System.Security.Cryptography.TripleDESCryptoServiceProvider());
        }
        public string TripleDESDecrypt(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(TripleDESDecrypt(buffer, rgbKey, rgbIV));
        }

        public byte[] AES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Encrypt(value, rgbKey, rgbIV, new global::System.Security.Cryptography.AesCryptoServiceProvider());
        }
        public string AES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            return global::System.Convert.ToBase64String(AES(global::System.Text.Encoding.UTF8.GetBytes(value), rgbKey, rgbIV));
        }
        public byte[] AESDecrypt(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            return Decrypt(value, rgbKey, rgbIV, new global::System.Security.Cryptography.AesCryptoServiceProvider());
        }
        public string AESDecrypt(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(AESDecrypt(buffer, rgbKey, rgbIV));
        }

        //public byte[] Encrypt(System.X.Enums.SymAlgs algorithm, byte[] bytes, byte[] rgbKey, byte[] rgbIV)
        //{
        //    using (var provider = Security.Cryptography.SymmetricAlgorithm.Create(algorithm.ToString()))
        //    using (var transform = provider.CreateEncryptor(rgbKey, rgbIV))
        //    using (var ms = new global::System.IO.MemoryStream())
        //    using (var cs = new global::System.Security.Cryptography.CryptoStream(ms, transform, global::System.Security.Cryptography.CryptoStreamMode.Write))
        //    {
        //        cs.Write(bytes, 0, bytes.Length);
        //        cs.FlushFinalBlock();
        //        return ms.ToArray();
        //    }
        //}
        //public byte[] Decrypt(System.X.Enums.SymAlgs algorithm, byte[] bytes, byte[] rgbKey, byte[] rgbIV)
        //{
        //    using (var provider = Security.Cryptography.SymmetricAlgorithm.Create(algorithm.ToString()))
        //    using (var transform = provider.CreateDecryptor(rgbKey, rgbIV))
        //    using (var ms = new global::System.IO.MemoryStream())
        //    using (var cs = new global::System.Security.Cryptography.CryptoStream(ms, transform, global::System.Security.Cryptography.CryptoStreamMode.Write))
        //    {
        //        cs.Write(bytes, 0, bytes.Length);
        //        cs.FlushFinalBlock();
        //        return ms.ToArray();
        //    }
        //}
        //public string Encrypt(System.X.Enums.SymAlgs algorithm, string value, string rgbKey, string rgbIV)
        //{
        //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
        //    byte[] rgbKeyArray = System.Text.Encoding.UTF8.GetBytes(rgbKey);
        //    byte[] rgbIVArray = System.Text.Encoding.UTF8.GetBytes(rgbIV);
        //    return global::System.Convert.ToBase64String(Encrypt(algorithm, buffer, rgbKeyArray, rgbIVArray));
        //}
        //public string Decrypt(System.X.Enums.SymAlgs algorithm, string value, string rgbKey, string rgbIV)
        //{
        //    byte[] buffer = global::System.Convert.FromBase64String(value);
        //    byte[] rgbKeyArray = System.Text.Encoding.UTF8.GetBytes(rgbKey);
        //    byte[] rgbIVArray = System.Text.Encoding.UTF8.GetBytes(rgbIV);
        //    return System.Text.Encoding.UTF8.GetString(Decrypt(algorithm, buffer, rgbKeyArray, rgbIVArray));
        //}

        public string MD5(string value)
        {
            return Hash(Enums.HashAlgs.MD5, value);
        }
        public string SHA1(string value)
        {
            return Hash(Enums.HashAlgs.SHA1, value);
        }
        public string SHA1(byte[] bytes)
        {
            return Hash(Enums.HashAlgs.SHA1, bytes);
        }
        public string SHA1(global::System.IO.Stream inputStream)
        {
            return Hash(Enums.HashAlgs.SHA1, inputStream);
        }
        public string SHA256(string value)
        {
            return Hash(Enums.HashAlgs.SHA256, value);
        }
        public string SHA256(byte[] bytes)
        {
            return Hash(Enums.HashAlgs.SHA256, bytes);
        }
        public string SHA256(global::System.IO.Stream inputStream)
        {
            return Hash(Enums.HashAlgs.SHA256, inputStream);
        }
        public string SHA512(string value)
        {
            return Hash(Enums.HashAlgs.SHA512, value);
        }
        string Hash(Enums.HashAlgs hash, byte[] bytes)
        {
            using (var provider = Security.Cryptography.HashAlgorithm.Create(hash.ToString()))
                return ToBitString(provider.ComputeHash(bytes));
        }
        string Hash(Enums.HashAlgs hash, string value)
        {
            using (var provider = Security.Cryptography.HashAlgorithm.Create(hash.ToString()))
            {
                char[] chars = value.ToCharArray();
                byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(chars, 0, chars.Length);
                return ToBitString(provider.ComputeHash(buffer, 0, buffer.Length));
            }
        }
        string Hash(Enums.HashAlgs hash, global::System.IO.Stream inputStream)
        {
            using (var provider = Security.Cryptography.HashAlgorithm.Create(hash.ToString()))
                return ToBitString(provider.ComputeHash(inputStream));
        }

        //非对称加解密
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

        public string RSASign(string value, string privateKey, System.X.Enums.HashAlgs hash)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return Convert.ToBase64String(provider.SignData(global::System.Text.Encoding.UTF8.GetBytes(value), global::System.Security.Cryptography.HashAlgorithm.Create(hash.ToString())));
            }
        }
        public bool RSAVerifySign(string value, string sign, string publicKey, System.X.Enums.HashAlgs hash)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                return provider.VerifyData(global::System.Text.Encoding.UTF8.GetBytes(value), global::System.Security.Cryptography.HashAlgorithm.Create(hash.ToString()), Convert.FromBase64String(sign));
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