using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetProject;
using Edam.Data.Books;

using util = Edam.Serialization;
using System.Xml.Linq;
using Edam.Data.AssetSchema;
using Edam.InOut;
using Edam.Data.Assets.Books;
using Edam.Data.AssetConsole;
using Edam.Data.Assets.AssetUseCases;
using System.IO;
using Edam.Data.Lexicon;

namespace Edam.Data.AssetUseCases
{

   /// <summary>
   /// Asset Use Case Map
   /// </summary>
   public class AssetUseCaseMap
   {

      #region -- 1.00 - Fields and properties definitions

      public string UseCaseId { get; set; } = Guid.NewGuid().ToString();
      public string NameId { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string VersionId { get; set; }

      public ProjectInfo Project { get; set; } = new ProjectInfo();

      public string SourceUriText { get; set; }
      public string TargetUriText { get; set; }

      public string TenantId { get; set; }
      public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
      public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

      public List<AssetDataMapItem> Items { get; set; } =
         new List<AssetDataMapItem>();

      [JsonIgnore]
      public AssetDataMapItem SelectedMapItem { get; set; } = null;

      private BookInfo m_Book;
      public BookInfo Book
      {
         get { return m_Book; }
      }

      private NamespaceInfo m_Namespace { get; set; }
      public NamespaceInfo Namespace
      {
         get { return m_Namespace; }
         set
         {
            m_Namespace = value;
            m_Book.Namespace = value;
         }
      }

      /// <summary>
      /// Allow to manage Semantics will activate the related sentence/text 
      /// similarities functionality.
      /// </summary>
      public LexiconSettingsInfo LexiconSettings { get; set; } = 
         new LexiconSettingsInfo();

      #endregion
      #region -- 1.50 - Constructure

      public AssetUseCaseMap()
      {
         m_Book = new BookInfo();
         m_Book.Namespace = Namespace;
      }

      #endregion
      #region -- 4.00 - Support Methods

      public void SetNamespace(NamespaceInfo ns)
      {
         Namespace = ns;
         m_Book.Namespace = ns;
      }

      #endregion
      #region -- 4.00 - Search and Find support

      /// <summary>
      /// Find an Asset Data Map Item using the MapItemId...
      /// </summary>
      /// <param name="itemId">id to search for</param>
      /// <returns>the instance of AssetDataMapItem is returned if found
      /// else null</returns>
      public AssetDataMapItem Find(string itemId)
      {
         return Items.Find((x) => x.MapItemId == itemId);
      }

      /// <summary>
      /// Find Asset-Data-Map-Item using a Cell reference ID that should be
      /// pointing to a source or target item withing the Map-Item.
      /// </summary>
      /// <param name="cell">cell with needed search info</param>
      /// <returns>list of found map-items is returned if any was found
      /// </returns>
      public List<AssetDataMapItem> Find(
         MapItemType type, BookletCellInfo cell)
      {
         List<AssetDataMapItem> items = new List<AssetDataMapItem>();

         foreach (var i in Items)
         {
            var l = type == MapItemType.Source ?
               i.SourceElement : i.TargetElement;
            var t = l.Find((x) => x.ItemId == cell.ReferenceId);
            if (t != null)
            {
               items.Add(i);
            }
         }

         return items;
      }

      /// <summary>
      /// Given an element path return all matching map-items.
      /// </summary>
      /// <param name="type">list type (source or target)</param>
      /// <param name="elementPath">element path to search</param>
      /// <returns>list of found map-items is returned if any was found
      /// </returns>
      public List<AssetDataMapItem> Find(
         MapItemType type, string elementPath)
      {
         List<AssetDataMapItem> items = new List<AssetDataMapItem>();

         foreach (var i in Items)
         {
            var l = type == MapItemType.Source ?
               i.SourceElement : i.TargetElement;
            var t = l.Find((x) => x.Path == elementPath);
            if (t != null)
            {
               items.Add(i);
            }
         }

         return items;
      }

      /// <summary>
      /// Select an Item for a given type (source or target).  If any item was
      /// found the first on the list will be set as the selected map-item.
      /// </summary>
      /// <param name="type">list type (source or target)</param>
      /// <param name="elementPath">element path to search</param>
      /// <returns>list of found map-items is returned if any was found
      /// </returns>
      public List<AssetDataMapItem> SelectItem(
         MapItemType type, string elementPath)
      {
         var i = Find(type, elementPath);
         if (i.Count > 0)
         {
            SelectedMapItem = i[0];
         }
         return i;
      }

      #endregion
      #region -- 4.00 - Add Map Item management

      public AssetDataMapItem Add(AssetDataElement element)
      {
         var i = Find(element.ElementPath);
         if (i == null)
         {
            AssetDataMapItem item = new AssetDataMapItem();
            item.ItemPath = element.ElementPath;
            Items.Add(item);
            i = item;
         }
         return i;
      }

      public void Add(MapItemType type, MapItemInfo item)
      {
         var l = type == MapItemType.Source ?
            SelectedMapItem.SourceElement : SelectedMapItem.TargetElement;
         var i = l.Find((x) => x.Path == item.Path);
         if (i == null)
         {
            l.Add(item);
         }
      }

      #endregion
      #region -- 4.00 - Delete map item management

      /// <summary>
      /// Delete Map Item.
      /// </summary>
      /// <param name="type">source or destination</param>
      /// <param name="item">item to delete</param>
      public void Delete(MapItemType type, MapItemInfo item)
      {
         var l = type == MapItemType.Source ?
            SelectedMapItem.SourceElement : SelectedMapItem.TargetElement;
         var i = l.Find((x) => x.Path == item.Path);
         if (i != null)
         {
            l.Remove(i);
         }
      }

      /// <summary>
      /// Delete map item source-target collections...
      /// </summary>
      /// <param name="item">asset data map item to delete</param>
      public void Delete(AssetDataMapItem item)
      {
         var mitem = Items.Find((x) => x.MapItemId == item.MapItemId);
         if (mitem == null)
         {
            return;
         }

         // delete all asset map item source - target collections...
         foreach(var i in mitem.SourceElement)
         {
            Book.Delete(i);
         }
         foreach (var i in mitem.TargetElement)
         {
            Book.Delete(i);
         }
      }

      #endregion
      #region -- 4.00 - File and serialization support

      /// <summary>
      /// Get file name based on given URI and version id.
      /// </summary>
      /// <param name="uriText">URI text</param>
      /// <param name="name">use case name</param>
      /// <param name="versionId">version ID</param>
      /// <returns>file name is returned</returns>
      public static string GetFileName(
         string uriText, string name, string versionId)
      {
         string fname = name;
         if (string.IsNullOrWhiteSpace(name))
         {
            fname = uriText.Replace("http://", string.Empty).Replace(".", "_").
               Replace('/', '_');
         }
         return fname + (string.IsNullOrWhiteSpace(versionId) ?
            string.Empty : "_" + versionId) + ".json";
      }

      /// <summary>
      /// Flatten-out the list of folder-files from a hierarchical list into
      /// a list of folder-files.
      /// </summary>
      /// <param name="folder">folder to inspect and flatten</param>
      /// <param name="children">the output children list</param>
      /// <returns>flatten list of children is returned</returns>
      public static List<FolderFileItemInfo> FromFolder(
         FolderFileItemInfo folder,
         List<FolderFileItemInfo> children)
      {
         foreach(var f in folder.Children)
         {
            if (f.Children.Count > 0)
            {
               foreach(var i in f.Children)
               {
                  FromFolder(i, children);
               }
               continue;
            }
            children.Add(f);
         }
         return children;
      }

      /// <summary>
      /// Flatten-out the list of folder-files from a hierarchical list into
      /// a list of folder-files.
      /// </summary>
      /// <param name="folderPath>folder path</param>
      /// <returns>flatten list of children is returned</returns>
      public static List<FolderFileItemInfo> FromFolder(string folderPath)
      {
         List<FolderFileItemInfo> children = new List<FolderFileItemInfo>();
         var folder = FolderFileReader.GetFolderFileInfo(folderPath);
         FromFolder(folder, children);
         return children;
      }

      /// <summary>
      /// Read Use Case from file...
      /// </summary>
      /// <param name="filePath">partial file path</param>
      /// <returns>instance of AssetMapUseCase is returned</returns>
      public static AssetUseCaseMap FromFile(string filePath)
      {
         if (!System.IO.File.Exists(filePath))
         {
            return null;
         }

         //JsonSerializerOptions? options = new()
         //{
         //   ReferenceHandler = ReferenceHandler.Preserve,
         //   WriteIndented = true
         //};
         //JsonSerializerOptions options = new JsonSerializerOptions
         //{ 
         //   PropertyNameCaseInsensitive = true,
         //   PropertyNamingPolicy = JsonNamingPolicy.CamelCase
         //};

         string jsonText = System.IO.File.ReadAllText(filePath);
         return JsonConvert.DeserializeObject<AssetUseCaseMap>(jsonText);
      }

      /// <summary>
      /// Fetch the use case matching with given URI and version Id.
      /// </summary>
      /// <param name="uriText"></param>
      /// <param name="versionId"></param>
      /// <returns></returns>
      public static AssetUseCaseMap FromUriVersion(
         string uriText, string name, string versionId)
      {
         return FromFile(GetFileName(uriText, name, versionId));
      }

      /// <summary>
      /// Write Use Case to file...
      /// </summary>
      /// <param name="useCase">use case to write</param>
      /// <param name="filePath">file path</param>
      public static void ToFile(AssetUseCaseMap useCase, string folderPath,
         string filePath)
      {
         AssetUseCaseLog.OpenFolder(folderPath);

         //JsonSerializerOptions? options = new()
         //{            
         //   ReferenceHandler = ReferenceHandler.Preserve,
         //   WriteIndented = true
         //};

         string jsonText =
            JsonConvert.SerializeObject(useCase);
         System.IO.File.WriteAllText(folderPath + filePath, jsonText);
      }

      /// <summary>
      /// Write to a file.
      /// </summary>
      /// <param name="useCase">Use Case Map</param>
      /// <param name="folderPath">folder path</param>
      /// <param name="uriText">URI</param>
      /// <param name="versionId">version id</param>
      public static void ToFile(AssetUseCaseMap useCase, string folderPath,
         string uriText, string versionId)
      {
         ToFile(useCase, folderPath, GetFileName(
            uriText, useCase.Name, versionId));
      }

      #endregion
      #region -- 4.00 - Support for Use Case Reporting from Mappings...

      /// <summary>
      /// Given an Asset Use Case Map prepare a related Asset Use Case...
      /// </summary>
      /// <remarks>
      /// The given map contains the information about source and target items
      /// to be used to generate an mapping Asset Data Element containing this
      /// information</remarks>
      /// <param name="map">Use Case map to generate an Asset Use Case from
      /// </param>
      /// <param name="mapper">mapping information</param>
      public static AssetUseCase ToAssetUseCase(
         AssetUseCaseMap map, BookMapper mapper)
      {
         BookMapItemList items = BookMapItemList.PrepareList(map, mapper);

         AssetUseCase uc = new AssetUseCase(mapper.Namespace, mapper.VersionId);
         uc.Name = map.Name;

         // iteract through each (source -> target) map items...
         int sequenceNo = 0;
         foreach(var item in map.Items)
         {
            int maxIndex = item.SourceElement.Count > item.TargetElement.Count ?
               item.SourceElement.Count : item.TargetElement.Count;

            // must include all source - target items - elements
            for(var index = 0; index < maxIndex; index++)
            {
               var ritem = new AssetUseCaseReportItem
               {
                  SequenceNo = sequenceNo,
                  Index = index,
                  MapItem = item
               };

               if (item.SourceElement.Count >= index + 1)
               {
                  ritem.Source = item.SourceElement[index].DataElement;
               }
               if (item.TargetElement.Count >= index + 1)
               {
                  ritem.Target = item.TargetElement[index].DataElement;
               }
               sequenceNo++;

               uc.MappedItems.Add(ritem);
            }
         }

         return uc;
      }

      /// <summary>
      /// Given a folder path get all the Use Cases and prepare an instance of 
      /// AssetUseCase per Use Case file.
      /// </summary>
      /// <remarks>REPORT STARTS HERE.</remarks>
      /// <param name="folderPath">folder path name</param>
      /// <param name="mapper">mapping information</param>
      public static AssetUseCaseList ToAssetUseCase(
         string folderPath, BookMapper mapper)
      {
         AssetUseCaseList useCaseList = new AssetUseCaseList();

         var files = AssetUseCaseMap.FromFolder(folderPath);

         // prepare a Use Case per 
         foreach (var i in files)
         {
            var uc = AssetUseCaseMap.FromFile(i.Full);
            uc.Name = i.Name;
            mapper.Book = uc.Book;

            useCaseList.Add(ToAssetUseCase(uc, mapper));
         }

         return useCaseList;
      }

      /// <summary>
      /// Given project arguments and a the use case folder path prepare the
      /// Use Case items.
      /// </summary>
      /// <remarks>REPORT STARTS HERE.</remarks>
      /// <param name="args">project arguments</param>
      /// <param name="useCaseFolderPath">Use Case folder path</param>
      /// <returns>instance list of AssetUseCase is returned</returns>
      public static List<AssetUseCase> ToUseCaseReport(
         AssetConsoleArgumentsInfo args, string projectPath,
         string useCaseFolderPath)
      {

         // if there are no data items just return...
         if (args.AssetDataItems.Count == 0)
         {
            return null;
         }

         string currentFolder = Directory.GetCurrentDirectory();
         AssetUseCaseList list = null;
         try
         {
            Directory.SetCurrentDirectory(projectPath);

            // prepare use-case list ordered by book/booklets map-items
            AssetData adata = args.AssetDataItems[0];
            BookMapper mapper = new BookMapper(
               adata.Items, args.Namespace,
               args.ProjectVersionId);
            list = ToAssetUseCase(useCaseFolderPath, mapper);

            // prepare the report...
            AssetUseCaseReport.ToOutput(args.OutputFile, adata, list,
               args.Report);
         }
         catch
         {

         }
         finally
         {
            if (currentFolder != null)
            {
               Directory.SetCurrentDirectory(currentFolder);
            }
         }


         return list;
      }

      #endregion

   }

}
