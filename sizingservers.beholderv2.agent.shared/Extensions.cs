using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace sizingservers.beholderv2.agent.shared
{
    public static class StringExtension {
        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string s) {
            ulong ul;
            double d;
            return ulong.TryParse(s, out ul) || double.TryParse(s, out d);
        }
        /// <summary>
        /// A simple way to encrypt a string.
        /// Example (don't use this): s.Encrypt("password", new byte[] { 0x59, 0x06, 0x59, 0x3e, 0x21, 0x4e, 0x55, 0x34, 0x96, 0x15, 0x11, 0x13, 0x72 });
        /// </summary>
        /// <param name="s"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(this string s, string password, byte[] salt) {
            var pdb = new PasswordDeriveBytes(password, salt);
            byte[] encrypted = Encrypt(System.Text.Encoding.Unicode.GetBytes(s), pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encrypted);
        }
        private static byte[] Encrypt(byte[] toEncrypt, byte[] key, byte[] IV) {
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = IV;
            //alg.Padding = PaddingMode.None;

            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.Close();
            return ms.ToArray();
        }
        /// <summary>
        /// A simple way to decrypt a string.
        /// Example (don't use this): s.Decrypt("password", new byte[] { 0x59, 0x06, 0x59, 0x3e, 0x21, 0x4e, 0x55, 0x34, 0x96, 0x15, 0x11, 0x13, 0x72 });
        /// </summary>
        /// <param name="s"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(this string s, string password, byte[] salt) {
            var pdb = new PasswordDeriveBytes(password, salt);
            byte[] decrypted = Decrypt(Convert.FromBase64String(s), pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decrypted);
        }
        private static byte[] Decrypt(byte[] toDecrypt, byte[] Key, byte[] IV) {
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            //alg.Padding = PaddingMode.None;

            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(toDecrypt, 0, toDecrypt.Length);
            try {
                cs.Close();
            }
            catch {
                //Don't care.
            }
            return ms.ToArray();
        }
        public static string Reverse(this string s) {
            var sb = new StringBuilder(s.Length); ;
            for (int i = s.Length - 1; i != -1; i--)
                sb.Append(s[i]);

            return sb.ToString();
        }
    }
}
