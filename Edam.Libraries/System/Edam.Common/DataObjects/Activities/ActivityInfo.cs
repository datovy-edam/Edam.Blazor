using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityInfo
   {

      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public References.ReferenceBaseType ReferenceType { get; set; }
      public String Alias { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }

      private Locations.LocationInfo m_Location = new Locations.LocationInfo();
      public Locations.LocationInfo Location
      {
         get { return m_Location; }
         set
         {
            if (value == null)
            {
               m_Location.ClearFields();
               return;
            }
            m_Location = value;
         }
      }

      public ActivityInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         ReferenceType = References.ReferenceBaseType.ActivityProgram;
         Alias = String.Empty;
         StartDate = Edam.NullDateTime.Value;
         EndDate = Edam.NullDateTime.Value;

         Location.ClearFields();
      }

   }

}
