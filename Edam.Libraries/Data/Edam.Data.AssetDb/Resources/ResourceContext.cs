using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Diagnostics;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetConsole;
using Edam.DataObjects.Assets;
using Edam.DataObjects.Services;
using Edam.DataObjects.Requests;
using static SQLite.SQLite3;

namespace Edam.Data.AssetManagement.Resources
{

   /// <summary>
   /// Asset Providers data and methods...
   /// </summary>
   public class ResourceContext : IResourceContext, IResourceCallHelpers,
      IResourceData
   {

      #region -- Fields and Properties declarations

      private const string CLASS = "ResourceContent";
      private ReferenceDataService m_DataService = null;
      private BaseGrammarType m_GrammarType = BaseGrammarType.XSD;

      private int m_Counter = 0;
      private readonly Dictionary<string, ElementTypeInfo> m_TypeDictionary = 
         new Dictionary<string, ElementTypeInfo>();
      private readonly Dictionary<
            string, AssetDataElement> m_ElementDictionary =
         new Dictionary<string, AssetDataElement>();

      public DataTransformResolverDelegate TransformResolver;

      private List<NamespaceInfo> m_Namespaces = 
         new List<NamespaceInfo>();

      private Uri m_RootDomainUri = null;
      private String m_OrganizationDomainId;
      private String m_UriExtension;

      private NamespaceInfo m_DefaultNamespace = null;
      private NamespaceInfo m_RootDomainTag = null;

      private List<DataDomain> m_Domains = null;
      private AssetDataElementList m_Elements = null;

      public List<DataDomain> DbDomains { get; set; }
      public List<DataTerm> DbTerms { get; set; }
      public AssetDataElementList DbElements { get; set; }

      public string VersionId { get; set; }

      /// <summary>
      /// The base context provide the ability to work through a Use Case 
      /// schema references subset and use the full schema to resolve types and
      /// elements.  The base should always be the super-set.
      /// </summary>
      public ResourceContext BaseContext { get; set; }

      public String UriExtension
      {
         get { return m_UriExtension; }
      }

      public List<NamespaceInfo> Namespaces
      {
         get { return m_Namespaces; }
      }

      public NamespaceInfo DefaultNamespace
      {
         get
         {
            if (m_DefaultNamespace == null)
            {
               if (m_Namespaces != null && m_Namespaces.Count != 0)
               {
                  m_DefaultNamespace = m_Namespaces[0];
               }
            }
            return m_DefaultNamespace;
         }
         set
         {
            m_DefaultNamespace = value;
         }
      }

      public List<DataDomain> Domains
      {
         get { return m_Domains; }
      }

      public AssetDataElementList Elements
      {
         get { return m_Elements; }
      }

      public String OrganizationDomainId
      {
         get { return m_OrganizationDomainId; }
      }

      public String RootDomainTag
      {
         get { return m_RootDomainTag.Uri.OriginalString; }
      }
      public Uri RootDomainUri
      {
         get { return m_RootDomainUri; }
      }

      public String TypeNameSuffix { get; set; }

      public BaseGrammarType ResourceGrammarType
      {
         get { return m_GrammarType; }
         set { m_GrammarType = value;}
      }

      #endregion
      #region -- Resolvers

      // delegates...
      public DataTextMapResolverDelegate DataTextMapResolver { get; set; }
      public BaseTypeResolverDelegate DataBaseTypeResolver { get; set; }
      public NamespaceResolverDelegate DataNamespaceResolver { get; set; }
      public TypeResolverDelegate DataTypeResolver { get; set; }
      public DataTransformResolverDelegate DataTransformResolver { get; set; }
      public DomainResolverDelegate DataDomainResolver { get; set; }
      public ElementTypeResolverDelegate ElementTypeResolver { get; set; }

      // call backs...
      public Func<IDataElement, List<IDataElement>>
         DataFindKeyCandidates { get; set; }

      /// <summary>
      /// Traversing and looking for the children of a child element.  See DDL
      /// schema support...
      /// </summary>
      public Func<IDataElement, List<IDataElement>, String, List<IDataElement>>
         DataGetChildrenOfChildren { get; set; }

      #endregion
      #region -- Construct - Destruct...

      /// <summary>
      /// Initialize, fetch base Domains, Terms, Elements...
      /// </summary>
      /// <param name="prefix"></param>
      /// <param name="rootDomainUri">root domain URI</param>
      /// <param name="organizationDomain">Organization URL for example:
      /// my.company.com</param>
      /// <param name="uriExtension">optional uri extension</param>
      public ResourceContext(String prefix,
         Uri rootDomainUri, String organizationDomain, String uriExtension,
         List<AssetData> assets, string versionId, Boolean fetchData = true)
      {
         VersionId = versionId;
         m_DefaultNamespace = new NamespaceInfo(prefix, rootDomainUri);
         m_RootDomainTag = m_DefaultNamespace;
         m_OrganizationDomainId = 
            organizationDomain ?? NamespaceInfo.EDD_ORG_DM_ID;
         m_RootDomainUri = rootDomainUri;
         m_UriExtension = uriExtension;
         TypeNameSuffix = Helpers.ConfigurationHelper.TYPE_NAME_SUFFIX;
         SetResolvers();

         m_DataService = ReferenceDataService.GetService();

         if (fetchData)
         {
            _ = GetDataAsync(organizationDomain, 
               rootDomainUri.OriginalString, null);
         }
         else if (assets != null)
         {
            SetContext(assets);
         }
      }

      public ResourceContext()
      {
         m_DataService = ReferenceDataService.GetService();
      }

      public ResourceContext(AssetData data)
      {
         PrepareContext(data);
      }

      public void Dispose()
      {
      }

      #endregion
      #region -- Database Access Get

      public async Task<DataReferenceFetchResult> GetDataAsync(
         string organizationId, string root, string elementName,
         DataReferenceOption option = 
            DataReferenceOption.DomainsTermsAndElements)
      {
         if (string.IsNullOrWhiteSpace(organizationId))
         {
            organizationId = Session.OrganizationId;
         }
         if (root == null)
         {
            root = String.Empty;
         }
         if (elementName == null)
         {
            elementName = String.Empty;
         }
         var response = await m_DataService.GetElementData(
            Session.SessionId, Session.Language,
            organizationId, root, elementName, option);
         if (response.Success && response.ResponseData != null)
         {
            m_Domains = DbDomains = response.ResponseData.Domains;
            DbTerms = response.ResponseData.Terms;
            DbElements = response.ResponseData.Elements;
            return response.ResponseData;
         }
         return null;
      }

      public static DataReferenceFetchResult GetData(
         string root, string termName,
         DataReferenceOption option =
            DataReferenceOption.DomainsTermsAndElements)
      {
         ResourceContext context = new ResourceContext();
         Task<DataReferenceFetchResult> t = null;
         DataReferenceFetchResult results = new DataReferenceFetchResult(null);
         try
         {
            t = context.GetDataAsync(null, root, termName, option);
            t.Wait();
            if (t.Status == TaskStatus.RanToCompletion)
            {
               results = t.Result;
            }
            t.Dispose();
         }
         catch (Exception ex)
         {
            results.Request.Root = root;
            results.Request.TermName = termName;
         }
         return results;
      }

      /// <summary>
      /// Fetch Domains, Terms and Elements related to given root.
      /// </summary>
      /// <param name="root"></param>
      /// <param name="termName"></param>
      /// <param name="option">[default: fetch Domains, Terms and Elements]
      /// </param>
      /// <returns>results log with inner list of AssetDataElement(s) is
      /// returned</returns>
      public static ResultsLog<AssetDataElementList> GetAssets(
         string root, string termName,
         DataReferenceOption option =
            DataReferenceOption.DomainsTermsAndElements)
      {
         ResultsLog<AssetDataElementList> results = 
            new ResultsLog<AssetDataElementList>();
         var r = GetData(root, termName);
         if (r.Success)
         {
            results.Data = AssetData.ToDataElement(r.Elements);
            results.Succeeded();
         }
         else
         {
            results.Failed(EventCode.Failed);
         }
         return results;
      }

      #endregion
      #region -- Database Access Batch

      /// <summary>
      /// Insert or Update a batch request...
      /// </summary>
      /// <param name="batchId"></param>
      /// <param name="domainUri"></param>
      /// <param name="versionId"></param>
      /// <param name="groupId"></param>
      /// <returns></returns>
      public ResultsLog<string> BatchInsertUpdate(
         string batchId, string domainUri, string versionId, string groupId)
      {
         ResultsLog<string> results = new ResultsLog<string>();
         var t = m_DataService.UpdateBatchData(
            null, null, null, batchId, domainUri, versionId, groupId);
         t.Wait();

         if (t.Status == TaskStatus.RanToCompletion)
         {
            if (t.Result != null)
            {
               results.Copy(t.Result.Results);
               results.Data = t.Result.ResponseData;
            }
         }
         t.Dispose();

         return results;
      }

      #endregion
      #region -- Database Access Save 

      /// <summary>
      /// Save assets collection.
      /// </summary>
      /// <param name="assets"></param>
      /// <param name="ns"></param>
      /// <param name="domainName"></param>
      /// <param name="type"></param>
      /// <returns></returns>
      public Diagnostics.IResultsLog Save(
         AssetDataElementList assets, NamespaceInfo ns, string domainName,
         AssetType type)
      {
         string groupId = type.ToString();
         Task<RequestResponseInfo<string>> t = null;
         Diagnostics.IResultsLog results = new Diagnostics.ResultLog();
         try
         {
            string batchId = null;
            ResultsLog<string> bresults = BatchInsertUpdate(
               null, ns.NamePath.DomainUri, assets.VersionId, groupId);
            int cnt = 0;

            if (bresults.Success)
            {
               batchId = bresults.Data;
            }
            else
            {
               return bresults;
            }

            foreach (var i in assets)
            {
               i.BatchId = batchId;
               i.SequenceId = (cnt + 1).ToString();
               t = m_DataService.UpdateElementData(
                  Session.SessionId, Session.OrganizationId,
                  ns.Prefix, ns.NamePath.DomainUri, domainName, i, batchId,
                  (int)type);
               t.Wait();
               if (t.Status == TaskStatus.RanToCompletion)
               {
                  if (t.Result != null)
                  {
                     results.Copy(t.Result.Results);
                  }
                  cnt++;
               }
            }

            bresults = BatchInsertUpdate(
               bresults.Data, ns.NamePath.DomainUri, assets.VersionId, groupId);

            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex.Message);
         }
         finally
         {
            if (t != null)
            {
               t.Dispose();
            }
         }
         return results;
      }

