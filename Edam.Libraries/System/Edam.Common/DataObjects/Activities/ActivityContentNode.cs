using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.DataObjects.Trees;

namespace Edam.DataObjects.Activities
{

   public enum ActivityContentTreeOption
   {
      Unknown = 0,
      JsonString = 1
   }

   public class ActivityContentNode : ITreeTag
   {

      public string Index { get; set; }
      public string Title { get; set; }
      public Object Tag { get; set; }

   }

   public class ActivityContentTreeHelper
   {
      public static readonly string ROOT_NODE_ID = "0";
      public static readonly string ROOT_NAME = "ROOT";

      public static ActivityContentNode[] Push(
         ActivityContentNode[] list, ActivityContentNode item)
      {
         Array.Resize<ActivityContentNode>(ref list, list.Length + 1);
         list[list.Length - 1] = item;
         return list;
      }

      public static ActivityContentNode[] ToArray(List<ActivityContentInfo> list)
      {
         ActivityContentNode node;
         ActivityContentNode[] l = new ActivityContentNode[0];
         foreach (var i in list)
         {
            node = new ActivityContentNode
            {
               Index = i.IdentifierId,
               Title = i.Alias,
               Tag = i
            };
            l = Push(l, node);
         }
         return l;
      }

      public static ResultsLog<TreeItem<ActivityContentNode>> ToTree(
         List<ActivityContentInfo> list, String rootName = null)
      {
         var results = new ResultsLog<TreeItem<ActivityContentNode>>();
         try
         {
            // add a root node before preparing the tree...
            var item = new ActivityContentInfo();
            item.IdentifierId = ROOT_NODE_ID;
            item.Alias = String.IsNullOrWhiteSpace(rootName) ?
               ROOT_NAME : rootName;
            list.Add(item);
            var l = ToArray(list);

            // prepare the tree...
            TreeCollection<ActivityContentNode> t =
               new TreeCollection<ActivityContentNode>(l);

            results.Data = t.Root;
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      public static ResultsLog<String> ToJson(List<ActivityContentInfo> list)
      {
         ResultsLog<String> results = new ResultsLog<string>();
         try
         {
            var l = ToArray(list);
            TreeCollection<ActivityContentNode> t =
               new TreeCollection<ActivityContentNode>(l);
            results.Data = t.ToJsonText();
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

   }

}
