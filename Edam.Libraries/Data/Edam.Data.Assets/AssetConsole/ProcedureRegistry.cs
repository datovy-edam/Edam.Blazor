using System;
using System.Collections.Generic;

using Edam.Diagnostics;

namespace Edam.Data.AssetConsole
{

   /// <summary>
   /// Manage the Asset Console Procedure registry.
   /// </summary>
   public class ProcedureRegistry
   {
      public List<ProcedureItem> List { get; set; }

      public ProcedureRegistry()
      {
         List = new List<ProcedureItem>();
      }

      /// <summary>
      /// Add Procedure to the registry.
      /// </summary>
      /// <param name="procedure">requesed procedue</param>
      /// <param name="executorHandler">instance of executor delegate</param>
      public void Add(AssetConsoleProcedure procedure,
         ExecutorHandler executorHandler)
      {
         var fItem = List.Find(x => x.Procedure == procedure);
         if (fItem != null)
            return;

         var i = new ProcedureItem(procedure, executorHandler);
         List.Add(i);
      }

      /// <summary>
      /// Add procedure item.
      /// </summary>
      /// <param name="procedure">instance of ProcessItem</param>
      public void Add(ProcedureItem procedure)
      {
         var fItem = List.Find(x => x.Procedure == procedure.Procedure);
         if (fItem != null)
            return;
         List.Add(procedure);
      }

      /// <summary>
      /// Find a registered procedure.
      /// </summary>
      /// <param name="procedure">requested procedure</param>
      /// <returns>if found, instance of ProcedureItem is returned</returns>
      public ProcedureItem Find(AssetConsoleProcedure procedure)
      {
         return List.Find(x => x.Procedure == procedure);
      }
   }

}
