using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization;

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class PresentationStepInfo : ObjectInfo, IObjectInfo
   {

      [DataMember]
      public List<String> Rows { get; set; }
      public PresentationStepInfo() : base()
      {
         Type = ResourceType.PresentationStep;
         Rows = new List<String>();
      }
      public new void ClearFields()
      {
         if (Rows == null)
            Rows = new List<string>();
         Rows.Clear();
      }
   }

}
