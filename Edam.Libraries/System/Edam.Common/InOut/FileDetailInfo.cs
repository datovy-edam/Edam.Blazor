using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inout = System.IO;

using Edam.InOut;

namespace Edam.InOut
{

   public class FileDetailInfo
   {
      public string Name { get; set; }
      public string Path { get; set; }
      public DateTime CreatedDate { get; set; }
      public DateTime UpdatedDate { get; set; }
      public long Size { get; set; }

      public string CreatedDateText
      {
         get { return CreatedDate.ToString("yyy-MM-dd hh:mm"); }
      }

      public static List<FileDetailInfo> GetFiles(string folderPath)
      {
         List<FileDetailInfo> items = new List<FileDetailInfo>();

         return items;
      }

      public static FileDetailInfo GetDetails(FolderFileItemInfo item)
      {
         FileDetailInfo d = new FileDetailInfo();
         inout.FileInfo finfo = new inout.FileInfo(item.Full);
         d.Path = finfo.FullName;
         d.Name = item.NameFull;
         d.CreatedDate = finfo.CreationTime;
         d.UpdatedDate = finfo.CreationTime;
         d.Size = finfo.Length;
         return d;
      }
   }

}
