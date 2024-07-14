using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Data;
using Newtonsoft.Json;

namespace Edam.DataObjects.Trees;


public enum TreeItemType
{
   Unknown = 0,
   Branch = 1,
   Leaf = 2
}

public interface ITreeItem
{
   String? Index { get; set; }
   String Title { get; set; }
   String Name { get; set; }
   Object? Tag { get; set; }
   String Icon { get; set; }
   int? Number { get; set; }
   TreeItemType Type { get; set; }
}

public class TreeItem : ITreeItem
{
   public Int16[] Level;

   public String? Index { get; set; } = null;
   public String Title { get; set; }
   public String Name { get; set; }
   public Object? Tag { get; set; }
   public String Icon { get; set; }
   public int? Number { get; set; }

   public TreeItemType Type { get; set; }

   public bool IsExpanded { get; set; } = false;

   public bool IsBranch
   {
      get { return Type == TreeItemType.Branch; }
   }

   public bool IsLeaf
   {
      get { return Type == TreeItemType.Leaf; }
   }

   public bool Visited { get; set; } = false;

   /// <summary>
   /// Clear Fields... using default values.
   /// </summary>
   public void ClearFields()
   {
      Index = "0";
      Title = String.Empty;
      Name = String.Empty;
      Tag = null;
      Type = TreeItemType.Leaf;
      Number = 0;
   }

   /// <summary>
   /// Duplicate item.  If item is being duplicated Visited will be true.
   /// </summary>
   /// <returns>return a duplicate copy of the item</returns>
   public TreeItem Duplicate()
   {
      TreeItem itm = new TreeItem();
      itm.Icon = Icon;
      itm.Number = Number;
      itm.Level = Level;
      itm.Index = Index;
      itm.Title = Title;
      itm.Name = Name;
      itm.Tag = Tag;
      itm.Type = Type;
      itm.Visited = true;
      return itm;
   }

   /// <summary>
   /// From a root element to 
   /// </summary>
   /// <param name="root"></param>
   /// <returns></returns>
   public static String ToDirectoryJsonText<T>(T? root)
   {

      JsonSerializerSettings s = new JsonSerializerSettings();
      s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      string jsonText = JsonConvert.SerializeObject(root, s);
      return jsonText;
   }

   /// <summary>
   /// From a JSON file system info to 
   /// </summary>
   /// <param name="jsonText"></param>
   /// <returns></returns>
   public static T? FromDirectoryJsonText<T>(string jsonText)
   {
      JsonSerializerSettings s = new JsonSerializerSettings();
      T? result = JsonConvert.DeserializeObject<T>(jsonText);
      return result;
   }
}


public class TreeItem<T> : TreeItem where T : ITreeItem
{

   public bool HasChildren
   {
      get { return Children != null && Children.Length > 0; }
   }

   public TreeItem<T>[] Children;
   public TreeItem<T> Parent { get; set; }

   public TreeItem() : base()
   {
   }

}

public class TreeWalker<T> where T : ITreeItem
{
   public TreeItem<T> Root { get; set; }
   public TreeItem<T> Current { get; set; }

   public TreeWalker(TreeItem<T> root)
   {
      Root = root;
      Current = root;
      Current.Parent = null;
   }

   private TreeItem<T>[] Push(TreeItem<T>[] list, TreeItem<T> item)
   {
      Array.Resize<TreeItem<T>>(ref list, list.Length + 1);
      list[list.Length - 1] = item;
      return list;
   }

   public void Add(TreeItem<T> newNode)
   {
      if (Current == null)
         return;

      newNode.Children = new TreeItem<T>[0];
      if (Current.Level.Length == newNode.Level.Length)
      {
         newNode.Parent = Current.Parent;
         if (Current.Parent != null)
         {
            if (Current.Parent.Children == null)
            {
               Current.Parent.Children = new TreeItem<T>[0];
            }
            Current.Parent.Children = Push(Current.Parent.Children, newNode);
            Current = newNode;
         }
      }
      else if (Current.Level.Length < newNode.Level.Length)
      {
         newNode.Parent = Current;
         if (Current.Children == null)
         {
            Current.Children = new TreeItem<T>[0];
         }
         Current.Children = Push(Current.Children, newNode);
         Current = newNode;
      }
      else
      {
         while (true)
         {
            if (Current.Level.Length > newNode.Level.Length)
            {
               Current = Current.Parent;
               continue;
            }
            Add(newNode);
            break;
         }
      }
   }
}

public class TreeCollection<T> where T : ITreeItem
{
   public static readonly String defaultDelimiter = "_";
   public TreeItem<T> Root { get; set; }

   public TreeCollection(T[] list)
   {
      TreeCollection<T>.Sort(list);
      Root = null;
      TreeWalker<T> w = null;
      for (var i = 0; i < list.Length; i++)
      {
         var it = list[i];
         var t = new TreeItem<T>
         {
            Index = it.Index,
            Title = it.Title,
            Tag = it.Tag,
            Level = ParseIndexHierarchy(it.Index)
         };
         if (w == null)
         {
            w = new TreeWalker<T>(t);
         }
         else
         {
            w.Add(t);
         }
      }
      this.Root = w == null ? new TreeItem<T>() : w.Root;
   }

   public String ToJsonText()
   {
      JsonSerializerSettings s = new JsonSerializerSettings();
      s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      string jsonText = JsonConvert.SerializeObject(Root, s);
      return jsonText;
   }

   public static Int16[] ParseIndexHierarchy(
      String index, String delimiter = null)
   {
      if (String.IsNullOrWhiteSpace(index))
         return new Int16[0];
      if (delimiter == null)
         delimiter = defaultDelimiter;
      var l = index.Split(new[] { delimiter }, StringSplitOptions.None);
      var o = new Int16[l.Length];
      for (var i = 0; i < l.Length; i++)
      {
         o[i] = Int16.Parse(l[i]);
      }
      return o;
   }

   public static void Sort(T[] list)
   {
      Array.Sort<T>(list, (a, b) => a.Index.CompareTo(b.Index));
   }

}

