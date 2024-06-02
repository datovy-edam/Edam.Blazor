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

namespace Edam.DataObjects.Services
{

   public class ActivityContentService
   {

      public static readonly string ACTIVITY_CONTENT = "ActivityContent";

      #region -- 4.00 - Activity Content Find

      public static async Task<RequestResponseInfo<List<ActivityContentInfo>>>
         GetContentRecord(
            String sessionId, LocaleLanguage? language,
            String organizationId, String programId, 
            ActivityContentTreeOption option)
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
         b.Add("ProgramId", programId);

         RequestResponseInfo<List<ActivityContentInfo>> data = 
            new RequestResponseInfo<List<ActivityContentInfo>>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = ACTIVITY_CONTENT + "/GetActivityContent" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<
               List<ActivityContentInfo>>();
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

      public static async Task<RequestResponseInfo<List<ActivityContentInfo>>>
         GetContentRecord(string programID)
      {
         return await GetContentRecord(null, null, null,
            programID, ActivityContentTreeOption.Unknown);
      }

      #endregion

   }

}
