using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.DataObjects.DataCodes;
using Edam.Application;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.Assets.Asset;
using Newtonsoft.Json;

namespace Edam.Data.AssetManagement
{

   public partial class DataElement : Asset.IDataElement
   {
      
      #region -- 1.0 - Properties and Fields Definitions

      protected QualifiedNameInfo m_EntityQualifiedName = null;
      protected QualifiedNameInfo m_ElementQualifiedName = null;
      protected QualifiedNameInfo m_TypeQualifiedName = null;

      private NamespaceInfo m_ElementNamespace;
      private List<QualifiedNameInfo> m_QualifiedTypeNames =
         new List<QualifiedNameInfo>();

      public NamespaceList Namespaces { get; set; }

      public const string BASE_UID = "uid";
      public const string BASE_UNO = "uno";
      public const string BASE_KEY = "key";
      public const string BASE_CHAR = "char";
      public const string BASE_AUTO_PK = "autopk";
      public const string BASE_IDENTITY = "identity";

      public const string ROOT = "root";
      public const string ELEMENT = "element";
      public const string ATTRIBUTE = "attribute";
      public const string TYPE = "type";
      public const string OBJECT = "object";

      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public DateTime ReferenceDate { get; set; }
      public string OrganizationId { get; set; }
      public string DataOwnerId { get; set; }
      public DateTime? ExpiredDate { get; set; }
      public string Root { get; set; }
      public string Domain { get; set; }
      public string Type { get; set; }
      public string Element { get; set; }
      public string BatchId { get; set; }
      public string Tags { get; set; }
      public string Guid { get; set; }

      public string[] TagItems
      {
         get { return Tags == null ? new String[0] : Tags.Split(","); }
      }

      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int ElementNo { get; set; }
      public int ParentNo { get; set; }

      public int AssetNo { get; set; }
      public int OrdinalNo { get; set; }

      public string ElementId { get; set; }
      public int TypeNo { get; set; }
      public string ElementUri { get; set; }

      public string ElementName
      {
         get
         {
            var ename = m_ElementQualifiedName == null ? String.Empty :
               m_ElementQualifiedName.Name;
            return ename;
         }
         set
         {
            if (String.IsNullOrWhiteSpace(value))
            {
               m_ElementQualifiedName = null;
               return;
            }
            m_ElementQualifiedName = new QualifiedNameInfo(value);
         }
      }

      public string RealName { get; set; }
      public string AlternateName { get; set; }

      public virtual string AnnotationText
      {
         get { return string.Empty; }
      }

      public virtual string CommentText { get; set; }

      public string ConstraintName { get; set; }
      public string ElementDataType { get; set; }
      public string ElementPath { get; set; }
      public string Description { get; set; }
      public int ElementTypeNo { get; set; }
      public int ElementGroupNo { get; set; }

      public ElementSequence ElementSequence { get; set; }

      public string ElementSequenceID
      {
         get { return ElementSequence.ToString(); }
         set { ElementSequence.FromString(value); }
      }

      public int StatusNo { get; set; }
      public short ValueTypeNo { get; set; }
      public int? MinLength { get; set; }
      public int? MaxLength { get; set; }
      public int? MinOccurrence { get; set; }
      public int? MaxOccurrence { get; set; }
      public bool? Nillable { get; set; }
      public string DefaultValue { get; set; }
      public string FixedValue { get; set; }
      public string SampleValue { get; set; }
      public string SchemaText { get; set; }
      public string SequenceId { get; set; }
      public string VersionId { get; set; }
      public string UpdateSessionId { get; set; }
      public string RecordStatusCode { get; set; }

      public int AutoGenerateTypeNo { get; set; }
      public int KeyTypeNo { get; set; }

      public string TypeName
      {
         get
         {
            return GetTypeName(m_QualifiedTypeNames, TypeQualifiedName);
         }
      }

      private IPropertiesBag m_PropertiesBag = null;
      [JsonIgnore]
      public IPropertiesBag PropertiesBag
      {
         get
         {
            return m_PropertiesBag;
         }
         set
         {
            m_PropertiesBag = value;
            m_PropertiesBagText = value == null ? null :
               value.ToJsonText(value);
         }
      }

