using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using Edam.Diagnostics;
using Edam.InOut;
using Edam.Application;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetManagement;
using Edam.Text;
using Edam.Data.AssetUseCases;
using Edam.Data.AssetReport;
using System.Security.Cryptography.X509Certificates;

namespace Edam.Data.AssetSchema
{

    /// <summary>
    /// An Asset is a collection of data-components/elements representing a 
    /// business domain, or artifact such as a form, report or other.
    /// </summary>
    public class AssetData
   {

      #region -- 1.00 - Properties and Fields

      //private static readonly string CLASS_NAME = "AssetData";
      //private static readonly string COMMA = ",";
      //private static readonly string SEMICOLUMN = ";";

      public string Title { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }

      public string CatalogName { get; set; }
      public string SchemaName { get; set; }

      public string VersionId { get; set; }

      private AssetDataElementList m_Items;
      public AssetDataElementList Items
      {
         get { return m_Items; }
         set { m_Items = value; }
      }

      #endregion
      #region -- 1.00 - Properties and Fields for Use Case Management

      protected AssetUseCaseList m_UseCases { get; set; }
      protected TableRowHeaderInfo m_UseCaseColumns { get; set; }
      protected List<AssetUseCaseElement> m_UseCasesMergedItems;

      public AssetUseCaseList UseCases
      {
         get { return m_UseCases; }
         set { m_UseCases = value; }
      }

      public TableRowHeaderInfo UseCaseColumns
      {
         get { return m_UseCaseColumns; }
         set { m_UseCaseColumns = value; }
      }

      public List<AssetUseCaseElement> UseCasesMergedItems
      {
         get { return m_UseCasesMergedItems; }
         set
         {
            m_UseCasesMergedItems = value;
         }
      }

      #endregion
      #region -- 1.00 - Properties and Fields for Namespaces Management

      protected NamespaceList m_Namespaces;
      protected NamespaceInfo m_DefaultNamespace = null;

      public string RootTargetNamespace { get; set; }

      public NamespaceList Namespaces
      {
         get
         {
            if (m_Namespaces == null)
               m_Namespaces = new NamespaceList();
            return m_Namespaces;
         }
         set
         {
            if (value == null)
            {
               m_Namespaces = new NamespaceList();
            }
            else
            {
               m_Namespaces = value;
            }
            if (m_DefaultNamespace == null)
            {
               SetDefaultNamespace(m_Namespaces);
            }
         }
      }

      public NamespaceInfo DefaultNamespace
      {
         get
         {
            return m_DefaultNamespace ?? GetDefaultNamespace(); 
         }
         set { m_DefaultNamespace = value; }
      }

      #endregion
      #region -- 1.50 - Constructure / Destructure

      public AssetData(NamespaceInfo ns, AssetType type, string versionId)
      {
         m_Items = new AssetDataElementList(ns, type, versionId);
         m_UseCaseColumns = new TableRowHeaderInfo();
         DefaultNamespace = ns;
         VersionId = versionId;
      }

      public AssetData(AssetDataElementList items)
      {
         m_Items = new AssetDataElementList(items);
         m_Items = items;
         m_UseCaseColumns = new TableRowHeaderInfo();
         m_DefaultNamespace = items.Namespace;
         VersionId = items.VersionId;
      }

      #endregion
      #region -- 4.00 - Support Methods

      /// <summary>
      /// Set Use Case information available in arguments... merging all use 
      /// cases into a list of AssetUseCaseElement(s).
      /// </summary>
      /// <param name="arguments">required arguments list</param>
      public void SetUseCases(AssetConsoleArgumentsInfo arguments)
      {
         if (arguments == null)
            throw new Exception("AssetData::SetUseCases: Missing Arguments");

         m_UseCases = arguments.UseCases;
         m_UseCasesMergedItems = AssetUseCase.MergeUseCases(
            arguments.UseCases, m_UseCaseColumns);
      }

      #endregion
      #region -- 4.00 - Namespace Management

      public NamespaceInfo GetDefaultNamespace()
      {
         foreach(var i in Namespaces)
         {
            if (i.IsW3CSchema)
               continue;
            m_DefaultNamespace = i;
         }
         return m_DefaultNamespace;
      }

      public void SetDefaultNamespace(List<NamespaceInfo> namespaces)
      {
         m_DefaultNamespace = AssetDataElement.GetDefaultNamespace(namespaces);
      }

