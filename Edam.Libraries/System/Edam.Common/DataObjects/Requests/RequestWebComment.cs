using System;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Requests
{

   public class RequestWebComment
   {

      public String RequestId { get; set; }
      public String OrganizationId { get; set; }
      public String Email { get; set; }
      public String Phone { get; set; }
      public String PostalCode { get; set; }
      public String GivenName { get; set; }
      public String Surname1 { get; set; }
      public String Surname2 { get; set; }
      public String Name { get; set; }
      public String Comments { get; set; }

      public RequestWebComment()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         RequestId = String.Empty;
         OrganizationId = String.Empty;
         Email = String.Empty;
         Phone = String.Empty;
         PostalCode = String.Empty;
         Name = String.Empty;
         GivenName = String.Empty;
         Surname1 = String.Empty;
         Surname2 = String.Empty;
         Comments = String.Empty;
      }

   }

}
