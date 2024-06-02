using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.Json.JsonExplore;
using Edam.Data.DataItem;
using System.Security.Cryptography.X509Certificates;
using Edam.Data.AssetManagement;
using Edam.Data.Assets.AssetSchema;
using Edam.Diagnostics;
using System.Xml.Linq;
using Edam.Data.AssetProject;
//using Jsonata.Net.Native.Json;

namespace Edam.Json.JsonInstanceReader
{

   public class JsonInstanceInspector : ItemInstance, IJsonInspector
   {

      public AssetConsoleArgumentsInfo Arguments { get; set; }
      public AssetData Asset { get; set; }
      public NamespaceInfo DefaultNamespace { get; set; }

      public JsonInstanceInspector(AssetConsoleArgumentsInfo arguments) :
         base(arguments.OrganizationId, arguments.Namespace, AssetType.Asset, 
            namespaces: null, mapper: null, arguments.ProjectVersionId)
      {
         //Project.GotoProject(arguments);
         AssetItem.TextMapper = DataTextMap.FromFile(arguments);
         DefaultNamespace = arguments.Namespace;
         Arguments = arguments;
      }

      public static ItemInstanceType GetType(JTokenType type)
      {
         switch(type)
         {
            case JTokenType.Array:
               return ItemInstanceType.Array;
            case JTokenType.Boolean:
               return ItemInstanceType.Boolean;
            case JTokenType.Date: 
               return ItemInstanceType.Date;
            case JTokenType.Float:
               return ItemInstanceType.Float;
            case JTokenType.Integer:
               return ItemInstanceType.Integer;
            case JTokenType.Bytes:
               return ItemInstanceType.Bytes;
            case JTokenType.Object:
               return ItemInstanceType.Object;
            case JTokenType.String: 
               return ItemInstanceType.String;
            case JTokenType.Null:
               return ItemInstanceType.Null;
            case JTokenType.Comment: 
               return ItemInstanceType.Comment;
            case JTokenType.Constructor:
               return ItemInstanceType.Constructor;
            case JTokenType.Property:
               return ItemInstanceType.Property;
            case JTokenType.Raw: 
               return ItemInstanceType.Raw;
            case JTokenType.Guid: 
               return ItemInstanceType.Guid;
            case JTokenType.TimeSpan: 
               return ItemInstanceType.TimeSpan;
            case JTokenType.Undefined:
               return ItemInstanceType.Undefined;
            case JTokenType.Uri:
               return ItemInstanceType.Uri;
            default:
               return ItemInstanceType.None;
         }
      }

      /// <summary>
      /// Get the Array Index from given path.
      /// </summary>
      /// <param name="path">path ending with index</param>
      /// <returns>array index is returned</returns>
      private int GetArrayIndex(string path)
      {
         int sindex = path.IndexOf('[');
         if (sindex == -1)
         {
            return -1;
         }
         sindex++;
         string indxText = path.Substring(sindex,path.Length - sindex);
         indxText = indxText.Replace("]", "");
         int.TryParse(indxText, out int index);
         return index;
      }

      /// <summary>
      /// Read Children...
      /// </summary>
      /// <param name="children">child list</param>
      /// <param name="parentType">parent type</param>
      /// <param name="childList"></param>
      public void ReadChildren(JEnumerable<JToken> children, 
         ItemInstanceTypeInfo parentType, List<ItemInstanceItemInfo> childList)
      {
         foreach (var item in children)
         {
            if (parentType.Type == ItemInstanceType.Array)
            {
               int index = GetArrayIndex(item.Path);
               if (index != -1 && index > 0)
               {
                  return;
               }
            }

            ItemInstanceItemInfo citem = new ItemInstanceItemInfo(); ;
            citem.Parent = parentType;

            if (item.Type == JTokenType.Property)
            {
               ReadProperty(item as JProperty, parentType, citem);

               JProperty iprop = item as JProperty;
               if (iprop != null)
               {
                  citem.Path = iprop.Path.ToString();
                  citem.Name = iprop.Name;
                  childList.Add(citem);
               }
            }
            else if (item.Type == JTokenType.Object)
            {
               foreach(var ctoken in item.Children<JToken>())
               {
                  citem = new ItemInstanceItemInfo();
                  citem.Parent = parentType;
                  ReadProperty(ctoken as JProperty, parentType, citem);
                  childList.Add(citem);
                  if (citem.Name == null)
                  {

                  }
               }
            }
            else if (parentType.Type == ItemInstanceType.Array)
            {
               JValue val = (JValue)item;
               citem.Value = val.ToString();
               citem.Type = GetType(item.Type);
               citem.Path = item.Path.ToString();
               citem.MaxOccursUnbounded = true;
               citem.SetName();

               parentType.Type = citem.Type;
               parentType.DataType = citem.Type.ToString();
               parentType.Value = val.ToString();
               parentType.IsValue = true;
               parentType.MaxOccursUnbounded = true;
               parentType.Name = citem.Name;
               parentType.OriginalName = citem.Name;

               childList.Add(parentType);
               break;
            }
            else
            {

            }
         }
      }

