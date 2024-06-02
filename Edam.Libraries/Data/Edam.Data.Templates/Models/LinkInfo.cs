using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public class LinkInfo : ObjectInfo, IObjectInfo
   {
      [DataMember]
      public String Title { get; set; }
      [DataMember]
      public String ParentElementName { get; set; }
      [DataMember]
      public String ChildElementName { get; set; }
      [DataMember]
      public String GroupElementName { get; set; }
      [DataMember]
      public String LinkElementName { get; set; }
      [DataMember]
      public String DescriptionElementName { get; set; }
      [DataMember]
      public String SelectedValue { get; set; }
      [DataMember]
      public String DefaultValue { get; set; }

      public LinkInfo() : base()
      {
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         Type = ResourceType.MapLink;
         ParentElementName = String.Empty;
         ChildElementName = String.Empty;
         GroupElementName = String.Empty;
         DescriptionElementName = String.Empty;
         Title = String.Empty;
         SelectedValue = null;
         DefaultValue = null;
      }
   }

}
