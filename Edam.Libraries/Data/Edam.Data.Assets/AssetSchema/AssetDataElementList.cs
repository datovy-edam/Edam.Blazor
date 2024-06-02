using Edam.Data.Asset;
using Edam.Data.AssetManagement;
using Edam.Data.AssetManagement.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetSchema
{

   public class AssetDataElementList : List<AssetDataElement>
   {

      private List<AssetDataElement> m_Types = new List<AssetDataElement>();
      private int m_Count = 0;

      public int ItemsCount
      {
         get { return m_Count; }
         set { m_Count = value; }
      }

      public List<AssetDataElement> Types
      {
         get { return m_Types; }
      }

      public NamespaceInfo Namespace { get; set; }
      public AssetType AssetType { get; set; }
      public string VersionId { get; set; }

      public AssetDataElementList(
         NamespaceInfo ns, AssetType type, string versionId)
      {
         Namespace = ns;
         AssetType = type;
         VersionId = versionId;
      }

      public AssetDataElementList(AssetDataElementList asset)
      {
         Namespace = asset.Namespace;
         AssetType = asset.AssetType;
         VersionId = asset.VersionId;
      }

      /// <summary>
      /// Given two elements, merge them as needed... with emphasis on the 
      /// PK and foreign key constraints.
      /// </summary>
      /// <param name="element">element to be merged with sibling</param>
      /// <param name="sibling">element to be inspected and use as a source for
      /// merging</param>
      public void MergeElements(
         AssetDataElement element, AssetDataElement sibling)
      {
         // merge primary key and auto-number properties
         if (sibling.KeyType == Asset.ConstraintType.key)
         {
            element.KeyType = sibling.KeyType;
         }
         if (sibling.AutoGenerateType == Asset.ConstraintType.autoGenerate)
         {
            element.AutoGenerateType = sibling.AutoGenerateType;
         }

         // merge constraints...
         if (sibling.Constraints != null)
         {
            foreach(var c in sibling.Constraints)
            {
               element.Constraints.Add(c);
            }
         }
      }

      /// <summary>
      /// Add element.
      /// </summary>
      /// <param name="element">element to add</param>
      public new void Add(AssetDataElement element)
      {
         // check to see if... the element already exists?
         var eelement = Find((x)=> x.EntityName == element.EntityName &&
            x.ElementName == element.ElementName);
         if (eelement != null)
         {
            MergeElements(eelement, element);
            return;
         }

         // add new element...
         AssetDataElement parent = null;
         if (element.IsType)
         {
            var t = m_Types.Find((x)=> x.ElementName == element.ElementName);
            if (t == null)
            {
               m_Types.Add(element);
            }
            else
            {
               parent = t;
            }
         }
         else if (!String.IsNullOrWhiteSpace(element.EntityName))
         {
            parent = m_Types.Find((x) => x.ElementName == element.EntityName);
         }
         element.OrdinalNo = ++ItemsCount;
         element.Parent = parent;
         element.ParentNo = parent != null ?
            parent.OrdinalNo : element.OrdinalNo;
         base.Add(element);
      }

      /// <summary>
      /// Get element namespace.
      /// </summary>
      /// <param name="element">element to inspect</param>
      /// <returns>found URI</returns>
      public static Uri GetNamespace(AssetDataElement element)
      {
         if (String.IsNullOrWhiteSpace(element.Namespace))
         {
            return new Uri(
               NamespaceInfo.HTTP_ROOT + element.Root + "/" + element.Domain);
         }
         return new Uri(element.NamespaceText);
      }

      /// <summary>
      /// Get element namespace.
      /// </summary>
      /// <param name="element">element to inspect</param>
      /// <returns>found URI</returns>
      public static Uri GetNamespace(DataElement element)
      {
         if (String.IsNullOrWhiteSpace(element.NamespaceText))
         {
            return new Uri(
               NamespaceInfo.HTTP_ROOT + element.Root + "/" + element.Domain);
         }
         return new Uri(element.NamespaceText);
      }

      /// <summary>
      /// Set namespace base on given element namespace.
      /// </summary>
      /// <param name="element">element to inspect</param>
      public static void SetNamespace(AssetDataElement element)
      {
         Uri ns = GetNamespace(element);
      }

      /// <summary>
      /// Set namespace base on given element namespace.
      /// </summary>
      /// <param name="element">element to inspect</param>
      public static void SetNamespace(DataElement element)
      {
         Uri ns = GetNamespace(element);
      }

      /// <summary>
      /// Get the Types of given list of element items.
      /// </summary>
      /// <param name="items">element items list</param>
      /// <returns></returns>
      /// <returns>the list of times and each with related children is returned
      /// </returns>
      public static AssetDataElementList GetTypes(AssetDataElementList items)
      {
         var types = from c in items
                     where c.IsType
                     select c;

         var l = types.ToList();
         AssetDataElementList list = new AssetDataElementList(items);
         list.AddRange(l);
         return list;
      }

      /// <summary>
      /// Get the Types of given list of element items matching given root.
      /// </summary>
      /// <param name="items">element items list</param>
      /// <param name="root">root to be matched</param>
      /// <returns>the list of times and each with related children is returned
      /// </returns>
      public static AssetDataElementList GetTypes(
         AssetDataElementList items, string root)
      {
         var types = from c in items
                     where c.Root == root && c.IsType
                     select c;

         var l = types.ToList();
         AssetDataElementList list = new AssetDataElementList(items);
         list.AddRange(l);
         return list;
      }

      /// <summary>
      /// Get the Types of given list of element items matching given uriText.
      /// </summary>
      /// <param name="items">element items list</param>
      /// <param name="uriText">uriText to be matched</param>
      /// <returns>the list of times and each with related children is returned
      /// </returns>
      public static AssetDataElementList GetUriTypes(
         AssetDataElementList items, string uriText)
      {
         var types = from c in items
                     where c.ElementUri == uriText && c.IsType
                     select c;

         var l = types.ToList();
         AssetDataElementList list = new AssetDataElementList(items);
         list.AddRange(l);
         return list;
      }

      public static AssetDataElementList GetChildren(
         List<AssetDataElement> elements, string root, string entityName,
         NamespaceInfo ns, AssetType type, string versionId)
      {
         var items = from c in elements
                     where c.EntityName == entityName
                     select c as AssetDataElement;

         var l = items.ToList();
         AssetDataElementList list =
            new AssetDataElementList(ns, type, versionId);
         list.AddRange(l);
         return list;
      }

      public static AssetDataElementList GetChildren(
         AssetDataElementList elements, string root, string domain,
         string entityName)
      {
         return GetChildren(
            (List<AssetDataElement>)elements, root, entityName,
            elements.Namespace, elements.AssetType, elements.VersionId);
      }

      /// <summary>
      /// Get the Data Type corresponding element type definition.
      /// </summary>
      /// <param name="items">list of items to be searched</param>
      /// <param name="element">ellement whose data-type will be searched
      /// </param>
      /// <returns>instance of AssetDataElement is returned if found</returns>
      public static AssetDataElement GetDataType(
         AssetDataElementList items, AssetDataElement element)
      {
         // if element is a not a type definition, then look at its DataType
         var typ = items.Find((x) => x.ElementName == element.DataType);
         if (typ != null)
         {
            element = typ;
         }
         return element;
      }

      /// <summary>
      /// Get the Children of a given element.
      /// </summary>
      /// <param name="items">list of items to search</param>
      /// <param name="element">based element type</param>
      /// <returns>Children of the type is returned</returns>
      public static AssetDataItem GetChildren(
         AssetDataElementList items, AssetDataElement element)
      {
         // if element is a not a type definition, then look at its DataType
         if (String.IsNullOrWhiteSpace(element.EntityName))
         {
            element = GetDataType(items, element);
         }

         // now get type children...
         AssetDataItem item = new AssetDataItem();
         item.Element = element;
         item.Children = GetChildren(
            items, element.Root, element.Domain, element.ElementName);
         return item;
      }

      /// <summary>
      /// Get the Children of the first element - type found in items.
      /// </summary>
      /// <param name="items">list of items to search</param>
      /// <returns>Children of the type is returned</returns>
      public static AssetDataItem GetChildren(AssetDataElementList items)
      {
         AssetDataItem item = new AssetDataItem();
         item.Element = items.Find((x) => x.IsType);
         item.Children = GetChildren(items,
            item.Element.Root, item.Element.Domain, item.Element.EntityName);
         return item;
      }

      public static AssetDataElementList GetUriElements(
         AssetDataElementList items, string uriText)
      {
         var types = from c in items
                     where String.IsNullOrWhiteSpace(c.EntityName) && 
                            c.ElementUri == uriText && c.IsElement
                     select c;

         var l = types.ToList();
         AssetDataElementList list = new AssetDataElementList(items);
         list.AddRange(l);
         return list;
      }

      /// <summary>
      /// return root element.
      /// </summary>
      /// <param name="rootName">root element name</param>
      /// <returns>if found the root element is returned</returns>
      public AssetDataElement GetRootElement(string rootName)
      {
         var element = Find((x) => x.ElementName == rootName
            && x.EntityQualifiedName == null);
         return element;
      }

   }

}
