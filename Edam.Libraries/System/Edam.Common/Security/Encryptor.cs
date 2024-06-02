using System;
using System.IO;
using System.Text;
using cnv = System.Convert;
using System.Security.Cryptography;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.

namespace Edam.Security.Cryptography //Edam.Security.Cryptography
{

   /// <summary>
   /// The following encryptor is usefull for .Net and Silverlight applications.
   /// Note that the Edam.Security.dll is only available in .Net applications
   /// but can't be available in Silverligh since it is written in C++/CLI and
   /// can't be compiled nor made available to Silverlight.  For this reason
   /// the following BaseEncryptor is provided.
   /// </summary>
   public sealed class Encryptor
   {

      private static Random random = new Random();

      /// <summary>
      /// Get Salt.
      /// </summary>
      /// <param name="password">password</param>
      /// <returns>return salt as a byte array</returns>
      private static SecretKey GetSaltKey(String password)
      {
         SecretKey key = new SecretKey();
         if (String.IsNullOrEmpty(password))
         {
            Security.ISecretKeys keys = SecuredKeysVault.GetSecretKeys();
            key.Key = Edam.Convert.ToString(keys.SaltBytes);
            key.Salt = keys.SaltBytes;
         }
         else
         {
            UTF8Encoding enc = new UTF8Encoding();
            key.Key = password;
            key.Salt = enc.GetBytes(password);
         }
         return key;
      }

      /// <summary>
      /// Encrypt the data and return result as a Base64 string.
      /// </summary>
      /// <param name="secretText">buffer to encrypt</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Encrypted string</returns>
      /// <remarks>the used encryption provider is RFC 2898 that Implements
      /// password-based key derivation functionality, PBKDF2, by using a
      /// pseudo-random number generator based on HMACSHA1.</remarks>
      public static Byte[] EncryptInBuffer(Byte[] buffer, String salt = null)
      {

         SecretKey saltKey = GetSaltKey(salt);
         Byte[] encryptBytes = null;

         using (AesManaged aes = new AesManaged())
         {
            Rfc2898DeriveBytes rfc =
               new Rfc2898DeriveBytes(saltKey.Key, saltKey.Salt);

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            using (ICryptoTransform encryptTransform = aes.CreateEncryptor())
            {
               using (MemoryStream encryptedStream = new MemoryStream())
               {
                  using (CryptoStream encryptor =
                     new CryptoStream(encryptedStream, encryptTransform,
                        CryptoStreamMode.Write))
                  {
                     encryptor.Write(buffer, 0, buffer.Length);
                     encryptor.Flush();
                     encryptor.Close();

                     encryptBytes = encryptedStream.ToArray();
                  }
               }
            }
         }

         return encryptBytes;
      }  // end of EncryptInBuffer

      /// <summary>
      /// Encrypt the data and return result as a Base64 string.
      /// </summary>
      /// <param name="secretText">String to encrypt</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Encrypted string</returns>
      /// <remarks>the used encryption provider is RFC 2898 that Implements
      /// password-based key derivation functionality, PBKDF2, by using a
      /// pseudo-random number generator based on HMACSHA1.</remarks>
      public static Byte [] EncryptInBuffer(
         String secretText, String salt = null)
      {
         Byte[] utfData = UTF8Encoding.UTF8.GetBytes(secretText);
         return EncryptInBuffer(utfData, salt);
      }  // end of EncryptInBuffer

