using System;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   public interface IMessageBox
   {
      void ShowMessageBox(String prompt, String message,
         Action<bool> action = null, MessageBoxType type = MessageBoxType.Done);
   }

}
