using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityFollowUpItemInfo
   {
      public Int64 SerialNo { get; set; }
      public DateTime ReferenceDate { get; set; }
      public DateTime ServiceStartTime { get; set; }
      public DateTime ServiceEndTime { get; set; }
      public Int16 PeriodInDays { get; set; }
      public ActivityFollowUpItemInfo()
      {
         ClearFields();
      }
      public void ClearFields()
      {
         SerialNo = 0;
         ReferenceDate = DateTime.Now;
         ServiceStartTime = DateTime.Now;
         ServiceEndTime = DateTime.Now;
         PeriodInDays = 30;
      }
   }

}
