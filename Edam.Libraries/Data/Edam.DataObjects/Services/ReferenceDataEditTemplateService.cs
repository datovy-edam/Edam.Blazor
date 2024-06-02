using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Text;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.DataObjects.Models;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Requests;

namespace Edam.DataObjects.Services
{

   /// <summary>
   /// Support for Data Edit Templates...
   /// </summary>
   public class ReferenceDataEditTemplateService
   {
      public const string REFERENCE_DATA = "ReferenceData";
      public const Int32 TEMPLATE_GROUP_ALL = Int16.MaxValue;

      /// <summary>
      /// Get root (parent) ElementInfo and (child) ElementNodeInfoes as a JSON
      /// string.  Use ReferenceDataEditTemplateHelper.ToElementGroup to convert
      /// those into an ElementGroup that can be used to visualize Reference 
      /// Data groups (ElementGroupItem) and children in a TreeView control.
      /// </summary>
      /// <param name="sessionId">(optional) session ID</param>
      /// <param name="language">(optional) language</param>
      /// <param name="organizationId">(optional) organization ID</param>
      /// <param name="optionNo">(optional) option number (default: ALL)</param>
      /// <returns>Returns instance of RequestResponseInfo with inner JSON 
      /// string</returns>
      public static async Task<RequestResponseInfo<string>>
         GetTemplate(
            String sessionId = null, LocaleLanguage? language = null, 
            String organizationId = null, Int16? optionNo = TEMPLATE_GROUP_ALL)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         LocaleLanguage lang = language ?? Edam.Application.Session.Language;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         //b.Add(QueryStringTag.Language, lang.ToString());
         b.Add(QueryStringTag.OrganizationId, oid);

         b.Add(QueryStringTag.Option, 
            (optionNo ?? TEMPLATE_GROUP_ALL).ToString());

         RequestResponseInfo<string> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_DATA + "/GetTemplateJson" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<string>();
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

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="language"></param>
      /// <param name="organizationId"></param>
      /// <param name="templateNo"></param>
      /// <param name="groupNo"></param>
      /// <param name="optionNo"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<string>>
         GetTemplate(Int64? templateNo, Int64? groupNo,
            String sessionId = null, LocaleLanguage? language = null, 
            String organizationId = null, Int16? optionNo = null)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         LocaleLanguage lang = language ?? Edam.Application.Session.Language;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         //b.Add(QueryStringTag.Language, lang.ToString());
         b.Add(QueryStringTag.OrganizationId, oid);

         b.Add(QueryStringTag.TemplateNo, 
            templateNo.HasValue ? templateNo.Value.ToString() : null);
         b.Add(QueryStringTag.GroupNo,
            groupNo.HasValue ? groupNo.Value.ToString() : null);
         b.Add(QueryStringTag.Option, optionNo.HasValue ? 
            optionNo.Value.ToString() : null);

         RequestResponseInfo<string> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_DATA + "/GetTemplateByTemplateNoJson" 
               + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<string>();
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

      private static WebApiClientInfo PrepareReferenceDataClient(
            Int64? templateNo,
            String sessionId = null, LocaleLanguage? language = null,
            String organizationId = null,
            ReferenceDataOption? optionNo = ReferenceDataOption.Unknown)
      {
         WebApiClientInfo cq = new WebApiClientInfo(
            sessionId, language, organizationId);

         cq.Builder.Add(QueryStringTag.TemplateNo,
            templateNo.HasValue ? templateNo.Value.ToString() : null);
         cq.Builder.Add(QueryStringTag.Option,
            optionNo.HasValue ? optionNo.Value.ToString() : null);

         return cq;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="language"></param>
      /// <param name="organizationId"></param>
      /// <param name="templateNo"></param>
      /// <param name="optionNo"></param>
      /// <returns></returns>
      public static RequestResponseInfo<List<string>>
         GetReferenceDataSync(Int64? templateNo,
            String sessionId = null, LocaleLanguage? language = null,
            String organizationId = null,
            ReferenceDataOption? optionNo = ReferenceDataOption.Unknown)
      {
         WebApiClientInfo h = PrepareReferenceDataClient(templateNo,
            sessionId, language, organizationId, optionNo);

         RequestResponseInfo<List<string>> data =
            new RequestResponseInfo<List<string>>();
         ResultLog results = h.Client.Results;
         try
         {
            var url = REFERENCE_DATA + "/GetReferenceDataJson" 
               + h.Builder.ToString();
            h.Client.GetDataAsText(url);
            data.ResponseData = h.Client.GetDataObjectFromJson<List<string>>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            h.Client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            h.Client.Dispose();
         }
         return data;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="language"></param>
      /// <param name="organizationId"></param>
      /// <param name="templateNo"></param>
      /// <param name="optionNo"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<List<string>>>
         GetReferenceData(Int64? templateNo,
            String sessionId = null, LocaleLanguage? language = null, 
            String organizationId = null,
            ReferenceDataOption? optionNo = ReferenceDataOption.Unknown)
      {
         WebApiClientInfo h = PrepareReferenceDataClient(templateNo,
            sessionId, language, organizationId, optionNo);

         RequestResponseInfo<List<string>> data = 
            new RequestResponseInfo<List<string>>();
         ResultLog results = h.Client.Results;
         try
         {
            var url = REFERENCE_DATA + "/GetReferenceDataJson"
               + h.Builder.ToString();
            await h.Client.GetAsync(url);
            data.ResponseData = h.Client.GetDataObjectFromJson<List<string>>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            h.Client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            h.Client.Dispose();
         }
         return data;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="organizationId"></param>
      /// <param name="node"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<string>>
         UpdateElementNode(ElementNodeInfo node,
            String sessionId = null, String organizationId = null)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);

         RequestResponseInfo<string> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_DATA + "/PostReferenceData" + b.ToString();
            await client.PostAsync<ElementNodeInfo>(url, node);
            data.ResponseData = 
               client.GetDataObjectFromJson<string>();
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

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="organizationId"></param>
      /// <param name="status"></param>
      /// <param name="node"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<string>>
         UpdateElementNodeStatus(
            String sessionId, String organizationId, ObjectStatus? status,
            ElementNodeInfo node)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.Status, 
            status.HasValue ? status.Value.ToString() : null);

         RequestResponseInfo<string> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_DATA + "/PostReferenceDataStatus" 
               + b.ToString();
            await client.PostAsync<ElementNodeInfo>(url, node);
            data.ResponseData = client.GetDataObjectFromJson<string>();
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

   }

}

