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
using Edam.DataObjects.References;
using Edam.DataObjects.Services;
using Edam.DataObjects.Dashboards;

namespace Edam.DataObjects.Services
{

   public class ReferenceGroupListService
   {
      public static readonly string REFERENCE_LIST = "ReferenceList";

      #region -- 4.0 - Reference List Item Group Find

      public static async Task<RequestResponseInfo<List<ReferenceListGroupInfo>>>
         GetRecords(
            String sessionId, LocaleLanguage? language, String organizationId,
            String referenceId, ReferenceListGroupType group, Int16 optionNo)
      {
         string rid = (String.IsNullOrEmpty(referenceId)) ? 
            String.Empty : referenceId;
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
         b.Add(QueryStringTag.ReferenceId, rid);
         b.Add(QueryStringTag.GroupNo, ((Int16)group).ToString());
         b.Add(QueryStringTag.OptionNo, optionNo.ToString());

         RequestResponseInfo<List<ReferenceListGroupInfo>> data = 
            new RequestResponseInfo<List<ReferenceListGroupInfo>>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_LIST + "/GetReferenceListItemGroup" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<
               List<ReferenceListGroupInfo>>();
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
      #region -- 4.0 - Reference List Item Group Update

      public static async Task<RequestResponseInfo<EventCode>>
         PostUpdates(String sessionId,
            LocaleLanguage? language, String organizationId, String referenceId,
            ReferenceListGroupType group, Int16 optionNo,
            List<ReferenceListGroupInfo> items)
      {
         string rid = (String.IsNullOrEmpty(referenceId)) ?
            String.Empty : referenceId;
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
         b.Add(QueryStringTag.ReferenceId, rid);
         b.Add(QueryStringTag.GroupNo, ((Int16)group).ToString());
         b.Add(QueryStringTag.OptionNo, optionNo.ToString());

         RequestResponseInfo<EventCode> data = 
            new RequestResponseInfo<EventCode>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_LIST 
               + "/PostReferenceListItemGroup" + b.ToString();
            await client.PostAsync<List<ReferenceListGroupInfo>>(url,items);
            data.ResponseData = client.GetDataObjectFromJson<EventCode>();
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
