using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Security.Cryptography;
using convert = Edam.Convert;

namespace Edam.Security
{

   /// <summary>
   /// Support for secret keys...
   /// </summary>
   public class SecretKeyList : ISecretKeys
   {

      private String m_MasterKey;
      private String m_InitializationVector;
      private String m_Salt;
      
      public Byte[] MasterKeyBytes
      {
         get
         {
            if (String.IsNullOrEmpty(m_MasterKey))
               return null;
            return convert.FromHexStringTo4BitEncodedBytes(
               m_MasterKey);
         }
      }
      public Byte[] InitializationVectorBytes
      {
         get
         {
            if (String.IsNullOrEmpty(m_InitializationVector))
               return null;
            return convert.FromHexStringTo4BitEncodedBytes(
               m_InitializationVector);
         }
      }
      public Byte[] SaltBytes
      {
         get
         {
            if (String.IsNullOrEmpty(m_Salt))
               return null;
            return convert.FromHexStringTo4BitEncodedBytes(
               m_Salt);
         }
      }

      /// <summary>
      /// Initialize Keys using given Hex strings.
      /// </summary>
      /// <param name="masterKeyHexString">hex string master-key</param>
      /// <param name="initializationVectorHexString">hex string 
      /// initialization-vector</param>
      public void SetStringKeys(
         String masterKeyHexString, String initializationVectorHexString)
      {
         SetHexKeys(masterKeyHexString, initializationVectorHexString);
      }

      /// <summary>
      /// Set Keys using given Byte(s) array.
      /// </summary>
      /// <param name="masterKey">master key</param>
      /// <param name="initializationVector">initialization vector</param>
      public void SetKeys(
         Byte [] masterKey, Byte [] initializationVector)
      {
         m_MasterKey = masterKey == null ? String.Empty :
            convert.From4BitEncodedToHexString(masterKey);
         m_InitializationVector =
            initializationVector == null ? String.Empty :
            convert.From4BitEncodedToHexString(initializationVector);
      }

      /// <summary>
      /// Set Keys using given Hex strings.
      /// </summary>
      /// <param name="masterKey">hex string master-key</param>
      /// <param name="initializationVector">hex string 
      /// initialization-vector</param>
      /// <param name="salt">hex string salt</param>
      public void SetHexKeys(
         String masterKey, String initializationVector)
      {
         m_MasterKey = String.IsNullOrEmpty(masterKey) ?
            String.Empty : masterKey;
         m_InitializationVector =
            String.IsNullOrEmpty(initializationVector) ?
               String.Empty : initializationVector;
      }

      /// <summary>
      /// Set salt using given Byte array.
      /// </summary>
      /// <param name="salt">salt byte array</param>
      public void SetSalt(Byte[] salt)
      {
         m_Salt = salt == null ? String.Empty :
            convert.From4BitEncodedToHexString(salt);
      }

      /// <summary>
      /// Set salt with given Hex string as the key.
      /// </summary>
      /// <param name="salt">salt as a Hex string</param>
      public void SetSaltHexKey(String salt)
      {
         m_Salt = String.IsNullOrEmpty(salt) ?
            String.Empty : salt;
      }

      /// <summary>
      /// Set salt with an arbitrary text string.
      /// </summary>
      /// <param name="salt">salt text string</param>
      public void SetSaltStringKey(String salt)
      {
         if (String.IsNullOrEmpty(salt))
         {
            m_Salt = null;
            return;
         }

         Byte [] b = convert.ToByteArray(salt);
         SetSalt(b);
      }
      
   }

}