      private string GetName(string name)
      {
         string lastName = name;
         bool notDone = true;
         while (notDone)
         {
            var n = Find(lastName);
            if (n == null)
            {
               break;
            }
            lastName = n.Name + "_";
         }
         return lastName;
      }

      /// <summary>
      /// Read Property.
      /// </summary>
      /// <param name="property">instance of JProperty</param>
      public ItemInstanceTypeInfo ReadProperty(
         JProperty property, ItemInstanceTypeInfo parent, 
         ItemInstanceItemInfo childItem)
      {
         ItemInstanceTypeInfo type = new ItemInstanceTypeInfo();
         type.Parent = parent;
         List<ItemInstanceItemInfo> children = new List<ItemInstanceItemInfo>();

         foreach (var child in property.Children<JToken>())
         {
            // if object create a new type
            if (child.Type == JTokenType.Object)
            {
               if (childItem != null)
               {
                  childItem.Type = GetType(child.Type);
                  childItem.Path = child.Path.ToString();
                  childItem.SetName();
                  childItem.DataType = childItem.Name + AssetItem.TypePostfix;
               }

               type.Path = child.Path.ToString();
               type.Type = GetType(child.Type);
               type.SetName();
               type.DataType = type.Name + AssetItem.TypePostfix;
            }
            else if (child.Type == JTokenType.Array && childItem != null)
            {
               if (childItem != null)
               {
                  childItem.Type = GetType(child.Type);
                  childItem.Path = child.Path.ToString();
                  childItem.SetName();
                  childItem.DataType = childItem.Name + AssetItem.TypePostfix +
                     AssetItem.TypePostfix;
               }

               type.Path = child.Path;
               type.Type = GetType(child.Type);
               type.SetName();
               type.Name = type.Name + "_";
               type.DataType = type.Name + AssetItem.TypePostfix;
               type.MaxOccursUnbounded = true;
            }
            else if (child.Type != JTokenType.Property && childItem != null)
            {
               JValue val = (JValue)child;
               childItem.Value = val.ToString();
               childItem.Type = GetType(val.Type);
               childItem.Path = child.Path;
               childItem.SetName();
               childItem.IsValue = true;
               if (childItem.Type == ItemInstanceType.Null)
               {
                  childItem.DataType = "String";
               }
               return type;
            }
            else
            {

            }

            ReadChildren(child.Children<JToken>(), type, children);

            if (type.IsValue)
            {
               childItem.Type = type.Type;
               childItem.DataType = type.DataType;
               childItem.IsValue = true;
               childItem.Value = type.Value;
               childItem.MaxOccursUnbounded = type.MaxOccursUnbounded;
            }
         }
         type.Children = children;

         if (!String.IsNullOrWhiteSpace(type.Name))
         {
            if (!Types.ContainsKey(type.Name) && !type.IsValue)
            {
               Types.Add(type.Name, type);
            }
         }
         return type;
      }

      /// <summary>
      /// Inspect given JSON document instance and prepare its AssetData
      /// representation.
      /// </summary>
      /// <param name="jsonPath">JSON file path</param>
      public AssetData Inspect(string jsonPath = null)
      {
         Asset = new AssetData(Arguments.Namespace, AssetType.Asset, null);
         if (String.IsNullOrWhiteSpace(jsonPath))
         {
            return null;
         }

         string jsonText = File.ReadAllText(jsonPath, Encoding.UTF8);

         ItemInstanceTypeInfo root = new ItemInstanceTypeInfo();
         root.Path = Arguments.Namespace.OrganizationDomainId + "." + 
            Arguments.RootElementName;
         root.SetName();

         dynamic result = JsonConvert.DeserializeObject(jsonText);
         foreach(dynamic item in result)
         {
            var child = ReadProperty(item as JProperty, null, null);
            root.Children.Add(child);
         }

         return ToAssetData(root);
      }

      /// <summary>
      /// Go through all provided JSON files.
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public IResultsLog ToAsset(AssetConsoleArgumentsInfo arguments)
      {
         IResultsLog resultsLog = new ResultLog();

         var uriList = UriResourceInfo.GetUriList(
           arguments.UriList, UriResourceType.json);
         foreach (var fname in uriList)
         {
            // prepare Asset Data definitions (one per schema)
            var adata = Inspect(fname);

            if (adata != null)
            {
               adata.Namespaces.Add(NamespaceInfo.GetW3CNamespace());
               List<AssetData> assets = new List<AssetData>();
               assets.Add(adata);
               if (arguments.AssetDataItems == null)
               {
                  arguments.AssetDataItems = new AssetDataList();
               }
               arguments.AssetDataItems.AddRange(assets);
            }
         }

         resultsLog.Succeeded();
         return resultsLog;
      }

      /// <summary>
      /// Inspect underlying JSON document instance and prepare its AssetData
      /// representation.
      /// </summary>
      public void Inspect()
      {
         ToAsset(Arguments);
      }

   }

}
