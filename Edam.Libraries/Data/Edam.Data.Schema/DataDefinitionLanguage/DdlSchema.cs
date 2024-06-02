using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Application;
using Edam.Data.Schema.SchemaObject;
using Edam.Data.AssetManagement;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetSchema;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Edam.Data.Schema.DataDefinitionLanguage
{

   public class DdlSchema : 
      AssetSchemaWriter, ISchemaWriter, IResourceCallHelpers
   {

      #region -- Static Strings...

      private const String CLASS_NAME = "DdlSchema";

      private const String FOWARD_SLASH = "/";
      private const String BLOCK_OPEN = "BEGIN";
      private const String BLOCK_CLOSE = "END";
      private const String LINE_COMMENT_START = "--";
      private const String VARCHAR = "varchar";
      private const String DEFAULT_MAP_ELEMENT_NAME = "[any]";

      #endregion
      #region -- Properties and Declarations...

      public int OrdinalNo { get; set; }

      private String m_DataDomainName = null;
      private DataDomain m_DataDomain;
      public String DataDomainName
      {
         get { return m_DataDomainName; }
      }

      public CatalogInfo m_Catalog;
      public CatalogInfo Catalog
      {
         get { return m_Catalog; }
      }

      public SchemaInfo m_Schema;

      public DataDomain Domain
      {
         get { return m_DataDomain; }
         set
         {
            if (value != null)
               m_DataDomainName = value.DomainName.Replace(" ", "");
            m_DataDomain = value;
         }
      }

      public DdlSchema Instance
      {
         get { return this; }
      }

      public String Name
      {
         get { return m_Schema.Name; }
      }

      public List<ResourceInfo> Items
      {
         get { return m_Schema.Items; }
      }

      /// <summary>
      /// Ddl code buffer...
      /// </summary>
      private StringBuilder m_OutText = new StringBuilder();

      /// <summary>
      /// Documentation buffer...
      /// </summary>
      private StringBuilder m_OutProperties = new StringBuilder();
      public string OutPropertiesText
      {
         get { return m_OutProperties.ToString();}
      }

      private Edam.Text.IndentText m_Indent = new Text.IndentText();

      public Edam.Text.IndentText Indent
      {
         get { return m_Indent; }
      }

      #endregion
      #region -- Constructor, Destructor...

      public DdlSchema(
         CatalogInfo catalog, SchemaInfo schema,  NamespaceInfo ns) : 
         base(VARCHAR)
      {
         m_Catalog = catalog;
         m_Schema = schema;
         Namespace = ns;
      }

      #endregion
      #region -- Override Output Methods...

      /// <summary>
      /// Output Column Definition
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="element"></param>
      /// <param name="tail"></param>
      /// <param name="IsAutoPrimaryKey"></param>
      protected override void OutputColumnDefinition(StringBuilder builder,
         IDataElement element, String tail, ConstraintType constraint)
      {
         string typeText = GetTypeDeclaration(element);
         builder.AppendLine(m_Indent.Identation + "[" + GetColumnName(element) +
            "] " + typeText + (constraint == ConstraintType.autoGenerate ? 
            " IDENTITY" : String.Empty) +
            " NOT NULL" + tail);
      }

      /// <summary>
      /// Output Column Definition
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="element"></param>
      /// <param name="typeName"></param>
      /// <param name="mapElement"></param>
      /// <param name="tail"></param>
      protected override void OutputColumnDefinition(
         StringBuilder builder, IDataElement element,
         string typeName, DataTextElementInfo mapElement, string tail)
      {
         OutputColumnDefinition(builder, element.ElementName,
            element.TypeEntityName, mapElement, tail);
      }

      /// <summary>
      /// Output Column Definition
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="element"></param>
      /// <param name="tail"></param>
      protected override void OutputColumnDefinition(StringBuilder builder,
         IDataElement element, String tail)
      {
         builder.AppendLine(m_Indent.Identation + "[" +
            element.ElementQualifiedName.OriginalName + "] " +
            GetTypeDeclaration(element,
               element.TypeQualifiedName.OriginalName,
               element.MaxLength.ToString()) +
            tail);
      }

      /// <summary>
      /// Output Comment
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="comment"></param>
      protected override void OutputComment(
         StringBuilder builder, string comment)
      {
         builder.AppendLine(m_Indent.Identation + "-- " + comment);
      }

      #endregion
      #region -- Output Annotation

      /// <summary>
      /// Output annotation / as an extended property - value.
      /// </summary>
      /// <param name="builder">text buffer for annotations</param>
      /// <param name="schemaName">schema name</param>
      /// <param name="entityName">entity name</param>
      /// <param name="elementName">element name</param>
      /// <param name="propertyName">property name</param>
      /// <param name="annotationText">annotation text (value)</param>
      private void OutputAnnotation(StringBuilder builder,
         string schemaName, string entityName, string elementName,
         string propertyName, string annotationText)
      {
         bool hasElement = !String.IsNullOrWhiteSpace(elementName);

         builder.AppendLine(m_Indent.Identation +
            "EXECUTE sp_addextendedproperty");
         builder.AppendLine(m_Indent.Identation +
            "   @name = '" + propertyName +
            "', @value = '" + annotationText + "',");
         builder.AppendLine(m_Indent.Identation +
            "   @level0type = N'SCHEMA',   @level0name = N'" + 
            schemaName + "',");
         builder.AppendLine(m_Indent.Identation +
            "   @level1type = N'TABLE',    @level1name = N'" + 
            entityName + "'" + (hasElement ? "," : ";"));

         if (hasElement)
         {
            builder.AppendLine(m_Indent.Identation +
               "   @level2type = N'COLUMN',   @level2name = N'" +
               elementName + "';");
         }
         builder.AppendLine();
      }

      #endregion
      #region -- Output Element : Column definition

      /// <summary>
      /// Get type text declaration.
      /// </summary>
      /// <param name="element">data element</param>
      /// <param name="dataType">data type</param>
      /// <param name="dataSize">data size</param>
      /// <returns>text type declaration is returned</returns>
      private new string GetTypeDeclaration(IDataElement element,
         String dataType, String dataSize)
      {
         object mapResult =
            base.GetTypeDeclaration(element, dataType, dataSize);
         DataTextElementInfo delement = mapResult as DataTextElementInfo;

         if (delement == null)
         {
            DataTextMapItem mapItem = mapResult as DataTextMapItem;
            if (mapItem != null && mapItem.TargetText != null)
            {
               if (mapItem.TargetText.ToUpper() != VARCHAR.ToUpper())
               {
                  return mapItem.TargetText.ToUpper();
               }
            }
         }

         // TODO: set the default size in the settings or as a config param...
         string dsize = dataSize == "-1" || 
            String.IsNullOrWhiteSpace(dataSize) ? "256" : dataSize;

         string dtype = VARCHAR;
         if (delement == null)
         {
            return VARCHAR.ToUpper() + "(" + dsize + ")";
         }

         //dataSize = "(" + (delement == null ?
         //   DEFAULT_SIZE : delement.BaseType.DataSize) + ")";
         dataSize = "(" + dsize + ")";

         // return type declaration
         return dtype.ToUpper() + dataSize;
      }

      private string GetTypeDeclaration(IDataElement element)
      {
         if (element.TypeQualifiedName == null)
         {
            element.TypeQualifiedName =
               new QualifiedNameInfo(element.ElementDataType);
         }

         if (String.IsNullOrWhiteSpace(element.TypeEntityName))
         {
            element.TypeEntityName = element.ElementQualifiedName.Name;
         }

         return GetTypeDeclaration(element,
               element.TypeQualifiedName.Name,
               element.MaxLength.ToString());
      }

      private string GetColumnName(IDataElement element)
      {
         string name = element.TypeEntityName;
         return DataElement.GetUnqualifyText(name); // name.Replace(":", "_");
      }

      public void AddDescription(StringBuilder builder, String description)
      {
         // write: -- <description>
         if (!String.IsNullOrWhiteSpace(description))
            builder.AppendLine(m_Indent.Identation + "-- " + description);
      }

      private void OutputColumnDefinition(
         StringBuilder builder, string elementName,
         DataTextElementBaseTypeInfo type, string tail)
      {
         //var eName = elementName.Replace(":", "_");
         var eName = DataElement.GetUnqualifyText(elementName, FOWARD_SLASH);
         builder.AppendLine(m_Indent.Identation + 
            "[" + eName + "] " + type.TypeName.ToUpper() +
            (String.IsNullOrWhiteSpace(type.DataSize) ?
               String.Empty : "(" + type.DataSize + ")") +
            (type.Nullable ? String.Empty : " NOT NULL") + tail);
      }

      private void OutputColumnDefinition(
         StringBuilder builder, IDataElement element, string elementName,
         DataTextElementBaseTypeInfo type, string tail)
      {
         var name = element == null || element.TypeEntityName == null ?
            elementName : element.TypeEntityName + "/" + elementName;
         OutputColumnDefinition(builder, name, type, tail);
      }

      private void OutputColumnDefinition(
         StringBuilder builder, string elementName,
         string typeName, DataTextElementInfo mapElement, string tail)
      {
         string name = String.IsNullOrWhiteSpace(typeName) ?
            elementName : typeName + FOWARD_SLASH + elementName;
         OutputColumnDefinition(builder, name, mapElement.BaseType, tail);
      }

      protected override void OutputConstraintDefinition(StringBuilder builder,
         string constraintName, string keyName, string foreignName,
         string foreignKeyName, string tail = ",")
      {
         builder.AppendLine(m_Indent.Identation + "CONSTRAINT fk_" +
            constraintName.Replace(".",String.Empty));
         builder.AppendLine(m_Indent.Identation + 
            "   FOREIGN KEY ([" + keyName + "])");
         builder.AppendLine(m_Indent.Identation + "   REFERENCES " +
            foreignName + "([" + foreignKeyName + "])" + tail);
      }

      private void OutputConstraintDefinition(StringBuilder builder,
         List<DataTextElementInstanceInfo> constraints, string tail = ",")
      {
         var p = GetDefaultPrimaryKeyName();
         int cnt = 0;
         int tcount = constraints.Count;
         foreach (var c in constraints)
         {
            cnt++;
            OutputConstraintDefinition(builder, c.Name, c.ElementName,
               c.DataTypeName, p.Name, 
               cnt < tcount ? "," : String.Empty);
         }
      }

      private DataTextElementBaseTypeInfo GetDefaultPrimaryKeyName()
      {
         if (m_MapDefaultProperty == null)
         {
            return null;
         }
         var p = m_MapDefaultProperty.PrimaryKey;
         if (p == null || p.Count == 0)
         {
            return null;
         }
         if (p.Count > 1)
         {
            throw new Exception(
               "CONSTRAINTS based on multiple keys not supported");
         }
         return p[0];
      }

      /// <summary>
      /// This is called when no primary key(s) were found in the data asset.
      /// Here and if provided the Map Element Properties will be reviewed to
      /// see if defaults (surgate keys) or other has been defaulted or be 
      /// specified for the given resource.
      /// </summary>
      /// <param name="builder">string builder instance</param>
      /// <param name="resource">parent resource</param>
      /// <returns>list of keys are returned if any were found</returns>
      private string GetElementPrimaryKeys(
         StringBuilder builder, string resourceName)
      {
         DataTextElementPropertyInfo p = 
            m_Mapper.FindElementProperty(resourceName);
         if (p == null)
         {
            p = m_MapDefaultProperty;
         }
         if (p == null)
         {
            return String.Empty;
         }

         string keys = null;
         int cnt = 0;
         int tcount = p.PrimaryKey.Count;
         foreach(var k in p.PrimaryKey)
         {
            cnt++;
            builder.AppendLine(m_Indent.Identation + "[" + k.Name + "] " +
               k.TypeName + (k.Identity ? " IDENTITY" : String.Empty) +
               " NOT NULL" + ",");

            keys += (keys == null ? "" : ",") + k.Name;
         }

         return keys;
      }

      /// <summary>
      /// Output Primary Key constraint... if there is no primary key defined
      /// then one is created...
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="resourceName"></param>
      /// <param name="keyList"></param>
      /// <param name="tail"></param>
      private void OutputPrimaryKeyDefinition(StringBuilder builder,
         string resourceName, string keyList, string tail)
      {
         if (String.IsNullOrWhiteSpace(keyList))
         {
            keyList = "_RECORD_NO_";
            builder.AppendLine(m_Indent.Identation + "[" + keyList + "] " +
               "BIGINT IDENTITY NOT NULL,");
         }

         builder.AppendLine(m_Indent.Identation +
            "CONSTRAINT pk_" + resourceName.Replace(".", "_") +
            " PRIMARY KEY (" +
               (String.IsNullOrWhiteSpace(keyList) ?
                  String.Empty : keyList) + ")" + tail);
      }

      private string GetKeyList(StringBuilder builder, 
         string resourceName, List<IDataElement> keys)
      {
         String keyList = null;
         if (keys != null)
         {
            foreach (var k in keys)
            {
               keyList += (keyList == null ? "" : ",")
                  + "[" + k.ElementQualifiedName.OriginalName + "]";
            }
         }

         if (keyList == null && m_Mapper.ElementProperty != null)
         {
            keyList = GetElementPrimaryKeys(builder, resourceName);
         }
         return keyList;
      }

      /// <summary>
      /// Output Primary Key Definition(s)...
      /// </summary>
      /// <param name="builder">string builder instance</param>
      /// <param name="resourceName">parent resource name</param>
      /// <param name="keys">found key list</param>
      private void OutputPrimaryKeyDefinition(StringBuilder builder,
         string resourceName, List<IDataElement> keys, string tail)
      {
         String keyList = GetKeyList(builder, resourceName, keys);
         OutputPrimaryKeyDefinition(builder, resourceName, keyList, tail);
      }

      /// <summary>
      /// Output additional properties defined in the MapText data component.
      /// </summary>
      /// <param name="builder">string builder instance</param>
      /// <param name="resource">parent resource</param>
      private void OutputAdditionalProperties(StringBuilder builder,
         string resourceName, 
         DataTextElementPropertyInfo additionalProperties)
      {
         DataTextElementPropertyInfo properties = 
            additionalProperties ?? GetAdditionalProperties(resourceName);

         if (properties == null)
         {
            return;
         }

         int cnt = -1;
         int tcount = properties.RecordTrackingItem.Count;
         foreach (var i in properties.RecordTrackingItem)
         {
            OutputColumnDefinition(builder, null, i.Name, i, ",");
            //   (cnt < tcount ? "," : String.Empty));
            cnt++;
         }
      }

      #endregion
      #region -- Outuput Resource : Table definition

      private void OutputCreateResource(StringBuilder builder, 
         string schemaName, string resourceName)
      {
         builder.AppendLine(m_Indent.Identation + "CREATE TABLE ["
            + schemaName + "].[" + resourceName + "] (");
         m_Indent.Push();
      }

      private void OutputCreateResourceDone(StringBuilder builder)
      {
         m_Indent.Pop();
         builder.AppendLine(");");
         builder.AppendLine("");
      }

      public void OutputConstraints(StringBuilder builder,
         AssetElementConstraintList items)
      {
         string cname;
         foreach(var c in items)
         {
            if (c.ContraintType != AssetElementContraintType.ForeignKey)
            {
               continue;
            }
            cname = c.ParentName + "_" + c.ElementName;
            OutputConstraintDefinition(builder, cname,
               c.ElementName, 
               "[" + c.ReferenceSchemaName + 
                  "].[" + c.ReferenceEntityName + "]",
               c.ReferenceElementName);
         }
      }

      /// <summary>
      /// Output associations...
      /// </summary>
      /// <param name="associations"></param>
      private void OutputAssociations(StringBuilder builder, string schemaName,
         DataTextElementPropertyInfo additionalProperties,
         List<DataTextElementInstanceInfo> associations)
      {
         var pk = GetDefaultPrimaryKeyName();
         foreach(var a in associations)
         {
            // output the create resource statement...
            OutputCreateResource(builder, schemaName, a.Name);

            // output association properties...
            string keyName = a.EntityName;
            string foreignName = a.EntityName;

            OutputColumnDefinition(builder, keyName + pk.Name, String.Empty, 
               additionalProperties.DefaultSurragateReference, ",");
            OutputColumnDefinition(
               builder, a.DataTypeName + pk.Name, String.Empty, 
               additionalProperties.DefaultSurragateReference, ",");

            // output remaining properties...
            OutputAdditionalProperties(builder, a.Name, additionalProperties);

            // output primary key (surrate key)
            String keyList = GetKeyList(builder, a.Name, null);
            OutputPrimaryKeyDefinition(builder, a.Name, keyList,
               m_MapDefaultProperty.ElementTraversing.ArrayToAssociation ?
                  "," : String.Empty);

            // output constraints...
            if (m_MapDefaultProperty.ElementTraversing.ArrayToAssociation)
            {
               m_AssociationCounter++;
               OutputConstraintDefinition(builder, keyName + "_Association_" +
                  m_AssociationCounter.ToString(), keyName + pk.Name,
                  foreignName, pk.Name, ",");

               m_AssociationCounter++;
               keyName = a.DataTypeName;
               foreignName = a.DataTypeName;
               OutputConstraintDefinition(builder, keyName + "_Association_" +
                  m_AssociationCounter.ToString(), keyName + pk.Name,
                  foreignName, pk.Name, String.Empty);
            }

            OutputCreateResourceDone(builder);
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="resource"></param>
      private List<DataTextElementInstanceInfo> OutputResourceDefinition(
         StringBuilder builder, string resourceName, NamespaceInfo ns,
         AssetDataElementList assets,
         DataTextElementInstanceInfo instance = null)
      {
         ApplicationLog console = new ApplicationLog();

         // check resource as an element to see if needs to be ignored...

         // is there is any Map Element resource available?
         if (IgnoreResourceElement(resourceName))
         {
#if DEBUG_
            //console.WriteLine("Skip: " + resourceName);
#endif
            return null;
         }

         // get additional properties...
         DataTextElementPropertyInfo additionalProperties =
            GetAdditionalProperties(resourceName);

         OutputState state = new OutputState(
            resourceName, additionalProperties, assets);
         state.Builder = builder;
         state.Console = console;

         // process resource...
         string instanceName = null;
         DataTextElementBaseTypeInfo instancePk = null;

         string schemaName = ns.NamePath.Schema;

         OutputCreateResource(builder, schemaName, resourceName);

         if (instance != null)
         {
            instancePk = GetDefaultPrimaryKeyName();
            instanceName = resourceName + "_" + instancePk.Name;
            OutputColumnDefinition(builder, instanceName, 
               "", instance.MapElement, ",");
         }

         OutputAsset(state);

         OutputAdditionalProperties(
            builder, resourceName, additionalProperties);
         OutputPrimaryKeyDefinition(builder, resourceName, state.Keys,
            state.Constraints == null || state.Constraints.Count == 0 ? 
               String.Empty : ",");

         OutputConstraints(builder, state.ConstraintItems);

         // add aditional constraints
         if (instance != null)
         {
            OutputConstraintDefinition(builder, instanceName, instanceName,
               instance.EntityName, instancePk.Name, 
               state.HasContraints ? "," : String.Empty);
         }

         OutputConstraintDefinition(builder, state.Constraints, ",");
         OutputCreateResourceDone(builder);

         OutputAssociations(
            builder, schemaName, additionalProperties, state.Constraints);

         return state.Objects;
      }

      /// <summary>
      /// Output Create Schema statements.
      /// </summary>
      /// <param name="schemaName">schema name</param>
      public void OutputCreateSchema(string schemaName)
      {
         m_OutText.AppendLine(LINE_COMMENT_START + " " + schemaName);
         m_OutText.AppendLine("CREATE SCHEMA [" + schemaName + "]");
         m_OutText.AppendLine("GO");
         m_OutText.AppendLine(String.Empty);
      }

      #endregion
      #region -- Prepare, Get Output

      private void PrepareObjectOutput(
         DataTextElementInstanceInfo element, NamespaceInfo ns)
      {
         if (ElementTypeResolver != null)
         {
            ElementTypeInfo type = ElementTypeResolver(
               element.Element.ElementDataType);
             var objects = OutputResourceDefinition(
               m_OutText, element.EntityName.Replace(".","_") + "_" +
               element.DataTypeName, ns,
               type.Children, element);
         }
      }

      /// <summary>
      /// Prepare Ouput for all Schema Items...
      /// </summary>
      /// <param name="mapper">asset/element mappings</param>
      public void PrepareOutput(DataTextMap mapper = null)
      {
         m_Mapper = mapper == null ? new DataTextMap() : mapper;
         m_MapDefaultProperty = 
            m_Mapper.FindElementProperty(DEFAULT_MAP_ELEMENT_NAME);

         m_OutText = new StringBuilder();
         m_Indent.Clear();

         OutputCreateSchema(m_Schema.Name);

         foreach (var resource in m_Schema.Items)
         {
            var l = resource.Resources.ToList<AssetDataElement>();
            AssetDataElementList list = new AssetDataElementList(
               m_Schema.Namespace, AssetType.Schema, m_Schema.VersionId);
            list.AddRange(l);

            // TODO: further investigate this...
            // ignore resources whose names are the same as the schema
            if (resource.Name == resource.Namespace.NamePath.Schema)
            {
               continue;
            }

            var objects = OutputResourceDefinition(
               m_OutText, resource.Name, resource.Namespace, list);

            if (objects != null)
            {
               // go through each object and output corresponding declarations
               foreach (var obj in objects)
               {
                  PrepareObjectOutput(obj, resource.Namespace);
               }
            }
         }
      }

      public void PrepareAnnotations()
      {
         // TODO: replace hardcoded values...
         foreach (var resource in m_Schema.Items)
         {
            OutputAnnotation(m_OutProperties, resource.Entity.Domain, 
               resource.Name, null, "Description", 
               resource.Entity.AnnotationText);
            foreach(var item in resource.Items)
            {
               OutputAnnotation(m_OutProperties, resource.Entity.Domain,
                  resource.Name, item.Name, "Description",
                  item.Element.AnnotationText);

               if (item.Element.Kind == DataElementKind.ExternalReference)
               {
                  OutputAnnotation(m_OutProperties, resource.Entity.Domain,
                     resource.Name, item.Name, "X_Reference",
                     item.Element.AnnotationText);
               }

               if (!String.IsNullOrWhiteSpace(item.Element.Tags))
               {
                  OutputAnnotation(m_OutProperties, resource.Entity.Domain,
                     resource.Name, item.Name, "Privacy",
                     item.Element.Tags);
               }
            }
         }
      }

      public new String ToString()
      {
         return m_OutText.ToString();
      }

      #endregion

   }

}
