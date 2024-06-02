using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class ObjectInfo : IObjectInfo
   {
      [DataMember]
      public ResourceType Type { get; set; }
      [DataMember]
      public Int64? ObjectNo { get; set; }
      [DataMember]
      public String Name { get; set; }
      [DataMember]
      public String Description { get; set; }
      [DataMember]
      public String Help { get; set; }
      [DataMember]
      public String Sample { get; set; }

      public ObjectInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         Type = ResourceType.Unknown;
         ObjectNo = null;
         Name = String.Empty;
         Description = String.Empty;
      }
   }

}
