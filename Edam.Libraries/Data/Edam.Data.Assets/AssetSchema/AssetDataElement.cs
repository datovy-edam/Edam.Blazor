using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetConsole;
using Newtonsoft.Json;

namespace Edam.Data.AssetSchema
{

   public class AssetDataElement : DataElement, IDataElement, IAssetElement
   {

      #region -- 1.0 - Properties and Fields

      private const string DEFAULT_VERSION = "v1r0";
      private const string CLASS_NAME = "AssetDataElement";

      private OccuranceInfo m_Occurance = new OccuranceInfo();
      private List<string> m_Annotations = new List<string>();

      public List<AssetDataElement> Attributes { get; set; }

      public short? AssetStatus
      {
         get => (short)StatusNo;
         set => StatusNo = value.HasValue ? (short)value.Value : 0;
      }

      public List<string> Annotation
      {
         get => m_Annotations;
         set => m_Annotations = value;
      }

      public override string AnnotationText
      {
         get { return GetAnnotattion(this); }
      }

      public string ElementQualifiedNameText
      {
         get { return m_ElementQualifiedName == null ? String.Empty :
               m_ElementQualifiedName.Name; }
         set
         {
            m_ElementQualifiedName = new QualifiedNameInfo(value);
            base.ElementName = value;
         }
      }

      public string Namespace
      {
         get => ElementUri;
         set => ElementUri = value;
      }

      public override string CommentText { get; set; }

      public bool IsNillable { get; set; }

      public string Occurs
      {
         get => GetOccurance().Text;
         set
         {
            m_Occurance.Text = value == null ?
            String.Empty : value.Replace(" ", "");
            MinOccurrence = (int)m_Occurance.MinOccurance;
            MaxOccurrence = (int)m_Occurance.MaxOccurance;
         }
      }

      public bool IsOptional
      {
         get { return m_Occurance.MinOccurance == 0; }
      }

      public bool IsList
      {
         get { return m_Occurance.MaxOccurance > 1; }
      }

      public bool IsString
      {
         get
         {
            return TypeQualifiedName == null ? true :
               TypeQualifiedName.OriginalName == "string";
         }
      }

      public string DataType
      {
         get => ElementDataType;
         set => ElementDataType = value;
      }

      public string DataTypeText
      {
         get { return DataType ?? TypeName; }
      }

      public decimal? Length { get; set; }

      DateTime? IAssetElement.LastUpdateDate
      {
         get => LastUpdateDate;
         set => LastUpdateDate = value ?? DateTime.MinValue;
      }

      public string UseCaseName { get; set; }

      public AssetProcessInfo ProcessInstructionsBag { get; set; }
      public string ProcessInstructionsBagText
      {
         get => ProcessInstructionsBag == null ? 
            String.Empty : ProcessInstructionsBag.ToJson();
         set
         {
            ProcessInstructionsBag = AssetProcessInfo.FromJson(value, this);
         }
      }

      public string MapTo
      {
         get
         {
            if (ProcessInstructionsBag == null || 
               ProcessInstructionsBag.Items.Count < 1)
            {
               return String.Empty;
            }
            return ProcessInstructionsBag.Items[0].Value;
         }
      }
      public string Function
      {
         get
         {
            if (ProcessInstructionsBag == null ||
               ProcessInstructionsBag.Items.Count < 2)
            {
               return String.Empty;
            }
            return ProcessInstructionsBag.Items[1].Value;
         }
      }
      public string Instructions
      {
         get
         {
            if (ProcessInstructionsBag == null ||
               ProcessInstructionsBag.Items.Count < 3)
            {
               return String.Empty;
            }
            return ProcessInstructionsBag.Items[2].Value;
         }
      }

      [JsonIgnore]
      public AssetDataElement Parent { get; set; }

      public object AssetInstance { get; set; }
      public virtual object AssetObject
      {
         get => this;
      }

      public String FullPath
      {
         get { return ElementPath; }
      }

      public Boolean IsMixed { get; set; }

      public int? AsssetNo { get; set; }

