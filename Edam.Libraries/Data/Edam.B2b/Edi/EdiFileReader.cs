using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole;
using Edam.Xml.OpenXml;
using Edam.DataObjects.Services;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Application;
using Edam.Diagnostics;

namespace Edam.B2b
{

   public class EdiFileReader : IDataAssets
   {

      /// <summary>
      /// Read excel workbook TAB based on arguments domain ID and store items
      /// in a database fetched based on procedure being executed based on 
      /// process info in the arguments.
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public IResultsLog ToDatabase(AssetConsoleArgumentsInfo arguments)
      {
         string fname = arguments.GetInputFileName();
         var results = ExcelDocumentReader.ReadDocument(
            fname, arguments.Domain.DomainId);

         if (!results.Success)
         {
            return results;
         }

         var serv = ExchangeService.GetService();
         var task = serv.PostExchangeDefinition(
            Session.SessionId, arguments.Process.OrganizationId,
            arguments.Domain.DomainId, arguments.Domain.Description,
            results.Data);
         task.Wait();
         if (task.Status == TaskStatus.RanToCompletion)
         {
            return task.Result.Results;
         }
         ResultLog dbResults = new ResultLog();
         dbResults.Failed(EventCode.Failed);
         return dbResults;
      }

      /// <summary>
      /// Given a workbook read its content and move content into an Asset
      /// items and return it.  This will read whatever content is on the 
      /// workbook.
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public IResultsLog ToAsset(AssetConsoleArgumentsInfo arguments)
      {
         string fname = arguments.GetInputFileName();
         var results = ExcelDocumentReader.ReadDocument(
            fname, arguments.Domain.DomainId);

         if (!results.Success)
         {
            return results;
         }

         Data.AssetSchema.AssetData adata = new Data.AssetSchema.AssetData(
            arguments.Namespace, AssetType.Schema, 
            arguments.Project.VersionId);
         adata.SetDefaultNamespace(arguments.Namespace);
         arguments.AssetDataItems = new AssetDataList();
         arguments.AssetDataItems.Add(adata);
         arguments.AssetDataItems[0].Items =
            Edi.EdiAsset.ToAssets(results.Data, arguments);
         ResultLog dbResults = new ResultLog();
         dbResults.Succeeded();
         return dbResults;
      }
      
   }

}
