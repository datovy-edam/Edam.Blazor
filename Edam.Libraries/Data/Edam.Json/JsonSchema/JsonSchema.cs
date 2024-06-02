using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;

namespace Edam.Json.JsonSchema
{

    public class JsonSchemaInfo
    {
        private readonly List<JsonComplexType> m_Definitions =
           new List<JsonComplexType>();
        private NamespaceList m_Namespaces;
        public NamespaceList Namespaces
        {
            get { return m_Namespaces; }
        }

        public List<JsonComplexType> Definitions
        {
            get { return m_Definitions; }
        }

        private readonly List<JsonPropertyInfo> m_Properties =
           new List<JsonPropertyInfo>();
        public List<JsonPropertyInfo> Properties
        {
            get { return m_Properties; }
        }

        public JsonSchemaInfo(NamespaceList namespaces)
        {
            m_Namespaces = namespaces ?? new NamespaceList();
        }
    }

}