      #endregion
      #region -- 1.5 - Constructures and Destructors...

      public AssetDataElement() : base()
      {
         ProcessInstructionsBag = null;
         Attributes = new List<AssetDataElement>();
      }

      #endregion
      #region -- 4.0 - Manage Occurance

      private OccuranceInfo GetOccurance()
      {
         m_Occurance.MinOccurance = MinOccurrence ?? (decimal)0.0;
         m_Occurance.MaxOccurance = MaxOccurrence ?? (decimal)1.0;
         return m_Occurance;
      }

      public void SetOccurance(string occurance)
      {
         OccuranceInfo o = new OccuranceInfo(occurance);
         MinOccurrence = (int)o.MinOccurance;
         MaxOccurrence = (int)o.MaxOccurance;
      }

      #endregion
      #region -- 4.0 - URI and Path Management

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public string GetFullPath()
      {
         if (!String.IsNullOrWhiteSpace(ElementPath))
            return ElementPath;
         return GetFullPath(ElementQualifiedName, m_TypeQualifiedName, Parent);
      }

      public void SetFullPath(String path)
      {
         ElementPath = path;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="namespaceText"></param>
      /// <returns></returns>
      protected String GetPrefix(String namespaceText)
      {
         if (Namespaces == null)
            return namespaceText;
         var item = Namespaces.Find(
            (x) => x.Uri.OriginalString == namespaceText);
         return (item == null) ? namespaceText : item.Prefix;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="qualifiedName"></param>
      /// <param name="typeQualifiedName"></param>
      /// <param name="parent"></param>
      /// <returns></returns>
      public static string GetFullPath(QualifiedNameInfo elementQualifiedName,
         QualifiedNameInfo typeQualifiedName, AssetDataElement parent)
      {
         string tqn = String.Empty;
         string pqn = String.Empty;

         if (elementQualifiedName == null)
            return String.Empty;

         if (parent != null)
         {
            pqn = parent.ElementQualifiedName.Name;
         }
         if (typeQualifiedName != null)
         {
            tqn = typeQualifiedName.Name;
         }

         return (String.IsNullOrWhiteSpace(pqn) ? String.Empty : pqn + "/")
            + elementQualifiedName.Name
            + (String.IsNullOrWhiteSpace(tqn) ? String.Empty : "::" + tqn);
      }

      /// <summary>
      /// Get Type From Path Item of the form: "path:item".
      /// </summary>
      /// <param name="item"></param>
      /// <param name="includePrefix"></param>
      /// <returns></returns>
      public static string GetTypeFromPathItem(
         string item, bool includePrefix = true)
      {
         string itm;
         if (!includePrefix)
         {
            var l = item.Split(":");
            itm = l.Length == 2 ? l[1] : l[0];
         }
         else
         {
            itm = item;
         }
         return itm;
      }

      #endregion
      #region -- 4.0 - Element, Attributes, Type... Management

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public List<AssetDataElement> GetAttributes()
      {
         return Attributes;
      }

      public void AddAnnotation(string text)
      {
         var i = Annotation.Find((x) => x == text);
         if (i == null)
         {
            Annotation.Add(text);
         }
      }

      /// <summary>
      /// Given an asset item get its annotation if any is available.
      /// </summary>
      /// <param name="item">asset item</param>
      /// <returns>the built annotation is returned</returns>
      public static String GetAnnotattion(IAssetElement item)
      {
         List<String> annotation = item.Annotation;
         if (annotation == null || annotation.Count == 0)
         {
            return Edam.Text.Convert.ToTitleCase(
               item.ElementQualifiedName.OriginalName);
         }

         System.Text.StringBuilder sb = new StringBuilder();
         int c = 0;
         //sb.Append("\"");
         foreach (var i in annotation)
         {
            if (c > 0)
               sb.Append("\n");
            sb.Append(i);
            c++;
         }
         //sb.Append("\"");
         return sb.ToString();
      }

      /// <summary>
      /// Get Default Namespace.
      /// </summary>
      /// <param name="namespaces"></param>
      /// <returns></returns>
      public static NamespaceInfo GetDefaultNamespace(
         List<NamespaceInfo> namespaces)
      {
         if (namespaces == null)
            namespaces = new List<NamespaceInfo>();
         if (namespaces.Count == 0)
         {
            return new NamespaceInfo("er", "http://null"); ;
         }
         return namespaces[0];
      }

      public QualifiedNameInfo AddQualifiedTypeName(
         string name, string prefix = null)
      {
         var n = QualifiedTypeNames.Find((x) => x.Name == name);
         if (n == null)
         {
            var qName = new QualifiedNameInfo(name);
            QualifiedTypeNames.Add(qName);
            qName.Prefix = prefix;
            n = qName;
         }
         return n;
      }

      public void AddQualifiedTypeName(QualifiedNameInfo qname)
      {
         var n = QualifiedTypeNames.Find((x) => x.Name == qname.Name);
         if (n == null)
         {
            var qName = new QualifiedNameInfo(qname.Name)
            {
               OriginalName = qname.OriginalName,
               Name = qname.Name,
               Prefix = qname.Prefix
            };
            QualifiedTypeNames.Add(qName);
         }
      }

      #endregion
      #region -- 4.0 - To/From Asset Data Element

      /// <summary>
      /// Prepare the base Asset Item.
      /// </summary>
      /// <param name="ns">namespace information</param>
      /// <param name="name">item name</param>
      /// <returns>instance of AssetDataElement is returned</returns>
      public static AssetElementInfo<IAssetElement> ToAssetElement(
         NamespaceInfo ns, string name)
      {
         QualifiedNameInfo typeQName = new QualifiedNameInfo("object");
         AssetElementInfo<IAssetElement> a = new AssetElementInfo<IAssetElement>
         {
            DataType = AssetDataElement.OBJECT,
            OriginalDataType = AssetDataElement.OBJECT,
            Namespace = ns.Uri.OriginalString,
            EntityQualifiedName = null,
            ElementQualifiedName = new QualifiedNameInfo(ns.Prefix, name),
            OriginalName = name,
            Occurs = String.Empty,
            Tags = String.Empty,
            ElementType = ElementType.type,
            AutoGenerateType = ConstraintType.none,
            KeyType = ConstraintType.nonkey,
            TypeQualifiedName = typeQName
         };
         a.QualifiedTypeNames.Add(typeQName);
         return a;
      }

      /// <summary>
      /// DataElement to IAsset...
      /// </summary>
      /// <param name="element"></param>
      /// <returns></returns>
      public static IAssetElement ToAsset(AssetDataElement element)
      {

         OccuranceInfo occurrance = new AssetManagement.OccuranceInfo(
            (decimal)element.MinOccurrence, (decimal)element.MaxOccurrence);

         AssetDataElement a = new AssetDataElement();
         a.Annotation.Add(element.Description);
         a.AssetInstance = element;
         a.ElementNo = element.ElementNo;
         a.OriginalName = element.OriginalName;
         a.AssetStatus = (short)element.StatusNo;
         a.CommentText = String.Empty;
         a.DataType = element.ElementDataType;
         a.OriginalDataType = element.OriginalDataType;
         a.DefaultValue = element.DefaultValue;
         a.EntityQualifiedName = element.EntityQualifiedName;
         a.ElementQualifiedName = element.ElementQualifiedName;
         a.FixedValue = element.FixedValue;
         a.ElementType = (ElementType)element.ElementTypeNo;
         a.IsNillable = true;
         a.LastUpdateDate = element.LastUpdateDate;
         a.Length = element.MaxLength;
         a.Namespace = element.ElementUri;
         a.Occurs = occurrance.Text;
         a.Parent = null;
         a.SampleValue = element.SampleValue;
         a.TypeQualifiedName = a.ElementQualifiedName;
         a.ProcessInstructionsBag = element.ProcessInstructionsBag;
         a.KeyType = element.KeyType;
         a.AutoGenerateType = element.AutoGenerateType;
         a.AssetNo = element.AssetNo;
         a.Tags = element.Tags;

         return a;
      }

      /// <summary>
      /// To map IAsset to a DataElement...
      /// </summary>
      /// <param name="asset"></param>
      /// <param name="ns"></param>
      /// <returns></returns>
      public static AssetDataElement ToDataElement(
         IAssetElement asset, NamespaceInfo ns = null)
      {
         if (asset == null)
            return null;

         // a namespace has been provided?
         var fullPath = asset.GetFullPath();
         var nspaceUri = ns == null ? asset.Namespace : ns.Uri.AbsolutePath;
         if (String.IsNullOrWhiteSpace(nspaceUri))
         {
            throw new Exception(CLASS_NAME + "::ToDataElement: "
               + "Namespace expected but not provided");
         }

         // get element namespace and prefix (if any)
         NamespaceInfo nspace;
         if (ns == null)
         {
            nspace = new NamespaceInfo(null, new Uri(nspaceUri));
         }
         else
         {
            nspace = ns;
         }
         var namespaces = new NamespaceList();
         namespaces.Add(nspace);

         if (string.IsNullOrWhiteSpace(nspace.Prefix))
         {
            var l = QualifiedNameInfo.GetPrefix(fullPath);
            nspace.Prefix = l.Count > 1 ? l[0] : String.Empty;
         }

         // assign element values
         var occurrance = new AssetManagement.OccuranceInfo(
            asset.MinOccurrence, asset.MaxOccurrence);

         AssetDataElement element = new AssetDataElement
         {
            Namespace = ns.Uri.OriginalString,
            Namespaces = namespaces,
            ReferenceDate = asset.LastUpdateDate ?? DateTime.UtcNow,
            ElementNo = asset.ElementNo,
            StatusNo = asset.AssetStatus ??
               (short)DataObjects.Objects.ObjectStatus.Active,
            Description = Edam.Text.Convert.ToString(asset.Annotation),
            Type = asset.EntityQualifiedNameText,
            ElementName = asset.ElementQualifiedName.Name,
            OriginalName = asset.ElementQualifiedName.Name,
            OriginalDataType = asset.OriginalDataType,
            ElementUri = asset.Namespace,
            Kind = asset.Kind,
            DefaultValue = asset.DefaultValue ?? String.Empty,
            FixedValue = asset.FixedValue ?? String.Empty,
            PropertiesBag = null, // asset.CommentText,
            SampleValue = asset.SampleValue ?? String.Empty,
            Nillable = asset.IsNillable,
            MinOccurrence = (int)occurrance.MinOccurance,
            MaxOccurrence = (occurrance.MaxOccurance >= int.MaxValue ?
               int.MaxValue : (int)occurrance.MaxOccurance),
            KeyType = asset.KeyType,
            AutoGenerateType = asset.AutoGenerateType,
            ElementDataType = asset.DataType,
            MinLength = 0,
            MaxLength = (int)(asset.Length ?? 255),
            TypeNo = (short)AssetType.Instance,
            ElementPath = fullPath,
            ElementTypeNo = (short)asset.ElementType,
            ProcessInstructionsBag = asset.ProcessInstructionsBag,
            EntityName = asset.EntityQualifiedName?.Name,
            TypeQualifiedName = asset.ElementQualifiedName,
            Tags = asset.Tags
         };

         if (String.IsNullOrWhiteSpace(element.ElementUri))
         {
            element.ElementUri = ns.Uri.AbsoluteUri;
         }

         if (String.IsNullOrWhiteSpace(element.Description))
         {
            element.Description = Text.Convert.ToTitleCase(element.ElementName);
         }

         return element;
      }

      public static void CompleteElementUpdate(
         AssetDataElement element, NamespaceInfo ns, string versionId = null)
      {
         element.ElementNo = 0;

         element.Root = ns.NamePath.Root;
         element.Domain = ns.NamePath.Domain;
         element.Element = element.ElementName;
         element.VersionId = String.IsNullOrWhiteSpace(versionId) ?
            DEFAULT_VERSION : versionId;
         element.UpdateSessionId = Edam.Application.Session.SessionId;
         element.ElementUri = String.IsNullOrWhiteSpace(element.ElementUri) ?
            element.ElementUriText : element.ElementUri;
         element.LastUpdateDate = element.ReferenceDate = DateTime.UtcNow;
         element.SchemaText = String.Empty;

         if (element.Type == null)
         {
            element.Type = String.Empty;
         }

         if (String.IsNullOrWhiteSpace(element.OrganizationId))
         {
            element.OrganizationId = Session.OrganizationId;
         }
         if (String.IsNullOrWhiteSpace(element.DataOwnerId))
         {
            element.DataOwnerId = Session.OrganizationId;
         }
      }

      public static void CompleteElementUpdate(AssetDataElement element)
      {
         if (element.TypeQualifiedName == null)
         {
            element.TypeQualifiedName = 
               new QualifiedNameInfo(element.ElementDataType);
         }
      }

      /// <summary>
      /// Create a copy of the data element.
      /// </summary>
      /// <param name="element"></param>
      /// <param name="ns"></param>
      /// <param name="versionId"></param>
      /// <returns></returns>
      public static AssetDataElement ToDataElement(
         AssetDataElement element, NamespaceInfo ns, string versionId)
      {
         var e = ToDataElement(element, ns);
         e.ElementNo = 0;

         CompleteElementUpdate(e, ns, versionId);

         e.PropertiesBagText = element.PropertiesBagText;
         e.ConstraintName = element.ConstraintName;
         e.Annotation = element.Annotation;
         e.SequenceId = element.SequenceId;
         e.EnumCodeSetAsJsonText = element.EnumCodeSetAsJsonText;

         return e;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="arguments"></param>
      /// <param name="asset"></param>
      /// <param name="ns"></param>
      /// <param name="elementCount"></param>
      /// <param name="versionId"></param>
      /// <returns></returns>
      public static AssetDataElement ToDataElement(
         AssetConsoleArgumentsInfo arguments, IAssetElement asset, NamespaceInfo ns,
         int elementCount, string versionId)
      {
         var element = ToDataElement(asset, ns);
         var delement = element.DeepCopy();
         delement.ElementNo = 0;

         delement.Root = ns.NamePath.Root;
         delement.Domain = ns.NamePath.Domain;
         delement.Element = delement.ElementName;
         delement.ElementName = delement.ElementName;
         delement.VersionId = versionId;
         delement.UpdateSessionId = Edam.Application.Session.SessionId;

         delement.ElementUri = String.IsNullOrWhiteSpace(delement.ElementUri) ?
            delement.ElementUriText : delement.ElementUri;

         if (String.IsNullOrWhiteSpace(delement.OrganizationId))
         {
            delement.OrganizationId = arguments.Process.OrganizationId;
         }
         if (String.IsNullOrWhiteSpace(delement.DataOwnerId))
         {
            delement.DataOwnerId = arguments.Process.OrganizationId;
         }

         return delement;
      }

      public static void ToDataElement(
         AssetDataElement element, string prefix, bool isLeafNode, string type)
      {
         var dt = DateTime.UtcNow;
         var uri = new NamespaceInfo(prefix, element.ElementUri);

         element.Root = uri.NamePath.Root;
         element.Domain = uri.NamePath.Domain;
         element.Type = type;
         element.Element = element.ElementName;
         element.CreatedDate = dt;
         element.LastUpdateDate = dt;
         element.ReferenceDate = dt;
         element.OrganizationId = Application.Session.OrganizationId;
         element.DataOwnerId = element.OrganizationId;
         element.ExpiredDate = null;
         element.VersionId = uri.NamePath.VersionId;
         element.ElementDataType = ElementBaseTypeInfo.STRING;
         element.MinLength = 0;
         element.MaxLength = 255;
         element.MinOccurrence = 0;
         element.MaxOccurrence = 1;
         element.UpdateSessionId = Application.Session.SessionId;
         element.RecordStatusCode = "A";
         element.StatusNo = 1;
         element.ValueTypeNo = 0;

      }

      #endregion

   }

}
