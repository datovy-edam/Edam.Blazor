using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization;

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class PresentationFlowInfo : ObjectInfo, IObjectInfo
   {

      [DataMember]
      public List<PresentationStepInfo> Steps { get; set; }
      public PresentationFlowInfo() : base()
      {
         Type = ResourceType.Presentation;
         Steps = new List<PresentationStepInfo>();
      }
      public new void ClearFields()
      {
         if (Steps == null)
            Steps = new List<PresentationStepInfo>();
         Steps.Clear();
      }
   }

}
