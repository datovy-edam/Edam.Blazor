using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
//using DbField = Edam.Data.DataField;
using Edam.DataObjects.DataCodes;
using Edam.Serialization;

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class PresentationContentInfo : ObjectInfo, IObjectInfo
   {
      [DataMember]
      public String GridTier { get; set; }
      [DataMember]
      public List<PresentationRowInfo> Rows { get; set; }
      [DataMember]
      public PresentationFlowInfo Presentation { get; set; }

      public PresentationContentInfo() : base()
      {
         Type = ResourceType.PresentationContent;
         Rows = new List<PresentationRowInfo>();
         Presentation = new PresentationFlowInfo();
      }

      public new void ClearFields()
      {
         if (Rows == null)
            Rows = new List<PresentationRowInfo>();
         if (Presentation == null)
            Presentation = new PresentationFlowInfo();
         Rows.Clear();
         Presentation.ClearFields();
      }

   }

}
