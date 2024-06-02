using System;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Entities;

namespace Edam.DataObjects.Devices
{

   public class DeviceInfo : IDeviceData
   {

      public string DeviceId { get; set; }
      public string DeviceName { get; set; }
      public string IpAddress { get; set; }

      public DeviceInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         DeviceId = String.Empty;
         DeviceName = String.Empty;
         IpAddress = String.Empty;
      }

   }

}
