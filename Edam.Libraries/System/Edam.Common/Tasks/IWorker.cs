using System;
using System.Threading;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.Tasks
{

   public interface IWorker<T>
   {
      T Data { get; set; }
      IResultsLog Results { get; set; }
      bool Success { get; }
      Action<Worker<T>> CallBack { get; set; }
      
      IResultsLog Done(bool doCallBack = true);
   }

}
