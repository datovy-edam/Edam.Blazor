using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Text;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.DataObjects.Entities;
using Edam.DataObjects.Requests;
using Edam.DataObjects.Services;
using Edam.DataObjects.Dashboards;

namespace Edam.DataObjects.Services
{

   public class PersonService
   {

      public static readonly string PERSON = "Person";

      #region -- 4.00 - Person Find

      public static async Task<RequestResponseInfo<List<PersonInfo>>>
         GetPersonRecord(
            String sessionId, LocaleLanguage? language,
            String organizationId, String entityId, String givenName,
            String middleName, String fatherLastName, String motherLastName,
            String fullName, String socialSecurityId, String driverLicenseId,
            String email, String phoneNumber, DateTime? birthDate,
            References.ReferenceListGroup option =
               References.ReferenceListGroup.NewProspect)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         LocaleLanguage lang = language.HasValue ? language.Value :
            Edam.Application.Session.Language;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.Language, lang.ToString());
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.EntityId, entityId);
         b.Add(QueryStringTag.GivenName, givenName);
         b.Add(QueryStringTag.MiddleName, middleName);
         b.Add(QueryStringTag.FatherLastName, fatherLastName);
         b.Add(QueryStringTag.MotherLastName, motherLastName);
         b.Add(QueryStringTag.FullName, fullName);
         b.Add(QueryStringTag.SocialSecurityId, socialSecurityId);
         b.Add(QueryStringTag.DriverLicenseId, driverLicenseId);
         b.Add(QueryStringTag.Email, email);
         b.Add(QueryStringTag.PhoneNumber, phoneNumber);
         b.Add(QueryStringTag.BirthDate, birthDate);
         b.Add(QueryStringTag.Option, option.ToString());

         RequestResponseInfo<List<PersonInfo>> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = PERSON + b.ToString();
            await client.GetAsync(url);
            data = client.GetDataObjectFromJson<
               RequestResponseInfo<List<PersonInfo>>>();
            results.Succeeded();
            if (results.Success)
            {
               
            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      public static async Task<RequestResponseInfo<List<PersonInfo>>>
         GetPersonRecord(String sessionId, LocaleLanguage? language,
            PersonInfo person, References.ReferenceListGroup option =
               References.ReferenceListGroup.NewProspect)
      {
         return await GetPersonRecord(sessionId, language, person.OrganizationId,
            person.EntityId, person.Name.GivenName, person.Name.MiddleName,
            person.Name.FatherSurname, person.Name.MotherSurname, 
            person.Description,
            person.AlternateId, person.DriverLicense.Id, person.Email,
            person.Phone.PhoneNumber, person.BirthDate, option);
      }

      #endregion
      #region -- 4.00 - Person Post

      public static async Task<RequestResponseInfo<string>> PostRecord(
         String sessionId, String requestId, PersonInfo person)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(person.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : person.OrganizationId;
         person.OrganizationId = oid;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.RequestId, requestId);

         RequestResponseInfo<string> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = PERSON + b.ToString();
            await client.PostAsync<PersonInfo>(url, person);
            data = client.GetDataObjectFromJson<RequestResponseInfo<string>>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      #endregion
      #region -- 4.00 - Person Delete

      public static async Task<RequestResponseInfo<string>> DeleteRecord(
         String sessionId, String organizationId, String referenceId,
         Edam.DataObjects.Objects.ObjectStatus status)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.ReferenceId, referenceId);
         b.Add(QueryStringTag.Status, (Int16)status);

         RequestResponseInfo<string> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = PERSON + b.ToString();
            await client.PostAsync(url);
            data = client.GetDataObjectFromJson<RequestResponseInfo<string>>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      #endregion

   }

}
