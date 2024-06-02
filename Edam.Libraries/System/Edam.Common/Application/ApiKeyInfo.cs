using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   /// <summary>
   /// Manage API key details and encryption...
   /// </summary>
   public class ApiKeyInfo
   {

      private String m_EncryptedKeyValue = null;

      public String ApplicationId { get; set; }
      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }

      public String ApiKeyValue
      {
         get
         { 
            return m_EncryptedKeyValue != null ?
               m_EncryptedKeyValue : String.Empty; 
         }
         set
         {
            if (m_EncryptedKeyValue != null)
               return;
            m_EncryptedKeyValue = value;
         }
      }

      public ApiKeyInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         ApplicationId = String.Empty;
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         m_EncryptedKeyValue = null;
      }

      /// <summary>
      /// Encrypt and Hash the key value property, original will no longer be
      /// accessible.
      /// </summary>
      /// <remarks>once encrypted the Api Key Value can't be set again.  If you
      /// want you can call ClearFields, setup the key details and try 
      /// encrypting again.
      /// </remarks>
      /// <returns>true is returned if all is OK</returns>
      public Boolean EncryptKeyValue()
      {
         if (String.IsNullOrEmpty(ApiKeyValue))
            return false;
         if (m_EncryptedKeyValue != null)
            return true;
         ApiKeyValue = String.Empty; // Edam.Security.Cryptography.Encryptor.
            // EncryptAndHash(ApiKeyValue);
         return true;
      }

   }

}
