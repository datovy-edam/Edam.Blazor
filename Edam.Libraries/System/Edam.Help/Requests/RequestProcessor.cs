using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Make sure to put the following entries in the App.config:
//
//       <!-- it is assume that the Verification URL will be prepared so
//           the verified request email is uniquely identified -->
//       <add key="NewUserVerificationUrl" value="http://local:3040/Home"/>
//       <add key="NewUserSmtpServerConfigKey" value="Default"/>

using Edam.DataObjects.Requests;
using Edam.DataObjects.SelfHelp;
using Edam.Application;
//using Edam.DataObjects.Notifications;

using Helper = Edam.DataObjects.SelfHelp;
using DataObj = Edam.DataObjects;

namespace Edam.Help.Requests
{

   /// <summary>
   /// Helper to process Email, Notification and other processes requests.
   /// </summary>
   public class RequestProcessor
   {

      #region -- 1.0 Fields and Properties

      public static readonly String WebAppUrlKey =
         Edam.Application.Resources.Strings.WebAppUrlKey;
      public static readonly String NewUserSmtpServerKey =
         "NewUserSmtpServerConfigKey";
      public static readonly String NewUserLdapServerConfigKey =
         "NewUserLdapServerConfigKey";

      public static readonly String UriUserLogin =
         "#userLogin";

      public static readonly String UriUserRegistration =
         "/#userRegistration?sessionId={0}&userId={1}&userEmail={2}" +
         "&userPhone={3}&organizationId={4}&registrationId={5}";

      public static readonly String UriUserRegistrationEmailVerified =
         "/#userRegistration?sessionId={0}&requestId={1}&organizationId={2}" +
         "&userEmail={3}";
      public static readonly String UriPasswordChangeEmailVerified =
         "/#userPasswordChange?sessionId={0}&requestId={1}&organizationId={2}" +
         "&userEmail={3}";

      private Edam.Diagnostics.ResultLog m_LogResults;
      private String m_RequestId = String.Empty;
      private String m_Email = String.Empty;

      /// <summary>
      /// Results Logged while processing requests.
      /// </summary>
      public Edam.Diagnostics.ResultLog LogResults
      {
         get { return m_LogResults; }
      }

      #endregion
      #region -- 2.0 Initialize and Constructors

      /// <summary>
      /// Initialize object.
      /// </summary>
      public void Initialize(String requestId = "", String email = "")
      {
         if (m_LogResults == null)
            m_LogResults = new Diagnostics.ResultLog();
         m_RequestId = String.IsNullOrEmpty(requestId) ?
            System.Guid.NewGuid().ToString() : requestId;
         m_Email = String.IsNullOrEmpty(email) ?
            String.Empty : email;
      }

      /// <summary>
      /// Initialize the Request-Processor.
      /// </summary>
      /// <param name="request">Request to manage/process</param>
      public RequestProcessor(Helper.RegistrationRequestInfo request)
      {
         if (request == null)
         {
            //throw new Exception(Edam.Application.Resources.
            //   ApplicationStrings.RequestMustBeDefined);
            // TODO: fix hardcoded text...
            throw new Exception("Request Must be Defined");
         }

         if (request != null)
            Initialize(request.RequestId, request.Email);
         else
            Initialize();
      }

      /// <summary>
      /// Initialize the Request-Processor.
      /// </summary>
      /// <param name="requestId">request Id.</param>
      /// <param name="email">email</param>
      public RequestProcessor(String requestId, String email)
      {
         Initialize(requestId, email);
      }

      #endregion
      #region -- 4.0 Get Configuration Data, URI's, and other resources

      /// <summary>
      /// Get the SMTP Server Configuration Key to be used to send emails.
      /// </summary>
      /// <returns>The Key value is returned</returns>
      public static String GetSmtpServerConfigurationKey()
      {
         return AppSettings.GetSectionString(NewUserSmtpServerKey);
      }

      /// <summary>
      /// Get Web Application URL.
      /// </summary>
      /// <returns>Web Application URL is returned</returns>
      public static String GetWebApplicationUrl()
      {
         return AppSettings.GetSectionString(WebAppUrlKey);
      }

