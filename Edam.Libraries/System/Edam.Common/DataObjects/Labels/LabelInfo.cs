using System;
using System.Collections.Generic;

//------------------------------------------------------------------------------
// Copied from Kif Library v5r0

namespace Edam.DataObjects.Labels
{

   /// <summary>
   /// 
   /// </summary>
   public class LabelInfo
   {
      public String LabelId { get; set; }
      public String LabelAssociationId { get; set; }
      public String LabelLinkId { get; set; }

      public LabelInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         LabelId            = String.Empty;
         LabelAssociationId = String.Empty;
         LabelLinkId        = String.Empty;
      }
   }

}
