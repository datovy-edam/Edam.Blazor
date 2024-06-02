using System;
using System.ComponentModel;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Devices;

namespace Edam.DataObjects.Entities
{

   public interface IUserData
   {
      int? UserNo { get; set; }
      string UserName { get; set; }
      string UserId { get; set; }
      string UserAlternateId { get; set; }
      string UserEmail { get; set; }
      string UserPassword { get; set; }
      PhoneInfo UserPhone { get; set; }
      string StateCode { get; set; }
      string PostalCode { get; set; }
      string PinNumber { get; set; }
   }

}
