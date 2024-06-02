using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;

namespace Edam.Application
{

   public class RuntimeHelper
   {

      public static void ApplicationStartUp()
      {
      }

      public void DisposeCleanUp()
      {
         // dispose inmediate collection of all generations...
         System.GC.Collect();
         // wait for the processing queue of finalizers has emptied that queue
         System.GC.WaitForPendingFinalizers();
      }

   }

}
