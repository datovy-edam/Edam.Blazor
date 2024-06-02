using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
   public partial class DataDictionary
   {
      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public int DictionaryNo { get; set; }
      public string OrganizationId { get; set; }
      public string DataOwnerId { get; set; }
      public string Name { get; set; }
      public string RootUri { get; set; }
      public string UpdateSessionId { get; set; }
      public string RecordStatusCode { get; set; }
   }
}
