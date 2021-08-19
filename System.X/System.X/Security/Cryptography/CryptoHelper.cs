namespace System.Security.Cryptography
{
    public sealed class CryptoHelper
    {

        public readonly byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        public readonly byte[] aesIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        byte[] GetHashBytes(string value, global::System.Security.Cryptography.HashAlgorithm hashAlgorithm)
        {
            char[] chars = value.ToCharArray();
            byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(chars, 0, chars.Length);
            return hashAlgorithm.ComputeHash(buffer, 0, buffer.Length);
        }
        byte[] GetTransformBytes(byte[] buffer, global::System.Security.Cryptography.ICryptoTransform transform)
        {
            using (var ms = new global::System.IO.MemoryStream())
            {
                using (var cs = new global::System.Security.Cryptography.CryptoStream(ms, transform, global::System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        //对称加解密 DES、IDEA、RC2、RC4、SKIPJACK、RC5、AES
        public byte[] EncryptDES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.DESCryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateEncryptor(rgbKey, rgbIV));
            }
        }
        public string EncryptDES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(value);
            return global::System.Convert.ToBase64String(EncryptDES(buffer, rgbKey, rgbIV));
        }
        public byte[] DecryptDES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.DESCryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateDecryptor(rgbKey, rgbIV));
            }
        }
        public string DecryptDES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(DecryptDES(buffer, rgbKey, rgbIV));
        }

        public byte[] EncryptDES3(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.TripleDESCryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateEncryptor(rgbKey, rgbIV));
            }
        }
        public string EncryptDES3(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(value);
            return global::System.Convert.ToBase64String(EncryptDES3(buffer, rgbKey, rgbIV));
        }
        public byte[] DecryptDES3(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.TripleDESCryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateDecryptor(rgbKey, rgbIV));
            }
        }
        public string DecryptDES3(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(DecryptDES3(buffer, rgbKey, rgbIV));
        }

        public byte[] EncryptAES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.AesCryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateEncryptor(rgbKey, rgbIV));
            }
        }
        public string EncryptAES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(value);
            return global::System.Convert.ToBase64String(EncryptAES(buffer, rgbKey, rgbIV));
        }
        public byte[] DecryptAES(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.AesCryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateDecryptor(rgbKey, rgbIV));
            }
        }
        public string DecryptAES(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(DecryptAES(buffer, rgbKey, rgbIV));
        }

        public byte[] EncryptRC2(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.RC2CryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateEncryptor(rgbKey, rgbIV));
            }
        }
        public string EncryptRC2(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Text.Encoding.UTF8.GetBytes(value);
            return global::System.Convert.ToBase64String(EncryptRC2(buffer, rgbKey, rgbIV));
        }
        public byte[] DecryptRC2(byte[] value, byte[] rgbKey, byte[] rgbIV)
        {
            using (var provider = new global::System.Security.Cryptography.RC2CryptoServiceProvider())
            {
                return GetTransformBytes(value, provider.CreateDecryptor(rgbKey, rgbIV));
            }
        }
        public string DecryptRC2(string value, byte[] rgbKey, byte[] rgbIV)
        {
            byte[] buffer = global::System.Convert.FromBase64String(value);
            return global::System.Text.Encoding.UTF8.GetString(DecryptRC2(buffer, rgbKey, rgbIV));
        }

        public string MD5(string value)
        {
            using (var provider = new global::System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                return ToBitString(GetHashBytes(value, provider));
            }
        }
        public string MD5(global::System.IO.Stream inputStream)
        {
            using (var provider = new global::System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                return ToBitString(provider.ComputeHash(inputStream));
            }
        }
        public string SHA1(string value)
        {
            using (var provider = new global::System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return ToBitString(GetHashBytes(value, provider));
            }
        }
        public string SHA1(global::System.IO.Stream inputStream)
        {
            using (var provider = new global::System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return ToBitString(provider.ComputeHash(inputStream));
            }
        }
        public string SHA256(string value)
        {
            using (var provider = new global::System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                return ToBitString(GetHashBytes(value, provider));
            }
        }
        public string SHA256(global::System.IO.Stream inputStream)
        {
            using (var provider = new global::System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                return ToBitString(provider.ComputeHash(inputStream));
            }
        }
        public string SHA384(string value)
        {
            using (var provider = new global::System.Security.Cryptography.SHA384CryptoServiceProvider())
            {
                return ToBitString(GetHashBytes(value, provider));
            }
        }
        public string SHA384(global::System.IO.Stream inputStream)
        {
            using (var provider = new global::System.Security.Cryptography.SHA384CryptoServiceProvider())
            {
                return ToBitString(provider.ComputeHash(inputStream));
            }
        }
        public string SHA512(string value)
        {
            using (var provider = new global::System.Security.Cryptography.SHA512CryptoServiceProvider())
            {
                return ToBitString(GetHashBytes(value, provider));
            }
        }
        public string SHA512(global::System.IO.Stream inputStream)
        {
            using (var provider = new global::System.Security.Cryptography.SHA512CryptoServiceProvider())
            {
                return ToBitString(provider.ComputeHash(inputStream));
            }
        }



        //非对称加解密
        public string EncryptRSA(string value, string publicKey)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                return Convert.ToBase64String(provider.Encrypt(global::System.Text.Encoding.UTF8.GetBytes(value), false));
            }
        }
        public string DecryptRSA(string value, string privateKey)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return global::System.Text.Encoding.UTF8.GetString(provider.Decrypt(Convert.FromBase64String(value), false));
            }
        }

        public string SignRSA(string value, string privateKey, global::System.Security.Cryptography.HashAlgorithm hashAlgorithm)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return Convert.ToBase64String(provider.SignData(global::System.Text.Encoding.UTF8.GetBytes(value), hashAlgorithm));
            }
        }
        public bool VerifySignRSA(string value, string sign, string publicKey, global::System.Security.Cryptography.HashAlgorithm hashAlgorithm)
        {
            using (var provider = new global::System.Security.Cryptography.RSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                return provider.VerifyData(global::System.Text.Encoding.UTF8.GetBytes(value), hashAlgorithm, Convert.FromBase64String(sign));
            }
        }

        //一般用于数字签名和认证
        public string SignDSA(string value, string privateKey)
        {
            using (var provider = new global::System.Security.Cryptography.DSACryptoServiceProvider())
            {
                provider.FromXmlString(privateKey);
                return global::System.Text.Encoding.UTF8.GetString(provider.CreateSignature(Convert.FromBase64String(value)));
            }
        }
        public bool VerifySignDSA(string value, string sign, string publicKey)
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