using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Data.AssetConsole;
using Edam.Data.Asset;
using System.Xml.Linq;
using Edam.DataObjects.Trees;

namespace Edam.Data.AssetSchema
{

   public class AssetDataTree
   {

      #region -- 1.00 - Fields and Properties

      private AssetData m_AssetData;
      private AssetDataElementList m_Items;
      private AssetDataTreeItem m_Root = null;
      private int m_MaxDepthCount = 0;
      private NamePath m_RootNamePath;

      public AssetDataTreeItem Root
      {
         get { return m_Root; }
      }

      public NamePath RootNamePath
      {
         get { return m_RootNamePath; }
      }

      private AssetDataNameUriRegistry m_Registry = null;

      #endregion
      #region -- 1.50 - Initialization and Constructures

      public AssetDataTree(AssetDataElementList items,
         NamespaceInfo defaultNamespace, int depthCount = 0)
      {
         m_Items = items;
         m_MaxDepthCount = depthCount;
         m_Registry = new AssetDataNameUriRegistry(defaultNamespace);
      }

      public AssetDataTree(AssetData asset,
         NamespaceInfo defaultNamespace, int depthCount = 0)
      {
         m_AssetData = asset;
         m_Items = asset.Items;
         m_MaxDepthCount = depthCount;
         m_Registry = new AssetDataNameUriRegistry(defaultNamespace);
      }

      #endregion
      #region -- 4.00 - Data Tree Model Preparation Support Methods

      public void SetRoot(AssetDataTreeItem root, string rootFullName)
      {
         m_Root = root;
         m_RootNamePath = NamePath.Parse(rootFullName);
      }

      public AssetDataTreeItem GetItem(AssetDataElement element)
      {
         AssetDataTreeItem item = new AssetDataTreeItem();
         item.Namespace = m_AssetData.GetNamespace(element);
         item.OriginalElement = element;
         item.Element = AssetDataElement.ToDataElement(
            element, item.Namespace, null);

         m_Registry.SetTitle(item);
         return item;
      }

      private List<AssetDataTreeItem> GetInheritedChildren(
         AssetDataElement element, AssetDataTreeItem parent)
      {
         List<AssetDataTreeItem> list = new List<AssetDataTreeItem>();
         AssetDataElement type = m_Items.Find(
            (x) => x.IsType && x.ElementName == element.DataType);
         if (type != null)
         {
            var ichildren = AssetDataElementList.GetChildren(
               m_Items, type.Root, type.DataType, 
               type.GetElementNamespace(), AssetType.Schema,
               element.VersionId);
            if (ichildren != null)
            {
               foreach (var child in ichildren)
               {
                  var tchild = GetItem(child);
                  tchild.Parent = parent;
                  list.Add(tchild);
               }
            }
         }
         return list;
      }

      public List<AssetDataTreeItem> GetChildren(
         AssetDataElement element, AssetDataTreeItem parent)
      {
         List<AssetDataTreeItem> list = new List<AssetDataTreeItem>();

         string elementName = element.ElementType == ElementType.element ||
            element.ElementType == ElementType.reference ?
            element.DataType : element.ElementName;

         var children = AssetDataElementList.GetChildren(
            m_Items, element.Root, elementName, element.GetElementNamespace(),
            AssetType.Schema, element.VersionId);
         foreach(var child in children)
         {
            var tchild = GetItem(child);
            tchild.Parent = parent;
            list.Add(tchild);
         }
         return list;
      }

      private int GetDepthCount(AssetDataTreeItem item)
      {
         int c = 0;
         while(item != null)
         {
            c++;
            if (item.Parent == null)
            {
               break;
            }
            item = item.Parent;
         }
         return c;
      }

      public void GetElementDeclarationChildren(
         AssetDataElement element, AssetDataTreeItem parent)
      {
         var items = from c in m_Items
                     where c.ElementName == element.DataType &&
                          (c.EntityName == String.Empty ||
                           c.EntityName == null) &&
                           c.ElementType == ElementType.element &
                           c.MaxOccurrence > 0
                     select c as AssetDataElement;
         if (items != null && items.Count() > 0)
         {
            AssetDataElement ielement = items.First();
            var children = from i in m_Items
                           where i.ElementName == ielement.DataType &&
                                 i.IsType
                           select i as AssetDataElement;
         }
      }

