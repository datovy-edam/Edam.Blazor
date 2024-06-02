using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetSchema
{

   /// <summary>
   /// The Asset Data Path is used to manage a text string delimited path that
   /// is being traversed.  It is assumed that each path item is unique and is
   /// not reused anywhere within a path.  This avoid the ambiguity while
   /// traversing and help to avoid re-entry or infinite loops.
   /// </summary>
   public class AssetDataPath
   {
      private readonly List<string> m_FullPath = new List<string>();

      public List<string> FullPath
      { 
         get { return m_FullPath; }
      }

      public AssetDataPath(string pathItem)
      {
         m_FullPath.Add(pathItem);
      }

      /// <summary>
      /// Add a new and unique item path.
      /// </summary>
      /// <param name="pathItem">path item to add</param>
      public void Add(string pathItem)
      {
         m_FullPath.Add(pathItem);
      }

      /// <summary>
      /// Walk through the path including newly found path elements... and
      /// return the rebuit path...
      /// </summary>
      /// <param name="delimitedText">text string that is delimited</param>
      /// <param name="delimiter">(optional) [default= '/'], path 
      /// delimiter</param>
      /// <returns>List of strings is returned</returns>
      public List<string> RebuildPath(
         string delimitedText, string delimiter = "/")
      {
         string[] path = delimitedText.Split(delimiter);
         List<string> rebuiltPath = new List<string>();
         string? pathItem = m_FullPath.Find((x) => x == path[0]);
         if (pathItem != null)
         {
            for (var i = 0; i < m_FullPath.Count; i++)
            {
               if (path[0] == m_FullPath[i])
               {
                  for (var c = 0; c < path.Length; c++)
                  {
                     rebuiltPath.Add(path[c]);
                  }
                  break;
               }
               rebuiltPath.Add(m_FullPath[i]);
            }
         }
         else
         {
            foreach (var x in m_FullPath)
            {
               rebuiltPath.Add(x);
            }
            foreach (var i in path)
            {
               rebuiltPath.Add(i);
            }
         }
         return rebuiltPath;
      }
   }

}