      /// <summary>
      /// Get Application URI.
      /// </summary>
      /// <returns>URI is returned</returns>
      public static String GetApplicationUri()
      {
         return GetWebApplicationUrl();
      }

      /// <summary>
      /// Get User Login URI.
      /// </summary>
      /// <returns>URI is returned</returns>
      public static String GetUserLoginUri()
      {
         return GetWebApplicationUrl() + UriUserLogin;
      }

      /// <summary>
      /// Ger User Registration Full Uri.
      /// </summary>
      /// <param name="data">instance of RegistrationRequestInfo</param>
      /// <returns>return the user registration full uri</returns>
      public static String GetUserRegistrationFullUri(
         Helper.RegistrationRequestInfo data)
      {
         return GetWebApplicationUrl() +
            String.Format(UriUserRegistration, data.SessionId, data.UserId,
            data.Email, data.Phone, data.OrganizationId, 
            data.RequestId);
      }

      /// <summary>
      /// Ger User Registration Full Uri.
      /// </summary>
      /// <param name="data">instance of RegistrationRequestInfo</param>
      /// <returns>return the user registration full uri</returns>
      public static String GetUserEmailVerifiedFullUri(
         Helper.RegistrationRequestInfo data)
      {
         String uri = String.Empty;
         switch(data.RequestType)
         {
            case Helper.SelfHelpRequest.NewRegistration:
               uri = UriUserRegistrationEmailVerified;
               break;
            case Helper.SelfHelpRequest.PasswordReset:
               uri = UriPasswordChangeEmailVerified;
               break;
         }
         return GetWebApplicationUrl() +
            String.Format(uri, data.SessionId, data.RequestId,
            data.OrganizationId, data.Email);
      }

      /// <summary>
      /// Get activity joined notification text.
      /// </summary>
      /// <param name="organizationId">organization Id.</param>
      /// <returns>prepared text is returned</returns>
      public static String GetActivityJoinedNotificationText(
         String organizationId)
      {
         String frmt = ApplicationHelper.GetString("NotifyActivityJoinDone");
         String label = String.Format(frmt, organizationId);
         return label;
      }

      /// <summary>
      /// Get Self-Help EMail Verified Text.
      /// </summary>
      /// <param name="data">request data</param>
      /// <param name="webApplicationUrl">web application URL</param>
      /// <returns>prepared text is returned</returns>
      public static String GetActivityEvaluationReviewRequestText(
         String webApplicationUrl = null)
      {
         if (String.IsNullOrEmpty(webApplicationUrl))
            webApplicationUrl = GetUserLoginUri();
         String frmt = ApplicationHelper.GetString("NotifyActivityRatingDone");
         String label = String.Format(frmt, webApplicationUrl);
         return label;
      }

      /// <summary>
      /// Get Self-Help EMail Verified Text.
      /// </summary>
      /// <param name="data">request data</param>
      /// <param name="webApplicationUrl">web application URL</param>
      /// <returns>prepared text is returned</returns>
      public String GetSelfHelpEmailVerifiedText(
         Helper.RegistrationRequestInfo data,
         String webApplicationUrl = null)
      {
         if (String.IsNullOrEmpty(webApplicationUrl))
            webApplicationUrl = GetUserEmailVerifiedFullUri(data);
         String frmt = ApplicationHelper.GetString("SelfHelpEmailVerified");
        // String label = String.Format(frmt, webApplicationUrl);
         String label = String.Format(frmt, data.RequestId);
         return label;
      }

      /// <summary>
      /// Get Self-Help Password Change request EMail Text.
      /// </summary>
      /// <param name="webApplicationUrl">web application URL</param>
      /// <returns></returns>
      public String GetSelfHelpEmailPasswordChangeText(
         String webApplicationUrl = null)
      {
         if (String.IsNullOrEmpty(webApplicationUrl))
            webApplicationUrl = GetWebApplicationUrl() +
               "?rno=" + m_RequestId;
         String frmt = ApplicationHelper.GetString(
            "SelfHelpEmailPasswordChangeRequest");
         String label = String.Format(frmt, webApplicationUrl);
         return label;
      }

