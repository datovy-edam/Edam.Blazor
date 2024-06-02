using System;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Devices
{

   public interface IDeviceData
   {
      string DeviceId { get; set; }
      string DeviceName { get; set; }
      string IpAddress { get; set; }
   }

}
