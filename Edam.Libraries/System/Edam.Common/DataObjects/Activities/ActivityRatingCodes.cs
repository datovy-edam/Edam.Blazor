using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Activities
{

   public class ActivityRatingCodes
   {

      public List<DataCodes.IDataCode> ActivityTypeGroups { get; set; }
      public List<DataCodes.IDataCode> ActivityResults { get; set; }
      public List<DataCodes.IDataCode> ActivityStatus { get; set; }
      public List<ActivityRatingInfo> ActivityRatings { get; set; }
      public List<Documents.DocumentReportStyleInfo> ReportStyles { get; set; }

   }

}
