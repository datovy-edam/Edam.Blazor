using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Data;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Documents
{

   [Table("DataDocumentItem")]
   public class DataDocumentItem : DataDocumentItemRegistry
   {
      public const string TABLE_NAME = "DataDocumentItem";

      #region -- 1.0 - Properties and definitions...

      #endregion
      #region -- 1.0 - Commands

      #endregion
      #region -- 1.5 - Initialize Resources

      public DataDocumentItem() : base()
      {
         SetTableName(TABLE_NAME);
      }

      #endregion
      #region -- 4.0 - Support Methods

      public static async Task<int> Delete(string name)
      {
         return await LocalDocumentStorageHelper.
            DeleteDocumentItem<DataDocumentItem>(name);
      }

      public static async Task<List<DataCodeInfo>> GetCodeSet(string name)
      {
         return await LocalDocumentStorageHelper.
            GetCodeSet<DataDocumentItem>(name);
      }

      public static async Task<int> SaveItem<T>(
         string name, T item, string description = null, bool deleteIt = true)
      {
         return await LocalDocumentStorageHelper.
            SaveItem<DataDocumentItem, T>(
               name, item, description, deleteIt);
      }

      public static async Task<T> GetItem<T>(string name)
      {
         return await LocalDocumentStorageHelper.
            GetItem<DataDocumentItem, T>(name);
      }

      #endregion
      #region -- 4.0 - Binary, Text and Json Serialization Support


      #endregion

   }

}
