using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace DataHelpers
{
    public static class Extensions
    {
        private const string Key = "SecureAutomatedEndToEndP";
        private const string IV = "SAEPNO01";
        public static string GetNotDBNull(this object str)
        {
            return (str == DBNull.Value || str == null) ? string.Empty : str.ToString().Trim();
        }

        public static bool IsNumeric(this string str, out int numeral)
        {
            return int.TryParse(str, out numeral);
        }

        public static bool GetNotDBNullBool(this object str)
        {
            return str != DBNull.Value && Convert.ToBoolean(str);
        }

        public static int GetNotDBNullInt(this object str)
        {
            return str == DBNull.Value ? 0 : (int)str;
        }
        public static string EncryptString(this string Value)
        {
            var ky = new ASCIIEncoding().GetBytes(Key);
            var InitVect = new ASCIIEncoding().GetBytes(IV);
            var InputBytes = new ASCIIEncoding().GetBytes(Value);
            var Des = new TripleDESCryptoServiceProvider();
            ICryptoTransform crypTrans = Des.CreateEncryptor(ky, InitVect);
            var encryptStream = new MemoryStream();
            var cryptStream = new CryptoStream(encryptStream, crypTrans, CryptoStreamMode.Write);
            if ((Value ?? "") == (string.Empty ?? ""))
                return string.Empty;
            cryptStream.Write(InputBytes, 0, InputBytes.Length);
            cryptStream.FlushFinalBlock();
            encryptStream.Position = 0;
            var result = encryptStream.ToArray();
            encryptStream.Read(result, 0, Convert.ToInt32(encryptStream.Length));
            cryptStream.Close();
            return Convert.ToBase64String(result);
        }
        public static string DecryptString(this string Value)
        {
            var ky = new ASCIIEncoding().GetBytes(Key);
            var InitVect = new ASCIIEncoding().GetBytes(IV);
            var InputBytes = Convert.FromBase64String(Value);
            var Des = new TripleDESCryptoServiceProvider();
            ICryptoTransform crypTrans = Des.CreateDecryptor(ky, InitVect);
            var decryptStream = new MemoryStream();
            var cryptStream = new CryptoStream(decryptStream, crypTrans, CryptoStreamMode.Write);
            if ((Value ?? "") == (string.Empty ?? ""))
                return string.Empty;
            cryptStream.Write(InputBytes, 0, InputBytes.Length);
            cryptStream.FlushFinalBlock();
            decryptStream.Position = 0;
            var result = new byte[Convert.ToInt32(decryptStream.Length - 1) + 1];
            decryptStream.Read(result, 0, Convert.ToInt32(decryptStream.Length));
            cryptStream.Close();
            return new ASCIIEncoding().GetString(result);
        }
    }
}
