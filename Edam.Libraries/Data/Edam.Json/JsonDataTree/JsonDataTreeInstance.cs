using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetSchema;
using Edam.Json.JsonSample;

namespace Edam.Json.JsonDataTree
{

   public class SibligCount
   {
      public int Index { get; set; }
      public int Sibligs { get; set; }
      public bool Continues
      {
         get { return Index != Sibligs; }
      }
      public SibligCount()
      {
      }
   }

   public class JsonDataTreeInstance
   {
      private string m_Indent = "  ";
      private string m_CurrentIndent = String.Empty;
      private Stack<string> m_Stack = new Stack<string>();
      private JsonInstance m_Instance = new JsonInstance();
      private int m_ArrayCount = 3;
      private int m_IdentLevel = 0;

      public string JsonText
      {
         get { return m_Instance.JsonText; }
      }

      public JsonDataTreeInstance()
      {
         m_CurrentIndent = String.Empty;
         m_Stack.Push(String.Empty);
      }

      private void Push()
      {
         m_Stack.Push(m_Indent);
         m_IdentLevel++;
         m_CurrentIndent += m_Indent;
         m_Instance.Indent = m_CurrentIndent;
      }
      private void Pop()
      {
         var val = m_Stack.Pop();
         m_IdentLevel--;
         m_CurrentIndent = String.Empty;
         for (var i= 0; i < m_IdentLevel; i++)
         {
            m_CurrentIndent += m_Indent;
         }
         m_Instance.Indent = m_CurrentIndent;
      }

      public JsonInstance PrepareItem(
         AssetDataTreeItem item, SibligCount sibligs = null)
      {
         bool isArray = false;
         int arrayCount = 1;
         if (item.Type == InOut.ItemType.Folder)
         {
            if (item.IsCollection)
            {
               m_Instance.AddArrayStart(item.Title);
               arrayCount = m_ArrayCount;
               isArray = true;
            }
            else
            {
               m_Instance.AddBlockStart(item.Title);
            }
            Push();
         }

         var sCount = new SibligCount();
         for(int ac = 0; ac < arrayCount; ac++)
         {
            if (isArray)
            {
               m_Instance.AddBlockStart();
               Push();
            }

            int c = 0;
            int t = item.Children.Count;
            sCount.Sibligs = item.Children.Count;
            foreach (var child in item.Children)
            {
               c++;
               sCount.Index = c;
               if (child.Type == InOut.ItemType.Folder)
               {
                  PrepareItem(child, sCount);
               }
               else
               {
                  m_Instance.AddProperty(
                     child.Title, child.GetSampleValue(), c != t);
               }
            }

            if (isArray)
            {
               Pop();
               m_Instance.AddBlockEnd(ac + 1 < arrayCount);
            }
         }

         if (item.Type == InOut.ItemType.Folder)
         {
            Pop();
            if (item.IsCollection)
            {
               m_Instance.AddArrayEnd(sibligs != null && sibligs.Continues);
            }
            else
            {
               m_Instance.AddBlockEnd(sibligs != null && sibligs.Continues);
            }
         }
         return m_Instance;
      }

      public void CloseInstance()
      {
         m_Instance.CloseInstance();
         m_Instance.AddEmptyLine();
      }

      public static JsonDataTreeInstance PrepareInstance(AssetDataTreeItem root)
      {
         JsonDataTreeInstance jsonInstance = new JsonDataTreeInstance();
         jsonInstance.Push();

         if (root.Children != null && root.Children.Count > 0)
         {
            jsonInstance.PrepareItem(root.Children[0]);
         }

         jsonInstance.Pop();
         jsonInstance.CloseInstance();
         return jsonInstance;
      }

   }

}
