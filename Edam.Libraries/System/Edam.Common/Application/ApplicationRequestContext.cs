using System;
using System.Collections.Generic;
using System.Web;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   /// <summary>
   /// Application Context
   /// </summary>
   public class ApplicationRequestContext
   {

      public String SessionId { get; set; }
      public String AlternateSessionId { get; set; }
      public String RequestId { get; set; }
      public String DeviceId { get; set; }
      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public String UserEmail { get; set; }
      public String UserHostAddress { get; set; }
      public Edam.Application.ApiKeyInfo ApiKey { get; set; }

      public ApplicationRequestContext()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         SessionId = String.Empty;
         AlternateSessionId = String.Empty;
         RequestId = String.Empty;
         DeviceId = String.Empty;
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         UserEmail = String.Empty;
         UserHostAddress = String.Empty;
         ApiKey = new ApiKeyInfo();
      }

      /// <summary>
      /// Manage Web-Application context...
      /// </summary>
      /// <param name="sessionId">session Id</param>
      /// <param name="organizationId">organization Id</param>
      /// <param name="referenceId">reference Id</param>
      /// <param name="deviceId">device Id</param>
      /// <returns>application context is returned</returns>
      public static ApplicationRequestContext GetContext(
         String sessionId, String organizationId, String referenceId,
         String deviceId)
      {
         // make sure that sessionId is not "undefined"...
         if (String.IsNullOrEmpty(sessionId))
            sessionId = String.Empty;
         if (sessionId.ToLower() ==
            Resources.Strings.Undefined.ToLower())
            sessionId = String.Empty;

         // switch to test/public database if not logged...
         Session.CheckSessionAndReference(
            sessionId, organizationId,
            Resources.Strings.DefaultDummyId);

         // prepare context
         ApplicationRequestContext c = new ApplicationRequestContext();

         //String httpSessId = HttpContext.Current == null ? null :
         //   (HttpContext.Current.Session?.SessionID);
         //String hostAddress = HttpContext.Current == null ? null :
         //   (HttpContext.Current.Request?.UserHostAddress);

         String httpSessId = null;
         String hostAddress = null;

         c.SessionId = sessionId;
         c.DeviceId = deviceId;
         c.OrganizationId = organizationId;
         c.ReferenceId = referenceId;
         c.AlternateSessionId = httpSessId ?? String.Empty;
         c.UserHostAddress = hostAddress ?? String.Empty;

         if (String.IsNullOrWhiteSpace(c.SessionId))
         {
            c.SessionId = System.Guid.NewGuid().ToString();
         }

         if (String.IsNullOrWhiteSpace(c.OrganizationId))
            c.OrganizationId = Resources.Strings.DefaultAgencyId;
         if (String.IsNullOrWhiteSpace(c.ReferenceId))
            c.ReferenceId = Resources.Strings.DefaultPublicId;
         return c;
      }

      /// <summary>
      /// Reset the database source to the default data source.
      /// </summary>
      public static void ResetDatabaseSourceToDefault()
      {
         Edam.Application.Session.ResetDatabaseSourceToDefault();
      }

   }

}
