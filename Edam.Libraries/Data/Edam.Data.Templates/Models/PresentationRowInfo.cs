using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization;

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class PresentationRowInfo : ObjectInfo, IObjectInfo
   {

      [DataMember]
      public List<PresentationColumnInfo> Columns { get; set; }
      public PresentationRowInfo() : base()
      {
         Type = ResourceType.PresentationRow;
         Columns = new List<PresentationColumnInfo>();
      }
   }

}
