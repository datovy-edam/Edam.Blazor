using System;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   public enum PolicyType
   {
      Unknown = 0,
      CanManageOrganization = 1,
      CanChangeReference = 2,
      CanUseApplication = 3,
      CanAdminSystem = 4,
      CanAdminApplication = 5,
      CanManageOrganizationData = 6,
      CanManageSelfData = 7
   }

}