      /// <summary>
      /// Set Default Namespace and if not included in the namespaces list add 
      /// it.
      /// </summary>
      /// <param name="ns">namespace to set as defualt</param>
      public void SetDefaultNamespace(NamespaceInfo ns)
      {
         m_DefaultNamespace = ns;
         if (m_Namespaces == null)
         {
            m_Namespaces = new NamespaceList();
         }
         var nsf = m_Namespaces.Find((x) => x.Uri.Equals(ns.Uri));
         if (nsf == null)
         {
            m_Namespaces.Add(ns);
         }
      }

      public NamespaceInfo GetNamespace(AssetDataElement element)
      {
         string[] l = element.ElementName.Split(':');
         string prefix = (l == null || l.Length == 1) ? 
            DefaultNamespace.Prefix : l[0] ;
         return new NamespaceInfo(
            prefix, element.Namespace, DefaultNamespace);
      }

      #endregion
      #region -- 4.00 - Add Assets and Items

      /// <summary>
      /// Add a Data Element
      /// </summary>
      /// <param name="asset"></param>
      public void Add(AssetDataElement asset)
      {
         Items.Add(asset);
      }

      /// <summary>
      /// Add an AssetData
      /// </summary>
      /// <param name="asset"></param>
      public void Add(AssetData asset)
      {
         Items.AddRange(asset.Items);
         NamespaceInfo.Merge(Namespaces, asset.Namespaces);
         m_UseCaseColumns.Add(asset.UseCaseColumns.Items);

         m_UseCases = new AssetUseCaseList();
         if (asset.UseCases != null)
            m_UseCases.AddRange(asset.UseCases);

         m_UseCasesMergedItems = new List<AssetUseCaseElement>();
         if (asset.UseCasesMergedItems != null)
            m_UseCasesMergedItems.AddRange(asset.UseCasesMergedItems);
      }

      #endregion
      #region -- 4.00 - Filter Asset Elements

      /// <summary>
      /// Return the list filtering all stand alone elements not related to a
      /// type (class).
      /// </summary>
      /// <returns></returns>
      public List<AssetDataElement> FilterTypes()
      {
         List<AssetDataElement> itms = Items.FindAll((x) =>
            (!(String.IsNullOrWhiteSpace(x.EntityQualifiedNameText) &&
              (x.ElementType == Data.Asset.ElementType.attribute ||
               x.ElementType == Data.Asset.ElementType.element)))
         );
         return itms;
      }

      #endregion
      #region -- 4.00 - Data Element Map, and Asset Elements Merge...

      /// <summary>
      /// Merge asset list/schemas into one.
      /// </summary>
      /// <param name="assets"></param>
      /// <param name="name"></param>
      /// <param name="catalogName"></param>
      /// <param name="schemaName"></param>
      /// <param name="description"></param>
      /// <param name="title"></param>
      /// <param name="ns"></param>
      /// <returns></returns>
      public static AssetData Merge(List<AssetData> assets, string name, 
         string catalogName, string schemaName, string description, 
         string title, NamespaceInfo ns, AssetType type, string versionId)
      {
         NamespaceList nsList = new NamespaceList();
         AssetData asset = new AssetData(ns, type, versionId);
         foreach(var a in assets)
         {
            asset.Items.AddRange(a.Items);
            foreach(var item in a.Namespaces)
            {
               nsList.Add(item);
            }
         }

         asset.Name = name;
         asset.Namespaces = nsList;
         asset.Description = description;
         asset.CatalogName = catalogName;
         asset.SchemaName = schemaName;
         asset.Title = title;

         asset.SetDefaultNamespace(ns);

         return asset;
      }

      /// <summary>
      /// Merge all Asset Data from a collection of those.
      /// </summary>
      /// <param name="items"></param>
      /// <returns></returns>
      public static AssetData Merge(List<AssetData> items, NamespaceInfo ns,
         AssetType type, string versionId)
      {
         if (items.Count == 1)
         {
            return items[0];
         }

         // merge assets and namespaces
         AssetData a = new AssetData(ns, type, versionId);
         foreach (var i in items)
         {
            NamespaceInfo.Merge(a.Namespaces, i.Namespaces);
            a.Add(i);
         }

         // complete all element definitions
         //a.Items = ToDataElement(a.Items, ns);

         return a;
      }

      /// <summary>
      /// Map all data items to base types based on to/from lists;
      /// </summary>
      /// <param name="mapper">instance of the Data Text Mapper</param>
      private void MapDataTypesText(DataTextMap mapper)
      {
         if (mapper.Items == null || mapper.Items.Count == 0)
            return;
         foreach (var asset in Items)
         {
            var e = mapper.MapText(asset.DataType, DataTextMapDirection.From);
            if (e != null)
            {
               asset.DataType = e;
               foreach (var a in asset.GetAttributes())
               {
                  var v = mapper.MapText(a.DataType, DataTextMapDirection.From);
                  if (v == null)
                     continue;
                  a.DataType = v;
               }
            }
         }
      }

