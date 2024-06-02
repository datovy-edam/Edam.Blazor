using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.Diagnostics;
using Edam.DataObjects.Requests;
using App = Edam.Application;
using Edam.DataObjects.Entities;
using Edam.Application;

namespace Edam.DataObjects.SelfHelp
{

   /// <summary>
   /// Registration Request details...
   /// </summary>
   public class RegistrationRequestRecord
   {

      /// <summary>
      /// Given stored procedure parameters and their values, create a provider
      /// to allow procedure execution.
      /// <param name="registration">registration info. to update</param>
      /// <returns>returns an instance of Result/s Log</returns>
      public static ResultLog UpdateRecord(
         RegistrationRequestInfo registration)
      {
         ResultLog result = new ResultLog();
         DataProvider p = DataProvider.CreateProcedure(
            "Help.HelpRegisterRequestInsertUpdate");

         String outSessionId = String.Empty;
         String outRequestId = String.Empty;
         Int16 optionNo = 0;

         //// create api-key now, if we where given an Organization Id.
         //if (!String.IsNullOrEmpty(registration.MyOrganizationId))
         //{
         //   String o = Edam.Security.Cryptography.Encryptor.Hash(
         //      registration.MyOrganizationId);
         //   registration.MyApiKey = 
         //      o + Edam.Text.UniqueId.GetUniqueSequentialId();
         //}

         Security.Password pwd =
            new Security.Password(registration.PasswordText);

         if (String.IsNullOrEmpty(registration.RequestId))
            registration.RequestId = String.Empty;
         if (String.IsNullOrEmpty(registration.Name.MiddleName))
            registration.Name.MiddleName = String.Empty;
         if (String.IsNullOrEmpty(registration.Name.FatherSurname))
            registration.Name.FatherSurname = String.Empty;
         if (String.IsNullOrEmpty(registration.Name.MotherSurname))
            registration.Name.MotherSurname = String.Empty;
         if (String.IsNullOrWhiteSpace(registration.OrganizationId))
            registration.OrganizationId =
               Edam.Application.Resources.Strings.DefaultAgencyId;

         p.Params.AddWithValue("@SessionId", registration.SessionId, 40);
         p.Params.AddWithValue("@SessionAlternateId",
            registration.SessionAlternateId, 40);
         p.Params.AddWithValue("@RequestId", registration.RequestId);
         p.Params.AddWithValue("@UserId", registration.UserId, 20);
         p.Params.AddWithValue("@RequestTypeNo", registration.RequestTypeNo);
         p.Params.AddWithValue("@StatusNo", (Int16)registration.RequestStatus);

         p.Params.AddWithValue("@GivenName", registration.Name.GivenName, 20);
         p.Params.AddWithValue("@MiddleName", registration.Name.MiddleName, 20);
         p.Params.AddWithValue("@FatherSurname",
            registration.Name.FatherSurname, 20);
         p.Params.AddWithValue("@MotherSurname",
            registration.Name.MotherSurname, 20);
         p.Params.AddWithValue("@FullName", registration.Name.FullName, 61);
         p.Params.AddWithValue("@BirthDate", registration.BirthDate);
         p.Params.AddWithValue("@OrganizationId",
            registration.OrganizationId, 20);
         p.Params.AddWithValue("@EMail", registration.Email, 128);
         p.Params.AddWithValue("@PhoneNumber", 
            registration.Phone.PhoneNumber, 20);

         p.Params.AddWithValue("@SocialSecurityId", 
            registration.SocialSecurityId, 20);
         p.Params.AddWithValue("@DriverLicenseId",
            registration.DriverLicense.Id, 20);
         p.Params.AddWithValue("@DriverLicenseIdIssuer",
            registration.DriverLicense.IdIssuer, 20);
         p.Params.AddWithValue("@StateProvince",
            registration.StateProvince, 20);
         p.Params.AddWithValue("@PostalCode", registration.PostalCode, 20);

         p.Params.AddWithValue("@Password", pwd.PasswordText, 1024);
         p.Params.AddWithValue("@SecurityQuestion",
            registration.SecurityQuestion, 20);
         p.Params.AddWithValue("@SecurityQuestionAnswer",
            registration.SecurityAnswer, 20);

         p.Params.AddWithValue("@UserHostAddress",
            registration.UserHostAddress, 40);

         p.Params.AddWithValue("@OutSessionId", outSessionId, 20, true);
         p.Params.AddWithValue("@OutRequestId", outRequestId, 20, true);
         p.Params.AddWithValue("@OptionNo", optionNo);

         if (p.Exec())
         {
            result.ReturnValue = p.GetReturnedValue();
            result.Succeeded(result.ReturnValue == 0);
            if (result.Success)
            {
               registration.SessionId =
                  p.GetOutputValue(p.Params.Count - 3) as String;
               registration.RequestId =
                  p.GetOutputValue(p.Params.Count - 2) as String;
            }
         }
         else
         {
            result.ReturnValue = ReturnCode.ExecProcedureFailed;
            result.Failed(EventCode.StoredProcedureCallFailed);
         }
         p.Dispose();

         return result;
      }  // end of UpdateRecord

