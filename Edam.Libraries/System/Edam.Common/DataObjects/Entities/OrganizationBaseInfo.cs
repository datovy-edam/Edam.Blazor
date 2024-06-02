using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Base (Minimum) Organization details.  Consider the OrganizationId equal 
   /// to the Center-ID.
   /// </summary>
   public class OrganizationBaseInfo
   {

      public ReferenceBaseType Type { get; set; }
      public String OrganizationId { get; set; }
      public String OrganizationName { get; set; }
      public String Alias { get; set; }
      public String AlternateId { get; set; }

      public OrganizationBaseInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Setup the default values for a new instance.
      /// </summary>
      public void ClearFields()
      {
         Type = ReferenceBaseType.Organization;
         OrganizationId = String.Empty;
         OrganizationName = String.Empty;
         Alias = String.Empty;
         AlternateId = String.Empty;
      }

   }

}
