using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam;
using Edam.Text;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;
using Edam.DataObjects.Assets;
using Edam.DataObjects.Requests;
using Edam.DataObjects.ReferenceData;
using Edam.Serialization;
using Edam.Application;

namespace Edam.DataObjects.Services
{

   public delegate Task<RequestResponseInfo<DataReferenceFetchResult>>
      GetElementHandler(
         String sessionId, LocaleLanguage? language, String organizationId,
         string root, string elementName, DataReferenceOption? option);
   public delegate Task<RequestResponseInfo<string>>
      UpdateElementHandler(string sessionId,
         string organizationId, string namespacePrefix, string domainUri,
         string domainName, AssetDataElement element, string batchId,
         int assetTypeNo);

   public delegate Task<RequestResponseInfo<DataReferenceFetchResult>>
      GetDomainHandler(
         string sessionId, string organizationId, string domainUri);
   public delegate Task<RequestResponseInfo<string>>
      UpdateDomainHandler(string sessionId, string organizationId, 
         DataDomain domain);

   public delegate Task<RequestResponseInfo<string>>
      UpdateBatchHandler(string sessionId, string organizationId,
         string dataOwnerId, string batchId, string domainUri, string versionId,
         string groupId);

   public class ReferenceDataService
   {

      /// <summary>
      /// (remote) Service Name
      /// </summary>
      public const string REFERENCE_DATA = "ReferenceData";

      public GetElementHandler GetElementData { get; set; }
      public UpdateElementHandler UpdateElementData { get; set; }
      public GetDomainHandler GetDomainData { get; set; }
      public UpdateDomainHandler UpdateDomainData { get; set; }
      public UpdateBatchHandler UpdateBatchData { get; set; }

      #region -- 4.00 - Get All (Remote)

      public static async Task<RequestResponseInfo<DataReferenceFetchResult>>
         GetElementDataRemote(
            String sessionId, LocaleLanguage? language, String organizationId,
            string root, string elementName, DataReferenceOption? option)
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
         b.Add("root", root);
         b.Add("elementName", elementName);
         b.Add(QueryStringTag.Option, 
            option.HasValue ? (short)option.Value : null);

         RequestResponseInfo<DataReferenceFetchResult> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_DATA + b.ToString();
            await client.GetAsync(url);
            data = client.GetDataObjectFromJson<
               RequestResponseInfo<DataReferenceFetchResult>>();
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
      #region -- 4.00 - Insert - Update Element (Remote)

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="organizationId"></param>
      /// <param name="namespacePrefix"></param>
      /// <param name="domainUri"></param>
      /// <param name="domainName"></param>
      /// <param name="element"></param>
      /// <returns></returns>
      public static async Task<RequestResponseInfo<string>>
         UpdateElementDataRemote(string sessionId,
            string organizationId, string namespacePrefix, string domainUri,
            string domainName, DataElement element, string batchId,
            int assetTypeNo)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add("namespacePrefix", namespacePrefix);
         b.Add("domainUri", domainUri);
         b.Add("domainName", domainName);

         RequestResponseInfo<string> data = null;
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = REFERENCE_DATA + b.ToString();
            await client.PostAsync<DataElement>(url, element);
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
      #region -- 4.00 - Get / Update Element (Local)

      public static async Task<RequestResponseInfo<DataReferenceFetchResult>>
         GetElementDataLocal(
            String sessionId, LocaleLanguage? language, String organizationId,
            string root, string elementName, DataReferenceOption? option)
      {
         RequestResponseInfo<DataReferenceFetchResult> response =
            new RequestResponseInfo<DataReferenceFetchResult>();

         DataReferenceFetchRequest request = new DataReferenceFetchRequest
         {
            SessionId = sessionId,
            OrganizationID = organizationId,
            Root = root,
            TermName = elementName,
            Option = option.HasValue ? option.Value :
               DataReferenceOption.DomainsTermsAndElements
         };

         var results = await Task.Run(() =>
            { 
               ResultsLog<DataReferenceFetchResult> r = 
                  DataElementRecord.DataRefereneGet(request);
               return r;
            });

         response.Results = results;
         response.ResponseData = results.Data;
         return response;
      }

