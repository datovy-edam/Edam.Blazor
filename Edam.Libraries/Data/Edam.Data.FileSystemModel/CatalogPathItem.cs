using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Edam.DataObjects.Medias;
using Edam.DataObjects.Trees;
using Edam.InOut;

namespace Edam.Data.FileSystemModel
{

   /// <summary>
   /// Manage a Catalog Path and related information...
   /// </summary>
   public class CatalogPathItem : FolderFileItemInfo
   {
      public MediaFormat MediaFormat { get; set; }
      public string ContentType { get; set; }
      public FileItemInfo Item { get; private set; }
      public CatalogItemInfo TreeItem { get; set; } = null;

      public new CatalogPathItem Parent { get; set; } = null;

      public CatalogPathItem(FileItemInfo item)
      {
         Item = item;
         FromFullPath(Item.FullPath);
      }

      /// <summary>
      /// Setup Full Path details...
      /// </summary>
      /// <param name="path">path</param>
      public void FromFullPath(string path)
      {
         base.FromFullPath(path, null);

         if (Path == null)
         {
            Path = path;
            Full = Path;
         }
         else
         { 
            Path = Path.Replace("\\", "/").
               Substring(DriverName.Length).Replace("//", "/");
            Full = (Path + "/" + NameFull).Replace("//", "/");
         }

         MediaFormat = MediaContentTypeHelper.GetMediaFormat(Extension);
         ContentType = MediaFormat == MediaFormat.Unknown ? String.Empty :
            MediaContentTypeHelper.ToContentTypeText(MediaFormat);

         if (MediaFormat == MediaFormat.Unknown &&
            !String.IsNullOrWhiteSpace(Extension))
         {
            Extension = String.Empty;
            ExtensionName = String.Empty;
            Name = NameFull;
         }

      }

   }

}
