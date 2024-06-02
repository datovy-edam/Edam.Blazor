using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Documents;
using Edam.DataObjects.Data;
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Dynamic;
using Edam.DataObjects.Models;
using Edam.Diagnostics;

namespace Edam.DataObjects.Logs
{

   [Table("DataChangeLogItem")]
   public class DataChangeLogItem : DataDocumentItemRegistry
   {
      public const string TABLE_NAME = "DataChangeLogItem";

      #region -- 1.0 - Properties and definitions...

      #endregion
      #region -- 1.0 - Commands

      #endregion
      #region -- 1.5 - Initialize Resources

      public DataChangeLogItem() : base()
      {
         SetTableName(TABLE_NAME);
      }

      #endregion
      #region -- 4.0 - Support Methods

      public static async Task<int> Delete(string name)
      {
         return await LocalDocumentStorageHelper.
            DeleteDocumentItem<DataChangeLogItem>(name);
      }

      public static async Task<List<DataCodeInfo>> GetCodeSet(string name)
      {
         return await LocalDocumentStorageHelper.
            GetCodeSet<DataChangeLogItem>(name);
      }

      public static async Task<int> SaveItem<T>(
         string name, T item, string description = null, bool deleteIt = true)
      {
         return await LocalDocumentStorageHelper.
            SaveItem<DataChangeLogItem, T>(
               name, item, description, deleteIt);
      }

      public static async Task<T> GetItem<T>(string name)
      {
         return await LocalDocumentStorageHelper.
            GetItem<DataChangeLogItem, T>(name);
      }

      /// <summary>
      /// Log Changes as found in the Dynamic-Table.
      /// </summary>
      /// <param name="table">object to be logged if any changes are found
      /// </param>
      /// <param name="setOriginalValues">true to reset the original values 
      /// using current values and original values to be the same, but first
      /// preserving the information about the changes</param>
      public static async void LogChanges(
         DynamicTableInfo table, bool setOriginalValues = false)
      {
         ResultsLog<ElementChangeLog> changes = table.PrepareChangeLog(
            setOriginalValues: setOriginalValues);
         if (changes.Success && changes.Data.HasChanges)
         {
            var t = await DataChangeLogItem.SaveItem<ElementChangeLog>(
               DataChangeLogItem.TABLE_NAME, changes.Data, "");
         }
      }

      #endregion
      #region -- 4.0 - Binary, Text and Json Serialization Support


      #endregion

   }

}
