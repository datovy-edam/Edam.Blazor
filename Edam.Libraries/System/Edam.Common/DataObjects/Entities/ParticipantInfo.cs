using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Entities
{

   public class ParticipantInfo
   {

      #region -- 1.00 Properties and Fields

      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public String ParticipantId { get; set; }

      public String EntityId { get; set; }
      public String GivenName { get; set; }
      public String MiddleName { get; set; }
      public String FatherLastName { get; set; }
      public String MotherLastName { get; set; }
      public String FullName { get; set; }
      public String Email { get; set; }
      public String PhoneNumber { get; set; }
      public DateTime BirthDate { get; set; }
      public String StateCode { get; set; }
      public String PostalCode { get; set; }

      public References.ReferenceBaseType Role { get; set; }

      #endregion

      public ParticipantInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         OrganizationId = Edam.Application.Resources.Strings.DefaultAgencyId;
         ParticipantId = String.Empty;
         EntityId = String.Empty;
         GivenName = String.Empty;
         MiddleName = String.Empty;
         FatherLastName = String.Empty;
         MotherLastName = String.Empty;
         FullName = String.Empty;
         Email = String.Empty;
         PhoneNumber = String.Empty;
         BirthDate = Edam.NullDateTime.Value;
         Role = References.ReferenceBaseType.Participant;
         StateCode = String.Empty;
         PostalCode = String.Empty;
      }

      public static void FixNullValues(ParticipantInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.EntityId =
            Edam.Convert.ToNotNullString(record.EntityId);
         record.GivenName =
            Edam.Convert.ToNotNullString(record.GivenName);
         record.FatherLastName =
            Edam.Convert.ToNotNullString(record.FatherLastName);
         record.MotherLastName =
            Edam.Convert.ToNotNullString(record.MotherLastName);
         record.FullName = Edam.Convert.ToNotNullString(record.FullName);
         record.Email = Edam.Convert.ToNotNullString(record.Email);
         record.PhoneNumber = Edam.Convert.ToNotNullString(record.PhoneNumber);
         record.StateCode = Edam.Convert.ToNotNullString(record.StateCode);
         record.PostalCode = Edam.Convert.ToNotNullString(record.PostalCode);
      }

   }

}
