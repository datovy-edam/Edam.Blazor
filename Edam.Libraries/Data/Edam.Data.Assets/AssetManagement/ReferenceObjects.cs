using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
   public partial class ReferenceObjects
   {
      public ReferenceObjects()
      {
         DataTerm = new HashSet<DataTerm>();
      }

      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public long ObjectNo { get; set; }
      public int TypeNo { get; set; }
      public short EntityTypeNo { get; set; }
      public Guid ReferenceGuid { get; set; }
      public string OrganizationId { get; set; }
      public string ReferenceId { get; set; }
      public string AlternateId { get; set; }
      public string Alias { get; set; }
      public string Description { get; set; }
      public short StatusNo { get; set; }

      public virtual ReferenceBaseTypes EntityTypeNoNavigation { get; set; }
      public virtual ObjectStatus StatusNoNavigation { get; set; }
      public virtual ReferenceTypes TypeNoNavigation { get; set; }
      public virtual ICollection<DataTerm> DataTerm { get; set; }
   }
}
