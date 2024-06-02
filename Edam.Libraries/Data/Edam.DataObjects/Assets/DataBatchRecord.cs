using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Data.AssetManagement;
using Edam.Data;
using Edam.DataObjects.ReferenceData;
using Edam.Diagnostics;

namespace Edam.DataObjects.Assets
{

   public class DataBatchRecord
   {

      public static ResultsLog<string> BatchInsertUpdate(
         string sessionId, string organizationId, string dataOwnerId,
         string batchId, string domainUri, string versionId, string groupId)
      {
         if (string.IsNullOrWhiteSpace(sessionId))
         {
            sessionId = Session.SessionId;
         }
         if (string.IsNullOrWhiteSpace(organizationId))
         {
            organizationId = Session.OrganizationId;
         }
         if (string.IsNullOrWhiteSpace(dataOwnerId))
         {
            dataOwnerId = Session.OrganizationId;
         }

         DataProvider provider = DataProvider.CreateProcedure(
            "Data.DataBatchUpsert",
            ReferenceDataHelper.GetConnectionStringKey());

         ResultsLog<string> results =
            new ResultsLog<string>();

         try
         {
            DataParameters p = provider.Params;

            string outBatchId = null;
            p.AddWithValue("@SessionId", sessionId);
            p.AddWithValue("@OrganizationId", organizationId.ToUpper());
            p.AddWithValue("@DataOwnerId", dataOwnerId, 20);
            p.AddWithValue("@BatchId", batchId, 40);
            p.AddWithValue("@DomainUri", domainUri, 2048);
            p.AddWithValue("@VersionId", versionId, 20);
            p.AddWithValue("@GroupId", groupId, 20);
            var r0 = p.AddWithValue("@OutBatchId", outBatchId, 40, true);

            if (provider.Exec())
            {
               results.Data = r0.Value.ToString();
               results.Succeeded();
            }
            else
            {
               results.ReturnValue = ReturnCode.ExecProcedureFailed;
               results.Failed(EventCode.StoredProcedureCallFailed);
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (provider != null)
               provider.Dispose();
         }
         return results;
      }

   }

}
