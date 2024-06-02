using System;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{
   
   public class MapVariableInfo
   {
      ElementItemInfo ParentItem { get; set; }
      MapVariableInfo ParentVariable { get; set; }
      String LinkElementName { get; set; }
      String ChildElementName { get; set; }
      Object Value { get; set; }
      MapVariableInfo()
      {
         this.ParentItem = null;
         this.ParentVariable = null;
         this.LinkElementName = "";
         this.ChildElementName = "";
         this.Value = null;
      }
   }

}
