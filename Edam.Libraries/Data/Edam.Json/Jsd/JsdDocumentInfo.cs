using System;
using System.Collections.Generic;
using System.Text;

using Edam.Data.Asset;

namespace Edam.Json.Jsd
{

   public class JsdDocumentInfo : JsdItemInfo
   {
      public JsdVersion Version { get; set; }
      public String Title { get; set; }

      public JsdDocumentInfo(NamespaceInfo ns, String title, JsdType type,
         JsdVersion version = JsdVersion.Draft04,
         Boolean additionalProperties = true) : 
         base(ns, title, type, additionalProperties)
      {
         Title = title;
         Version = version;
      }
   }

}
