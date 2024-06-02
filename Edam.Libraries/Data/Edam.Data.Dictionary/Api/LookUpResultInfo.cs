using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Diagnostics;

namespace Edam.Data.Dictionary.Api
{

   public class LookUpResultInfo : ILookUpResult
   {

      public DictionaryContext Context { get; set; } = null;
      public Object ContextInstance
      {
         get { return Context; }
      }

      public IResultsLog ResultsLog { get; set; } = new ResultLog();

      public ITermInfo CurrentTerm { get; set; } = null;
      public ITermInfo ProcessedTerm { get; set; } = null;

      public int Acknowleged { get; set; }
      public int NotAcknowledge { get; set; }

      public int Found { get; set; }
      public int NotFound { get; set; }

      public LookUpResultInfo()
      {
         Acknowleged = 0;
         NotAcknowledge = 0;
      }

   }

}
