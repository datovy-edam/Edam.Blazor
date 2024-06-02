using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Security
{

   public enum SecretRecoverability
   {
      Unknown = 0,
      Recoverable = 1,
      Unrecoverable = 2
   }

   public interface ISecuredKeysVault
   {
      ISecretKeys GetSecretKeys();
      String GetSecret(String secretMessage, 
         SecretRecoverability recoverability =
            SecretRecoverability.Unrecoverable);
      //Edam.Diagnostics.IResultsLog TestKeys();
   }

}
