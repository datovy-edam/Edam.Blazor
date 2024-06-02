using System;
using System.Collections.Generic;
using System.IO;

namespace Edam.InOut
{

   public class FolderFileReader
   {
      public List<string> m_Files = new List<string>();
      public List<string> Items
      {
         get { return m_Files; }
      }
      public FolderFileReader(string path)
      {
         GetAllFilePaths(path);
      }
      private void GetAllFilePaths(string path)
      {
         if (Directory.Exists(path))
         {
            string[] SubDirs = Directory.GetDirectories(path);
            //m_Files.AddRange(SubDirs);
            m_Files.AddRange(Directory.GetFiles(path));
            foreach (string subdir in SubDirs)
               GetAllFilePaths(subdir);
         }
         else if (File.Exists(path))
         {
            m_Files.Add(path);
         }
      }

      /// <summary>
      /// Get File Path Names for all items in given path. If path is a file and
      /// ends with given extention a list with it will be returned; if path is
      /// a folder it is scanned looking for files with given extentions. Any
      /// found sub-folder will be further scanned until all folders had been
      /// visited.
      /// </summary>
      /// <param name="path">file or folder path</param>
      /// <param name="extention">requested extention (should include dot.
      /// </param>
      /// <param name="items">(optional) list in were to add newly found files
      /// </param>
      /// <returns>a list of file name paths is returned</returns>
      public static List<string> GetFilePathNames(
         string path, string extention, List<string> items = null)
      {
         if (File.Exists(path))
         {
            if (path.EndsWith(extention))
            {
               List<string> lf = items ?? new List<string>();
               lf.Add(path);
               return lf;
            }
            return new List<string>();
         }

         var r = new FolderFileReader(path);
         var l = r.Items.FindAll((x) => x.EndsWith(extention));
         if (items != null)
         {
            items.AddRange(l);
            l = items;
         }
         return l;
      }

      /// <summary>
      /// Get all Folder/File items...
      /// </summary>
      /// <param name="item">initialized item that must contain its full path
      /// </param>
      /// <returns></returns>
      public static FolderFileItemInfo GetAllItems(
         FolderFileItemInfo item, FolderFileItemInfo parent)
      {
         if (!Directory.Exists(item.Full))
         {
            return null;
         }

         string[] SubDirs = Directory.GetDirectories(item.Full);
         List<string> files = new List<string>();
         files.AddRange(Directory.GetFiles(item.Full));

         // add files...
         foreach (var i in files)
         {
            item.AddFile(i, parent);
         }

         foreach (string subdir in SubDirs)
         {
            var f = item.AddFolder(subdir, parent);
            GetAllItems(f, item);
         }
         return item;
      }

      /// <summary>
      /// Given a folder path get all its children and their children...
      /// </summary>
      /// <param name="folderPath">folder full path</param>
      /// <returns>InOutItemInfo is return</returns>
      public static FolderFileItemInfo GetFolderFileInfo(string folderPath)
      {
         FolderFileItemInfo item = new FolderFileItemInfo(folderPath, null);
         item.Type = ItemType.Folder;
         if (string.IsNullOrWhiteSpace(folderPath) ||
            !Directory.Exists(folderPath))
         {
            return item;
         }
         return GetAllItems(item, null);
      }

   }

}
