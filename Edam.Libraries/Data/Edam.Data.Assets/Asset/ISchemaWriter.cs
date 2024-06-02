using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.Asset
{

   public interface ISchemaOutput
   {

   }

   public interface ISchemaWriter
   {
      //List<NamespaceInfo> Namespaces { get; }
      NamespaceInfo Namespace { get; set; }
   }

}
