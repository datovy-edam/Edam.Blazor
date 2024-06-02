using Edam.Data.AssetManagement;
using Edam.DataObjects.Assets;
using Edam.DataObjects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Data.Asset;
using Edam.DataObjects.ReferenceData;
using Edam.Diagnostics;
using Edam.Net.Web;
using Edam.Text;
using Edam.Data.AssetSchema;

namespace Edam.DataObjects.Services
{

   public delegate Task<RequestResponseInfo<List<CodeSetInfo>>>
      GetCodeSetHandler(string sessionId, string organizationId, 
         string codeSetUri, int optionNo);
   public delegate Task<RequestResponseInfo<string>>
      UpdateCodeSetHandler(string sessionId, CodeSetInfo codeSet, 
         int optionNo);

   public delegate Task<RequestResponseInfo<List<CodeInfo>>>
      GetCodeHandler(string sessionId, string organizationId, long codeSetNo,
         int optionNo);
   public delegate Task<RequestResponseInfo<string>>
      UpdateCodeHandler(string sessionId, CodeInfo code, int optionNo);

   public class DataCodeService
   {
      public const string CODESET_DATA = "DataCode";

      public GetCodeSetHandler GetCodeSetData { get; set; }
      public UpdateCodeSetHandler UpdateCodeSetData { get; set; }
      public GetCodeHandler GetCodeData { get; set; }
      public UpdateCodeHandler UpdateCodeData { get; set; }

      #region -- 4.00 - Code Set (Remote)

      public static async Task<RequestResponseInfo<List<CodeSetInfo>>>
         GetCodeSetDataRemote(string sessionId, string organizationId,
         string codeSetUri, int optionNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add("codeSetUri", codeSetUri);
         b.Add(QueryStringTag.Option, optionNo.ToString());

         RequestResponseInfo<List<CodeSetInfo>> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = CODESET_DATA + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<
               List<CodeSetInfo>>();
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

      public static async Task<RequestResponseInfo<string>>
         UpdateCodeSetDataRemote(
            string sessionId, CodeSetInfo codeSet, int optionNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(codeSet.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : codeSet.OrganizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.Option, optionNo.ToString());

         RequestResponseInfo<string> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = CODESET_DATA + b.ToString();
            await client.PostAsync<CodeSetInfo>(url, codeSet);
            data = client.GetDataObjectFromJson<
               RequestResponseInfo<string>>();
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
      #region -- 4.00 - Code (Remote)

      public static async Task<RequestResponseInfo<List<CodeInfo>>>
         GetCodeDataRemote(string sessionId, string organizationId, 
            long codeSetNo, int optionNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add("codeSetNo", codeSetNo.ToString());
         b.Add(QueryStringTag.Option, optionNo.ToString());

         RequestResponseInfo<List<CodeInfo>> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = CODESET_DATA + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = 
               client.GetDataObjectFromJson<List<CodeInfo>>();
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

      public static async Task<RequestResponseInfo<string>>
         UpdateCodeDataRemote(
            string sessionId, CodeInfo codeSet, int optionNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(codeSet.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : codeSet.OrganizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.Option, optionNo.ToString());

         RequestResponseInfo<string> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = CODESET_DATA + b.ToString();
            await client.PostAsync<CodeInfo>(url, codeSet);
            data = client.GetDataObjectFromJson<
               RequestResponseInfo<string>>();
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
      #region -- 4.00 - Code Set (Local)

      public static async Task<RequestResponseInfo<List<CodeSetInfo>>>
         GetCodeSetDataLocal(
            string sessionId, string organizationId, string codeSetUri,
            int optionNo)
      {
         RequestResponseInfo<List<CodeSetInfo>> response =
            new RequestResponseInfo<List<CodeSetInfo>>();

         var results = await Task.Run(() =>
         {
            ResultsLog<List<CodeSetInfo>> r =
               DataCodeSetRecord.DataCodeSetGet(
                  sessionId, organizationId, codeSetUri, optionNo);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.Data;
         return response;
      }

      public static async Task<RequestResponseInfo<string>>
         UpdateCodeSetDataLocal(string sessionId, CodeSetInfo codeSet,
            int optionNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(codeSet.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : codeSet.OrganizationId;
         String did = (String.IsNullOrWhiteSpace(codeSet.DataOwnerId)) ?
            Edam.Application.Session.OrganizationId : codeSet.DataOwnerId;

         IResultsLog results = await Task.Run(() =>
         {
            IResultsLog r = DataCodeSetRecord.DataCodeSetUpsert(
               sessionId, codeSet, optionNo);
            return r;
         });

         RequestResponseInfo<string> response =
            new RequestResponseInfo<string>();

         response.Results.Copy(results);
         response.ResponseData = results.Success ?
            EventCode.Success.ToString() :
            EventCode.InsertUpdateFailed.ToString();

         return response;
      }

      #endregion
      #region -- 4.00 - Code (Local)

      public static async Task<RequestResponseInfo<List<CodeInfo>>>
         GetCodeDataLocal(
            string sessionId, string organizationId, long codeSetNo,
            int optionNo)
      {
         RequestResponseInfo<List<CodeInfo>> response =
            new RequestResponseInfo<List<CodeInfo>>();

         var results = await Task.Run(() =>
         {
            ResultsLog<List<CodeInfo>> r =
               DataCodeSetRecord.DataCodeGet(
                  sessionId, organizationId, codeSetNo, optionNo);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.Data;
         return response;
      }

      public static async Task<RequestResponseInfo<string>>
         UpdateCodeDataLocal(string sessionId, CodeInfo code, int optionNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(code.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : code.OrganizationId;
         String did = (String.IsNullOrWhiteSpace(code.DataOwnerId)) ?
            Edam.Application.Session.OrganizationId : code.DataOwnerId;

         IResultsLog results = await Task.Run(() =>
         {
            IResultsLog r = DataCodeSetRecord.DataCodeUpsert(
               sessionId, code, optionNo);
            return r;
         });

         RequestResponseInfo<string> response =
            new RequestResponseInfo<string>();

         response.Results.Copy(results);
         response.ResponseData = results.Success ?
            EventCode.Success.ToString() :
            EventCode.InsertUpdateFailed.ToString();

         return response;
      }

      #endregion
      #region -- 4.00 - Reference Data Service (entries)

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
      public static DataCodeService GetService()
      {
         DataCodeService s = new DataCodeService();
         var k = ReferenceDataHelper.GetConnectionStringKey();
         if (String.IsNullOrWhiteSpace(k))
         {
            s.GetCodeSetData = DataCodeService.GetCodeSetDataRemote;
            s.UpdateCodeSetData = DataCodeService.UpdateCodeSetDataRemote;
            s.GetCodeData = DataCodeService.GetCodeDataRemote;
            s.UpdateCodeData = DataCodeService.UpdateCodeDataRemote;
         }
         else
         {
            s.GetCodeSetData = DataCodeService.GetCodeSetDataLocal;
            s.UpdateCodeSetData = DataCodeService.UpdateCodeSetDataLocal;
            s.GetCodeData = DataCodeService.GetCodeDataLocal;
            s.UpdateCodeData = DataCodeService.UpdateCodeDataLocal;
         }
         return s;
      }

      #endregion

   }

}
