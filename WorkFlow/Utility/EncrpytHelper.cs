namespace TY.Utility
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class EncrpytHelper
    {
        private static string CRYPT_KEY = "pass@word#1";

        public static string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return Decrypt(text, CRYPT_KEY);
        }

        public static string Decrypt(string text, string key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(key));
            provider.Mode = CipherMode.ECB;
            ICryptoTransform transform = provider.CreateDecryptor();
            string str = "";
            try
            {
                byte[] inputBuffer = Convert.FromBase64String(text);
                str = Encoding.UTF8.GetString(transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
            }
            catch (Exception)
            {
                return text;
            }
            return str;
        }

        public static string Encrypt(string text)
        {
            return Encrypt(text, CRYPT_KEY);
        }

        public static string Encrypt(string text, string key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(key));
            provider.Mode = CipherMode.ECB;
            ICryptoTransform transform = provider.CreateEncryptor();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(transform.TransformFinalBlock(bytes, 0, bytes.Length));
        }
    }
}

