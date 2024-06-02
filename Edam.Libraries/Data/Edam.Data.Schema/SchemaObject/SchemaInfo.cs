using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using System.Xml.Linq;

namespace Edam.Data.Schema.SchemaObject
{

   public interface IObjectInfo
   {
      string Name { get; set; }
   }

   public class ObjectInfo<T> where T : IObjectInfo
   {
      public string Name { get; set; }
      public List<T> Items { get; set; }

      public ObjectInfo()
      {
         Items = new List<T>();
      }

      public T Add(string itemName)
      {
         var s = Items.Find((x) => { return x.Name == itemName; });
         if (s != null)
            return s;
         s = (T)Activator.CreateInstance(typeof(T), new object[]
         {
            Name = itemName
         });
         return s;
      }
   }

   public class ElementInfo: IObjectInfo
   {
      public IDataElement Element { get; set; }
      public string Name { get; set; }
      public string DataType { get; set; }
      public string DataSize { get; set; }

      public bool IsNullable { get; set; }
      public bool IsKey { get; set; }
      public bool IsAutoGenerate { get; set; }

      public AssetElementInfo<IAssetElement> ToAsset(
         ResourceInfo parent, NamespaceInfo ns)
      {
         QualifiedNameInfo typeQName = new QualifiedNameInfo(DataType);
         AssetElementInfo<IAssetElement> a = new AssetElementInfo<IAssetElement>
         {
            DataType = DataType,
            Namespace = ns.Uri.OriginalString,
            EntityQualifiedName = parent == null ?
               null : new QualifiedNameInfo(ns.Prefix,parent.Name),
            MinOccurrence = (IsNullable ? 0 : 1),
            MaxOccurrence = 1,
            Occurs = "(" + (IsNullable ? "0" : "1") + ":1)",
            ElementQualifiedName = new QualifiedNameInfo(ns.Prefix,Name),
            ElementType = ElementType.element,
            KeyType = IsKey ?
               ConstraintType.key : ConstraintType.nonkey,
            AutoGenerateType = IsAutoGenerate ? 
               ConstraintType.autoGenerate : ConstraintType.none,
            IsNillable = IsNullable,
            TypeQualifiedName = typeQName
         };
         a.QualifiedTypeNames.Add(typeQName);
         if (decimal.TryParse(DataSize, out decimal len))
            a.Length = len;
         return a;
      }
   }

   public class ResourceInfo: ObjectInfo<ElementInfo>
   {
      public AssetDataElement Entity { get; set; } = null;
      public static readonly string IS_NULLABLE = "yes";
      private List<AssetDataElement> m_Items;
      public int OrdinalNo { get; set; }
      public NamespaceInfo Namespace { get; set; }
      public List<AssetDataElement> Resources
      {
         get { return m_Items; }
      }
      public ResourceInfo()
      {
         base.Items = new List<ElementInfo>();
         m_Items = new List<AssetDataElement>();
      }

      public ElementInfo Add(
         string elementName, string dataType, string dataSize,
         string isNullable, bool isKey, bool isIdentity)
      {
         var s = base.Items.Find((x) => { return x.Name == elementName; });
         if (s != null)
            return s;
         s = new ElementInfo
         {
            Name = elementName,
            DataType = dataType,
            DataSize = dataSize,
            IsNullable = (isNullable.ToLower() == IS_NULLABLE),
            IsKey = isKey,
            IsAutoGenerate = isIdentity
         };
         base.Items.Add(s);
         return s;
      }

      public static ResourceInfo FromDataElement(IDataElement element)
      {
         ResourceInfo r = new ResourceInfo();
         r.Name = element.ElementName;
         return r;
      }

   }

   public class SchemaInfo
   {
      public NamespaceInfo Namespace { get; set; }
      public string VersionId { get; set; }
      public string Name { get; set; }
      public List<ResourceInfo> Items { get;set;}

      public SchemaInfo(string versionId)
      {
          Items = new List<ResourceInfo>();
          VersionId = versionId;
      }

      public ResourceInfo Add(string tableName)
      {
         var s = Items.Find((x) => { return x.Name == tableName; });
         if (s != null)
            return s;
         s = new ResourceInfo
         {
            Name = tableName
         };
         Items.Add(s);
         return s;
      }

   }

