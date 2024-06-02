using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.
using Encryptor = Edam.Security.Cryptography.Encryptor;

namespace Edam.Security
{

   /// <summary>
   /// Manage a text securely by Hashing and encrypting a given password text...
   /// </summary>
   public class Password
   {

      //private Boolean m_ClearTextOnly = false;
      private Boolean m_EncryptTextAlways = true;

      private String m_HashedValue;
      private Cryptography.SecretText m_SecretText =
         new Cryptography.SecretText();

      /// <summary>
      /// Get the ClearText value.  If ClearTextOnly has not been specified this
      /// will returned the encrypted password.
      /// </summary>
      public String ClearText
      {
         get
         {
            String ctext = Encryptor.Decrypt(m_SecretText.ClearText);
            return ctext;
         }
      }

      /// <summary>
      /// Get/Set a password. Be aware that when you set a value it will be
      /// first Hashed then Encrypted therefore don't expect to recover the
      /// original password.
      /// </summary>
      public String PasswordText
      {
         get
         {
            return m_EncryptTextAlways ?
               m_SecretText.EncryptedText : (m_HashedValue == null ?
               m_SecretText.ClearText : m_SecretText.EncryptedText);
         }
         set
         {
            if (String.IsNullOrEmpty(value))
               value = String.Empty;
            if (m_EncryptTextAlways)
            {
               m_HashedValue = Encryptor.HashAndEncrypt(value);
               m_SecretText.ClearText = m_HashedValue;
            }
            else
               m_SecretText.ClearText = value;
         }
      }

      /// <summary>
      /// Initialize the password and Encrypt as specified.
      /// </summary>
      /// <param name="passwordText">password text</param>
      /// <param name="option">password option</param>
      public Password(String passwordText, SecretOption option)
      {
         String pwd = passwordText;
         if (option == SecretOption.Encrypt)
            m_EncryptTextAlways = true;
         else if (option == SecretOption.NotHashed)
         {
            m_EncryptTextAlways = false;
            pwd = Encrypt(passwordText);
         }
         PasswordText = pwd;
      }

      /// <summary>
      /// Initialize the password and Hash always.
      /// </summary>
      /// <param name="passwordText">password text</param>
      public Password(String passwordText)
      {
         m_EncryptTextAlways = true;
         PasswordText = passwordText;
      }

      /// <summary>
      /// Initialize the password object.
      /// </summary>
      public Password()
      {
         m_EncryptTextAlways = false;
         m_SecretText.ClearFields();
      }
      
      /// <summary>
      /// Set default values for new instances or reset fields.
      /// </summary>
      public void ClearFields()
      {
         m_EncryptTextAlways = true;
         m_HashedValue = String.Empty;
         m_SecretText.ClearFields();

         PasswordText = String.Empty;
      }

      /// <summary>
      /// Get the Hash of a given string.
      /// </summary>
      /// <param name="passwordText">secret or password to hash</param>
      /// <returns>the hash of the given string is returned.</returns>
      public static String Hash(String passwordText)
      {
         if (String.IsNullOrEmpty(passwordText))
            passwordText = String.Empty;
         return Edam.Security.Cryptography.Encryptor.Hash(passwordText);
      }

      /// <summary>
      /// Get the Encrypted value of given string.
      /// </summary>
      /// <param name="passwordText">secret or password to hash</param>
      /// <returns>the encrypt value of the given string is returned.</returns>
      public static String Encrypt(String passwordText)
      {
         if (String.IsNullOrEmpty(passwordText))
            passwordText = String.Empty;
         return Edam.Security.Cryptography.Encryptor.Encrypt(passwordText);
      }

      /// <summary>
      /// Decrypt password.
      /// </summary>
      /// <param name="encryptedPassword">text to decrypt</param>
      /// <returns>clear-text is returned.</returns>
      public static String Decrypt(String encryptedPassword)
      {
         if (String.IsNullOrEmpty(encryptedPassword))
            encryptedPassword = String.Empty;
         return Edam.Security.Cryptography.Encryptor.Decrypt(encryptedPassword);
      }

      /// <summary>
      /// Given a clear-text password hash and encrypt it.
      /// </summary>
      /// <param name="passwordText">clear-text password</param>
      /// <returns>hashed and encrypted password text is returned</returns>
      public static String HashAndEncrypt(String passwordText)
      {
         return Edam.Security.Cryptography.Encryptor.
                  EncryptAndHash(passwordText);
      }

      /// <summary>
      /// Get Default Application Configured Passcode using the
      /// 'DefaultAgentPasscode' key. (One Way Passcode)
      /// </summary>
      /// <returns>decrypted passcode</returns>
      public static Password GetDefaultAppConfigPasscode()
      {
         String passCode = Edam.Application.AppSettings.
            GetString("DefaultAgentPasscode");
         String decryptedPasscode = Decrypt(passCode);
         return new Password(decryptedPasscode);
      }

      /// <summary>
      /// Get Default Application Configured Passcode using the
      /// 'DefaultAgentPasscode' key.
      /// </summary>
      /// <returns>decrypted passcode</returns>
      public static Password GetDefaultAppConfigTwoWayPasscode()
      {
         String passCode = Edam.Application.AppSettings.
            GetString("DefaultAgentPasscode");
         String decryptedPasscode = Decrypt(passCode);
         return new Password(decryptedPasscode, SecretOption.NotHashed);
      }

      /// <summary>
      /// Get application password text (hashed and encrypted).
      /// </summary>
      /// <remarks>Note that the original secret clear text will never be 
      /// recovered since it is hashed before it is encrypted</remarks>
      /// <param name="secretClearText">secret clear text</param>
      /// <returns>returns the encrypted password</returns>
      public static String GetEncryptedPassword(String secretClearText)
      {
         Edam.Security.Password p = new Security.Password(secretClearText);
         return p.PasswordText;
      }

   }

}