      /// <summary>
      /// Given a RequestNo, marked it as verified.
      /// <param name="context">application request context</param>
      /// <returns>returns an instance of Result/s Log</returns>
      public static ResultsLog<
         SelfHelp.RegistrationEmailVerifiedInfo> AccountEmailVerifiedUpdate(
            App.ApplicationRequestContext context, Int16 optionNo = 0)
      {
         ResultsLog<SelfHelp.RegistrationEmailVerifiedInfo> result =
            new ResultsLog<SelfHelp.RegistrationEmailVerifiedInfo>();
         result.Data = new SelfHelp.RegistrationEmailVerifiedInfo();

         DataProvider p = DataProvider.CreateProcedure(
            "Help.HelpRequestEMailVerified");

         p.Params.AddWithValue("@SessionId", context.SessionId, 40);
         p.Params.AddWithValue("@SessionAlternateId", 
            context.AlternateSessionId, 40);
         p.Params.AddWithValue("@OrganizationId", context.OrganizationId, 20);
         p.Params.AddWithValue("@Email", context.UserEmail, 128);
         p.Params.AddWithValue("@RequestId", context.RequestId, 20);
         p.Params.AddWithValue("@OptionNo", optionNo);

         if (p.OpenReader())
         {
            result.Data =
               DataReader.GetObject<RegistrationEmailVerifiedInfo>(p.Reader);
            result.Succeeded();
         }
         else
            result.Failed((Exception)null);

         p.Dispose();

         return result;
      }  // end of EMailVerified

