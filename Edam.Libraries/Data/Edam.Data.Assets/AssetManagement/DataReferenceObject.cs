using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
   public partial class DataReferenceObject
   {
      public DataReferenceObject()
      {
         DataNote = new HashSet<DataNote>();
      }

      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public DateTime ReferenceDate { get; set; }
      public string ReferenceId { get; set; }
      public int ReferenceTypeNo { get; set; }
      public string AliasId { get; set; }
      public string AlternateId { get; set; }
      public string Description { get; set; }
      public int StatusNo { get; set; }
      public string UpdateSessionId { get; set; }
      public string RecordStatusCode { get; set; }

      public virtual DataReferenceType ReferenceTypeNoNavigation { get; set; }
      public virtual DataReferenceStatus StatusNoNavigation { get; set; }
      public virtual ICollection<DataNote> DataNote { get; set; }
   }
}
