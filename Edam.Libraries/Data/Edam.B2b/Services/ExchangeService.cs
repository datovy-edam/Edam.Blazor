using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Requests;
using Edam.DataObjects.B2b;
using Edam.Application;
using Edam.DataObjects.ReferenceData;

namespace Edam.DataObjects.Services
{

   public delegate Task<RequestResponseInfo<string>>
         UpdateExchangeCodeHandler(
            string sessionId, string dataOwnerId, string codeId,
            string description);
   public delegate Task<RequestResponseInfo<string>>
         UpdateExchangeDefinitionHandler(string sessionId,
            string dataOwnerId, string namespacePrefix, 
            string namespaceDescription, List<List<string>> rows);

   public class ExchangeService
   {

      public static readonly string EXCHANGE = "Exchange";

      public UpdateExchangeCodeHandler PostExchangeCode { get; set; }
      public UpdateExchangeDefinitionHandler 
         PostExchangeDefinition { get; set; }

      #region -- 4.00 - Exchange Code Post

      public static async Task<RequestResponseInfo<string>> PostCodeRecordLocal(
         string sessionId, string dataOwnerId, string codeId,
         string description)
      {
         if (String.IsNullOrWhiteSpace(sessionId))
         {
            sessionId = Session.SessionId;
         }
         if (String.IsNullOrWhiteSpace(dataOwnerId))
         {
            dataOwnerId = Session.OrganizationId;
         }

         RequestResponseInfo<string> response = 
            new RequestResponseInfo<string>();

         var results = await Task.Run(() =>
            EdiExchangeRecord.UpdateExchangeCodeRecord(
               sessionId, dataOwnerId, codeId, description));

         return results;
      }

      #endregion
      #region -- 4.00 - Exchange Definition Post

      public static async Task<RequestResponseInfo<string>> 
         PostDefinitionRecordLocal(
            string sessionId, string dataOwnerId, string namespacePrefix,
            string namespaceDescription, List<List<string>> items)
      {
         if (String.IsNullOrWhiteSpace(sessionId))
         {
            sessionId = Session.SessionId;
         }
         if (String.IsNullOrWhiteSpace(dataOwnerId))
         {
            dataOwnerId = Session.OrganizationId;
         }

         var response = await Task.Run(() =>
            EdiExchangeRecord.PostDefinitions(
               sessionId, dataOwnerId, namespacePrefix, 
               namespaceDescription, items));

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
      public static ExchangeService GetService()
      {
         ExchangeService s = new ExchangeService();
         var k = ReferenceDataHelper.GetConnectionStringKey();
         if (String.IsNullOrWhiteSpace(k))
         {
            s.PostExchangeCode = ExchangeService.PostCodeRecordLocal;
            s.PostExchangeDefinition = 
               ExchangeService.PostDefinitionRecordLocal;
         }
         else
         {
            s.PostExchangeCode = ExchangeService.PostCodeRecordLocal;
            s.PostExchangeDefinition = 
               ExchangeService.PostDefinitionRecordLocal;
         }
         return s;
      }

      #endregion

   }

}
