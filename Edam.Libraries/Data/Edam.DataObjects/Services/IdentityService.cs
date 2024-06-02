 using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Text;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.DataObjects.Entities;
using Edam.DataObjects.Requests;
using Edam.DataObjects.SelfHelp;
using Edam.DataObjects.Users;
using Edam.Application;
using App = Edam.Application;

namespace Edam.DataObjects.Services
{

   public delegate Task<RequestResponseInfo<UserLoggedInfo>>
         GetLoginRequestHandler(
            //[FromUri]
            String sessionId, String organizationId, String userEmail,
            String password, String deviceId = null,
            String apiApplicationId = null, String apiReferenceId = null,
            String apiKeyValue = null);
   public delegate Task<RequestResponseInfo<String>>
         GetLogoutRequestHandler(
            //[FromUri]
            String sessionId, String requestId,
            String organizationId, String referenceId);
   public delegate Task<RequestResponseInfo<String>>
         GetPasswordChangeVerificationHandler(
            //[FromUri]
            String sessionId, String requestId, String deviceId,
            String organizationId, String email, String password,
            RequestValidationStatus request);

   public class IdentityService
   {

      /// <summary>
      /// (remote) Service Name
      /// </summary>
      public const string USER_ACCOUNT = "UserAccount";

      public GetLoginRequestHandler GetLoginRequest { get; set; }
      public GetLogoutRequestHandler GetLogoutRequest { get; set; }
      public GetPasswordChangeVerificationHandler
         GetPasswordChangeVerification { get; set; }

      #region -- 4.0 - Login and Logout (Remote)

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="organizationId"></param>
      /// <param name="userEmail"></param>
      /// <param name="password"></param>
      /// <param name="deviceId"></param>
      /// <param name="apiApplicationId"></param>
      /// <param name="apiReferenceId"></param>
      /// <param name="apiKeyValue"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<UserLoggedInfo>>
         GetLoginRequestRemote(
            //[FromUri]
            String sessionId, String organizationId, String userEmail,
            String password, String deviceId = null,
            String apiApplicationId = null, String apiReferenceId = null,
            String apiKeyValue = null)
      {
         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sessionId);
         b.Add(QueryStringTag.DeviceId, deviceId);
         b.Add(QueryStringTag.OrganizationId, organizationId);
         b.Add(QueryStringTag.UserEmail, userEmail);
         b.Add(QueryStringTag.Password, password);
         b.Add(QueryStringTag.ApiApplicationId, apiApplicationId);
         b.Add(QueryStringTag.ApiReferenceId, apiReferenceId);
         b.Add(QueryStringTag.ApiKeyValue, apiKeyValue);

