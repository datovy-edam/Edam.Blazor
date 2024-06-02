using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;

namespace Edam.Gsql
{

   public class GsqlSchema
   {

      #region -- Static Strings...

      private const String CLASS_NAME = "GsqlSchema";
      private const String VERTEX = "VERTEX";
      private const String EDGE = "EDGE";
      private const String DEFAULT_VERSION = "1.0";

      #endregion
      #region -- Properties and Declarations...

      private DataTextMap m_DataTextMap = new DataTextMap();

      public NamespaceList Namespaces { get; } =
         new NamespaceList();
      public NamespaceInfo Namespace { get; set; }

      private GsqlDocumentInfo m_Document;

      private StringBuilder m_OutText = new StringBuilder();
      private readonly Edam.Text.IndentText m_Indent = new Text.IndentText();

      public GsqlSchema Instance
      {
         get { return this; }
      }

      public Edam.Text.IndentText Indent
      {
         get { return m_Indent; }
      }

      #endregion
      #region -- Delegate Properties...

      public BaseTypeResolverDelegate BaseTypeResolver { get; set; }
      public NamespaceResolverDelegate NamespaceResolver { get; set; }
      public TypeResolverDelegate TypeResolver { get; set; }
      public DataTransformResolverDelegate TransformResolver { get; set; }

      #endregion
      #region -- Constructor, Destructor...

      public void Initialize(NamespaceInfo ns, String title, GsqlItemType type,
         string version = DEFAULT_VERSION,
         Boolean additionalProperties = true)
      {
         m_Document =
            new GsqlDocumentInfo(ns, title, type, version, additionalProperties);
         Namespace = ns;
      }

      public GsqlSchema(NamespaceInfo ns, String title, GsqlItemType type,
         string version = DEFAULT_VERSION,
         Boolean additionalProperties = true)
      {
         Initialize(ns, title, type, version, additionalProperties);
      }

      public GsqlSchema(GsqlDocumentInfo document)
      {
         SetDocument(document);
      }

      public GsqlSchema()
      {
         Initialize(null, null, GsqlItemType.Graph);
      }

      #endregion
      #region -- Namespace Support

      public void AddNamespace(NamespaceInfo ns)
      {
         if (ns == null)
            return;
         NamespaceInfo i = Namespaces.Find((x) => x.Prefix == ns.Prefix);
         if (i == null)
            Namespaces.Add(ns);
      }

      public void Add(string prefix, string uri)
      {
         NamespaceInfo ns = new NamespaceInfo(prefix, uri);
         if (Namespace.Prefix != prefix)
            AddNamespace(ns);
      }

      #endregion
      #region -- Support Methods

      public void SetDataTextMap(DataTextMap map)
      {
         if (map == null)
         {
            m_DataTextMap = new DataTextMap();
            return;
         }
         m_DataTextMap = map;
      }

      public void SetDocument(GsqlDocumentInfo document)
      {
         m_Document = document;
      }

      public void AddPush()
      {
         m_Indent.Push();
      }

      public void AddPop()
      {
         m_Indent.Pop();
      }

      #endregion
      #region -- Add Vertices and Edges

      public void AddElement(string schemaObjectName, string name)
      {
         m_OutText.AppendLine(m_Indent.Identation +
            "CREATE " + schemaObjectName + " " + name + " (");
         m_Indent.Push();
      }

      public void AddProperty(
         string propertyName, string typeName, Boolean isPrimaryKey,
         Boolean continues)
      {
         string pk = isPrimaryKey ? " PRIMARY KEY" : String.Empty;
         m_OutText.AppendLine(m_Indent.Identation + propertyName + " " +
            typeName + pk + (continues ? "," : String.Empty));
      }

      public void AddPropertyEnd(Boolean continues = false)
      {
         m_Indent.Pop();
         m_OutText.AppendLine(m_Indent.Identation + ")" +
            (continues ? "," : String.Empty));
      }

      public void AddVertex(IDataElement element,
         List<AssetDataElement> children, Boolean continues)
      {
         AddElement(VERTEX, element.ElementQualifiedName.OriginalName);
         int count = 0;
         int lastCount = children.Count - 1;
         foreach(var i in children)
         {
            string dataType = m_DataTextMap.MapText(
               i.ElementDataType, DataTextMapDirection.To);
            AddProperty(i.ElementQualifiedName.OriginalName, dataType.ToUpper(), 
               i.KeyType == ConstraintType.key, count != lastCount);
            count++;
         }
         m_Indent.Pop();
         m_OutText.AppendLine(")");
         m_OutText.AppendLine("");
      }

      #endregion
      #region -- Document Support

      public void DocumentStart()
      {
      }

      public void WriteDocument()
      {
         StringBuilder body = m_OutText;

         m_OutText = new StringBuilder();
         m_Indent.Clear();
         DocumentStart();
         m_OutText.Append(body);
         DocumentEnd();
      }

      public void DocumentEnd()
      {
         string func = CLASS_NAME + ":" + "DocumentEnt::";
         m_Indent.Pop();
         if (m_Indent.IdentCount != 0)
            throw new Exception(func + "INVALID IDENTATION.");
      }

      public new String ToString()
      {
         return m_OutText.ToString();
      }

      #endregion

   }

}
