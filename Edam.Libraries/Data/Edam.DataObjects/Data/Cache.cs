using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.DataObjects.Documents;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using app = Edam.Application;

namespace Edam.DataObjects.Data
{

   public class Cache //: app.ICache
   {

      public async Task<D> GetItem<D>(string name) 
      {
         return await LocalDocumentStorageHelper.
            GetItem<DataDocumentItem, D>(name);
      }

      public async Task<int> SaveItem<T>(
         string name, T item, string description = null)
      {
         return await LocalDocumentStorageHelper.
            SaveItem<DataDocumentItem,T>(name, item, description);
      }

   }

}