      /// <summary>
      /// Get Request Veirification Text and optionaly assign the given 
      /// hyperlink reference.
      /// </summary>
      /// <param name="hreference">hyperlink reference</param>
      /// <returns>text string with verification html text is returned.
      /// </returns>
      public static String GetRequestVerificationText(String hreference,
         Boolean emailVerificationRequested = false)
      {
         String t = emailVerificationRequested ?
            ApplicationHelper.GetString("SelfHelpEmailVerified") :
            ApplicationHelper.GetString("SelfHelpRequestVerificationNeeded");
         if (String.IsNullOrWhiteSpace(hreference))
            return t;
         return String.Format(t, hreference);
      }

      #endregion
      #region -- 4.0 Email (and Verification) Support
      /*
      /// <summary>
      /// Send EMail...
      /// </summary>
      /// <param name="message">verify EMail message, it should be fully
      /// formatted message that will be used as the body of an EMail to the
      /// user</param>
      /// <param name="subject">(optional) subject; [default] a welcome
      /// message is sent</param>
      /// <param name="recipients">(optional) recipients</param>
      /// <returns>true is returned if all is OK</returns>
      public Boolean SendEMail(String message, String subject = "",
         List<Edam.DataObjects.Entities.RecipientInfo> recipients = null)
      {
         Boolean allGood;
         Int16 goodCnt = 0, badCnt = 0;
         String smtpConfigKey = GetSmtpServerConfigurationKey();

         if (String.IsNullOrEmpty(subject))
            subject = ApplicationHelper.GetString("SelfHelpEmailWelcome");

         try
         {
            if (recipients == null)
            {
               allGood = Edam.Net.Smtp.SmtpClient.SendMessage(smtpConfigKey,
                  m_Email, message, subject);
               if (allGood)
                  m_LogResults.Succeeded();
               else
                  m_LogResults.Failed(
                     Diagnostics.EventCode.RequestProcessorEmailSendFailed);
            }
            else
            {
               foreach (DataObj.Entities.RecipientInfo r in recipients)
               {
                  allGood = Edam.Net.Smtp.SmtpClient.SendMessage(smtpConfigKey,
                     r.Email, message, subject);
                  if (allGood)
                  {
                     m_LogResults.Succeeded();
                     goodCnt++;
                  }
                  else
                  {
                     m_LogResults.Failed(
                        Diagnostics.EventCode.RequestProcessorEmailSendFailed);
                     badCnt++;
                  }
               }
            }
         }
         catch(Exception ex)
         {
            m_LogResults.Failed(ex);
         }

         return m_LogResults.Success;
      }

      /// <summary>
      /// Send Notification.
      /// </summary>
      /// <param name="notification">notification to send</param>
      /// <returns>true if all is OK</returns>
      public Boolean SendEMail(INotificationInfo notification)
      {
         if (notification == null)
            return false;
         String subject = notification.Alias;
         if (String.IsNullOrEmpty(subject))
         {
            if ((notification.Messages != null) &&
                (notification.Messages.Count > 0))
            {
               subject = notification.Messages[0].Subject;
            }
         }
         return SendEMail(notification.ToHtml(), subject);
      }

      /// <summary>
      /// Send Notifications.
      /// </summary>
      /// <param name="batch">notifications batch</param>
      /// <returns>true if all is ok</returns>
      public Boolean SendEMail(NotificationBatchInfo batch)
      {
         String subject = batch.Subject;
         String notifs = "NOTIFICATION";
         if (String.IsNullOrEmpty(subject))
         {
            subject = notifs;
            if (batch.Recipients.Count > 0)
               subject += "S";
         }
         return SendEMail(batch.ToHtml(), subject, batch.Recipients);
      }

      /// <summary>
      /// Send message base on request to end user using the request user
      /// EMail.
      /// </summary>
      /// <param name="data">instance of RegistrationRequestInfo</param>
      /// <param name="message">(optional) message</param>
      /// <returns>true is returned if all is OK</returns>
      public Boolean SendEMail(
         Helper.RegistrationRequestInfo data, String message, 
         Boolean validateEmail)
      {
         String mess = message;
         if (String.IsNullOrWhiteSpace(mess))
         {
            String appUrl = GetApplicationUri();
            String href = validateEmail ? data.RequestId :
               appUrl + "/requestProcessor/" + data.SessionId + "/" +
                  data.OrganizationId + "/" + data.RequestId + "/" +
                  data.RequestType.ToString();
            if (data.RequestType == SelfHelpRequest.EmailValidationRequest ||
               validateEmail)
               mess = String.Format(GetRequestVerificationText(href, validateEmail));
         }

         if (String.IsNullOrWhiteSpace(mess))
            return true;
         if (!String.IsNullOrEmpty(data.RequestId))
            m_RequestId = data.RequestId;
         if (!String.IsNullOrEmpty(data.Email))
            m_Email = data.Email;
         return SendEMail(mess);
      }

      /// <summary>
      /// Send the password change requested EMail message to end user using
      /// the request user EMail.
      /// </summary>
      /// <returns>true is returned if all is OK</returns>
      public Boolean SendPasswordChangeEmailMessage()
      {
         String mess = GetSelfHelpEmailPasswordChangeText();
         return SendEMail(mess);
      }

      /// <summary>
      /// Send Verification Request.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="requestId">reqeust Id.</param>
      /// <param name="request">request</param>
      /// <param name="messageText">message text</param>
      /// <returns>Request Response is returned, status will provide information
      /// about sent request...</returns>
      public RequestResponseInfo<Boolean> SendVerificationRequest(
         String sessionId, String requestId, SelfHelpRequest request, 
         String messageText = null)
      {
         RequestResponseInfo<Boolean> response =
            new RequestResponseInfo<bool>();
         return response;
      }
      */