      private string m_PropertiesBagText;
      public string PropertiesBagText
      {
         get
         {
            return m_PropertiesBagText;
         }
         set
         {
            m_PropertiesBagText = value;
            if (m_PropertiesBag != null && value != null)
            {
               m_PropertiesBag = m_PropertiesBag.FromJsonText(value);
            }
         }
      }

      public string EntityQualifiedNameText
      {
         get
         {
            return m_EntityQualifiedName == null ? null :
            m_EntityQualifiedName.Name;
         }
         set
         {
            EntityQualifiedName = new QualifiedNameInfo(value);
         }
      }

      public QualifiedNameInfo EntityQualifiedName
      {
         get => m_EntityQualifiedName;
         set => m_EntityQualifiedName = value;
      }

      public string EntityName
      {
         get
         {
            return EntityQualifiedName == null ? String.Empty :
               EntityQualifiedName.Name;
         }
         set
         {
            if (String.IsNullOrWhiteSpace(value))
            {
               EntityQualifiedName = null;
               return;
            }
            var prefix = Namespaces == null ? NamespaceList.DEFAULT_PREFIX :
               Namespaces.GetDefaultPrefix(value);
            EntityQualifiedName = new QualifiedNameInfo(prefix, value);
         }
      }

      public string EntityPath { get; set; }

      public List<QualifiedNameInfo> QualifiedTypeNames
      {
         get { return m_QualifiedTypeNames; }
         set { m_QualifiedTypeNames = value; }
      }

      public QualifiedNameInfo ElementQualifiedName
      {
         get => m_ElementQualifiedName;
         set => m_ElementQualifiedName = value;
      }

      public QualifiedNameInfo TypeQualifiedName
      {
         get => m_TypeQualifiedName;
         set => m_TypeQualifiedName = value;
      }

      public ElementType ElementType
      {
         get => (ElementType)ElementTypeNo;
         set => ElementTypeNo = (int)value;
      }

      public string ElementTypeText
      {
         get { return ElementType.ToString(); }
      }

      public ElementGroup ElementGroup
      {
         get => (ElementGroup)ElementGroupNo;
         set => ElementGroupNo = (int)value;
      }

      public ConstraintType AutoGenerateType
      {
         get => (ConstraintType)AutoGenerateTypeNo;
         set => AutoGenerateTypeNo = (int)value;
      }

      public ConstraintType KeyType
      {
         get => (ConstraintType)KeyTypeNo;
         set => KeyTypeNo = (int)value;
      }

      public string ResourceId
      {
         get
         {
            String id = Root + "/" + Domain + "/" + Type +
               (Element == null ? String.Empty : "/" + Element);
            return id.Replace('.','/');
         }
      }

      public string ElementUriText
      {
         get
         {
            var type = String.IsNullOrWhiteSpace(Type) ?
               String.Empty : "/" + Type;
            return "http://" + Root + "/" + Domain + type
               + (IsAttribute ? "@" : "/") + Element;
         }
      }

      public bool IsRoot
      {
         get { return ElementType == ElementType.root; }
      }

      public bool IsAttribute
      {
         get { return ElementType == ElementType.attribute; }
      }

      public bool IsElement
      {
         get
         { 
            return ElementType == ElementType.element ||
                   ElementType == ElementType.attribute;
         }
      }

      public bool IsGrouped
      {
         get
         {
            return ElementGroup == ElementGroup.OptionAll ||
               ElementGroup == ElementGroup.OptionAny ||
               ElementGroup == ElementGroup.OptionOne;
         }
      }

      public bool IsType
      {
         get
         {
            return ElementType == ElementType.type ||
                   ElementType == ElementType.root ||
                   ElementType == ElementType.view ||
                   ElementType == ElementType.procedure;
         }
      }

      public bool IsEnumerator
      {
         get { return ElementType == ElementType.enumerator; }
      }

      public String NamespaceText
      {
         get
         {
            return ElementUri;
         }
      }

      public String EntityElementNameText
      {
         get 
         { 
            return EntityQualifiedName == null ? 
               ElementQualifiedName.OriginalName : 
               EntityQualifiedName.OriginalName + "_" +
                  ElementQualifiedName.OriginalName;
         }
      }

      public NamespaceInfo ElementNamespace
      {
         get
         {
            return m_ElementNamespace == null ? 
               GetElementNamespace() : m_ElementNamespace;
         }
      }

