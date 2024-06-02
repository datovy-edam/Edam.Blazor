using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Asset
{

   public class CodeSetInfo
   {
      public string OrganizationId { get; set; }
      public int DomainNo { get; set; }
      public long CodeSetNo { get; set; }
      public string CodeSetId { get; set; }
      public string CodeSetUri { get; set; }
      public string CodeSetName { get; set; }
      public string VersionId { get; set; }
      public string DataOwnerId { get; set; }
      public string RecordStatusCode { get; set; }

      public List<CodeInfo> Codes { get; set; } = new List<CodeInfo>();
   }

}
