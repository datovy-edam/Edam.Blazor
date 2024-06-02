using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.InOut;

namespace Edam.Json.Jsd
{

   public class JsdSet
   {
      private readonly List<JsdSchema> m_Items = new List<JsdSchema>();
      private readonly IWriter m_Writer;
      public List<JsdSchema> Items
      {
         get { return m_Items; }
      }
      public JsdSet(IWriter writer)
      {
         m_Writer = writer;
      }
      public void AddSchema(JsdSchema schema)
      {
         m_Items.Add(schema);
      }
      public void CompileSchemas(Func<Exception,string> errorNotification)
      {

      }
      public void GenerateSchemas(List<NamespaceInfo> namespaces)
      {
         foreach(var s in m_Items)
         {
            //if (s.Items.Count == 0)
            //{
            //   continue;
            //}
            m_Writer.Write(s.Namespace.NamePath.FullName, s.ToString());
         }
      }
   }

}