      #endregion
      #region -- 4.0 Directory Services Support

      /*
      /// <summary>
      /// Submit new User Registration request to the Directory Services Server.
      /// </summary>
      /// <returns>the Service Result Code is returned (Success if all is OK)
      /// </returns>
      public Edam.Services.ServiceResultCode SubmitDirectoryUserRegistration(
         Helper.RegistrationRequestInfo request)
      {

         // get LDAP / Directory Services key values...
         Edam.Net.Ldap.LdapServiceInfo s =
            Edam.Services.ServicesSession.Configurations.FindLdapService(
               NewUserLdapServerConfigKey);
         String usersCN = Edam.Net.Ldap.
            DirectoryServiceClient.GetDirectoryServiceUsersCN();
         Edam.Services.ServiceResultCode result =
            Edam.Services.ServiceResultCode.Unknown;

         // prepare user CN request ...
         Edam.Net.Ldap.LdapUserInfo u = new Edam.Net.Ldap.LdapUserInfo();

         u.UserTitle = request.FullName;
         u.UserId = request.UserId;
         u.Password = request.PasswordText;
         u.Mobile = request.PhoneNumber;
         u.Mail = request.Email;
         u.Initials = String.Empty;
         u.Guid = String.Empty;
         u.GivenName = request.GivenName;
         u.SurName = request.FatherLastName;
         u.Description = request.FullName;
         u.DisplayName = request.FullName;
         
         String dName = null;
         if (!String.IsNullOrEmpty(u.SurName))
            dName = u.SurName;
         if (!String.IsNullOrEmpty(u.GivenName))
            dName = u.GivenName + (dName == null ? String.Empty : " ") + dName;

         u.DistinguishedName = "CN=" + dName + ", " + usersCN;

         // submit request...
         if ((s != null) && (!String.IsNullOrEmpty(usersCN)))
         {
            Edam.Net.Ldap.DirectoryServiceClient c =
               new Net.Ldap.DirectoryServiceClient(s);
            result = c.CreateUserAccount(usersCN, u);
         }

         return result;
      }
      */
      #endregion

   }

}