      /// <summary>
      /// Decrypt a string
      /// </summary>
      /// <param name="encryptedBytes">Encrypted secret as a Base64 string
      /// </param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Decrypted string</returns>
      /// <remarks>the used encryption provider is RFC 2898 that Implements
      /// password-based key derivation functionality, PBKDF2, by using a
      /// pseudo-random number generator based on HMACSHA1.</remarks>
      public static Byte[] DecryptInBuffer(
         Byte[] encryptedBytes, String salt = null)
      {
         
         SecretKey saltKey = GetSaltKey(salt);
         String decryptedString = String.Empty;
         Byte[] decryptBytes = null;

         using (var aes = new AesManaged())
         {
            Rfc2898DeriveBytes rfc =
               new Rfc2898DeriveBytes(saltKey.Key, saltKey.Salt);
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
            {
               using (MemoryStream decryptedStream = new MemoryStream())
               {
                  CryptoStream decryptor =
                     new CryptoStream(decryptedStream, decryptTransform,
                        CryptoStreamMode.Write);
                  decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                  decryptor.Flush();
                  decryptor.Close();

                  decryptBytes = decryptedStream.ToArray();
                  decryptor.Dispose();
               }
            }
         }

         return decryptBytes;
      }  // end of DecryptInBuffer

      /// <summary>
      /// Decrypt a string
      /// </summary>
      /// <param name="secretText">Encrypted secret as a Base64 string</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Decrypted string</returns>
      /// <remarks>the used encryption provider is RFC 2898 that Implements
      /// password-based key derivation functionality, PBKDF2, by using a
      /// pseudo-random number generator based on HMACSHA1.</remarks>
      public static Byte [] DecryptInBufer(
         String secretText, String salt = null)
      {
         Byte[] encryptedBytes = System.Convert.FromBase64String(secretText);
         return DecryptInBuffer(encryptedBytes, salt);
      }  // end of DecryptInBuffer

      /// <summary>
      /// Encrypt the data and return result as a Base64 string.
      /// </summary>
      /// <param name="secretText">String to encrypt</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Encrypted string</returns>
      /// <remarks>the used encryption provider is RFC 2898 that Implements
      /// password-based key derivation functionality, PBKDF2, by using a
      /// pseudo-random number generator based on HMACSHA1.</remarks>
      public static String Encrypt(String secretText, String salt = null)
      {
         Byte[] utfData = UTF8Encoding.UTF8.GetBytes(secretText);
         Byte[] encryptedData = EncryptInBuffer(utfData, salt);
         if (encryptedData == null)
            return String.Empty;

         return System.Convert.ToBase64String(encryptedData); 
      }  // end of Encrypt
 
      /// <summary>
      /// Decrypt a string
      /// </summary>
      /// <param name="secretText">Encrypted secret as a Base64 string</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Decrypted string</returns>
      /// <remarks>
      /// - the used encryption provider is RFC 2898 that Implements
      ///   password-based key derivation functionality, PBKDF2, by using a
      ///   pseudo-random number generator based on HMACSHA1;
      /// - empty or null 'secretText' will not be processed and an empty string
      ///   will be returned.
      /// </remarks>
      public static String Decrypt(String secretText, String salt = null) 
      {
         if (String.IsNullOrEmpty(secretText))
            return String.Empty;
         Byte[] encryptedBytes = System.Convert.FromBase64String(secretText);
         Byte[] bytes = DecryptInBuffer(encryptedBytes, salt);
         return UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
      }  // end of Decrypt

      /// <summary>
      /// Not an encryption the following Hash function will help in converting
      /// the given secretText into an obfuscated text.  Hashing is one-way
      /// only therefore you will not be able to un-Hash the output of this
      /// method.
      /// </summary>
      /// <param name="secretText">text to hash</param>
      /// <returns>the hashed string is returned</returns>
      /// <remarks>Hash is performed using SHA256</remarks>
      public static String Hash(String secretText)
      {
         SHA256 sh = new SHA256Managed();
         Byte[] secretBytes = Encoding.UTF8.GetBytes(secretText);
         Byte[] hashedBytes = sh.ComputeHash(secretBytes);
         String result = BitConverter.ToString(hashedBytes);

         sh.Dispose();

         return result.Replace("-",String.Empty);
      }  // end of Hash

      /// <summary>
      /// Encrypt and Hash secret.
      /// </summary>
      /// <param name="secretText">clear text secret to encrypt and hash</param>
      /// <returns>the hash of the encrypted secret is returned</returns>
      public static String EncryptAndHash(String secretText)
      {
         String secret = Encryptor.Encrypt(secretText);
         String hash = Encryptor.Hash(secret);
         return hash;
      }

