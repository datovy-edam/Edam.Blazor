using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Documents
{

   public class DocumentReportStyleInfo
   {

      public String StyleId { get; set; }
      public String Name { get; set; }
      public String StyleUrlPath { get; set; }
      public String ReportPath { get; set; }

      public void ClearFields()
      {
         StyleId = String.Empty;
         Name = String.Empty;
         StyleUrlPath = String.Empty;
         ReportPath = String.Empty;
      }

   }

}