      private List<DataCodeInfo> m_CodeSetItems = null;
      private string m_EnumCodeSetAsJsonText = String.Empty;
      public string EnumCodeSetAsJsonText
      {
         get { return m_EnumCodeSetAsJsonText; }
         set
         {
            m_CodeSetItems = null;
            m_EnumCodeSetAsJsonText = value; 
         }
      }
      public List<DataCodeInfo> EnumItems
      {
         get
         {
            if (m_CodeSetItems == null)
            {
               m_CodeSetItems = 
                  DataCodeHelper.FromJson(m_EnumCodeSetAsJsonText);
            }
            return m_CodeSetItems;
         }
         set { m_CodeSetItems = value; }
      }
      public int EnumCount
      {
         get
         {
            if (String.IsNullOrWhiteSpace(m_EnumCodeSetAsJsonText) || 
               EnumItems == null)
            {
               return 0;
            }
            return EnumItems.Count;
         }
      }

      private AssetElementConstraintList m_Constraints { get; set; }
      public AssetElementConstraintList Constraints
      {
         get { return m_Constraints; }
      }

      public string ConstraintsText
      {
         get { return AssetElementConstraintList.ToJson(m_Constraints); }
         set { m_Constraints = AssetElementConstraintList.FromJson(value); }
      }

      // runtime only properties...
      public DataTypeInfo TypeElement { get; set; }
      public string TypeEntityName { get; set; }
      public DataTextElementInfo MapElement { get; set; }

      public int? Precision { get; set; }
      public int? Scale { get; set; }

      public virtual DataGroupItemInfo GroupItemTypeNoNavigation { get; set; }
      public virtual DataReferenceStatus StatusNoNavigation { get; set; }
      public virtual DataElementInfo TypeNoNavigation { get; set; }
      public virtual ObjectValueTypes ValueTypeNoNavigation { get; set; }

      public DataElementKind Kind { get; set; } = DataElementKind.Property;

      private string m_OriginalName = null;
      public string OriginalName
      {
         get
         {
            if (m_OriginalName == null)
            {
               m_OriginalName = ElementQualifiedName.OriginalName;
            }
            return m_OriginalName;
         }
         set { m_OriginalName = value; }
      }
      public string OriginalDataType { get; set; }

      public DataElementInclusionType InclusionType { get; set; } = 
         DataElementInclusionType.Unknown;

      #endregion
      #region -- 1.5 - Constructor/Destructor

      public DataElement()
      {
         this.ElementSequence = new ElementSequence();
         ExpiredDate = new DateTime(2100, 1, 1);
         ReferenceDate = DateTime.UtcNow;
         PropertiesBag = AppAssembly.FetchInstance<IPropertiesBag>(
            AssetResourceHelper.ASSET_PROPERTIES_BAG);
         MapElement = null;
         m_EnumCodeSetAsJsonText = string.Empty;
         ParentNo = 0;
         m_Constraints = new AssetElementConstraintList();
         Guid = System.Guid.NewGuid().ToString();
      }

      #endregion
      #region -- 4.0 - Support Methods

      public static string GetUnqualifyText(
         string qualifiedText, string separator = "/", 
         string newSeparator = "_")
      {
         string token;
         string unqualifiedText = qualifiedText;
         int indx = 0;
         do
         {
            string [] l = unqualifiedText.Split(separator);
            if (l.Length == 0)
            {
               break;
            }
            token = null;
            foreach(string s in l)
            {
               indx = s.IndexOf(':');
               if (indx != -1)
               {
                  token = s;
                  break;
               }
            }
            if (token == null)
            {
               break;
            }
            unqualifiedText = 
               unqualifiedText.Replace(token.Substring(0, indx + 1), "");
         } while (true);
         return unqualifiedText.Replace(separator,newSeparator);
      }

