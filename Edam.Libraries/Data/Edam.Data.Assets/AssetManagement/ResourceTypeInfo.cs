using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.AssetSchema;

namespace Edam.Data.AssetManagement.Resources
{

   public class ElementTypeInfo
   {
      public String Key { get; set; }
      public AssetDataElement Element { get; set; }
      public AssetDataElementList Children { get; set; }
   }

}
