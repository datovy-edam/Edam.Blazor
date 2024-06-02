using Edam.Data.Dictionary;
using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Dictionary.Api
{

   public interface ILookUpResult
   {

      Object ContextInstance { get; }
      IResultsLog ResultsLog { get; set; }

      ITermInfo CurrentTerm { get; set; }
      ITermInfo ProcessedTerm { get; set; }

      int Acknowleged { get; set; }
      int NotAcknowledge { get; set; }

      int Found { get; set; }
      int NotFound { get; set; }

   }

}
