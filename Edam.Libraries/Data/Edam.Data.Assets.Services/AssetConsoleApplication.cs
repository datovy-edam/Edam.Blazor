using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole.Services;

namespace Edam.Data.AssetConsole
{

   public class AssetConsoleApplication
   {
      public static void ApplicationInitialization(string[] args)
      {
         //Tests.XmlNavigatorTest.TestNavigation();

         AssetServiceHelper.Initialize();
         AssetServiceHelper.ProcessArguments(args);
      }
   }

}
