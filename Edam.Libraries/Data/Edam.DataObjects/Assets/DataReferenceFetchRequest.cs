using System;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Assets
{
   public class DataReferenceFetchRequest
   {
      public string SessionId { get; set; }
      public string OrganizationID { get; set; }
      public string Root { get; set; }
      public string DomainUri { get; set; }
      public string TermName { get; set; }
      public DataReferenceOption Option { get; set; }
   }
}