   public class CatalogInfo
   {
      public NamespaceInfo Namespace { get; set; }
      public string VersionId { get; set; }
      public string Name { get; set; }
      public List<SchemaInfo> Schemas { get; set; }
      public List<SchemaResourceConstraint> Constraints { get; set; }

      public CatalogInfo(string versionId)
      {
         Schemas = new List<SchemaInfo>();
         Namespace = new NamespaceInfo("ddl", "http://www.Edam.com/ddl");
         VersionId = versionId;
      }

      public SchemaInfo Add(string schemaName)
      {
         var s = Schemas.Find((x) => { return x.Name == schemaName; });
         if (s != null)
            return s;
         s = new SchemaInfo(VersionId);
         Schemas.Add(s);
         return s;
      }

      public SchemaInfo Find(NamespaceInfo ns, bool addIfNotFound = false)
      {
         var i = Schemas.Find((x) => { return x.Name == ns.NamePath.Schema; });
         if (i == null)
         {
            if (!addIfNotFound)
            {
               return null;
            }
                var s = new SchemaInfo(VersionId)
                {
                    Name = ns.NamePath.Schema,
                    Namespace = ns
                };
                Schemas.Add(s);
            return s;
         }
         return i;
      }
   }

   public class SchemaResourceConstraint
   {
      public string ConstraintSchema { get; set; }
      public string ConstraintName { get; set; }
      public string TableSchema { get; set; }
      public string TableName { get; set; }
      public string ColumnName { get; set; }

      public string ChildNodeName
      {
         get { return TableSchema + "." + TableName; }
      }

      public string ReferencedSchema { get; set; }
      public string ReferencedTableName { get; set; }
      public string ReferencedColumnName { get; set; }

      public string ParentNodeName
      {
         get { return ReferencedSchema + "." + ReferencedTableName; }
      }
   }

   public class SchemaResourceIndex
   {
      public readonly static string PK = "pk";

      public string CatalogName { get; set; }
      public string SchemaName { get; set; }
      public string ResourceName { get; set; }
      public string ElementName { get; set; }
      public string OrdinalPosition { get; set; }
      public string KeyType { get; set; }
      public string IndexName { get; set; }

      // TODO: investigate a better way to spot PK's
      public Boolean IsPrimaryKey
      {
         get
         {
            return IndexName != null && IndexName.Length >= 2 ?
               IndexName.Substring(0, 2).ToLower() == PK : false;
         }
      }
   }

   public class ResourceMetadataInfo
   {
      public bool AllowDBNull { get; set; }
      public int ColumnOrdinal { get; set; }
      public string ColumnName { get; set; }
      public int ColumnSize { get; set; }
      public string DataTypeName { get; set; }
      public bool IsAutoIncrement { get; set; }
      public bool IsIdentity { get; set; }
      public bool IsKey { get; set; }
      public bool IsBlob { get; set; }
      public bool IsUniqueTimestamp { get; set; }

      public ResourceMetadataInfo()
      {
      }
   }

   public class SchemaResource
   {
      public string CatalogName { get; set; }
      public string SchemaName { get; set; }
      public string ResourceName { get; set; }
      public string ElementName { get; set; }
      public string DataType { get; set; }
      public string DataSize { get; set; }
      public string OrdinalPosition { get; set; }
      public string ColumnDefault { get; set; }
      public string IsNullable { get; set; }
      public List<SchemaResourceIndex> Indexes { get; set; }
      public List<SchemaResourceConstraint> Constraints { get; set; }
      public ResourceMetadataInfo Metadata { get; set; }
      public SchemaResource()
      {
         Indexes = new List<SchemaResourceIndex>();
         Constraints = new List<SchemaResourceConstraint>();
      }

      public static void SynchronizeIndexes(SchemaResource resource)
      {
         if (resource.Metadata.IsKey)
         {
            return;
         }

         var f = resource.Indexes.Find(
            (x) => x.ElementName == resource.ElementName);
         if (f != null)
         {
            resource.Metadata.IsKey = f.IsPrimaryKey;
            
         }
      }

      public static void SynchronizeData(List<SchemaResource> resources)
      { 
         foreach(var r in resources)
         {
            SynchronizeIndexes(r);
         }
      }
   }

}
