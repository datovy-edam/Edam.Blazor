using System;
using System.ComponentModel;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Devices;

namespace Edam.DataObjects.RecordManagement
{

   public interface IRecordManagementData
   {
      DateTime? CreatedDate { get; set; }
      DateTime? LastUpdateDate { get; set; }
   }

}
