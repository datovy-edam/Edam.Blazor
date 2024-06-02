using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.

namespace Edam.Security.Cryptography // Edam.Security.Cryptography
{

   /// <summary>
   /// Protect default encryption for text strings.
   /// </summary>
   public class SecretText
   {
      private String m_EncryptedText;

      /// <summary>
      /// Get/Set encrypted data as Clear-Text
      /// </summary>
      public String ClearText
      {
         get { return Encryptor.Decrypt(m_EncryptedText); }
         set
         {
            if (String.IsNullOrEmpty(value))
               value = String.Empty;
            m_EncryptedText = Encryptor.Encrypt(value);
         }
      }

      /// <summary>
      /// Get encrypted text
      /// </summary>
      public String EncryptedText
      {
         get { return m_EncryptedText; }
      }

      /// <summary>
      /// Initialize the secret text
      /// </summary>
      /// <param name="text">secret (clear) text</param>
      /// </param>
      public SecretText(String text = null)
      {
         ClearText = text;
      }

      /// <summary>
      /// Set secret (encrypted) text.
      /// </summary>
      /// <param name="text">encrypted text</param>
      public void SetSecretText(String text)
      {
         string txt = String.IsNullOrWhiteSpace(text) ? String.Empty : text;
         ClearText = Encryptor.Decrypt(txt);
      }

      /// <summary>
      /// Set default values for new instances or reset fields.
      /// </summary>
      public void ClearFields()
      {
         m_EncryptedText = String.Empty;
      }

   }

}
