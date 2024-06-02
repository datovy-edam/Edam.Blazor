using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Data.Asset;
using Edam.DataObjects.Services;
using Edam.Diagnostics;

namespace Edam.DataObjects.Assets
{

   public class CodeSetService
   {

      public DataCodeService DataCodeService { get; set; }
      public CodeSetService()
      {
         DataCodeService = DataCodeService.GetService();
      }

      public static IResultsLog Save(List<CodeSetInfo> codeSet)
      {
         IResultsLog results = new ResultLog();
         return results;
      }

   }

}
