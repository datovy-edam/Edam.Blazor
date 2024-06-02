using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Requests;
using Edam.DataObjects.ReferenceData;
using Edam.Data;
using Edam.B2b.Edi;
using Edam.Diagnostics;

namespace Edam.DataObjects.B2b
{

   public class EdiExchangeRecord
   {

      public static RequestResponseInfo<string> UpdateExchangeCodeRecord(
         string sessionId, string dataOwnerId, string codeId,
         string description)
      {
         DataProvider provider = DataProvider.CreateProcedure(
            "B2B.ExchangeCodeInsertUpdate",
            ReferenceDataHelper.GetConnectionStringKey());

         ResultsLog<string> results = new ResultsLog<string>();

         try
         {
            DataParameters p = provider.Params;

            p.AddWithValue("@SessionId", sessionId);
            p.AddWithValue("@DataOwnerId", dataOwnerId.ToUpper());
            p.AddWithValue("@CodeId", codeId);
            p.AddWithValue("@Description", description);

            if (provider.Exec())
            {
               results.Data = codeId;
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

         RequestResponseInfo<string> response =
            new RequestResponseInfo<string>();
         response.Results = results;
         response.ResponseData = results.Data;

         return response;
      }

      public static RequestResponseInfo<ExchangeDefinitionInfo> 
         UpdateExchangeDefinitionRecord(
            string sessionId, ExchangeDefinitionInfo item)
      {
         DataProvider provider = DataProvider.CreateProcedure(
            "B2B.ExchangeDefinitionInsertUpdate",
            ReferenceDataHelper.GetConnectionStringKey());

         ResultsLog<ExchangeDefinitionInfo> results =
            new ResultsLog<ExchangeDefinitionInfo>();

         try
         {
            DataParameters p = provider.Params;

            p.AddWithValue("@SessionId", sessionId);
            p.AddWithValue("@DataOwnerId", item.DataOwnerId.ToUpper());
            p.AddWithValue("@ItemNo", item.ItemNo);
            p.AddWithValue("@ExchangeCode", item.ExchangeCode, 40);
            p.AddWithValue("@SegmentName", item.SegmentName, 128);
            p.AddWithValue("@EntityName", item.EntityName, 128);
            p.AddWithValue("@EntityElementName", item.EntityElementName, 128);
            p.AddWithValue("@Position", item.Position, 20);
            p.AddWithValue("@SegmentCode", item.SegmentCode, 20);
            p.AddWithValue("@SegmentRepeat", item.SegmentRepeat, 20);
            p.AddWithValue("@SegmentRequiredType",
               item.SegmentRequiredType, 20);
            p.AddWithValue("@SegmentReference", item.SegmentReference, 20);
            p.AddWithValue("@DataElementID", item.ElementID, 20);
            p.AddWithValue("@DataElementType", item.ElementType, 20);
            p.AddWithValue("@DataElement", item.Element, 128);
            p.AddWithValue("@DataElementDescription", 
               item.ElementDescription, 512);
            p.AddWithValue("@DataElementRequiredType", 
               item.ElementRequiredType, 20);
            p.AddWithValue("@DataType", item.DataType, 20);
            p.AddWithValue("@MinimumLength", item.MinimumLength);
            p.AddWithValue("@MaximumLength", item.MaximumLength);
            p.AddWithValue("@Loop", item.DataType, 20);

            var r1 = p.AddWithValue("@OutItemNo", (Int32)0, true);

            if (provider.Exec())
            {
               results.Data = item;
               item.ItemNo = Convert.ToInt32(r1.Value);
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

         RequestResponseInfo<ExchangeDefinitionInfo> response =
            new RequestResponseInfo<ExchangeDefinitionInfo>();
         response.Results = results;
         response.ResponseData = results.Data;

         return response;
      }

      /// <summary>
      /// First row in List of Lists contains the column/headers...
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="dataOwnerId"></param>
      /// <param name="namespacePrefix">unique edi exchange ID</param>
      /// <param name="items"></param>
      /// <returns></returns>
      public static RequestResponseInfo<string>
         PostDefinitions(
            string sessionId, string dataOwnerId, string namespacePrefix,
            string namespaceDescription, List<List<string>> rows)
      {
         ExchangeDefinitionHelper helper = 
            new ExchangeDefinitionHelper(rows[0]);

         ExchangeDefinitionInfo def;
         RequestResponseInfo<string> response =
            new RequestResponseInfo<string>();

         // insert / update exchange-code
         EdiExchangeRecord.UpdateExchangeCodeRecord(
            sessionId, dataOwnerId, namespacePrefix, namespaceDescription);

         // insert / update definitions

         int count = 0;
         foreach (var row in rows)
         {
            if (count == 0)
            {
               count++;
               continue;
            }
            def = helper.GetDefinition(row);
            def.ExchangeCode = namespacePrefix;
            def.DataOwnerId = dataOwnerId;
            def.ItemNo = -1;
            EdiExchangeRecord.UpdateExchangeDefinitionRecord(sessionId, def);
            count++;
         }
         return response;
      }

   }

}
