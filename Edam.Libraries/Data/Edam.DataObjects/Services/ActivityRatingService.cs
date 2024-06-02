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

   public class ActivityRatingService
   {

      public static readonly string ACTIVITY_RATING = "ActivityRating";

      #region -- 4.00 - Activity Rating Find

      public static async Task<RequestResponseInfo<ActivityPeriodRatingCodes>>
         GetPeriodRatingRecord(String sessionId, LocaleLanguage? language,
         String organizationId, String periodId, String programId, 
         Int16 referenceGroupTypeNo, String entityId, Int16 option)
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
         b.Add("PeriodId", periodId);
         b.Add("ProgramId", programId);
         b.Add("EntityId", entityId);
         b.Add("ReferenceGroupTypeNo", referenceGroupTypeNo.ToString());
         b.Add(QueryStringTag.Option, option.ToString());

         RequestResponseInfo<ActivityPeriodRatingCodes> data =
            new RequestResponseInfo<ActivityPeriodRatingCodes>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = ACTIVITY_RATING + "/GetActivityRatingPeriod" 
               + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = 
               client.GetDataObjectFromJson<ActivityPeriodRatingCodes>();
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

      #endregion
      #region -- 4.00 - Activity Rating Post

      public static async Task<RequestResponseInfo<String>> PostRating(
         String sessionId, short optionNo, ActivityPeriodRatingInfo ratings)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(ratings.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : 
            ratings.OrganizationId;
         ratings.OrganizationId = oid;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OptionNo, optionNo);

         RequestResponseInfo<String> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = ACTIVITY_RATING + "/PostActivityRatingPeriod" 
               + b.ToString();
            await client.PostAsync<ActivityPeriodRatingInfo>(url, ratings);
            data.ResponseData = client.GetDataObjectFromJson<String>();
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

      #endregion

   }

}