      /// <summary>
      /// Given a RequestNo, marked it as verified.
      /// <param name="context">session Id.</param>
      /// <param name="entityId">entity Id.</param>
      /// <param name="newPassword">new password</param>
      /// <param name="status">status of request</param>
      /// <param name="outRequestId">output request Id.</param>
      /// <param name="outStatusNo">output status No.</param>
      /// <returns>returns an instance of Result/s Log</returns>
      public static ResultLog ChangePasswordUpdate(
         App.ApplicationRequestContext context,
         String entityId, Edam.Security.Password newPassword,
         Requests.RequestValidationStatus status, out String outRequestId,
         out Requests.RequestValidationStatus outStatus)
      {
         ResultLog result =
            new ResultLog();

         outRequestId = String.Empty;
         outStatus = Requests.RequestValidationStatus.Unknown;
         Int16 outStatusNo = 0;

         DataProvider p = DataProvider.CreateProcedure(
            "Help.HelpRequestPasswordChangeUpdate");

         p.Params.AddWithValue("@SessionId", context.SessionId, 40);
         p.Params.AddWithValue("@SessionAlternateId",
            context.AlternateSessionId, 40);
         p.Params.AddWithValue("@OrganizationId", context.OrganizationId, 20);
         p.Params.AddWithValue("@EntityId", entityId, 20);
         p.Params.AddWithValue("@Email", context.UserEmail, 128);
         p.Params.AddWithValue("@RequestId", context.RequestId, 20);
         p.Params.AddWithValue("@NewPassword", newPassword.PasswordText, 1024);
         p.Params.AddWithValue("@StatusNo", (Int16)status);

         p.Params.AddWithValue("OutRequestId", outRequestId, 20, true);
         p.Params.AddWithValue("OutStatusNo", outStatusNo);

         if (p.Exec())
         {
            result.ReturnValue = p.GetReturnedValue();
            result.Succeeded(result.ReturnValue == 0);
            if (result.Success)
            {
               outRequestId =
                  p.GetOutputValue(p.Params.Count - 2) as String;
               outStatusNo =
                  p.GetOutputValue(p.Params.Count - 1, (Int16) 0);
               outStatus = (Requests.RequestValidationStatus)outStatusNo;
            }
         }
         else
         {
            result.ReturnValue = ReturnCode.ExecProcedureFailed;
            result.Failed(EventCode.StoredProcedureCallFailed);
         }
         p.Dispose();

         // make sure we do have a Request Id.
         if (outRequestId == null)
         {
            outRequestId = String.Empty;
            result.Failed("Email or record not found with given details.");
         }

         return result;
      }

      /// <summary>
      /// User Logout.
      /// </summary>
      /// <param name="context">application context</param>
      /// <param name="password">password</param>
      /// <returns>result-log is returned</returns>
      public static ResultsLog<
         Entities.UserLoggedInfo> UserLogin(
            App.ApplicationRequestContext context, 
            Edam.Security.Password password)
      {
         Entities.UserLoggedInfo loggedRecord;
         ResultsLog<Entities.UserLoggedInfo> result =
            new ResultsLog<Entities.UserLoggedInfo>();

         context.ApiKey.EncryptKeyValue();

         DataProvider p = DataProvider.CreateProcedure(
            "Help.HelpRequestLoginUser");

         p.Params.AddWithValue("@SessionId", context.SessionId, 40);
         p.Params.AddWithValue("@AlternateSessionId",
            context.AlternateSessionId, 40);
         p.Params.AddWithValue("@OrganizationId", context.OrganizationId, 20);
         p.Params.AddWithValue("@UserHostAddress", context.UserHostAddress, 40);
         p.Params.AddWithValue("@Email", context.UserEmail, 128);
         p.Params.AddWithValue("@SecretMessage", password.PasswordText, 1024);
         p.Params.AddWithValue("@ApiApplicationId",
            context.ApiKey.ApplicationId, 20);
         p.Params.AddWithValue("@DeviceId", context.DeviceId, 60);
         p.Params.AddWithValue("@ApiAgentId", context.ApiKey.ReferenceId, 20);
         p.Params.AddWithValue("@ApiKeyValue", context.ApiKey.ApiKeyValue, 128);

         if (p.OpenReader())
         {
            loggedRecord =
               DataReader.GetObject<Entities.UserLoggedInfo>(p.Reader);
            while (p.Reader.Read()) ;
            if (p.Reader.NextResult())
               loggedRecord.Organizations =
                  DataReader.GetList<Entities.OrganizationInfo>(p.Reader);
            while (p.Reader.Read()) ;
            if (p.Reader.NextResult())
               loggedRecord.Policies =
                  DataReader.GetList<App.BasePolicyInfo>(p.Reader);
            if (p.Reader.NextResult())
            {
               loggedRecord.PreferencesBag =
                  DataReader.GetObject<References.PreferencesBag>(p.Reader);
               while (p.Reader.Read()) ;
            }
            p.Reader.NextResult();
            while (p.Reader.Read()) ;
            result.Data = loggedRecord;

            result.ReturnValue = p.GetReturnedValue();
            result.Succeeded(result.ReturnValue == 0);
         }
         else
         {
            result.ReturnValue = ReturnCode.ExecProcedureFailed;
            result.Failed(EventCode.StoredProcedureCallFailed);
         }
         p.Dispose();

         return result;
      }

