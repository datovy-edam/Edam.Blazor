
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Diagnostics;

namespace Edam.Data.Dictionary.Api
{

   public interface IDictionaryApi : IDisposable
   {

      ITermInfo Parse(string jsonText);
      ResultsLog<ITermInfo?> LookUp(ITermInfo term);
      ILookUpResult LookUp(
         List<ITermInfo> terms, Action<ILookUpResult> callBack = null,
         int topCount = 0);
      void LookUp(Action<ILookUpResult> callBack = null, int topCount = 0);
      IDictionaries GetDictionaries();

   }

}
