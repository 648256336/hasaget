using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CoreUtility
{
    /// <summary>
    ///     密码工具类
    /// </summary>
    public class CryptoUtils
    {
        /// <summary>
        ///     AES解密密钥
        /// </summary>
        private const string AesDecryptKey = "^&lt;kaolaxiu8&gt;+_*@";
        /// <summary>
        ///     DES解密密钥
        /// </summary>
        private const string DesEncryptKey = "ka0Iaxlu";

        /// <summary>
        /// 生成唯一编号
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GenUniqueNumber(int n)
        {
            const string keleyiStr = "0123456789";
            var rtn = new char[n];
            var gid = Guid.NewGuid();
            var ba = gid.ToByteArray();
            var result = String.Empty;
            for (var i = 0; i < n; i++)
            {
                rtn[i] = keleyiStr[((ba[i] + ba[(n + i)]) % 8)];
                result = result + rtn[i];
            }
            return result;
        }
        /// <summary>
        /// 生成唯一编号
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GenUniqueString(int n)
        {
            const string keleyiStr = "123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
            var rtn = new char[n];
            var gid = Guid.NewGuid();
            var ba = gid.ToByteArray();
            var result = String.Empty;
            for (var i = 0; i < n; i++)
            {
                rtn[i] = keleyiStr[((ba[i] + ba[(n + i)]) % 33)];
                result = result + rtn[i];
            }
            return result;
        }


        /// <summary>
        /// 生成唯一编号
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GenUniqueString(int n, string keleyiStr)
        {
            var rtn = new char[n];
            var gid = Guid.NewGuid();
            var ba = gid.ToByteArray();
            var result = String.Empty;
            for (var i = 0; i < n; i++)
            {
                rtn[i] = keleyiStr[(ba[i % ba.Length]) % keleyiStr.Length];
                result = result + rtn[i];
            }
            return result;
        }

        #region Base64加密解密

        /// <summary>
        ///     Base64加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encrypt(string input)
        {
            return Base64Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        ///     Base64加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符编码</param>
        /// <returns></returns>
        public static string Base64Encrypt(string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }

        /// <summary>
        ///     Base64解密
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <returns></returns>
        public static string Base64Decrypt(string input)
        {
            return Base64Decrypt(input, new UTF8Encoding());
        }

        /// <summary>
        ///     Base64解密
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Base64Decrypt(string input, Encoding encode)
        {
            try
            {
                return encode.GetString(Convert.FromBase64String(input));
            }
            catch (Exception)
            {
                return input;
            }
        }
        #endregion

        #region Hex
        public static byte[] HexToByte(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }
            return arr;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 87);

            //switch (hexType)
            //{
            //    case HexType.Uppercase:
            //            return val - (val < 58 ? 48 : 55);
            //    case HexType.Lowercase:
            //            return val - (val < 58 ? 48 : 87);
            //    case HexType.TwoCombined:
            //            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
            //    default:
            //        break;
            //}
            return 0;
        }
        public static string ByteToHex(byte[] Bdata)
        {
            try
            {
                return BitConverter.ToString(Bdata).Replace("-", "");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string HexToUtf8(string hex)
        {
            var bt = HexToByte(hex);
            return Encoding.UTF8.GetString(bt);
        }
        #endregion

        #region DES加密解密
        //默认密钥向量
        private static readonly byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DesEncrypt(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var dCSP = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DesEncrypt(string encryptString)
        {
            return DesEncrypt(encryptString, DesEncryptKey);
        }
        /// <summary>
        ///     DES加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="key">8位字符的密钥字符串</param>
        /// <param name="iv">8位字符的初始化向量字符串</param>
        /// <returns></returns>
        public static string DesEncrypt(string data, string key, string iv)
        {
            byte[] byKey = Encoding.ASCII.GetBytes(key);
            byte[] byIv = Encoding.ASCII.GetBytes(iv);

            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream();
            var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIv), CryptoStreamMode.Write);

            var sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        ///     DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
        /// <returns></returns>
        public static string DesDecrypt(string data, string key, string iv)
        {
            byte[] byKey = Encoding.ASCII.GetBytes(key);
            byte[] byIv = Encoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream(byEnc);
            var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIv), CryptoStreamMode.Read);
            var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }


        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DesDecrypt(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                var DCSP = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DesDecrypt(string decryptString)
        {
            return DesDecrypt(decryptString, DesEncryptKey);
        }
        #endregion

        #region MD5加密

        /// <summary>
        ///     MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string Md5Encrypt(string input)
        {
            return Md5Encrypt(input, new UTF8Encoding());
        }

        //public static string EncryptMd5(string password)
        //{
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(password, FormsAuthPasswordFormat.MD5.ToString());
        //}

        /// <summary>
        ///     MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Md5Encrypt(string input, Encoding encode)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encode.GetBytes(input));
            var sb = new StringBuilder(32);
            foreach (byte b in t)
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        ///     MD5对文件流加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Md5Encrypt(Stream stream)
        {
            MD5 md5Serv = MD5.Create();
            byte[] buffer = md5Serv.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (byte var in buffer)
                sb.Append(var.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        ///     MD5加密(返回16位加密串)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Md5Encrypt16(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            string result = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(input)), 4, 8);
            result = result.Replace("-", "");
            return result;
        }

        #endregion

        #region 3DES 加密解密

        /// <summary>
        ///     DES3加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Des3Encrypt(string data, string key)
        {
            var des = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// <summary>
        ///     DES3解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Des3Decrypt(string data, string key)
        {
            var des = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform desDecrypt = des.CreateDecryptor();

            byte[] buffer = Convert.FromBase64String(data);
            string result = Encoding.ASCII.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            return result;
        }

        #endregion

        #region AES加解密
        //默认密钥向量 
        private static readonly byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string HexAesDecrypt(string showText, string key, string iv)
        {
            byte[] cipherText = HexToByte(showText);
            return ByteAesDecrypt(cipherText, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
        }
        public static string HexAesDecrypt(string showText, byte[] key, byte[] iv)
        {
            byte[] cipherText = HexToByte(showText);
            return ByteAesDecrypt(cipherText, key, iv);
        }
        public static string ByteAesDecrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            //byte[] cipherText = Convert.ToByte(hex, 16);
            SymmetricAlgorithm aes = Rijndael.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptBytes = new byte[cipherText.Length];
            using (var ms = new MemoryStream(cipherText))
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.Read(decryptBytes, 0, decryptBytes.Length);
                    cs.Close();
                    ms.Close();
                }
            }
            Span<byte> arr = new Span<byte>(decryptBytes);
            //byte[] temp = null;
            int len = 0;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (arr[i] == 0)
                {
                    continue;
                }
                else
                {
                    len = i + 1;
                    //temp = new byte[i + 1];
                    break;
                }
            }

            //for (int i = 0; i < temp.Length; i++)
            //{
            //    if (arr[i] != 0)
            //    {
            //        temp[i] = arr[i];
            //    }
            //}
            return Encoding.UTF8.GetString(arr.Slice(0, len).ToArray());
        }
        public static string AESEncryptHex(string plainText, string key, string iv)
        {
            //分组加密算法
            SymmetricAlgorithm aes = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组 
            byte[] temp = inputByteArray;
            //设置密钥及密钥向量
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(temp, 0, temp.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();//得到加密后的字节数组
                    cs.Close();
                    ms.Close();
                }
            }
            return CryptoUtils.ByteToHex(cipherBytes);
        }
        public static string AESEncryptHex(string plainText, byte[] key, byte[] iv)
        {
            //分组加密算法
            SymmetricAlgorithm aes = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组 
            byte[] temp = inputByteArray;
            //设置密钥及密钥向量
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(temp, 0, temp.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();//得到加密后的字节数组
                    cs.Close();
                    ms.Close();
                }
            }
            return CryptoUtils.ByteToHex(cipherBytes);
        }

        public static string AESEncryptBase64(string plainText, byte[] key, byte[] iv)
        {
            //分组加密算法
            SymmetricAlgorithm aes = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组 
            byte[] temp = inputByteArray;
            //设置密钥及密钥向量
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(temp, 0, temp.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();//得到加密后的字节数组
                    cs.Close();
                    ms.Close();
                }
            }
            return Convert.ToBase64String(cipherBytes);
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            string plaintext;
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        #endregion

        #region hash
        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            using (SHA256Managed hashstring = new SHA256Managed())
            {
                byte[] hash = hashstring.ComputeHash(bytes);
                string hashString = string.Empty;
                foreach (byte x in hash) { hashString += String.Format("{0:x2}", x); }
                return hashString;
            }
        }
        #endregion
    }
}
