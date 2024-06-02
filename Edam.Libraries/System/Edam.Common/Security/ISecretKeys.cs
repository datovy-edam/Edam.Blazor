using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Security
{

   public interface ISecretKeys
   {

      Byte[] MasterKeyBytes { get; }
      Byte[] InitializationVectorBytes { get; }
      Byte[] SaltBytes { get; }

      void SetKeys(Byte[] masterKey, Byte[] initializationVector);
      void SetHexKeys(String masterKey, String initializationVector);
      void SetStringKeys(String masterKey, String initializationVector);

      void SetSalt(Byte[] salt);
      void SetSaltHexKey(String salt);
      void SetSaltStringKey(String salt);

   }

}
