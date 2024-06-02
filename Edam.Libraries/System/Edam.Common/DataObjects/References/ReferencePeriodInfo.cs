using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.References
{

   public class ReferencePeriodInfo
   {

      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public String ReferencePeriodId { get; set; }
      public ReferencePeriod Period
      {
         get { return PeriodDate.Period; }
         set { PeriodDate.Period = value; }
      }
      public String PeriodId { get; set; }
      public DateTime? Start { get; set; }
      public DateTime? End { get; set; }
      public ReferencePeriodDate PeriodDate { get; set; }

      public ReferencePeriodInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         if (PeriodDate == null)
            PeriodDate = new ReferencePeriodDate();
         PeriodId = ReferencePeriodDate.GetReferencePeriodId(this);

         OrganizationId = Edam.Application.Session.OrganizationId;
         ReferenceId = String.Empty;
         ReferencePeriodId = String.Empty;
         Period = ReferencePeriod.Week;
         Start = null;
         End = null;
      }

   }

}
