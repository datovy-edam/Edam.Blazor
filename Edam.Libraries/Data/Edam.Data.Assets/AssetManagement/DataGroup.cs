using System;
using System.Collections.Generic;

using Edam.Data.AssetManagement;

namespace Edam.Data.AssetManagement
{
   public partial class DataGroup
   {
      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public int GroupNo { get; set; }
      public string GroupId { get; set; }
      public int GroupTypeNo { get; set; }
      public string AlternateId { get; set; }
      public string GroupUri { get; set; }
      public string Path { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public short StatusNo { get; set; }
      public string UpdateSessionId { get; set; }
      public string RecordStatusCode { get; set; }

      public virtual DataGroupInfo GroupTypeNoNavigation { get; set; }
      public virtual ObjectStatus StatusNoNavigation { get; set; }
   }
}
