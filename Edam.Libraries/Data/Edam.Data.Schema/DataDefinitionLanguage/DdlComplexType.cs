using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;

namespace Edam.Data.Schema.DataDefinitionLanguage
{

   public class DdlComplexType
   {
      public DataTypeInfo Type { get; set; }
      public IDataElement Element { get; set; }
      public List<IDataElement> Children { get; set; }
   }

}
