using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Diagnostics;

namespace Edam.Language
{

   public interface IInterpreter
   {
      IResultsLog ExecuteScript(
         string scriptName, string functionName,
         Parameters? parameters = null, string? scriptPath = null);
      void Dispose();
   }

}
