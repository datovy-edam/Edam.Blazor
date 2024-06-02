using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
   public partial class DataTerm
   {
      //public string NodeId99cc8a4b005e4a5197b446cc609093d4 { get; set; }
      public DateTime CreatedDate { get; set; }
      public DateTime ReferenceDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public string OrganizationId { get; set; }
      public string DataOwnerId { get; set; }
      public DateTime? ExpiredDate { get; set; }
      public string Root { get; set; }
      public string Domain { get; set; }
      public string Type { get; set; }
      public string Element { get; set; }
      public int TermNo { get; set; }
      public string TermId { get; set; }
      public string TermUri { get; set; }
      public string TermName { get; set; }
      public string Description { get; set; }
      public decimal? ConfidenceScore { get; set; }
      public int StatusNo { get; set; }
      public string UpdateSessionId { get; set; }
      public string RecordStatusCode { get; set; }

      public virtual ReferenceObjects ReferenceObjects { get; set; }
      public virtual DataReferenceStatus StatusNoNavigation { get; set; }
   }
}
