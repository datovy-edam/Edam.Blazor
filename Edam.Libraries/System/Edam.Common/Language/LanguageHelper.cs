using Edam.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Language
{

   public class LanguageHelper
   {
      public const string LANGUAGE_PHYTHON = "PythonIntepreter";

      public static IInterpreter GetPythonLanguageInstance()
      {
         return AppAssembly.FetchInstance<IInterpreter>(LANGUAGE_PHYTHON);
      }

   }

}
