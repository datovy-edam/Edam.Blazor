using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Security;
using Edam.Diagnostics;

namespace Edam.Security.SecurityVault.Keys
{

   /// <summary>
   /// Support for managing Security Secret Keys from a different store and
   /// provide them to the class-library to be used while encrypting and 
   /// decrypting messages.
   /// </summary>
   public class SecurityKeys : ISecuredKeysVault
   {

      // TODO: Change Keys or manage them as needed here
      private static String m_MasterKey = "054BDF833A845E91";
      private static String m_InitializationKey =
         "F8A98DD45E0A1543B4C4BF832DA730";
      private static readonly String m_Salt =
         "BC0238D45FFF362482A0C395BCDFF3284E00";

      /// <summary>
      /// Get Secret Keys from wherever you store them.
      /// </summary>
      /// <returns>instance of ISecretKeys is returned</returns>
      public ISecretKeys GetSecretKeys()
      {
         Edam.Security.SecretKeyList s = new Edam.Security.SecretKeyList();
         s.SetHexKeys(m_MasterKey, m_InitializationKey);
         s.SetSaltHexKey(m_Salt);
         return s;
      }

      /// <summary>
      /// Encrypt a message with your keys.
      /// </summary>
      /// <param name="message">message to encrypt</param>
      /// <param name="recoverability">(optional) recoverability
      /// [default: Unrecoverable]</param>
      /// <returns>text string with your encrypted message is returned</returns>
      public String GetSecret(String message,
         SecretRecoverability recoverability =
            SecretRecoverability.Unrecoverable)
      {
         Password pwd;

         if (recoverability != SecretRecoverability.Recoverable)
            // hash and encrypt the secret [default]
            // this is a one-way password encryption (unrecallable)
            pwd = new Security.Password(message);
         else
            // this is two-way password encryption (recallable)
            pwd = new Security.Password(
               message, Security.SecretOption.NotHashed);

         return pwd.PasswordText;
      }

      /*
      /// <summary>
      /// Test encrypt/decrypt with keys.
      /// </summary>
      /// <returns>IResultsLog is returned, check Success to see if all is OK, 
      /// else exception or error messages will be found in the Messages
      /// property</returns>
      public IResultsLog TestKeys()
      {
         ResultLog results = new Diagnostics.ResultLog();
         try
         {
            String func = "SecurityKeys::TestKeys";

            // encrypt / decrypt...
            String secret = "my-social-security";
            String encryptedSecret =
               Cryptography.Encryptor.Encrypt(secret);
            if (String.IsNullOrEmpty(encryptedSecret))
               throw new SecuredKeysVaultException(
                  func + ": failed encryption.");

            String originalSecret =
               Cryptography.Encryptor.Decrypt(encryptedSecret);
            if (secret != originalSecret)
               throw new SecuredKeysVaultException(
                  func + ": failed decryption.");

            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }
      */

   }

}
