using System;
using System.Threading;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.Tasks
{

   /// <summary>
   /// Tasks worker helper.
   /// </summary>
   /// <typeparam name="T">type of inner Data</typeparam>
   public class Worker<T> : IWorker<T>
   {
      
      public T Data { get; set; }
      public IResultsLog Results { get; set; }
      public Action<Worker<T>> CallBack { get; set; }
      public bool Success
      {
         get
         {
            return Results.Success;
         }
      }

      /// <summary>
      /// Initialize the worker.
      /// </summary>
      /// <param name="results">(optional) results set to use</param>
      public Worker(IResultsLog results = null)
      {
         if (results == null)
            results = new ResultLog();
         Results = results;
      }

      /// <summary>
      /// Call inner CallBack method, and clean-up...
      /// </summary>
      /// <param name="doCallBack">(optional) call inner CallBack 
      /// if any is available, (default: true)</param>
      /// <returns>IResultsLog is returned</returns>
      public IResultsLog Done(bool doCallBack = true)
      {
         if (Results != null)
            Results.Succeeded();
         if (CallBack != null && doCallBack)
            CallBack(this);
         return Results;
      }

   }

}
