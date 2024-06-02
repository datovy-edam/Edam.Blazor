using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization;

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class PresentationColumnInfo : ObjectInfo, IObjectInfo
   {
      [DataMember]
      public short ColumnSpan { get; set; }
      [DataMember]
      public short RowSpan { get; set; }

      public PresentationColumnInfo() : base()
      {
         Type = ResourceType.PresentationColumn;
         ColumnSpan = 1;
         RowSpan = 1;
      }
   }

}
