using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Data.AssetSchema
{

   public class AssetDataNameUriRegistry
   {

      public NamespaceInfo DefaultNamespace { get; set; }

      /// <summary>
      /// The name dictionary is needed to make sure that names used throughout
      /// different namespaces are property prefixed if name collisions are 
      /// found.
      /// </summary>
      private Dictionary<string, List<AssetDataTreeItemUri>>
         m_NameUriDictionary =
            new Dictionary<string, List<AssetDataTreeItemUri>>();

      public AssetDataNameUriRegistry(NamespaceInfo defaultNamespace)
      {
         DefaultNamespace = defaultNamespace;
      }

      #region -- 4.00 - Name - URI Registration

      private List<AssetDataTreeItemUri> FindRegisteredList(string name)
      {
         List<AssetDataTreeItemUri> item = null;
         if (m_NameUriDictionary.TryGetValue(name, out item))
         {
         }
         return item;
      }

      private AssetDataTreeItemUri FindRegisteredItem(
         List<AssetDataTreeItemUri> list, string uriText)
      {
         return list.Find((x) => x.UriText == uriText); ;
      }

      /// <summary>
      /// Register Item name and uri to later manage name collisions.
      /// </summary>
      /// <param name="element"></param>
      public void RegisterItem(AssetDataElement element)
      {
         AssetDataTreeItemUri item = null;
         var list = FindRegisteredList(
            element.ElementQualifiedName.OriginalName);
         if (list != null)
         {
            item = FindRegisteredItem(list, element.ElementUri);
         }

         if (list == null)
         {
            list = new List<AssetDataTreeItemUri>();
            m_NameUriDictionary.Add(
               element.ElementQualifiedName.OriginalName, list);
         }
         if (item == null)
         {
            item = new AssetDataTreeItemUri();
            item.Name = element.ElementQualifiedName.OriginalName;
            item.UriText = element.ElementUri;
            list.Add(item);
         }
      }

      private AssetDataTreeItemUri FindRegisteredItem(AssetDataElement element)
      {
         AssetDataTreeItemUri item = null;
         var list = FindRegisteredList(
            element.ElementQualifiedName.OriginalName);
         if (list != null)
         {
            item = FindRegisteredItem(list, element.ElementUri);
         }
         return item;
      }

      private bool DoNameUriCollied(AssetDataElement element)
      {
         if (element.ElementUri == DefaultNamespace.Uri.AbsoluteUri)
         {
            return false;
         }

         AssetDataTreeItemUri item = null;
         var list = FindRegisteredList(
            element.ElementQualifiedName.OriginalName);
         if (list != null)
         {
            item = FindRegisteredItem(list, element.ElementUri);
         }
         if (item == null)
         {
            throw new Exception("Element Name and Uri not registered!");
         }
         return list.Count > 1;
      }

      private string GetElementTitle(AssetDataElement element)
      {
         return element.ElementQualifiedName.OriginalName;
         //bool collied = DoNameUriCollied(element);
         //return collied ? element.ElementName.Replace(':', '_') :
         //   element.ElementQualifiedName.OriginalName;
      }

      public void SetTitle(AssetDataTreeItem item)
      {
         item.SetTitle(GetElementTitle(item.OriginalElement));
      }

      #endregion

   }

}