      /// <summary>
      /// Get Asset element Namespace if not already assigned.
      /// </summary>
      /// <returns></returns>
      public NamespaceInfo GetElementNamespace()
      {
         if (m_ElementNamespace == null)
         {
            if (Namespaces == null)
            {
               Namespaces = new NamespaceList();
            }
            var ns = Namespaces.Find(
               (x) => x.Prefix == ElementQualifiedName.Prefix);
            if (ns != null)
            {
               m_ElementNamespace = ns;
            }
            else if (Namespaces.Count > 0)
            {
               m_ElementNamespace = Namespaces[0];
            }
            else
            {
               m_ElementNamespace = null;
            }
         }
         return m_ElementNamespace;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="types"></param>
      /// <returns></returns>
      public static string GetTypeName(
         List<QualifiedNameInfo> types, QualifiedNameInfo type)
      {
         if (type != null)
         {
            return type.Name;
         }

         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         String typeName = OBJECT;
         foreach (var i in types)
         {
            if (i.OriginalName == OBJECT)
               continue;
            if (sb.Length > 0)
               sb.Append("; ");
            sb.Append(i.Name);
            typeName = i.Name;
         }
         // TODO: maybe support list of types separated by ';'...
         string name = typeName; // sb.ToString();
         return (String.IsNullOrWhiteSpace(name)) ? OBJECT : name;
      }

      #endregion
      #region -- 4.0 - Manage Properties Bag

      //public static string PropertiesBagToJsonText(object propertiesBag)
      //{
      //   if (propertiesBag != null)
      //   {
      //      IPropertiesBag bag = propertiesBag as IPropertiesBag;
      //      if (bag != null)
      //      {
      //         return bag.ToJsonText(propertiesBag);
      //      }
      //   }
      //   return null;
      //}

      //public object PropertiesBagFromJsonText(
      //   string jsonText, object propertiesBag)
      //{
      //   if (jsonText != null)
      //   {
      //      IPropertiesBag bag = propertiesBag as IPropertiesBag;
      //      if (bag != null)
      //      {
      //         return bag.FromJsonText(jsonText);
      //      }
      //   }
      //   return null;
      //}

      #endregion
      #region -- 4.0 - Data Element Setup / Copy

      public static void SetupDataElement(AssetDataElement item)
      {
         string prefix = item.ElementQualifiedName.Prefix;
         NamespaceInfo ns = new NamespaceInfo(prefix, item.ElementUri);
         if (String.IsNullOrWhiteSpace(item.Root))
         {
            item.Root = ns.NamePath.Root;
         }
         if (String.IsNullOrWhiteSpace(item.Domain))
         {
            item.Domain = ns.NamePath.Domain;
         }
         if (String.IsNullOrWhiteSpace(item.Element))
         {
            item.Element = item.ElementName;
         }

         if (String.IsNullOrWhiteSpace(item.Type))
         {
            if (item.IsElement && item.EntityQualifiedName != null)
            {
               item.Type = item.EntityQualifiedName.Name;
            }
         }

         if (!String.IsNullOrWhiteSpace(item.Type) &&
            item.EntityQualifiedName == null)
         {
            item.EntityQualifiedName = new QualifiedNameInfo(item.Type);
         }
      }

      /// <summary>
      /// Make an exact duplicate of this...
      /// </summary>
      /// <returns></returns>
      public AssetDataElement DeepCopy()
      {
         AssetDataElement copy = new AssetDataElement();
         IDataElement e = copy;

         e.DefaultValue = this.DefaultValue;
         e.Description = this.Description;
         e.ElementDataType = this.ElementDataType;
         e.ElementId = this.ElementId;
         e.ElementName = this.ElementName;
         e.ElementNo = this.ElementNo;
         e.ElementPath = this.ElementPath;
         e.KeyType = this.KeyType;
         e.AutoGenerateType = this.AutoGenerateType;
         e.ElementUri = this.ElementUri;
         e.FixedValue = this.FixedValue;
         e.ElementTypeNo = this.ElementTypeNo;
         e.LastUpdateDate = this.LastUpdateDate;
         e.MaxLength = this.MaxLength;
         e.MaxOccurrence = this.MaxOccurrence;
         e.MinLength = this.MinLength;
         e.MinOccurrence = this.MinOccurrence;
         e.Nillable = this.Nillable;
         e.PropertiesBagText = this.PropertiesBagText;
         e.ReferenceDate = this.ReferenceDate;
         e.SampleValue = this.SampleValue;
         e.StatusNo = this.StatusNo;
         e.TypeNo = this.TypeNo;
         e.ValueTypeNo = this.ValueTypeNo;
         e.Tags = this.Tags;
         e.OriginalName = this.OriginalName;

         e.Type = this.Type;
         e.Root = this.Root;
         e.Domain = this.Domain;

         SetupDataElement(copy);
         return copy;
      }

      #endregion

   }

}
