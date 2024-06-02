using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using languages = Edam.Language;

namespace Edam.Data.Lexicon.Semantics
{

   public class ModuleInfo
   {
      public string ScriptName { get; set; }
      public string MethodName { get; set; }
      public languages.Parameters Parameters = new languages.Parameters();

      public void PrepareParameters(string text1, string text2)
      {
         Parameters.Clear();
         Parameters.Add("text1", text1);
         Parameters.Add("text2", text2);
      }
   }

}