      /// <summary>
      /// This is an application local.  This version only supports one login
      /// account for the application.  Therefore if the user have never login
      /// it will create an account using given context.
      /// </summary>
      /// <param name="context"></param>
      /// <param name="password"></param>
      /// <returns></returns>
      public static async Task<ResultsLog<UserLoggedInfo>> UserLoginApplicationLocal(
            App.ApplicationRequestContext context,
            Edam.Security.Password password)
      {
         ResultsLog<UserLoggedInfo> result = new ResultsLog<UserLoggedInfo>();
         var doc = await Data.LocalDocumentStorageHelper.
            GetItem<Documents.DataDocumentItem, UserLoggedInfo>("UserInfo");

         if (doc == null)
         {
            UserLoggedInfo u = new UserLoggedInfo();
            u.Alias = Environment.UserName;
            u.Status = UserLoggedStatus.Active;
            u.SessionId = Session.SessionId;

            u.Policies.Add(
               BasePolicyInfo.GetPolicy(PolicyType.CanUseApplication));
            u.Policies.Add(
               BasePolicyInfo.GetPolicy(PolicyType.CanAdminApplication));
            u.Policies.Add(
               BasePolicyInfo.GetPolicy(PolicyType.CanManageOrganization));
            u.Policies.Add(
               BasePolicyInfo.GetPolicy(PolicyType.CanManageSelfData));

            u.IsActive = true;
            u.Email = context.UserEmail;
            u.OrganizationId = context.OrganizationId;
            u.RequestId = Guid.NewGuid().ToString();
            result.Data = u;
            result.Succeeded();

            var uresult = Data.LocalDocumentStorageHelper.
               SaveItem<Documents.DataDocumentItem, UserLoggedInfo>(
                  "UserInfo", u);
         }
         else
         {
            result.Data = doc;
            result.Succeeded();
         }

         // setup session information
         Session.OrganizationId = result.Data.OrganizationId;

         return result;
      }

      /// <summary>
      /// User Logout.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="organizationId">Organization Id.</param>
      /// <param name="referenceId">reference Id.</param>
      /// <param name="requestId">request Id.</param>
      /// <returns>result-log is returned</returns>
      public static ResultLog UserLogout(
         String sessionId,   String organizationId,
         String referenceId, String requestId)
      {
         ResultLog result = new ResultLog();

         DataProvider p = DataProvider.CreateProcedure(
            "Help.HelpRequestLogoutUser");

         p.Params.AddWithValue("@SessionId", sessionId, 40);
         p.Params.AddWithValue("@OrganizationId", organizationId, 20);
         p.Params.AddWithValue("@ReferenceId", referenceId, 20);
         p.Params.AddWithValue("@RequestId", requestId, 20);

         if (p.Exec())
         {
            result.ReturnValue = p.GetReturnedValue();
            result.Succeeded(result.ReturnValue == 0);
         }
         else
         {
            result.ReturnValue = ReturnCode.ExecProcedureFailed;
            result.Failed(EventCode.StoredProcedureCallFailed);
         }
         p.Dispose();

         return result;
      }