      /// <summary>
      /// Map Element details...
      /// </summary>
      /// <param name="mapper"></param>
      private void MapDataElement(DataTextMap mapper)
      {
         foreach(var asset in Items)
         {
            var e = mapper.FindElement(asset.ElementName);
            if (e != null && e.TypeName == DataTextElementType.CodeSet)
            {
               asset.ElementType = ElementType.enumerator;
            }
         }
      }

      /// <summary>
      /// Given a Text Map File Path map entries;
      /// </summary>
      /// <param name="textMapFilePath">text map file-path</param>
      public void MapDataTypes(String textMapFilePath)
      {
         if (String.IsNullOrWhiteSpace(textMapFilePath))
            return;
         DataTextMap mapper = DataTextMap.FromFile(textMapFilePath);
         MapDataTypesText(mapper);
         MapDataElement(mapper);
      }

      #endregion
      #region -- 4.00 - Use Case Reconciliation

      /// <summary>
      /// Add Item to given list...  These items are from the Asset and are 
      /// copied here for later be used in a Use Case items reconciliation 
      /// process.  Within reconciling relevant Use Case Items will be matched.
      /// This means that this element may not be part of the Use Case.
      /// </summary>
      /// <param name="items">list to add the element/element too</param>
      /// <param name="columns">list of column headers</param>
      /// <param name="element">element to be added</param>
      private void UseCaseItemAdd(
         List<AssetDataElement> items,
         TableRowHeaderInfo columns, AssetDataElement element)
      {
         // add item to Use Cases Merged Items (Asset Items not necessarily part
         // of or relevant for the Use Case.
         List<AssetUseCaseElement> elements = null;
         if (m_UseCasesMergedItems != null)
         {
            elements = m_UseCasesMergedItems.FindAll(
               (x) => x.ElementPath == element.GetFullPath());

            // define all the found... elements column names 
            foreach (var eItem in elements)
            {
               columns.Add(eItem.Name);
            }
         }

         items.Add(element);
      }

      /// <summary>
      /// Reconcile Use Cases and return Assets Report information...
      /// </summary>
      /// <returns>output instance of AssetReportInfo</returns>
      public ReportInfo ReconcileUseCases()
      {
         List<AssetDataElement> itms =
            new List<AssetDataElement>();

         TableRowHeaderInfo columns = new TableRowHeaderInfo();

         // make a copy of the Asset Items (elements) these Items may or may not
         // be included in the Use Case...
         foreach (var i in Items)
         {
            UseCaseItemAdd(itms, columns, i);
            foreach (var a in i.Attributes)
            {
               UseCaseItemAdd(itms, columns, a);
            }
         }

         // reconcile the Asset Use Cases with the existing Asset Use Cases
         if (m_UseCasesMergedItems != null)
         {
            AssetUseCase.Reconcile(itms, m_UseCases);
         }

         if (m_UseCases == null)
         {
            m_UseCases = new AssetUseCaseList();
         }

         //var columns = m_UseCaseColumns.Headers.Count == 0 ?
         if (m_UseCases.Count > 0 && m_UseCaseColumns.Items.Count == 0)
         {
            m_UseCaseColumns = m_UseCases[0].Instructions.Columns;
         }

         // prepare report details...
         ReportInfo report = new ReportInfo
         {
            Namespaces = Namespaces,
            Items = itms,
            AssetCustomColumns = columns,
            UseCases = m_UseCases ?? new AssetUseCaseList(),
            UseCaseColumns = m_UseCaseColumns,
            UseCasesMergedItems = m_UseCasesMergedItems
         };

         return report;
      }

      /// <summary>
      /// Reconcile Use Cases by going through all provided "assets"...
      /// </summary>
      /// <param name="assets">Assets to be reconcile with Use Case Items
      /// </param>
      /// <param name="useCases">use case to be reconciled</param>
      public static void ReconcileUseCases(
         List<AssetData> assets, AssetUseCaseList useCases)
      {
         foreach(var asset in assets)
         {
            if (asset == null)
            {
               continue;
            }

            asset.UseCasesMergedItems = new List<AssetUseCaseElement>();
            asset.UseCases = useCases;

            var report = asset.ReconcileUseCases();

            asset.UseCaseColumns = report.UseCaseColumns;
            asset.UseCases = report.UseCases;
            asset.UseCasesMergedItems = report.UseCasesMergedItems;
            break;
         }
      }

