using System;
using io = System.IO;
using System.Collections.Generic;
using Edam.DataObjects.Models;

namespace Edam.InOut
{
   public enum ItemType
   {
      Unknow = 0,
      Folder = 1,
      File = 2
   }

   public class ItemBaseInfo
   {

      public List<FolderFileItemInfo> Children { get; set; }
      public FolderFileItemInfo Parent { get; set; }
      public ItemType Type { get; set; }
      public String Path { get; set; }
      public String Name { get; set; }
      public String NameFull { get; set; }
      public String Extension { get; set; }
      public String ExtensionName { get; set; }
      public List<BaseColumnInfo> Columns { get; set; }

      public bool IsFullPath
      {
         get { return NameFull.Contains("c:/") ||
               NameFull.Contains("c:\\"); }
      }

      public string Full { get; set; }

      public bool IsJson
      {
         get { return FileExtension.IsJson(ExtensionName); }
      }

      public ItemBaseInfo()
      {
         Children = new List<FolderFileItemInfo>();
      }

      /// <summary>
      /// Given a full-path set file information.
      /// </summary>
      /// <param name="fullPath">full path</param>
      public void FromFullPath(String fullPath, FolderFileItemInfo parent)
      {
         Full = fullPath;
         Extension = io.Path.GetExtension(fullPath);
         ExtensionName = String.IsNullOrWhiteSpace(Extension) ? 
            String.Empty : Extension[1..];
         Name = io.Path.GetFileNameWithoutExtension(fullPath);
         NameFull = io.Path.GetFileName(fullPath);
         Path = io.Path.GetDirectoryName(fullPath);
         Parent = parent;
      }

      //public String Full
      //{
      //   get
      //   {
      //      var ext = String.IsNullOrWhiteSpace(Extension) ?
      //         String.Empty : "." + Extension;
      //      var fp = Path + "/" + Name;
      //      return fp + (fp.LastIndexOf(ext) == -1 ? ext : String.Empty);
      //   }
      //}
   }

   public class FolderFileItemInfo : ItemBaseInfo
   {

      private void Initialize()
      {
         Type = ItemType.File;
      }

      public FolderFileItemInfo() : base()
      {
         Initialize();
      }
      public FolderFileItemInfo(
         string rootPath, FolderFileItemInfo parent) : base()
      {
         Initialize();
         FromFullPath(rootPath, parent);
      }

      public FolderFileItemInfo AddFolder(
         string name, FolderFileItemInfo parent)
      {
         FolderFileItemInfo i = new FolderFileItemInfo(name, parent);
         i.Type = ItemType.Folder;
         Children.Add(i);
         return i;
      }

      public void AddFile(string name, FolderFileItemInfo parent)
      {
         FolderFileItemInfo i = new FolderFileItemInfo(name, parent);
         Children.Add(i);
      }

   }

   public class FileInfo : FolderFileItemInfo
   {
      public FileInfo() : base()
      {

      }
      public FileInfo(string folderPath, FolderFileItemInfo parent) : 
         base(folderPath, parent)
      {

      }
   }

   public class FolderInfo : FolderFileItemInfo
   {
      public FolderInfo() : base()
      {

      }
      public FolderInfo(string folderPath, FolderFileItemInfo parent) : 
         base(folderPath, parent)
      {
      }
   }

}
