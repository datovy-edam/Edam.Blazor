using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Entities
{

   public class EntityAgentPayerInfo
   {

      public List<EntityPayerInfo> Payers { get; set; }
      public List<OrganizationAgentInfo> Agencies { get; set; }

   }

}
