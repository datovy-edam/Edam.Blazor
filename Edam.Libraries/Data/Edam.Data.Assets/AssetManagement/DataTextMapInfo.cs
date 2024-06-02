using System;

namespace Edam.Data.AssetManagement
{

   public class DataTextMapItem
   {
      public DataTextMapDirection Direction { get; set; }
      public String SourceText { get; set; }
      public String TargetText { get; set; }
      public DataTextMapItem()
      {

      }
      public DataTextMapItem(String source, String target,
         DataTextMapDirection direction)
      {
         Direction = direction;
         SourceText = source;
         TargetText = target;
      }
   }

}
