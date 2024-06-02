using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Documents
{

   public class DocumentSignatureInfo
   {

      public DocumentReportStyleInfo ReportStyle { get; set; }
      public DateTime SignatureDate { get; set; }
      public String SignatorName { get; set; }
      public String SignatorEmail { get; set; }
      public String SignatorPassword { get; set; }

      public void ClearFields()
      {
         ReportStyle = new DocumentReportStyleInfo();
         ReportStyle.ClearFields();
         SignatureDate = DateTime.Now;
         SignatorName = String.Empty;
         SignatorEmail = String.Empty;
         SignatorPassword = String.Empty;
      }

      /// <summary>
      /// Check that needed signature components were provided
      /// </summary>
      /// <returns>true if signature info is complete</returns>
      public Boolean Validate()
      {
         if (String.IsNullOrEmpty(SignatorName) ||
             String.IsNullOrEmpty(SignatorEmail) ||
             String.IsNullOrEmpty(SignatorPassword))
            return false;
         return true;
      }

      /// <summary>
      /// Check that needed signature components were provided
      /// </summary>
      /// <returns>true if signature info is complete</returns>
      public static Boolean Validate(DocumentSignatureInfo value)
      {
         if (value == null)
            return false;
         return value.Validate();
      }

   }

}