      public async Task<Diagnostics.IResultsLog> SaveAsync(
         AssetDataElementList assets, NamespaceInfo ns, string domainName,
         AssetType type)
      {
         var results = await Task<Diagnostics.IResultsLog>.Run(
            () => Save(assets, ns, domainName, type));
         return results;
      }

      /// <summary>
      /// Save Asset Asynchronously
      /// </summary>
      /// <param name="assets"></param>
      /// <param name="ns"></param>
      /// <param name="domainName"></param>
      /// <returns></returns>
      public static async Task<Diagnostics.IResultsLog> SaveAssetAsync(
         AssetDataElementList asset, NamespaceInfo ns, string domainName,
         AssetType type)
      {
         ResourceContext context = new ResourceContext();
         Diagnostics.IResultsLog results = new Diagnostics.ResultLog();
         try
         {
            var t = await context.SaveAsync(asset, ns, domainName, type);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      #endregion
      #region -- Manage Resolvers

      private void SetResolvers()
      {
         DataBaseTypeResolver =
            new BaseTypeResolverDelegate(BaseTypeResolver);
         DataTypeResolver =
            new TypeResolverDelegate(TypeResolver);
         DataNamespaceResolver =
            new NamespaceResolverDelegate(NamespaceResolver);
         DataDomainResolver =
            new DomainResolverDelegate(DomainResolver);
         DataTextMapResolver = null;
         DataTransformResolver = null;

         ElementTypeResolver = new ElementTypeResolverDelegate(FindType);

         DataFindKeyCandidates = FindKeyCandidates;
         DataGetChildrenOfChildren = GetChildrenOfChildren;
      }

      #endregion
      #region -- Manage Namespaces

      private String GeneratePrefix()
      {
         return "ns" + (m_Counter++).ToString().PadLeft(4, '0');
      }

      public NamespaceInfo FindNamespace(String namespaceText)
      {
         var ns = m_Namespaces.Find((x) => 
            x.Uri.OriginalString == namespaceText);
         if (ns == null)
         {
            ns = m_DefaultNamespace.Uri.OriginalString == namespaceText ?
               m_DefaultNamespace : null;
         }
         if (ns == null && BaseContext != null)
         {
            ns = BaseContext.FindNamespace(namespaceText);
         }
         return ns;
      }

      public void AddNamespace(String namespaceText)
      {
         NamespaceInfo i = FindNamespace(namespaceText);
         if (i == null)
         {
            i = new NamespaceInfo(null, namespaceText, m_OrganizationDomainId,
               m_UriExtension);
            m_Namespaces.Add(i);
            var d = m_Domains.Find((x) => x.DomainUri == namespaceText);
            i.Prefix = d == null ? GeneratePrefix() : d.Prefix;
         }
      }

      public void SetNamespace(DataElement element)
      {
         Uri ns = AssetDataElementList.GetNamespace(element);

         // register namespace if needed...
         AddNamespace(ns.OriginalString);
      }

      #endregion
      #region -- Manage Types and their registration...

      public AssetDataElementList GetUriTypes(string uriText)
      {
         var types = from c in DbElements
                     where c.Namespace == uriText && c.IsType
                     select c;
         //m_Elements = t;

         // set namespaces
         //foreach (var i in DbElements)
         //{
         //   SetNamespace(i);
         //}
         var l = types.ToList();
         AssetDataElementList list = 
            new AssetDataElementList(
               DefaultNamespace, AssetType.Schema, VersionId);
         list.AddRange(l);
         return list;
      }

      public AssetDataElementList GetAllTypes(string root)
      {
         var list = AssetDataElementList.GetTypes(DbElements, root);

         // set namespaces
         foreach (var i in DbElements)
         {
            SetNamespace(i);
         }
         return list;
      }

      public AssetDataElementList GetAllUriTypes(string uriText)
      {
         var list = AssetDataElementList.GetUriTypes(DbElements, uriText);

         // set namespaces
         foreach (var i in DbElements)
         {
            SetNamespace(i);
         }
         return list;
      }

      public AssetDataElementList GetAllUriElements(string uriText)
      {
         return AssetDataElementList.GetUriElements(DbElements, uriText);
      }

      public List<AssetDataElement> GetAllElements(string root)
      {
         var types = from c in DbElements
                     where c.Root == root && !c.IsType
                     select c;
         //m_Elements = t;

         // set namespaces
         foreach (var i in DbElements)
         {
            SetNamespace(i);
         }

         var l = types.ToList();
         AssetDataElementList list = new AssetDataElementList(
            DefaultNamespace, AssetType.Schema, VersionId);
         list.AddRange(l);
         return list;
      }

      public AssetDataElementList GetTypeChildren(IDataElement element)
      {
         return AssetDataElementList.GetChildren(
            m_Elements, element.Root, element.Domain, element.ElementName);
      }

      public void RegisterTypes(List<AssetDataElement> types)
      {
         foreach(var t in types)
         {
            var clist = GetTypeChildren(t);
            ElementTypeInfo i = new ElementTypeInfo
            {
               Key = GetResourceKey(t),
               Element = t,
               Children = clist
            };
            AddType(i);
         }
      }

      public void RegisterElements(List<AssetDataElement> elements)
      {
         foreach (var t in elements)
         {
            AddElement(t);
         }
      }

      private int RegisterTypes()
      {
         var c = m_TypeDictionary.Count;
         var types = GetAllTypes(m_DefaultNamespace.NamePath.Root);
         RegisterTypes(types);
         return m_TypeDictionary.Count - c;
      }

      private int RegisterElements()
      {
         var c = m_TypeDictionary.Count;
         var elements = GetAllElements(m_DefaultNamespace.NamePath.Root);
         RegisterElements(elements);
         return m_TypeDictionary.Count - c;
      }

      #endregion
      #region -- Find and Add types

      /// <summary>
      /// All resources are uniquely identified by their URI's, given an element
      /// return its URI.
      /// </summary>
      /// <param name="element">element to get it's resource URI</param>
      /// <returns>String URI is returned</returns>
      public String GetResourceKey(DataElement element)
      {
         return element.ElementName;
      }

      /// <summary>
      /// Find type.
      /// </summary>
      /// <param name="typeName">type name</param>
      /// <returns>returns the found type else null if not available</returns>
      public ElementTypeInfo FindType(String typeName)
      {
         ElementTypeInfo item;
         if (!m_TypeDictionary.TryGetValue(typeName, out item))
         {
            var t = Elements.Find((x) => x.ElementName == typeName);
            if (t != null)
            {
               var childs = GetTypeChildren(t);
               item = new ElementTypeInfo();
               item.Children = childs;
               item.Element = t;
               item.Key = typeName;
               m_TypeDictionary.Add(typeName, item);
            }
            else
            {
               item = null;
            }
         }
         return item;
      }

      /// <summary>
      /// Add or replace the list of data-elements associated with a type.
      /// </summary>
      /// <param name="resourceId">resource id</param>
      /// <param name="type">element</param>
      public void AddType(ElementTypeInfo type)
      {
         String resourceId = GetResourceKey(type.Element);
         var t = FindType(resourceId);
         if (t == null)
         {
            m_TypeDictionary.Add(resourceId, type);
         }
         else
            m_TypeDictionary[resourceId] = type;
      }

      public ElementTypeInfo GetChildren(String resourceId)
      {
         if (String.IsNullOrWhiteSpace(resourceId))
            return null;
         return FindType(resourceId);
      }

      public Dictionary<string, ElementTypeInfo> GetTypeDictionary()
      {
         return m_TypeDictionary;
      }

      public String UriRemoveLast(String uriText)
      {
         var i = uriText.LastIndexOf('/');
         if (i == -1)
            return uriText;
         return uriText.Remove(i);
      }

      /// <summary>
      /// Given an IDataElement get its namespace.
      /// </summary>
      /// <param name="element">element</param>
      /// <returns>instance of NamespaceInfo is returned</returns>
      public NamespaceInfo NamespaceResolver(String namespaceText)
      {
         return FindNamespace(namespaceText);
      }

      public DataDomain DomainResolver(String namespaceText)
      {
         return m_Domains.Find((x) => x.DomainUri == namespaceText);
      }

      /// <summary>
      /// Given a element ... get element type (if any)
      /// </summary>
      /// <param name="element">element in question</param>
      /// <returns>instance of DataTypeInfo is returned</returns>
      public DataTypeInfo FindType(
         IDataElement element, Boolean nullIfNotFound = false)
      {
         String func = "ResourceContext::TypeResolver: ";
         AssetDataElement de;
         Resources.ElementTypeInfo t = FindType(element.TypeName);
         // if t == null is an element not a type...
         if (t == null)
         {
            // if not found but the asset is a Type then try to find it using
            // its self... if found then update its TypeName to self!  this
            // is what happens when nameless types are defined in the schema.
            if (element.IsType)
            {
               t = FindType(element.ElementName);
            }
            if (t == null)
            {
               //if (nullIfNotFound)
               return null;

               // TODO: catalog issues
               //throw new Exception(func +
               //   "Resource ID not Found (" + typeName + ")");
            }
            element.TypeQualifiedName = new QualifiedNameInfo(
               element.ElementQualifiedName.Prefix,
               element.ElementQualifiedName.OriginalName);
         }
         de = t.Element;
         NamespaceInfo ns = FindNamespace(
            AssetDataElementList.GetNamespace(de).OriginalString);
         return new DataTypeInfo(de, ns, TypeNameSuffix);
      }

      public DataTypeInfo TypeResolver(IDataElement element)
      {
         return FindType(element, false);
      }

      /// <summary>
      /// A data type text is a reference to an existing type that may be define
      /// in the parent schema or any other schema.  At times a specification of
      /// this data type may not contain the root or domain for those so this 
      /// method will add missing components of the resource ID if those are 
      /// missing on the given dataTypeText.
      /// </summary>
      /// <param name="dataTypeText">the element data type  as text</param>
      /// <returns>A resource URI (text) is returned</returns>
      public String DataTypeToResourceId(String dataTypeText)
      {
         // TODO: fix hard-coded spx...
         if (dataTypeText.IndexOf('.') == -1)
            return m_RootDomainTag + "spx/" + dataTypeText;
         return m_RootDomainTag + dataTypeText.Replace('.', '/');
      }

      private string FindID(String resourceId)
      {
         String rid = DataTypeToResourceId(resourceId);
         return rid.Replace(NamespaceInfo.HTTP_ROOT, "");
      }

      public ElementTypeInfo FindTypeElement(string resourceId)
      {
         return FindType(resourceId);
      }

      public DataTypeInfo BaseTypeResolver(
         string resourceId, Boolean nullIfNotFound)
      {
         if (BaseContext != null)
         {
            return BaseContext.BaseTypeResolver(resourceId, nullIfNotFound);
         }

         // is this a Type?  if so, it is a "base"?
         var baseType = ElementBaseTypeInfo.GetDataTypeInfo(
            resourceId, m_GrammarType);
         if (baseType != null)
         {
            return baseType;
         }

         // not a base, try to find it or navigate to find it...
         String func = "ResourceControl::BaseTypeResolver: ";
         ElementTypeInfo t = FindType(resourceId);
         if (t == null)
         {
            // maybe this is refering to another element as equal!
            var e = FindElement(resourceId);
            if (e == null)
            {
               if (nullIfNotFound)
                  return null;
               throw new Exception(func +
                  " Resource Not Found (" + resourceId + ")");
            }
            else
               return BaseTypeResolver(e.ElementDataType, nullIfNotFound);
         }

         NamespaceInfo ns = FindNamespace
            (AssetDataElementList.GetNamespace(t.Element).OriginalString);
         return new DataTypeInfo(t.Element, ns, TypeNameSuffix);
      }

      private DataTypeInfo GetDataType(IDataElement element)
      {
         NamespaceInfo ns = FindNamespace(element.ElementUri);
         return new DataTypeInfo(element, ns, String.Empty); // TypeNameSuffix);
      }

      private List<AssetDataElement> FindResourceElement(string resourceId)
      {
         var e = from c in m_Elements
                 where c.ResourceId == resourceId || 
                       c.ElementName == resourceId
                 select c;
         return e.ToList<AssetDataElement>();
      }

      public DataTypeInfo BaseTypeResolver(
         IDataElement element, Boolean nullIfNotFound)
      {
         const string FUNC = "BaseTypeResolver";

         // is this a Type?  if so, it is a "base"?
         if (ElementBaseInfo.IsBaseElementType(element))
         {
            return GetDataType(element);
         }

         DataTypeInfo tinfo = BaseTypeResolver(
            element.TypeName, nullIfNotFound);

         // try to find element if base-type was not found...
         if (tinfo == null)
         {
            string resourceId = element.Root + "." + element.ElementDataType;
            var x = FindResourceElement(resourceId.Replace(".","/"));
            if (x == null || x.Count == 0)
            {
               return null;
               //throw new Exception(CLASS + "." + FUNC + "::Resource ID (" +
               //   element.ResourceId + ") not found");
            }
            tinfo = BaseTypeResolver(x[0], nullIfNotFound);
         }
         return tinfo;
      }

      public DataTypeInfo BaseTypeResolver(IDataElement element)
      {
         return BaseTypeResolver(element, true);
      }

      #endregion
      #region -- Find and Add Elements...

      public AssetDataElement FindElement(String resourceId)
      {
         if (!m_ElementDictionary.TryGetValue(
            resourceId, out AssetDataElement item))
            return null;
         return item;
      }

      /// <summary>
      /// Add or replace a given data-element.
      /// </summary>
      /// <param name="resourceId">resource id</param>
      /// <returns>returns 1 if added, 0 if found, else something is wrong
      /// </returns>
      public Int16 AddElement(AssetDataElement element)
      {
         short rval;
         if (element == null)
            return -1;

         String resourceId = GetResourceKey(element);
         var list = FindElement(resourceId);

         if (list == null)
         {
            m_ElementDictionary.Add(resourceId, element);
            rval = 1;
         }
         else
         {
            m_ElementDictionary[resourceId] = element;
            rval = 0;
         }

         return rval;
      }

      /// <summary>
      /// Try to find an element with the given resource ID and add all those 
      /// found, that it is expected to be just one.
      /// </summary>
      /// <param name="resourceId">resource ID</param>
      public Int16 AddElement(String resourceId)
      {
         Int16 cnt = 0;
         var e = FindResourceElement(resourceId);
         foreach(var i in e)
         {
            if (AddElement(i) == 1)
               cnt++;
         }
         return cnt;
      }

      public Dictionary<string, AssetDataElement> GetElementDictionary()
      {
         return m_ElementDictionary;
      }

      /// <summary>
      /// Given a data element figure get its declaration...
      /// </summary>
      /// <param name="element">element to find</param>
      /// <returns>instance of DataTypeInfo is returned</returns>
      private ElementBaseInfo FindElement(
         ElementBaseInfo element, Boolean nullIfNotFound)
      {
         String func = "ResourceContext::ElementResolver: ";
         String resourceId = element.BaseElement.ElementName;

         AssetDataElement de = FindElement(resourceId);

         // element could had not been registered, so try to find in the
         // elements repository list and register it...
         if (de == null)
         {
            Int16 cnt = AddElement(resourceId);
            if (cnt > 1)
            {
               element.ResolutionResult = ElementResolutionResult.Duplicate;
               // TODO: catalog issues
               throw new Exception(func +
                  "Found duplicate resource IDs (" + resourceId + ")");
            }
            else if (cnt == 0)
            {
               element.ResolutionResult = ElementResolutionResult.NotFound;
               // TODO: catalog issues
               throw new Exception(func +
                  "Resource ID not found in Elements (" + resourceId + ")");
            }
            element.ResolutionResult = ElementResolutionResult.Added;
            return FindElement(element, nullIfNotFound);
         }

         if (element.ResolutionResult == ElementResolutionResult.Unknown)
            element.ResolutionResult = ElementResolutionResult.Found;
         element.Namespace = FindNamespace(
            AssetDataElementList.GetNamespace(de).OriginalString);
         return element;
      }

      public ElementBaseInfo ResolveElement(ElementBaseInfo element)
      {
         return FindElement(element, false);
      }

      public ElementBaseInfo ElementResolver(
         IDataElement element, Boolean nullIfNotFound = false)
      {
         var b = new ElementBaseInfo(element, null, TypeNameSuffix);
         return FindElement(b, nullIfNotFound);
      }

      #endregion
      #region -- Find Foreign Key Candidates

      /// <summary>
      /// Inspect the children of a given element type and find all child
      /// elements that are part of a key.
      /// </summary>
      /// <param name="element">(parent) element to inspect</param>
      /// <returns>A list of all found child elements that are part of a key
      /// are returned, if none is found the list will be empty</returns>
      public List<IDataElement> FindKeyCandidates(IDataElement element)
      {
         List<IDataElement> l = new List<IDataElement>();
         var children = GetTypeChildren(element);
         foreach(var i in children)
         {
            if (ElementBaseInfo.IsKeyElement(i))
               l.Add(i);
         }
         return l;
      }

      #endregion
      #region -- Find and Instantiate Inherited Elements

      /// <summary>
      /// Add Element to given list.
      /// </summary>
      /// <param name="list"></param>
      /// <param name="element"></param>
      /// <param name="typeEntityName"></param>
      private void AddElement(List<IDataElement> list, IDataElement element,
         string typeEntityName)
      {
         var f = list.Find((x) => x.EntityElementNameText ==
            element.EntityElementNameText);
         if (f == null)
         {
            var elementCopy = element.DeepCopy();
            elementCopy.TypeEntityName = typeEntityName;
            elementCopy.MapElement = element.MapElement;
            list.Add(elementCopy);
         }
      }

      #endregion
      #region -- Get Children of Children... (as needed)

      private ElementTypeInfo GetElementTypeInfo(
         IDataElement element, List<IDataElement> list,
         String parentName = null)
      {
         var typeElement = FindType(element.ElementDataType);
         if ((typeElement == null || typeElement.Children.Count == 0) &&
            String.IsNullOrWhiteSpace(parentName))
            return null;

         return typeElement;
      }

      /// <summary>
      /// Check if the element and name has associated Map Element, if so check
      /// traverse action and add it to list as needed...
      /// </summary>
      /// <param name="element"></param>
      /// <param name="name"></param>
      /// <param name="list"></param>
      /// <param name="parentName"></param>
      private bool CheckMapElement(DataElement element, string name,
         List<IDataElement> list, String parentName)
      {
         if (DataTextMapResolver != null)
         {
            DataTextElementInfo mapElement = DataTextMapResolver(
               name) as DataTextElementInfo;
            if (mapElement != null)
            {
               if (mapElement.TraverseAction ==
                  DataTextElementTraverseAction.Ignore)
               {
                  element.MapElement = mapElement;
                  AddElement(list, element, parentName);
                  return true;
               }
            }
         }
         return false;
      }

      /// <summary>
      /// Add Children... and Children of Children...
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="list"></param>
      /// <param name="parentName"></param>
      private void AddChildren(ElementTypeInfo parent, List<IDataElement> list,
         String parentName, ApplicationLog console, KeyValueDictionary visitor)
      {
         // investigate if this child is a collection
         if (parent.Element.MaxOccurrence > 1)
         {
            AddElement(list, parent.Element, parentName);
         }

#if DEBUG_
         // provide some diagnostics trace information...
         ApplicationLog.WriteLine();
         console.WriteLine(
            "Visit: [" + console.Indent.IdentCount.ToString() + "] " +
            parentName ?? String.Empty);
#endif
         // check if this entry has been visited and update inspect count...
         KeyValueEntry entry = visitor.Add(parent.Element.ElementName, parent);
         if (parent.Children.Count > 0)
         {
            entry.InspectCount++;
            if (entry.InspectCount > visitor.MaxInspectCount)
            {
               return;
            }
         }

         // is this element a Map-Element? if so act upon it...
         if (CheckMapElement(
            parent.Element, parent.Element.ElementName, list, parentName))
         {
            return;
         }

         // type has children, process those...
         string typeElementName;
         foreach (var i in parent.Children)
         {
            // check child type for a Map Element, if so continue...
            if (CheckMapElement(i, i.ElementDataType, list, parentName))
            {
               continue;
            }

            // investigate child element
            ElementTypeInfo childElement = 
               GetElementTypeInfo(i, list, i.ElementName);
            if (childElement == null)
            {
               // get element base type and add it as a child...
               i.TypeElement = i.TypeElement != null ? i.TypeElement :
                  BaseTypeResolver(i.ElementDataType, true);
               AddElement(list, i, parentName);
#if DEBUG_
               console.WriteLine("Added: " + parentName + " " +
                  i.ElementName);
#endif
               continue ;
            }

            // check if all are attributes and/or base types, if so, done...
            typeElementName =
               parentName + "/" + childElement.Element.ElementName;
            int inspectCount = 0;

            foreach(var c in childElement.Children)
            {
               var eType = c.TypeElement != null ? c.TypeElement :
                  BaseTypeResolver(i.ElementDataType, true);
               if (c.TypeElement == null)
               {
                  c.TypeElement = eType;
               }
               if (c.IsAttribute || eType == null || eType.IsBase)
               {
                  AddElement(list, c, typeElementName);
                  continue;
               }
               inspectCount++;
            }

            if (inspectCount == 0)
            {
               continue;
            }
#if DEBUG_
            console.Indent.Push();
#endif
            AddChildren(childElement, list, typeElementName, console, visitor);
#if DEBUG_
            console.Indent.Pop();
#endif
         }
      }

      /// <summary>
      /// Get all base type child elements and recursively look for the
      /// child of the child if the child data-type is a type and not an base
      /// type.
      /// </summary>
      /// <param name="element"></param>
      /// <param name="list"></param>
      /// <returns></returns>
      private List<IDataElement> GetChildrenOfChildren(
         IDataElement element, List<IDataElement> list,
         String parentName = null)
      {
         List<IDataElement> l = list ?? new List<IDataElement>();

         // fetch element type and child list...
         ElementTypeInfo typeElement = GetElementTypeInfo(
            element, list, element.ElementName);

         if (parentName == null)
            parentName = element.ElementName;

         // type not found... so there are no more children... add as child
         ApplicationLog console = new ApplicationLog();

         if (typeElement == null)
         {
            // get element base type and add it as a child...
            element.TypeElement = BaseTypeResolver(
               element.ElementDataType, true); ;
            AddElement(l, element, parentName);
#if DEBUG_
            console.WriteLine("Added: " + parentName + " " +
               element.ElementName);
#endif
            return l;
         }

         KeyValueDictionary visitor = new KeyValueDictionary();
         AddChildren(typeElement, l, element.ElementName + "/" + 
            typeElement.Element.ElementName, console,
            visitor);

         return l;
      }

      #endregion
      #region -- Context Registration

      public void PrepareContext(AssetData data)
      {
         //m_OrganizationDomainId = data.DefaultNamespace.NamePath.Domain;
         //m_RootDomainUri = data.DefaultNamespace.Uri;
         //m_DefaultNamespace = data.DefaultNamespace;
         m_UriExtension = String.Empty;
         TypeNameSuffix = Helpers.ConfigurationHelper.TYPE_NAME_SUFFIX;
         VersionId = data.Items.VersionId;

         m_Namespaces = data.Namespaces;
         if (DefaultNamespace == null)
         {
            DefaultNamespace = data.DefaultNamespace;
         }

         DbDomains = new List<DataDomain>();
         DbTerms = new List<DataTerm>();
         DbElements = new AssetDataElementList(
            DefaultNamespace, AssetType.Schema, VersionId);
         foreach (var n in data.Namespaces)
         {
            DbDomains.Add(new DataDomain(n));
         }
         DbElements.AddRange(data.Items);

         m_Domains = DbDomains;
         m_Elements = DbElements;

         SetResolvers();
         RegisterContext();
      }

      public void SetContext(List<AssetData> assets)
      {
         if (assets == null || assets.Count == 0)
         {
            return;
         }
         if (assets[0].Items == null)
         {
            assets[0].Items = new AssetDataElementList(
               DefaultNamespace, AssetType.Schema, VersionId);
         }
         PrepareContext(assets[0]);
      }

      /// <summary>
      /// Get ResourceContext instance...
      /// </summary>
      /// <remarks>
      /// You will call these method instead of RegisterContext to get the 
      /// context without fetching the reference data and registering the 
      /// elements since you may just want to save a bunch of records.
      /// </remarks>
      /// <param name="arguments">arguments list</param>
      public static ResourceContext GetContext(
         AssetConsoleArgumentsInfo arguments)
      {
         String orgDomain = arguments.OrganizationDomainUri;
         String uri = arguments.Namespace.Uri.AbsoluteUri;
         Uri ddeUri = new Uri(uri);
         String extension = arguments.InFileExtension;

         return new ResourceContext(
            arguments.Namespace.Prefix, ddeUri, orgDomain, extension, 
            arguments.AssetDataItems, arguments.Project.VersionId, false);
      }

      public static ResourceContext GetContext(
         List<AssetData> assets, string versionId)
      {
         if (assets == null || assets.Count == 0)
         {
            return null;
         }
         if (assets[0].Items == null)
         {
            assets[0].Items = new AssetDataElementList(
               assets[0].DefaultNamespace, AssetType.Schema, versionId);
         }

         var cntx = new ResourceContext(assets[0]);
         //cntx.RegisterContext();
         cntx.PrepareContext(assets[0]);
         return cntx;
      }

      /// <summary>
      /// Base on the provided root (URI) read the databse and register all the
      /// related elements.  Usually this is done to then search over the 
      /// domain-URI's, terms and elements as needed.
      /// </summary>
      public void RegisterContext()
      {
         var t = RegisterTypes();
         var e = RegisterElements();

         // ASSET_RESOURCE_DATA_NAME is used to read/write data into a given
         // ContextDb
         var result = Edam.Application.AppAssembly.RegisterInstance(
            Edam.Data.AssetSchema.
               AssetResourceHelper.ASSET_RESOURCE_DATA_NAME, this);
         if (result == RegistryType.Unknown)
         {
            // TODO: register the label elsewhere...
            throw new Exception("ResourceContext::RegisterContext: " +
               "Failed Registring Type");
         }
      }

      /// <summary>
      /// Prepare Context instance base on provided arguments and returned
      /// registered context (see RegisterContext() method for other options).
      /// </summary>
      /// <param name="arguments">URI/Domain context</param>
      /// <returns>instance of a ResourceContext is returned</returns>
      public static Object RegisterContext(AssetConsoleArgumentsInfo arguments)
      {
         ResourceContext context = GetContext(arguments);
         context.RegisterContext();
         return (Object)context;
      }

      public static Object RegisterContext(string[] args)
      {
         AssetConsoleArgumentsInfo arguments =
            AssetConsoleArgumentsInfo.FromArguments(args);
         return RegisterContext(arguments);
      }

#endregion

   }

}
