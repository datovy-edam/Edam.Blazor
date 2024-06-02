using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;

// -----------------------------------------------------------------------------

namespace Edam.Services
{

   [XmlType("Type")]
   public class TypeInfo
   {
      private Assembly m_Assembly = null;
      public String Key { get; set; }
      public String Alias { get; set; }
      public String AssemblyName { get; set; }
      public String TypeName { get; set; }

      [XmlIgnore]
      public Assembly Assembly
      {
         get { return m_Assembly; }
         set { m_Assembly = value; }
      }
   }

}
