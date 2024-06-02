using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Application;

namespace Edam.Security
{

   public class SecuredKeysVault
   {

      private const String m_VaultAssemblyAndTypeKey = 
         "VaultAssemblyAndTypeKey";
      private static ISecretKeys m_Keys = null;
      
      static SecuredKeysVault()
      {
         OpenVault();
      }
      
      /// <summary>
      /// Open Keys Vault...
      /// </summary>
      /// <returns>ResultsLog is returned with the 'vault' object instance or 
      /// null if it failed to be loaded</returns>
      public static ResultsLog<ISecuredKeysVault> GetVault()
      {
         ResultsLog<ISecuredKeysVault> results = 
            new ResultsLog<ISecuredKeysVault>();

         try
         {
            String assAndType = 
               AppSettings.GetSectionString(m_VaultAssemblyAndTypeKey);

            String[] tokens = assAndType.Split(';');
            String assemblyName = tokens[0];
            String typeName = tokens[1];

            Edam.Security.ISecuredKeysVault vault =
               Edam.Services.TypeActivator.Activate(assemblyName, typeName) as
                  Edam.Security.ISecuredKeysVault;
            if (vault == null)
               throw new SecuredKeysVaultException(
                  "Security::Vault: Can't load keys Vault");

            results.Data = vault;
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }

         return results;
      }

      /// <summary>
      /// Open Keys Vault...
      /// </summary>
      /// <returns>IResultsLog is returned</returns>
      public static IResultsLog OpenVault()
      {
         if (m_Keys != null)
            CloseVault();

         ResultsLog<ISecuredKeysVault> results = GetVault();

         if (results.Success)
            m_Keys = results.Data.GetSecretKeys();

         return results;
      }

      /// <summary>
      /// Close Vault...
      /// </summary>
      public static void CloseVault()
      {
         if (m_Keys != null)
         {
            m_Keys.SetHexKeys(null, null);
            m_Keys.SetSaltHexKey(null);
         }
         m_Keys = null;
      }

      /// <summary>
      /// Get Secret Keys...
      /// </summary>
      /// <returns>secret keys are returned</returns>
      public static ISecretKeys GetSecretKeys()
      {
         return m_Keys;
      }
   }

}
