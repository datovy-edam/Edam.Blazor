using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.Asset
{

   /// <summary>
   /// Provides information about an element type (also see ElementTypeInfo).
   /// </summary>
   public class DataTypeInfo : ElementBaseInfo
   {

      public DataTypeInfo(
         IDataElement baseElement, NamespaceInfo ns = null,
         String typeNameSuffix = null) : base(baseElement, ns, typeNameSuffix)
      {
         //if (IsElement)
         //   throw new Exception("DataTypeInfo::DataTypeInfo: Not a Type (" +
         //      baseElement.ResourceId + ")");
      }

      public DataTypeInfo(QualifiedNameInfo name, ElementBaseType type) :
         base(name, type)
      {
         DataType = this;
      }

      public DataTypeInfo() : base(null, null, null)
      {

      }

   }

}
