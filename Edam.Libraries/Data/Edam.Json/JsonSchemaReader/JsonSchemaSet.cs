using System;
using System.Collections.Generic;
using System.Text;

using Edam.Data.Asset;
using Edam.Json.JsonSchema;

namespace Edam.Json.JsonSchemaReader
{

   public class JsonSchemaSet
   {
      private List<JsonSchemaInfo> m_Schemas = new List<JsonSchemaInfo>();

      private NamespaceInfo m_Namespace;
      public NamespaceInfo Namespace
      {
         get { return m_Namespace; }
         set { m_Namespace = value; }
      }

      private string m_VersionId;
      public string VersionId
      {
         get { return m_VersionId; }
      }

      private NamespaceList m_Namespaces;
      public NamespaceList Namespaces
      {
         get { return m_Namespaces; }
      }

      public int Count
      {
         get { return m_Schemas.Count; }
      }

      public List<JsonSchemaInfo> Schemas
      {
         get { return m_Schemas; }
      }
      public JsonSchemaSet(NamespaceList namespaces)
      {
         m_Namespaces = namespaces ?? new NamespaceList();
      }
      public void Add(JsonSchemaInfo schema)
      {
         m_Schemas.Add(schema);
      }
   }

}
