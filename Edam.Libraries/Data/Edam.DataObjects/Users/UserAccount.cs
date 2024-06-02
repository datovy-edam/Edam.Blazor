using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Security;
using Edam.Diagnostics;
using Edam.DataObjects.SelfHelp;

using UserData = Edam.DataObjects.Entities;
using ReqResp = Edam.DataObjects.Requests;
using DataObj = Edam.DataObjects;
using Requests = Edam.DataObjects.Requests;
using App = Edam.Application;

namespace Edam.DataObjects.Users
{

   /// <summary>
   /// Proivde User Account services while in the Self Help module or others.
   /// </summary>
   public class UserAccount
   {

      #region -- 4.0 Manage API Registration

      /// <summary>
      /// Given an API key details and its new key Value set it up and update
      /// database key registration...
      /// </summary>
      /// <param name="key">key details</param>
      /// <param name="keyValueClearText">key value clear text</param>
      /// <returns>results is returned</returns>
      public static ResultsLog<App.ApiKeyInfo>
         UpdateApiKeyValue(App.ApiKeyInfo key, String keyValueClearText)
      {
         ResultsLog<App.ApiKeyInfo> results =
            new ResultsLog<App.ApiKeyInfo>();
         results.Data = key;
         if (String.IsNullOrEmpty(keyValueClearText))
         {
            results.Failed("Key value clear text must be provided.");
            return results;
         }

         // TODO: update key value in the database...

         return results;
      }

      #endregion
      #region -- 4.0 Manage Registration Token

      /*
      /// <summary>
      /// Get Registration Token.
      /// </summary>
      /// <param name="sessionId">session Id</param>
      /// <param name="deviceId">device Id</param>
      /// <param name="request">user identification details</param>
      /// <returns>request response is returned with registered user data
      /// </returns>
      public static ReqResp.RequestResponseInfo<UserData.IUserBaseInfo>
         GetRegistrationToken(String sessionId, String deviceId,
            RegistrationRequestInfo request)
      {
         ResultLog results = null;
         if (String.IsNullOrWhiteSpace(request.OrganizationId))
            request.OrganizationId =
               Edam.Application.Resources.Strings.DefaultAgencyId;

         try
         {
            results = RegistrationRequestRecord.UpdateRecord(request);
         }
         catch (System.Exception ex)
         {
            results = new ResultLog();
            results.Failed(ex);
         }

         // start preparing response resources...
         ReqResp.RequestResponseInfo<UserData.IUserBaseInfo> response =
            new ReqResp.RequestResponseInfo<UserData.IUserBaseInfo>();

         // verify email
         Edam.Help.Requests.RequestProcessor p =
            new Help.Requests.RequestProcessor(request);
         Boolean emailSent = p.SendEMail(request, null, false);

         // repare response
         response.SessionId = request.SessionId;
         response.RequestId = request.RequestId;

         UserData.UserBaseInfo u = new UserData.UserBaseInfo();
         u.UserId = request.UserId;
         u.GivenName = String.Empty;
         u.FatherLastName = String.Empty;
         u.DisplayName = String.Empty;
         u.Email = request.Email;
         u.PhoneNumber = request.PhoneNumber;
         u.OrganizationId = request.OrganizationId;

         response.ResponseData = u;
         response.Status = emailSent ?
            ReqResp.RequestStatus.Pending : ReqResp.RequestStatus.Failed;

         if (response.Status == ReqResp.RequestStatus.Pending)
         {
            //response.Message = Edam.Application.Resources.
            //   ApplicationStrings.SelfHelpPleaseCheckMailbox;
            // TODO: fix this hardcoded string
            response.Message = "Check Mailbox";
            response.HtmlMessage = true;
            response.Success = true;
         }
         else
         {
            //response.Message = Edam.Application.Resources.
            //   ApplicationStrings.SomethingIsWrongWhileSubmitting;
            // TODO: fix this hardcoded string
            response.Message = "Something Went Wrong While Submitting";
            response.HtmlMessage = false;
            response.Success = true;
         }

         return response;
      }
      */

      #endregion
      #region -- 4.0 Manage User Account

