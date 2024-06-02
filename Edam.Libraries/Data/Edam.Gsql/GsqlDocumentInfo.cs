using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Data.Asset;

namespace Edam.Gsql
{

   public class GsqlDocumentInfo : GsqlItemInfo
   {
      public String Version { get; set; }
      public String Title { get; set; }

      public GsqlDocumentInfo(NamespaceInfo ns, String title, GsqlItemType type,
         string version = "",
         Boolean additionalProperties = true) :
         base(ns, title, type, additionalProperties)
      {
         Title = title;
         Version = version;
      }
   }

}
