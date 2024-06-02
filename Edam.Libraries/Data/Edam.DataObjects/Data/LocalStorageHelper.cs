using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Data;
using Edam.DataObjects.Documents;
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.ReferenceData;

namespace Edam.DataObjects.Data
{

   public class LocalDocumentStorageHelper
   {

      public static void InitializeAll()
      {
         CreateTable<DataDocumentItem>();
         CreateTable<ReferenceDataItem>();
      }

      public static ResultsLog<bool> CreateTable<T>() 
         where T : IDataDocumentItem, new()
      {
         ResultsLog<bool> results = new ResultsLog<bool>();
         LDbConnection c = null;
         Task<CreateTableResult> tresults = null;
         try
         {
            c = new LDbConnection();
            tresults = c.CreateTableAsync<T>();
            tresults.Wait();
            if (tresults.Status == TaskStatus.RanToCompletion)
            {
               results.Succeeded();
               results.Data = true;
            }
            else
            {
               results.Failed(EventCode.Failed);
               results.Data = false;
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (tresults != null)
            {
               tresults.Dispose();
            }
            if (c != null)
            {
               c.Dispose();
            }
            c = null;
         }
         return results;
      }

      public static async Task<List<T>> GetDocumentItem<T>(
         string name) where T : IDataDocumentItem, new()
      {
         return await DataDocumentItemHelper.GetDocumentByName<T>(name);
      }

      public static async Task<int> DeleteDocumentItem<T>(string name)
         where T : IDataDocumentItem, new()
      {
         var results = await DataDocumentItemHelper.GetDocumentByName<T>(name);
         ResultsLog<int> rlog = new ResultsLog<int>();
         if (results.Count == 0)
         {
            return 0;
         }
         return await DataDocumentItemHelper.DeleteDocument(results[0].IdNo);
      }

      public static async Task<int> SaveItem<T,D>(
         string name, D item, string description = null, bool deleteIt = true)
         where T : IDataDocumentItem, new()
      {
         // have only one copy
         if (deleteIt)
         {
            var r = await DeleteDocumentItem<T>(name);
         }

         // insert item
         string jsonText = DataDocumentItem.ToJson<D>(item);
         byte[] binaryData = DataDocumentItem.ToBinary(jsonText);
         return await DataDocumentItemHelper.
            InsertUpdate<T>(
               name, binaryData,
               Medias.MediaContentType.application_json, description,
               version: null, dataOwnerId: null);
      }

      public static async Task<L> GetItem<T,L>(string name)
         where T : IDataDocumentItem, new()
      {
         L group = default(L);
         var results = await
            DataDocumentItemHelper.GetDocumentByName<T>(name);
         if (results.Count > 0)
         {
            IDataDocumentItem m = results[0];
            group = DataDocumentItem.FromJson<L>(m.BinaryData);
         }
         return group;
      }

      public static async Task<List<DataCodeInfo>> GetCodeSet<T>(string name)
         where T : IDataDocumentItem, new()
      {
         return await GetItem<T,List<DataCodeInfo>>(name);
      }

   }

}