      /// <summary>
      /// Hash and Encrypt secret.
      /// </summary>
      /// <param name="secretText">clear text secret to hash then encrypt</param>
      /// <returns>the hash of the encrypted secret is returned</returns>
      public static String HashAndEncrypt(String secretText)
      {
         String hash = Encryptor.Hash(secretText);
         String secret = Encryptor.Encrypt(hash);
         return secret;
      }

      /// <summary>
      /// Generate a 'strong' random password / string.
      /// </summary>
      /// <param name="size">(opional) string size (default = 12)</param>
      /// <returns>the generated random string is returned</returns>
      public static String RandomString(Int32 size = 12)
      {
         Int32 Ascii_a = 97, Ascii_A = 65, AsciiConst = Ascii_A;
         StringBuilder rtext = new StringBuilder();
         for (var i = 0; i < size; i++)
         {
            switch (random.Next(0, 2))
            {
               case 0:
                  rtext.Append(
                     (cnv.ToChar(
                        cnv.ToInt32(
                           Math.Floor(26 * random.NextDouble() + AsciiConst)))));
                  break;
               case 1:
                  rtext.Append(random.Next(1, 10));
                  AsciiConst = AsciiConst == Ascii_a ? Ascii_A : Ascii_a;
                  break;
            }
         }
         return rtext.ToString();
      }

      /*
      /// <summary>
      /// Encrypt the data and return result as a Base64 string.
      /// </summary>
      /// <param name="secretText">String to encrypt</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Encrypted string</returns>
      /// <remarks>the used encryption provider is RFC 2898 that Implements
      /// password-based key derivation functionality, PBKDF2, by using a
      /// pseudo-random number generator based on HMACSHA1.</remarks>
      public static String Encrypt(String secretText, String salt = null)
      {
         Byte[] utfData = UTF8Encoding.UTF8.GetBytes(secretText);
         Byte[] encryptedData = utfData; // EncryptInBuffer(utfData, salt);
         if (encryptedData == null)
            return String.Empty;

         return cnv.ToBase64String(encryptedData);
      }  // end of Encrypt

      /// <summary>
      /// Decrypt a string
      /// </summary>
      /// <param name="secretText">Encrypted secret as a Base64 string</param>
      /// <param name="salt">(optional) password that will be used by the
      /// encryption provider. If not provided default or application configured
      /// salt is used.</param>
      /// <returns>Decrypted string</returns>
      /// <remarks>
      /// - the used encryption provider is RFC 2898 that Implements
      ///   password-based key derivation functionality, PBKDF2, by using a
      ///   pseudo-random number generator based on HMACSHA1;
      /// - empty or null 'secretText' will not be processed and an empty string
      ///   will be returned.
      /// </remarks>
      public static String Decrypt(String secretText, String salt = null)
      {
         if (String.IsNullOrEmpty(secretText))
            return String.Empty;
         Byte[] encryptedBytes = cnv.FromBase64String(secretText);
         Byte[] bytes = encryptedBytes; // DecryptInBuffer(encryptedBytes, salt);
         return UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
      }  // end of Decrypt

      /// <summary>
      /// Not an encryption the following Hash function will help in converting
      /// the given secretText into an obfuscated text.  Hashing is one-way
      /// only therefore you will not be able to un-Hash the output of this
      /// method.
      /// </summary>
      /// <param name="secretText">text to hash</param>
      /// <returns>the hashed string is returned</returns>
      /// <remarks>Hash is performed using SHA256</remarks>
      public static String Hash(String secretText)
      {
         //SHA256 sh = new SHA256Managed();
         UTF8Encoding enc = new UTF8Encoding();
         Byte[] secretBytes = enc.GetBytes(secretText);
         Byte[] hashedBytes = secretBytes; // sh.ComputeHash(secretBytes);
         String result = BitConverter.ToString(hashedBytes);
         
         return result.Replace("-", String.Empty);
      }  // end of Hash
      */
   }

}
