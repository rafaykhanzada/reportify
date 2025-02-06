using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class AESHelper
    {
        static string orignalText = "Hello World!";

        //Generate Random Key
        //Transforming Plain Text to CipherText
        //32 bytes for AES-256
        //Symmetric = public key for both
        //Assymmetric = public private combination
        static byte[] key = GenerateRandomBytes(32);

        // Generate IV -- random value used for encryption to secure it
        // to handle case when multiple identical plaintext is getting encrypted
        // ensures identical texts with same key produce different ciphertext
        //always 16 bytes for IV because is the size of aes block
        static byte[] iv = GenerateRandomBytes(16);

        static string encryptedText = Encrypt(orignalText,key,iv);
       
        string decryptedText = Decrypt(encryptedText,key,iv);


        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            //Aes abstract base class for all aes implementations to overwritten
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                //Create an encryptor to perform the stream transform
                // Symmetric encrypt object
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key,aesAlg.IV);

                //Create the streams used for encryption
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using(CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }


        public static string Decrypt(string encryptedText, byte[] key, byte[] iv)
        {
            byte[] cipherText = Convert.FromBase64String(encryptedText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }


        public static byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
