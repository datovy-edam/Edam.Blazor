using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement;
using Edam.Diagnostics;
using Edam.Application;
using Edam.Data.Schema.SchemaObject;

namespace Edam.Data.Schema.SchemaObject
{
   public class SchemaDataObject
   {

      public static ResultsLog<List<SchemaResourceConstraint>>
         SchemaConstraintsGet(string connectionString, string sessionId = null)
      {
         ResultsLog<List<SchemaResourceConstraint>> results =
            new ResultsLog<List<SchemaResourceConstraint>>();

         if (String.IsNullOrWhiteSpace(sessionId))
         {
            sessionId = Session.SessionId;
         }

         DataProvider p = DataProvider.CreateProcedure(
            "Helper.HelperDatabaseForeignKeyGet", null, connectionString);
         try
         {
            p.Params.AddWithValue("@SessionId", sessionId, 40);

            if (p.OpenReader())
            {
               results.Data = 
                  DataReader.GetList<SchemaResourceConstraint>(p.Reader);
               results.Succeeded();
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return results;
      }

   }
}
