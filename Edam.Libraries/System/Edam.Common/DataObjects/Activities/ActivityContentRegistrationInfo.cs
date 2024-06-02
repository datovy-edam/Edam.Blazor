using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityContentRegistrationInfo
   {
      public String OrganizationId { get; set; }
      public String ProgramId { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public Objects.ObjectScope Scope { get; set; }
      public String ApplicationId { get; set; }
      public String TemplateId { get; set; }
      public String Description { get; set; }
      public String SpanishDescription { get; set; }
      public Int16 OptionNo { get; set; }

      public ActivityContentRegistrationInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         ProgramId = String.Empty;
         Status = Objects.ObjectStatus.Active;
         Scope = Objects.ObjectScope.Private;
         ApplicationId = String.Empty;
         TemplateId = String.Empty;
         Description = String.Empty;
         SpanishDescription = String.Empty;
         OptionNo = 0;
      }
   }

}