      /// <summary>
      /// Get Registration Id's Codes
      /// </summary>
      /// <param name="sessionId">session Id</param>
      /// <param name="language">language No</param>
      /// <param name="groupNo">group No</param>
      /// <returns>Registration Ids codes is returned</returns>
      public static ResultsLog<RegistrationIdsCodes> 
         GetRegistrationIdsCodes(
            String sessionId, Edam.LocaleLanguage language, Int16 groupNo)
      {
         ResultsLog<RegistrationIdsCodes> results =
            new Diagnostics.ResultsLog<RegistrationIdsCodes>();
         results.Data = new RegistrationIdsCodes();
         DataProvider p = null;

         try
         {
            p = DataProvider.CreateProcedure(
               "Help.HelpRegistrationIdsCodesGet");

            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@LanguageNo", (Int16)language);
            p.Params.AddWithValue("@GroupNo", groupNo);
            if (p.OpenReader())
            {
               results.Data.LocationCountries =
                  DataReader.GetList<DataCodes.DataCodeInfo>(p.Reader);
               p.Reader.NextResult();
               results.Data.LocationStates =
                  DataReader.GetList<Locations.LocationStateInfo>(p.Reader);
               p.Reader.NextResult();
               results.Data.Organizations =
                  DataReader.GetList<DataCodes.DataCodeInfo>(p.Reader);
               p.Reader.NextResult();
               results.Data.Applications =
                  DataReader.GetList<DataCodes.DataCodeInfo>(p.Reader);
               p.Reader.NextResult();

               results.ReturnValue = p.GetReturnedValue();
               results.Succeeded(results.ReturnValue == 0);
            }
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return results;
      }

      /// <summary>
      /// Get Media Codes Response.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="language">language</param>
      /// <param name="organizationId">organization Id</param>
      /// <param name="referenceId">reference Id</param>
      /// <param name="groupNo">group No</param>
      /// <param name="optionNo">option No</param>
      /// <returns>Request Response is returned with the MediaCodes</returns>
      public static Requests.RequestResponseInfo<RegistrationIdsCodes> 
         GetRegistrationIdsCodesResponse(
            String sessionId, Edam.LocaleLanguage language, Int16 groupNo)
      {
         ResultsLog<RegistrationIdsCodes> result = 
            GetRegistrationIdsCodes(sessionId, language,  groupNo);
         Requests.RequestResponseInfo<RegistrationIdsCodes> response =
            new Requests.RequestResponseInfo<RegistrationIdsCodes>(result);
         response.ResponseData = result.Success ? result.Data : null;
         response.Status = result.Success ?
            Requests.RequestStatus.Completed : Requests.RequestStatus.Failed;
         response.Success = result.Success;
         return response;
      }

      /// <summary>
      /// Update Request Status.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="organizationId">organization Id.</param>
      /// <param name="requestId">request Id.</param>
      /// <param name="status">status</param>
      /// <returns>result-log is returned</returns>
      public static ResultLog UpdateStatus(
         String sessionId, String organizationId, String requestId, 
         Requests.RequestStatus status)
      {
         ResultLog result =
            new ResultLog();

         DataProvider p = null;

         try
         {
            p = DataProvider.CreateProcedure("Help.HelpRequestChangeStatus");

            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@OrganizationId", organizationId, 20);
            p.Params.AddWithValue("@RequestId", requestId, 20);
            p.Params.AddWithValue("@StatusNo", (Int16)status);

            if (p.Exec())
            {
               result.ReturnValue = p.GetReturnedValue();
               result.Succeeded(result.ReturnValue == 0);
            }
            else
            {
               result.ReturnValue = ReturnCode.ExecProcedureFailed;
               result.Failed(EventCode.StoredProcedureCallFailed);
            }
         }
         catch(Exception ex)
         {
            result.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return result;
      }

      /// <summary>
      /// Update Request Status.
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="organizationId">organization Id.</param>
      /// <param name="requestId">request Id.</param>
      /// <param name="status">status</param>
      /// <returns>result-log is returned</returns>
      public static Requests.RequestResponseInfo<String>
         UpdateStatusRequest(
            String sessionId, String organizationId, String requestId, 
            Requests.RequestStatus status)
      {
         var result = UpdateStatus(
            sessionId, organizationId, requestId, status);
         Requests.RequestResponseInfo<String> response =
            new Requests.RequestResponseInfo<String>(result);
         response.ResponseData = result.Success ?
            Edam.Application.Resources.Strings.Success :
            Edam.Application.Resources.Strings.Failed;
         return response;
      }

      /// <summary>
      /// Validate Request.
      /// </summary>
      /// <param name="sessionId">session ID</param>
      /// <param name="organizationId">organization ID</param>
      /// <param name="requestId">request ID</param>
      /// <param name="request">request ID</param>
      /// <returns>true is returned if done</returns>
      public static RequestResponseInfo<Boolean> ValidateRequestResponse(
         String sessionId, String organizationId, String requestId, 
         SelfHelpRequest request)
      {
         RequestStatus status =
            request == SelfHelpRequest.EmailValidationRequest ?
               RequestStatus.Validated : RequestStatus.Unknown;
         ResultLog results = null;
         if (status != RequestStatus.Unknown)
         {
            results = UpdateStatus(sessionId, organizationId, requestId,
               status);
         }
         else
         {
            results = new ResultLog();
            //String m = Edam.Application.Resources.Strings.self
            //   ApplicationStrings.SelfHelpVerificationFailed +
            //   Edam.Application.Resources.
            //      ApplicationStrings.SelfHelpUnknownRequestType;
            results.Failed(EventCode.Failed);
         }
         RequestResponseInfo<Boolean> response = 
            new RequestResponseInfo<bool>(results);
         response.ResponseData = results.Success;
         return response;
      }

      /// <summary>
      /// Insert Web-Comment...
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="request">web comment request</param>
      /// <returns>result-log is returned</returns>
      public static ResultLog InsertComment(
         String sessionId, RequestWebComment request)
      {
         ResultLog result =
            new ResultLog();

         DataProvider p = null;
         String outSessId = String.Empty;
         String outRequestId = String.Empty;

         if (String.IsNullOrWhiteSpace(request.Name))
            request.Name = PersonName.GetFullName(
               request.GivenName, String.Empty,
               request.Surname1, request.Surname2);

         try
         {
            p = DataProvider.CreateProcedure(
               "Help.HelpRequestWebCommentInsert");

            p.Params.AddWithValue("@SessionId", sessionId, 40);
            p.Params.AddWithValue("@OrganizationId",
               request.OrganizationId, 20);
            p.Params.AddWithValue("@GivenName", request.GivenName, 20);
            p.Params.AddWithValue("@Surname1", request.Surname1, 20);
            p.Params.AddWithValue("@Surname2", request.Surname2, 20);
            p.Params.AddWithValue("@Name", request.Name, 20);
            p.Params.AddWithValue("@Email", request.Email, 128);
            p.Params.AddWithValue("@Phone", request.Phone, 20);
            p.Params.AddWithValue("@PostalCode", request.PostalCode, 20);
            p.Params.AddWithValue("@MessageData",
               request.Comments, request.Comments.Length);
            p.Params.AddWithValue("@OutSessionId", outSessId, 20, true);
            p.Params.AddWithValue("@OutRequestId", outRequestId, 20, true);

            if (p.Exec())
            {
               result.ReturnValue = p.GetReturnedValue();
               result.Succeeded(result.ReturnValue == 0);
               if (result.Success)
               {
                  request.RequestId = outRequestId;
               }
            }
            else
            {
               result.ReturnValue = ReturnCode.ExecProcedureFailed;
               result.Failed(EventCode.StoredProcedureCallFailed);
            }
         }
         catch (Exception ex)
         {
            result.Failed(ex);
         }
         finally
         {
            if (p != null)
               p.Dispose();
         }

         return result;
      }

      /// <summary>
      /// Insert Web-Comment...
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="request">web comment request</param>
      /// <returns>request-response is returned</returns>
      public static Requests.RequestResponseInfo<String>
         InsertCommentRequest(String sessionId, RequestWebComment request)
      {
         var result = InsertComment(sessionId, request);
         Requests.RequestResponseInfo<String> response =
            new Requests.RequestResponseInfo<String>(result);
         response.ResponseData = result.Success ?
            request.RequestId : String.Empty;
         return response;
      }

   }

}

