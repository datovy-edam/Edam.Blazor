using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;

namespace Edam.Gsql
{

   public class GsqlItemInfo
   {

      public readonly static String VERTEX = "vertex";
      public readonly static String EDGE = "edge";

      public NamespaceInfo Namespace { get; set; }
      public String ItemName { get; set; }
      public String UriPrefix { get; set; }
      public IDataElement Element { get; set; }
      public String Description { get; set; }
      public GsqlItemType Type { get; set; }
      public String TypeText
      {
         get { return GsqlTypeInfo.ToString(Type); }
      }
      public Boolean AdditionalProperties { get; set; }

      public GsqlItemInfo(NamespaceInfo ns, String name, GsqlItemType type,
         Boolean additionalProperties = true)
      {
         Namespace = ns;
         ItemName = name;
         Type = type;
         AdditionalProperties = additionalProperties;
      }

      public static String ToString(GsqlItem item)
      {
         return item.ToString();
      }

   }

}
