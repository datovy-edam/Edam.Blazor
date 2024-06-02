using System;
using System.ComponentModel;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Devices;

namespace Edam.DataObjects.Entities
{

   public interface IOrganizationData
   {
      string OrganizationId { get; set; }
      string OrganizationName { get; set; }
   }

}