         RequestResponseInfo<UserLoggedInfo> data = 
            new RequestResponseInfo<UserLoggedInfo>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = USER_ACCOUNT + "/GetUserSession" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<UserLoggedInfo>();
            results.Succeeded();
            if (results.Success)
            {
               Edam.Application.Session.SetUser(data.ResponseData);
            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="requestId"></param>
      /// <param name="organizationId"></param>
      /// <param name="referenceId"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<String>>
         GetLogoutRequestRemote(
            //[FromUri]
            String sessionId, String requestId,
            String organizationId, String referenceId)
      {
         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sessionId);
         b.Add(QueryStringTag.RequestId, requestId);
         b.Add(QueryStringTag.OrganizationId, organizationId);
         b.Add(QueryStringTag.ReferenceId, referenceId);

         RequestResponseInfo<String> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = USER_ACCOUNT + "/GetUserSessionClose" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<String>();
            results.Succeeded();
            if (results.Success)
            {
               Edam.Application.Session.LogoutUser();
               results.Succeeded();
               results.ResultValueObject = results.Success ?
                  Edam.Application.Resources.Strings.Success :
                  Edam.Application.Resources.Strings.Failed;
            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      #endregion
      #region -- 4.0 - User Password Change (Remote)

      public static async Task<RequestResponseInfo<String>>
         GetPasswordChangeVerificationRemote(
            //[FromUri]
            String sessionId, String requestId, String deviceId,
            String organizationId, String email, String password,
            RequestValidationStatus request)
      {
         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sessionId);
         b.Add(QueryStringTag.RequestId, requestId);
         b.Add(QueryStringTag.DeviceId, deviceId);
         b.Add(QueryStringTag.OrganizationId, organizationId);
         b.Add(QueryStringTag.Email, email);
         b.Add(QueryStringTag.Password, password);
         b.Add(QueryStringTag.Request, ((Int16)request).ToString());

         RequestResponseInfo<String> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = USER_ACCOUNT + "/GetChangeVerification" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<String>();
            results.Succeeded();
            if (results.Success)
            {
               Edam.Application.Session.LogoutUser();
            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      #endregion
      #region -- 4.0 - Get User Alerts Remote

      public static async Task<RequestResponseInfo<UserAlertInfo>>
         GetUserAlertInfo(
            String sessionId, Edam.LocaleLanguage language,
            String organizationId, Int16 option)
      {
         sessionId = Edam.Convert.ToNotNullString(sessionId);
         organizationId = Edam.Convert.ToNotNullString(organizationId);

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sessionId);
         //b.Add(QueryStringTag.Language, language.ToString());
         b.Add(QueryStringTag.OrganizationId, organizationId);
         b.Add(QueryStringTag.Option, option.ToString());

         RequestResponseInfo<UserAlertInfo> data = 
            new RequestResponseInfo<UserAlertInfo>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = USER_ACCOUNT + "/GetUserAlerts" + b.ToString();
            await client.GetAsync(url);
            data = client.GetDataObjectFromJson<
               RequestResponseInfo<UserAlertInfo>>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }

         return data;
      }

      public static async Task<RequestResponseInfo<UserAlertInfo>>
         GetUserAlertInfo(Int16 option = 0)
      {
         return await GetUserAlertInfo(Session.SessionId, Session.Language,
            Session.OrganizationId, option);
      }

      #endregion
      #region -- 4.0 - Login and Logout (Local)

      public static async Task<RequestResponseInfo<UserLoggedInfo>>
         GetLoginRequestLocal(
            //[FromUri]
            String sessionId, String organizationId, String userEmail,
            String password, String deviceId = null,
            String apiApplicationId = null, String apiReferenceId = null,
            String apiKeyValue = null)
      {
         RequestResponseInfo<UserLoggedInfo> response =
            new RequestResponseInfo<UserLoggedInfo>();

         App.ApplicationRequestContext context =
            App.ApplicationRequestContext.GetContext(sessionId, organizationId,
               String.Empty, deviceId);
         context.UserEmail = userEmail;

         var results = await Task.Run(() =>
         {
            Edam.Security.Password pw = new Security.Password(password);
            ResultsLog<UserLoggedInfo> r =
               RegistrationRequestRecord.UserLogin(context, pw);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.Data;
         return response;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="requestId"></param>
      /// <param name="organizationId"></param>
      /// <param name="referenceId"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<String>>
         GetLogoutRequestLocal(
            //[FromUri]
            String sessionId, String requestId,
            String organizationId, String referenceId)
      {
         RequestResponseInfo<String> response =
            new RequestResponseInfo<String>();

         var results = await Task.Run(() =>
         {
            ResultLog r =
               RegistrationRequestRecord.UserLogout(sessionId, organizationId,
                  referenceId, requestId);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.ReturnText;
         return response;
      }

      #endregion
      #region -- 4.0 - User Password Change (Local)

      public static async Task<RequestResponseInfo<String>>
         GetPasswordChangeVerificationLocal(
            //[FromUri]
            String sessionId, String requestId, String deviceId,
            String organizationId, String email, String password,
            RequestValidationStatus request)
      {
         RequestResponseInfo<String> response =
            new RequestResponseInfo<String>();

         App.ApplicationRequestContext context =
            App.ApplicationRequestContext.GetContext(sessionId, organizationId,
               String.Empty, deviceId);

         var results = await Task.Run(() =>
         {
            Edam.Security.Password pw = new Security.Password(password);
            ResultLog r =
               RegistrationRequestRecord.ChangePasswordUpdate(context,
                  String.Empty, pw, request, out string outRequestId,
                  out RequestValidationStatus outStatus);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.ReturnText;
         return response;
      }

      #endregion
      #region -- 4.0 - Login and Logout (Application Local)

      public static async Task<RequestResponseInfo<UserLoggedInfo>>
         GetLoginRequestApplicationLocal(
            //[FromUri]
            String sessionId, String organizationId, String userEmail,
            String password, String deviceId = null,
            String apiApplicationId = null, String apiReferenceId = null,
            String apiKeyValue = null)
      {
         RequestResponseInfo<UserLoggedInfo> response =
            new RequestResponseInfo<UserLoggedInfo>();

         App.ApplicationRequestContext context =
            App.ApplicationRequestContext.GetContext(sessionId, organizationId,
               String.Empty, deviceId);
         context.UserEmail = userEmail;

         var results = await Task.Run(() =>
         {
            Edam.Security.Password pw = new Security.Password(password);
            var r =
               RegistrationRequestRecord.UserLoginApplicationLocal(context, pw);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.Data;
         return response;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="requestId"></param>
      /// <param name="organizationId"></param>
      /// <param name="referenceId"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<String>>
         GetLogoutRequestApplicationLocal(
            //[FromUri]
            String sessionId, String requestId,
            String organizationId, String referenceId)
      {
         RequestResponseInfo<String> response =
            new RequestResponseInfo<String>();

         var results = await Task.Run(() =>
         {
            ResultLog r =
               RegistrationRequestRecord.UserLogout(sessionId, organizationId,
                  referenceId, requestId);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.ReturnText;
         return response;
      }

      #endregion
      #region -- 4.0 - User Password Change (Application Local)

      public static async Task<RequestResponseInfo<String>>
         GetPasswordChangeVerificationApplicationLocal(
            //[FromUri]
            String sessionId, String requestId, String deviceId,
            String organizationId, String email, String password,
            RequestValidationStatus request)
      {
         RequestResponseInfo<String> response =
            new RequestResponseInfo<String>();

         App.ApplicationRequestContext context =
            App.ApplicationRequestContext.GetContext(sessionId, organizationId,
               String.Empty, deviceId);

         // TODO: implement support for identity local scope
         var results = await Task.Run(() =>
         {
            Edam.Security.Password pw = new Security.Password(password);
            ResultLog r =
               RegistrationRequestRecord.ChangePasswordUpdate(context,
                  String.Empty, pw, request, out string outRequestId,
                  out RequestValidationStatus outStatus);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.ReturnText;
         return response;
      }

      #endregion
      #region -- 4.0 - Reference Data Service (entries)

      /// <summary>
      /// Get delegate based data service handlers based on app configuration.
      /// </summary>
      /// <remarks>
      /// If the application-settings define the 
      /// "ReferenceData.ConnectionStringKey" (key) then it is assumed that 
      /// local DB support is available, else the configured remote service
      /// will be used.
      /// </remarks>
      /// <returns></returns>
      public static IdentityService GetService()
      {

         IdentityService s = new IdentityService();
         var iscope = App.AppSettings.GetSectionString(
            App.ApplicationHelper.IDENTITY_SCOPE);

         if (!String.IsNullOrWhiteSpace(iscope) &&
            iscope == App.ApplicationHelper.IDENTITY_SCOPE_LOCAL)
         {
            s.GetLoginRequest = GetLoginRequestApplicationLocal;
            s.GetLogoutRequest = GetLogoutRequestApplicationLocal;
            s.GetPasswordChangeVerification =
               IdentityService.GetPasswordChangeVerificationApplicationLocal;
            return s;
         }

         var k = App.AppSettings.GetSectionString(
            App.ApplicationHelper.IDENTITY_CONNECTION_KEY);

         if (String.IsNullOrWhiteSpace(k))
         {
            s.GetLoginRequest = IdentityService.GetLoginRequestLocal;
            s.GetLogoutRequest = IdentityService.GetLogoutRequestLocal;
            s.GetPasswordChangeVerification =
               IdentityService.GetPasswordChangeVerificationLocal;
         }
         else
         {
            s.GetLoginRequest = IdentityService.GetLoginRequestRemote;
            s.GetLogoutRequest = IdentityService.GetLogoutRequestRemote;
            s.GetPasswordChangeVerification =
               IdentityService.GetPasswordChangeVerificationRemote;
         }
         return s;
      }

      #endregion

   }

}