      /*
      /// <summary>
      /// Update User Account.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="requestId">request Id.</param>
      /// <param name="account">account info to update</param>
      /// <param name="request">request type</param>
      /// <param name="status">status type</param>
      /// <param name="hreference">hyperlink request</param>
      /// <returns>request response is returned</returns>
      public static ReqResp.RequestResponseInfo<String>
         UpdateUserAccount(String sessionId, String requestId,
            UserData.UserAccountInfo account,
            SelfHelpRequest request = SelfHelpRequest.NewRegistration,
            Requests.RequestStatus status = ReqResp.RequestStatus.Requested,
            String hreference = "")
      {
         DataObj.SelfHelp.RegistrationRequestInfo r =
            new DataObj.SelfHelp.RegistrationRequestInfo(account);

         r.SessionId = sessionId;
         r.RequestType = request;
         r.RequestStatus = status;
         r.RequestId = requestId;
         r.Email = r.Email;
         r.OrganizationId = account.OrganizationId;

         ResultLog results = null;
         try
         {
            results =
               DataObj.SelfHelp.RegistrationRequestRecord.UpdateRecord(r);

            if (results.Success)
            {
               r.RequestType = SelfHelpRequest.EmailValidationRequest;
               Edam.Help.Requests.RequestProcessor p =
                  new Help.Requests.RequestProcessor(r);
               Boolean emailSent = p.SendEMail(r, null, true);
            }
         }
         catch (Exception ex)
         {
            results = new ResultLog();
            results.Failed(ex);
         }
         ReqResp.RequestResponseInfo<String> response =
            new ReqResp.RequestResponseInfo<String>(results);
         response.ResponseData = results.Success ? r.RequestId : String.Empty;
         response.SessionId = r.SessionId;
         response.RequestId = r.RequestId;
         response.Success = results.Success;
         response.Status = results.Success ? ReqResp.RequestStatus.Completed :
            ReqResp.RequestStatus.Failed;
         return response;
      }
      */

      /// <summary>
      /// User Email was Verified, get its details.
      /// </summary>
      /// <param name="context">application request context</param>
      /// <param name="status">status</param>
      /// <returns>request response is returned</returns>
      public static ReqResp.RequestResponseInfo<
         RegistrationEmailVerifiedInfo> GetUserEmailVerifiedDetails(
            App.ApplicationRequestContext context,
            ReqResp.RequestValidationStatus status)
      {
         ResultsLog<RegistrationEmailVerifiedInfo> results = null;

         try
         {
            results = RegistrationRequestRecord.AccountEmailVerifiedUpdate(
                  context, (Int16)status);
         }
         catch (Exception ex)
         {
            results = new ResultsLog<RegistrationEmailVerifiedInfo>();
            results.Failed(ex);
         }

         ReqResp.RequestResponseInfo<RegistrationEmailVerifiedInfo> response =
            new ReqResp.RequestResponseInfo<
               RegistrationEmailVerifiedInfo>(results);
         return response;
      }

      /*
      /// <summary>
      /// Change Password Requested.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="requestId">request Id.</param>
      /// <param name="organizationId">organization Id.</param>
      /// <param name="email">user Email</param>
      /// <param name="request">status</param>
      /// <param name="password">password</param>
      /// <returns>request response is returned</returns>
      public static ReqResp.RequestResponseInfo<String> ChangePassword(
         App.ApplicationRequestContext context,
         String password, ReqResp.RequestValidationStatus request)
      {
         ResultLog results = null;
         Password pwd = new Password(password);
         String outRequestId = String.Empty;
         Requests.RequestValidationStatus outStatus =
            Requests.RequestValidationStatus.Unknown;

         RegistrationRequestInfo reg =
            new DataObjects.SelfHelp.RegistrationRequestInfo();

         reg.Verified =
            request == Requests.RequestValidationStatus.ValidEmail;
         reg.RequestStatus = Requests.RequestStatus.InProgress;
         reg.RequestType = SelfHelpRequest.PasswordReset;
         reg.Email = context.UserEmail;
         reg.RequestId = context.RequestId;

         try
         {
            results = RegistrationRequestRecord.ChangePasswordUpdate(
               context, String.Empty, pwd, request,
               out outRequestId, out outStatus);

            // verify email
            if ((results.Success) &&
                (request == Requests.RequestValidationStatus.
                   PendingValidation))
            {
               reg.RequestType = SelfHelpRequest.EmailValidationRequest;
               reg.RequestId = outRequestId;
               Edam.Help.Requests.RequestProcessor p =
                  new Help.Requests.RequestProcessor(reg);
               Boolean emailSent = p.SendEMail(reg, null, true);
            }
         }
         catch (Exception ex)
         {
            results = new ResultsLog<RegistrationEmailVerifiedInfo>();
            results.Failed(ex);
         }

         ReqResp.RequestResponseInfo<String> response =
               new ReqResp.RequestResponseInfo<String>(results);
         response.ResponseData = response.Success ?
            Edam.Application.Resources.Strings.Success :
            Edam.Application.Resources.Strings.Failed;
         return response;
      }
      */

