using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using SQLite;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Data;
using Edam.Data;
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Documents;
using Edam.Diagnostics;
using Edam.DataObjects.Models;
using Edam.Serialization;
using Edam.DataObjects.Dynamic;

namespace Edam.DataObjects.ReferenceData
{

   [Table("ReferenceDataItem")]
   public class ReferenceDataItem : DataDocumentItemRegistry
   {

      #region -- 1.0 - Properties and definitions...

      public const string DATA_JSON = "Data.Json";
      public const string TABLE_NAME = "ReferenceDataItem";

      #endregion
      #region -- 1.0 - Commands

      //public ICommand LoginCommand { protected set; get; }
      //public ICommand LogoutCommand { protected set; get; }
      //public ICommand SaveRecordCommand { protected set; get; }

      #endregion
      #region -- 1.5 - Initialize Resources

      public ReferenceDataItem() : base()
      {
         SetTableName(TABLE_NAME);
      }

      public static void InitializeInstance()
      {
         LocalDocumentStorageHelper.CreateTable<ReferenceDataItem>();
      }

      #endregion
      #region -- 4.0 - Support Methods

      public static async Task<int> Delete(string name)
      {
         return await LocalDocumentStorageHelper.
            DeleteDocumentItem<ReferenceDataItem>(name);
      }

      public static async Task<List<DataCodeInfo>> GetCodeSet(string name)
      {
         return await LocalDocumentStorageHelper.
            GetCodeSet<ReferenceDataItem>(name);
      }

      public static async Task<int> SaveItem<T>(
         string name, T item, string description = null, bool deleteIt = true)
      {
         return await LocalDocumentStorageHelper.
            SaveItem<ReferenceDataItem, T>(
               name, item, description, deleteIt);
      }

      public static async Task<T> GetItem<T>(string name)
      {
         return await LocalDocumentStorageHelper.
            GetItem<ReferenceDataItem, T>(name);
      }

      /// <summary>
      /// Save Item as JSON text.
      /// </summary>
      /// <param name="name"></param>
      /// <param name="items"></param>
      /// <returns></returns>
      public static async Task<int> SaveDataItemsAsJson(
         string name, List<dynamic> items)
      {
         string nm = name + DATA_JSON;
         string json = DynamicTableInfo.ToJson(nm, items);
         return await SaveItem<string>(name, json, name, true);
      }

      /// <summary>
      /// Get Reference Data Items...
      /// </summary>
      /// <param name="name">name of the collection</param>
      /// <returns>returns the Observable Collection</returns>
      public static async Task<ObservableCollection<dynamic>> 
         GetDataItemsFromJson(string name)
      {
         string nm = name + DATA_JSON;
         var r = await GetItem<string>(name);
         if (!String.IsNullOrWhiteSpace(r))
         {
            return DynamicTableInfo.FromJson(r);
         }
         return null;
      }

      #endregion

   }

}
