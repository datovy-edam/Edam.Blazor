using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Json.Jsd;

namespace Edam.Json.Jsd
{

   public class JsdComplexType
   {
      public DataTypeInfo Type { get; set; }
      public IDataElement Element { get; set; }
      public List<AssetDataElement> Children { get; set; }
   }

}
