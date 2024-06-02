using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;

namespace Edam.DataObjects.Assets
{

   public class DataReferenceFetchResult
   {
      public DataReferenceFetchRequest Request { get; set; }

      public List<DataDomain> Domains { get; set; }
      public List<DataTerm> Terms { get; set; }
      public AssetDataElementList Elements { get; set; }

      public bool Success
      {
         get { return Elements != null; }
      }

      public DataReferenceFetchResult(DataReferenceFetchRequest request)
      {
         Request = request ?? new DataReferenceFetchRequest();
      }
   }

}