      #endregion
      #region -- 4.00 - Find Element or Root Element

      public AssetDataElement FindRootElement(string elementName)
      {
         if (m_Items == null)
         {
            return null;
         }
         var item1 = Items.Find((x) => x.ElementName == elementName &&
            x.EntityName == String.Empty &&
            x.ElementType == ElementType.element);
         if (item1 != null)
         {
            elementName = item1.DataType;
         }

         var item2 = Items.Find((x) => 
            x.ElementName == elementName && x.IsType);
         return item2;
      }

      #endregion
      #region -- 4.00 - To, From file, assets and others...

      /// <summary>
      /// Given file details prepare an Asset Report.
      /// </summary>
      /// <param name="file">file details</param>
      /// <param name="options">report options</param>
      /// <returns>asset as a string that was reported</returns>
      public String ToOutput(InOut.FileInfo file, ReportOptions options)
      {
         var report = ReconcileUseCases();
         report.Options = options;
         AssetReportBuilder b = new AssetReportBuilder();
         return b.ToWorkbookFile(file, report);
      }

      /// <summary>
      /// Output to a database with info given in the arguements.
      /// </summary>
      /// <param name="items">items</param>
      /// <param name="ns">namespace info</param>
      public static AssetDataElementList ToDataElement(
         AssetDataElementList items, NamespaceInfo ns)
      {
         // prepare items list...
         int elementCount = 0;
         AssetDataElementList itms = new AssetDataElementList(items);

         foreach (var i in items)
         {
            NamespaceInfo nsf = new NamespaceInfo(
               i.ElementQualifiedName.Prefix, i.Namespace, ns);

            string versionId = nsf.NamePath.VersionId;

            if (String.IsNullOrWhiteSpace(versionId))
            {
               versionId = "v1r0";
            }

            var delement = AssetDataElement.ToDataElement(i, nsf, versionId);
            itms.Add(delement);
            elementCount++;

            // manage use cases
            //var uc = m_UseCasesMergedItems.FindAll(
            //   (x) => x.ElementPath == delement.ElementPath);

            // manage attributes
            if (i.Attributes != null && i.Attributes.Count > 0)
            {
               NamespaceInfo nsi = new NamespaceInfo(nsf.Prefix, i.Namespace);
               foreach(var a in i.Attributes)
               {
                  var attribute = AssetDataElement.ToDataElement(a, nsi);
                  itms.Add(attribute);
               }
            }
         }
         return itms;
      }

      /// <summary>
      /// Prepare the list adding necessary additional details as needed.
      /// </summary>
      /// <param name="items"></param>
      /// <returns></returns>
      public static AssetDataElementList ToDataElement(
         List<DataElement> items, NamespaceInfo ns, AssetType type,
         string versionId)
      {
         AssetDataElementList l = new AssetDataElementList(ns, type, versionId);
         foreach (var i in items)
         {
            l.Add(i.DeepCopy());
         }
         return l;
      }

      /// <summary>
      /// Prepare the list adding necessary additional details as needed.
      /// </summary>
      /// <param name="items"></param>
      /// <returns></returns>
      public static AssetDataElementList ToDataElement(
         AssetDataElementList items)
      {
         foreach (var i in items)
         {
            DataElement.SetupDataElement(i);
         }
         return items;
      }

      /// <summary>
      /// Output to a database with info given in the arguements.
      /// </summary>
      public static IResultsLog ToDatabase(IResourceData context,
         AssetDataElementList items, NamespaceInfo ns, string domainName,
         AssetType type)
      {
         return context.Save(items, ns, domainName, type);
      }

      /// <summary>
      /// Output to a file, database or as specify in the arguments.
      /// </summary>
      /// <param name="arguments">arguments</param>
      public void ToOutput(
         IResourceData context, AssetConsoleArgumentsInfo arguments)
      {
         if (arguments.ToFile)
         {
            // the proper way to output will be figured out based on
            // output file extension... (see AssetReportBuilder::GetBuilder)
            ToOutput(arguments.OutputFile, arguments.Report);
         }
         else if (arguments.ToDatabase)
         {
            if (context == null)
            {
               context = (IResourceData)AppAssembly.FetchInstance(
                  AssetResourceHelper.ASSET_RESOURCE_DATA_NAME);
            }
            ToDatabase(
               context, Items, arguments.Namespace, arguments.Process.Name,
               AssetType.Asset);
         }
      }

      #endregion

   }

}
