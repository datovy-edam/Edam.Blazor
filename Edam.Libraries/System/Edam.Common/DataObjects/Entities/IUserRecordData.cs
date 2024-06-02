using System;

// -----------------------------------------------------------------------------
using Edam.DataObjects.RecordManagement;

namespace Edam.DataObjects.Entities
{

   public interface IUserRecordData : 
      IRecordManagementData, IOrganizationData, IUserData
   {
   }

}
