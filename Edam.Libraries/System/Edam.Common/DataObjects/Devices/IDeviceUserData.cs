using System;
using System.ComponentModel;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Devices;
using Edam.DataObjects.RecordManagement;

namespace Edam.DataObjects.Entities
{

   public interface IDeviceUserData : 
      IRecordManagementData, IDeviceData, IUserRecordData, IUserData
   {

   }

}
