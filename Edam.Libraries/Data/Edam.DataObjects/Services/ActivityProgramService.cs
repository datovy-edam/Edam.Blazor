using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Text;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.DataObjects.Requests;
using Edam.DataObjects.Activities;
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Services
{

   public class ActivityProgramService
   {

      public static readonly string ACTIVITY_PROGRAM = "ActivityProgram";

      #region -- 4.00 - Activity Program Find

      public static async Task<RequestResponseInfo<List<ActivityProgramInfo>>>
         GetProgramRecord(
            String sessionId, LocaleLanguage? language,
            String organizationId, String entityId, String referenceId,
            String programId, ActivityProgramType programType,
            ObjectStatus status, String alias, String abstractText)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         LocaleLanguage lang = language.HasValue ? language.Value :
            Edam.Application.Session.Language;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         //b.Add(QueryStringTag.Language, lang.ToString());
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.EntityId, entityId);
         b.Add(QueryStringTag.ReferenceId, referenceId);
         b.Add("ProgramId", programId);
         b.Add("ProgramType", programType.ToString());
         b.Add(QueryStringTag.Status, status.ToString());
         b.Add(QueryStringTag.Alias, alias);
         b.Add("AbstractText", abstractText);

         RequestResponseInfo<List<ActivityProgramInfo>> data = 
            new RequestResponseInfo<List<ActivityProgramInfo>>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = ACTIVITY_PROGRAM + "/GetActivityProgram" + b.ToString();
            await client.GetAsync(url);
            data = client.GetDataObjectFromJson<
               RequestResponseInfo<List<ActivityProgramInfo>>>();
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

      public static async Task<RequestResponseInfo<List<ActivityProgramInfo>>>
         GetProgramRecord(
            String sessionId, LocaleLanguage? language, String organizationId)
      {
         return await GetProgramRecord(sessionId, language, organizationId,
            String.Empty, String.Empty, String.Empty,
            ActivityProgramType.Unknown, ObjectStatus.Unknown, String.Empty,
            String.Empty);
      }

      public static async Task<RequestResponseInfo<List<ActivityProgramInfo>>>
         GetProgramRecord()
      {
         return await GetProgramRecord(null, null, null,
            String.Empty, String.Empty, String.Empty,
            ActivityProgramType.Unknown, ObjectStatus.Unknown, String.Empty,
            String.Empty);
      }

      #endregion

   }

}