      public static async Task<RequestResponseInfo<string>> 
         UpdateElementDataLocal(
            string sessionId, string organizationId, string namespacePrefix,
            string domainUri, string domainName, AssetDataElement element,
            string batchId, int assetTypeNo)
      {
         if (String.IsNullOrWhiteSpace(element.UpdateSessionId))
         {
            element.UpdateSessionId = sessionId;
         }
         if (String.IsNullOrWhiteSpace(element.OrganizationId))
         {
            element.OrganizationId = organizationId;
         }
         if (String.IsNullOrWhiteSpace(element.DataOwnerId))
         {
            element.DataOwnerId = organizationId;
         }

         NamespaceInfo ns = new NamespaceInfo(namespacePrefix, domainUri);

         IResultsLog results = await Task.Run(() =>
         {
            IResultsLog r = DataElementRecord.ElementInsertUpdate(
               element, ns, domainName, batchId, assetTypeNo);
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
      #region -- 4.00 - Insert - Update Domain (Local)

      public static async Task<RequestResponseInfo<DataReferenceFetchResult>>
         GetDomainDataLocal(
            String sessionId, String organizationId, string domainUri)
      {
         RequestResponseInfo<DataReferenceFetchResult> response =
            new RequestResponseInfo<DataReferenceFetchResult>();

         var results = await Task.Run(() =>
         {
            ResultsLog<DataReferenceFetchResult> r =
               DataElementRecord.DomainGet(
                  sessionId, organizationId, domainUri);
            return r;
         });

         response.Results = results;
         response.ResponseData = results.Data;
         return response;
      }

      public static async Task<RequestResponseInfo<string>>
         UpdateDomainDataLocal(
            string sessionId, string organizationId, DataDomain domain)
      {
         if (String.IsNullOrWhiteSpace(organizationId))
         {
            organizationId = Session.OrganizationId;
         }

         if (String.IsNullOrWhiteSpace(domain.UpdateSessionId))
         {
            domain.UpdateSessionId = sessionId;
         }
         if (String.IsNullOrWhiteSpace(domain.OrganizationId))
         {
            domain.OrganizationId = organizationId;
         }
         if (String.IsNullOrWhiteSpace(domain.DataOwnerId))
         {
            domain.DataOwnerId = organizationId;
         }


         IResultsLog results = await Task.Run(() =>
         {
            IResultsLog r = DataElementRecord.DomainInsertUpdate(domain);
            return r;
         });

         RequestResponseInfo<string> response =
            new RequestResponseInfo<string>();

         response.Results.Copy(results);
         response.ResponseData = results.Success ?
            domain.DomainId : String.Empty;

         return response;
      }

      #endregion
      #region -- 4.00 - Insert - Update Domain (Local)

      public static async Task<RequestResponseInfo<string>>
         UpdateBatchDataLocal(
            string sessionId, string organizationId,
            string dataOwnerId, string batchId, string domainUri,
            string versionId, string groupId)
      {
         if (String.IsNullOrWhiteSpace(organizationId))
         {
            organizationId = Session.OrganizationId;
         }
         if (String.IsNullOrWhiteSpace(sessionId))
         {
            sessionId = Session.SessionId;
         }
         if (String.IsNullOrWhiteSpace(dataOwnerId))
         {
            dataOwnerId = Session.OrganizationId;
         }

         IResultsLog results = await Task.Run(() =>
         {
            IResultsLog r = DataBatchRecord.BatchInsertUpdate(
               sessionId, organizationId, dataOwnerId, batchId, domainUri,
               versionId, groupId);
            return r;
         });

         RequestResponseInfo<string> response =
            new RequestResponseInfo<string>();

         response.Results.Copy(results);
         response.ResponseData = results.Success ?
            results.DataObject.ToString() : String.Empty;

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
      public static ReferenceDataService GetService()
      {
         ReferenceDataService s = new ReferenceDataService();
         var k = ReferenceDataHelper.GetConnectionStringKey();
         if (String.IsNullOrWhiteSpace(k))
         {
            s.GetElementData = ReferenceDataService.GetElementDataRemote;
            s.UpdateElementData = ReferenceDataService.UpdateElementDataRemote;
            s.GetDomainData = ReferenceDataService.GetDomainDataLocal;
            s.UpdateDomainData = ReferenceDataService.UpdateDomainDataLocal;
            s.UpdateBatchData = ReferenceDataService.UpdateBatchDataLocal;
         }
         else
         {
            s.GetElementData = ReferenceDataService.GetElementDataLocal;
            s.UpdateElementData = ReferenceDataService.UpdateElementDataLocal;
            s.GetDomainData = ReferenceDataService.GetDomainDataLocal;
            s.UpdateDomainData = ReferenceDataService.UpdateDomainDataLocal;
            s.UpdateBatchData = ReferenceDataService.UpdateBatchDataLocal;
         }
         return s;
      }

      #endregion

   }

}