      /// <summary>
      /// Prepare Tree using given asset data-tree item and parent...
      /// </summary>
      /// <param name="treeItem">tree item</param>
      /// <param name="parent">parent item</param>
      /// <returns>instance of tree item is returned</returns>
      public AssetDataTreeItem PrepareTree(
         AssetDataTreeItem treeItem, AssetDataTreeItem parent)
      {
         // look-out for infinite loops... or stop after max-depth is found
         if (m_MaxDepthCount != 0)
         {
            int depthCount = GetDepthCount(treeItem);
            if (depthCount > m_MaxDepthCount)
            {
               return treeItem;
            }
         }

         // get the inherited children
         List<AssetDataTreeItem> ichildren = null;
         //if (treeItem.Element.ElementType == ElementType.type)
         //{
            ichildren = GetInheritedChildren(treeItem.Element, parent);
         //}

         // get the children
         var echildren = GetChildren(treeItem.Element, parent);

         if (ichildren != null)
         {
            treeItem.Children.AddRange(ichildren);
         }
         treeItem.Children.AddRange(echildren);

         // process all children
         foreach (var child in treeItem.Children)
         {
            if (child.Children.Count > 0)
            {
               continue;
            }

            if (!child.IsValueBaseType)
            {
               PrepareTree(child, treeItem);
            }
         }
         if (treeItem.Children.Count > 0)
         {
            treeItem.Type = InOut.ItemType.Folder;
         }
         return treeItem;
      }

      /// <summary>
      /// Register Name URI.
      /// </summary>
      /// <param name="items">Items to register</param>
      public void RegisterNameUri(List<AssetData> items)
      {
         foreach (AssetData a in items)
         {
            foreach(var i in a.Items)
            {
               m_Registry.RegisterItem(i);
            }
         }
      }

      /// <summary>
      /// Set Title for given item.
      /// </summary>
      /// <param name="item">item</param>
      public void SetTitle(AssetDataTreeItem item)
      {
         m_Registry.SetTitle(item);
      }

      /// <summary>
      /// Given a list of Asset Data collection fetch corresponding tree.
      /// </summary>
      /// <param name="items">one or more Asset Data collections...</param>
      /// <param name="rootElement">Root element to search form and build tree 
      /// for</param>
      /// <param name="defaultNamespace">default namespace</param>
      /// <param name="maxDepthCount">max depth</param>
      /// <returns>instance of AssetDataTree is returned</returns>
      public static AssetDataTree GetDataTree(
         List<AssetData> items, string rootElement,
         NamespaceInfo defaultNamespace, int maxDepthCount = 0)
      {

         AssetDataTree tree = null;
         foreach (AssetData asset in items)
         {
            tree = new AssetDataTree(asset, defaultNamespace, maxDepthCount);
            tree.RegisterNameUri(items);

            AssetDataElement root = asset.FindRootElement(rootElement);
            if (root == null)
            {
               continue;
            }
            AssetDataTreeItem titem = tree.GetItem(root);

            // remove trailing namespace prefix...
            var ename = rootElement.Split(':');
            titem.SetTitle(ename.Length > 1 ? ename[1] : ename[0]);

            tree.SetRoot(titem, rootElement);

            tree.PrepareTree(titem, null);
            break;
         }

         if (tree == null || tree.Root == null)
         {
            return null;
         }

         // wrap root element and make it a child of a container
         AssetDataTreeItem troot = new AssetDataTreeItem
         {
            Parent = null,
            OriginalElement = tree.Root.Element,
            Element = tree.Root.Element
         };
         tree.SetTitle(troot);
         troot.Children.Add(tree.Root);
         tree.SetRoot(troot, rootElement);

         return tree;
      }

      /// <summary>
      /// Get Data Tree derived with information provided in the console 
      /// arguments...
      /// </summary>
      /// <param name="arguments">arguments</param>
      /// <param name="maxDepthCount">max depth to scan for (default : 0)
      /// </param>
      /// <returns>instance of AssetDataTree is returned</returns>
      public static AssetDataTree GetDataTree(
         AssetConsoleArgumentsInfo arguments, int maxDepthCount = 0)
      {
         NamespaceInfo ns = arguments.GetDefaultNamespace();
         return GetDataTree(
            arguments.AssetDataItems, arguments.RootElementName, 
            ns, maxDepthCount);
      }

      #endregion

   }

}
