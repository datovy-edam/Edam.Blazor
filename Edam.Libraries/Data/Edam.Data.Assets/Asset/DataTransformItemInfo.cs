using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.Asset
{
   public class DataTransformItemInfo
   {
      public DataTransformItem Type { get; set; }
      public String SourceId { get; set; }
      public String TargetId { get; set; }
      public DataTransformItemInfo(String sourceId, String targetId,
         DataTransformItem type = DataTransformItem.ResourceIdTransform)
      {
         Type = type;
         SourceId = sourceId;
         TargetId = targetId;
      }
      public String Transform(String sourceId)
      {
         return TargetId;
      }
   }
}
