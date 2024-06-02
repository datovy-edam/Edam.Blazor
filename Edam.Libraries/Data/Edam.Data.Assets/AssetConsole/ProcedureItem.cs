using System;
using System.Collections.Generic;

using Edam.Diagnostics;

namespace Edam.Data.AssetConsole
{

   /// <summary>
   /// Asset Console Execution Handler.
   /// </summary>
   /// <typeparam name="T">DBContext Type</typeparam>
   /// <param name="arguments">instance of AssetConsoleArgumentsInfo</param>
   /// <returns></returns>
   public delegate IResultsLog
      ExecutorHandler(AssetConsoleArgumentsInfo arguments);

   /// <summary>
   /// Procedure Item.
   /// </summary>
   /// <typeparam name="TDBContext"></typeparam>
   public class ProcedureItem
   {
      private readonly AssetConsoleProcedure m_Procedure;
      private readonly ExecutorHandler m_Executor;

      public AssetConsoleProcedure Procedure
      {
         get { return m_Procedure; }
      }
      public string ProcedureName
      {
         get { return Procedure.ToString(); }
      }

      /// <summary>
      /// Procdure Item constructor.
      /// </summary>
      /// <param name="procedure">procedure</param>
      /// <param name="executor">instance of ExecutorHandler delegate</param>
      public ProcedureItem(
         AssetConsoleProcedure procedure, ExecutorHandler executor)
      {
         m_Procedure = procedure;
         m_Executor = executor;
      }

      /// <summary>
      /// Execute the procedure using given arguments and DB Context.
      /// </summary>
      /// <param name="arguments">required procedure parameters</param>
      /// <param name="dbContext">DB Context (not needed for all procedures)
      /// </param>
      /// <returns>instance of resutls log is returned</returns>
      public IResultsLog Execute(AssetConsoleArgumentsInfo arguments)
      {
         IResultsLog results;
         try
         {
            results = m_Executor(arguments);
         }
         catch (Exception ex)
         {
            results = new ResultsLog<EventCode>();
            results.Failed(ex.Message);
         }
         return results;
      }

   }

}