      #endregion
      #region -- 4.0 Manage User Loggin / Logout

      public static ReqResp.RequestResponseInfo<
         UserData.UserLoggedInfo> GetLoginRequest(
            String sessionId, String deviceId,
            String organizationId, String userEmail, String password,
            String apiApplicationId, String apiReferenceId, String apiKeyValue)
      {
         String referenceId = String.Empty;

         // get web application context
         App.ApplicationRequestContext c =
            App.ApplicationRequestContext.GetContext(
               sessionId, organizationId, referenceId, deviceId);
         organizationId = String.IsNullOrEmpty(organizationId) ?
            c.OrganizationId : organizationId;
         referenceId = c.ReferenceId;

         // prepare application API key...
         if (c.ApiKey == null)
            c.ApiKey = new Edam.Application.ApiKeyInfo();

         c.DeviceId = deviceId;
         c.ApiKey.OrganizationId = organizationId;
         c.ApiKey.ReferenceId = apiReferenceId;
         c.ApiKey.ApplicationId = apiApplicationId;
         c.ApiKey.ApiKeyValue = apiKeyValue;
         c.ApiKey.EncryptKeyValue();

         Edam.Application.Session.ResetDatabaseSourceToDefault();

         c.SessionId = sessionId;
         c.UserEmail = userEmail;

         // ok, try to login the user and get a valid session Id...
         Edam.Security.Password pwd = new Password(password);
         //password, SecretOption.NotHashed);
         var response = UserAccount.LoginUser(c, pwd);

         return response;
      }

      /// <summary>
      /// Login User.
      /// </summary>
      /// <param name="context">application context</param>
      /// <param name="password">password</param>
      /// <returns>request-response instance is returned</returns>
      public static ReqResp.RequestResponseInfo<UserData.UserLoggedInfo>
         LoginUser(App.ApplicationRequestContext context, Password password)
      {
         ResultsLog<UserData.UserLoggedInfo> results = null;

         try
         {
            context.ApiKey.EncryptKeyValue();
            results = RegistrationRequestRecord.UserLogin(context, password);
         }
         catch (Exception ex)
         {
            if (results == null)
               results = new ResultsLog<UserData.UserLoggedInfo>();
            results.Failed(ex);
         }

         // prepare response
         ReqResp.RequestResponseInfo<UserData.UserLoggedInfo> response =
            new Requests.RequestResponseInfo<UserData.UserLoggedInfo>(
               results);
         if (results.Data != null)
         {
            response.SessionId = results.Data.SessionId;
            response.RequestId = results.Data.RequestId;
         }
         response.ResponseData = results.Data;
         response.Status = (results.Data != null) && (results.Success) ?
            Requests.RequestStatus.Completed : Requests.RequestStatus.Failed;
         return response;
      }

      /// <summary>
      /// Logout User.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="organizationId">organization Id.</param>
      /// <param name="referenceId">reference Id.</param>
      /// <param name="requestId">original login requestrequest Id.</param>
      /// <returns>request-response instance is returned</returns>
      public static ReqResp.RequestResponseInfo<String> LogoutUser(
         String sessionId, String organizationId,
         String referenceId, String requestId)
      {
         ResultLog results = null;
         try
         {
            results = RegistrationRequestRecord.UserLogout(
               sessionId, organizationId, referenceId, requestId);
         }
         catch (Exception ex)
         {
            if (results == null)
               results = new ResultLog();
            results.Failed(ex);
         }

         // prepare response
         ReqResp.RequestResponseInfo<String> response =
            new Requests.RequestResponseInfo<String>(results);
         response.SessionId = sessionId;
         response.RequestId = requestId;
         response.ResponseData = results.Message;
         response.Status = (results.Success) ?
            Requests.RequestStatus.Completed : Requests.RequestStatus.Failed;
         return response;
      }

      #endregion

   }

}
