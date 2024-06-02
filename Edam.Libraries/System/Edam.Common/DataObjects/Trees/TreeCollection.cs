using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Edam.DataObjects.Trees
{

   public interface ITreeTag
   {
      String Index { get; set; }
      String Title { get; set; }
      Object Tag { get; set; }
   }

   public class TreeItem<T> where T : ITreeTag
   {
      public Int16[] Level;

      public String Index { get; set; }
      public String Title { get; set; }
      public Object Tag { get; set; }
      public TreeItem<T>[] Items;
      public TreeItem<T> Parent { get; set; }

      public void ClearFields()
      {
         Index = "0";
         Title = String.Empty;
         Tag = null;
         Items = new TreeItem<T>[0];
         Parent = null;
      }
   }

   public class TreeWalker<T> where T : ITreeTag
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

         newNode.Items = new TreeItem<T>[0];
         if (Current.Level.Length == newNode.Level.Length)
         {
            newNode.Parent = Current.Parent;
            if (Current.Parent != null)
            {
               if (Current.Parent.Items == null)
               {
                  Current.Parent.Items = new TreeItem<T>[0];
               }
               Current.Parent.Items = Push(Current.Parent.Items, newNode);
               Current = newNode;
            }
         }
         else if (Current.Level.Length < newNode.Level.Length)
         {
            newNode.Parent = Current;
            if (Current.Items == null)
            {
               Current.Items = new TreeItem<T>[0];
            }
            Current.Items = Push(Current.Items, newNode);
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

   public class TreeCollection<T> where T : ITreeTag
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

}
