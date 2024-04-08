using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptTest
{
    public class Program
    {
        private static byte[] IV = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};
        private static string passPhrase = "AdamEmmaViktorOg";

        static void Main(string[] args)
        {
            byte[] ba = Encoding.Unicode.GetBytes("Dusko Misljencevic");
            string base64 = Convert.ToBase64String(ba);
            byte[] ba2 = Convert.FromBase64String(base64);
            string decoded = Encoding.Unicode.GetString(ba2);

            string inputText = "Dusko Misljencevic";
            string strEncrypted = Encrypt(inputText, passPhrase);
            string strDecrypted = Decrypt(strEncrypted, passPhrase);
        }

        private static byte[] DeriveKeyFromPassword(string password)
        {
            return Encoding.Unicode.GetBytes(password);
        }

        private static string Encrypt(string inputText, string passphrase)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKeyFromPassword(passphrase);
                aes.IV = IV;
                using (MemoryStream output = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] inputBytes = Encoding.Unicode.GetBytes(inputText);
                        cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                        //cryptoStream.Write(Encoding.Unicode.GetBytes(inputText));

                        cryptoStream.FlushFinalBlock();
                        return Convert.ToBase64String(output.ToArray());
                    }
                }
            }
        }

        private static string Decrypt(string strEncrypted, string passphrase)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKeyFromPassword(passphrase);
                aes.IV = IV;
                byte[] encrypted = Convert.FromBase64String(strEncrypted);
                using (MemoryStream input = new MemoryStream(Convert.FromBase64String(strEncrypted)))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            cryptoStream.CopyTo(output);
                            return Encoding.Unicode.GetString(output.ToArray());
                        }
                    }
                }
            }
        }
    }
}
