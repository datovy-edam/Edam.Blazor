using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Asset
{

   public class CodeInfo
   {
      public string OrganizationId { get; set; }
      public long CodeSetNo { get; set; }
      public long IdNo { get; set; }
      public string CodeId { get; set; }
      public string AlternateId { get; set; }
      public string VersionId { get; set; }
      public string Description { get; set; }
      public string CategoryId { get; set; }
      public string DataOwnerId { get; set; }
      public string RecordStatusCode { get; set; }
   }

}
