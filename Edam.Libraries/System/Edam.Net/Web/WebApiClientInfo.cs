using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Net.Web;
using Edam.Services;
using Edam.Application;
using Edam.Text;

namespace Edam.Net.Web
{

   public class WebApiClientInfo
   {
      private WebApiClient m_Client; 
      public WebApiClient Client
      {
         get { return m_Client; }
      }
      private QueryStringBuilder m_Builder;
      public QueryStringBuilder Builder
      {
         get { return m_Builder; }
      }

      private void Initialize()
      {
         m_Builder = new QueryStringBuilder();
         m_Client = GetWebApiClient();
      }

      public WebApiClientInfo()
      {
         Initialize();
      }

      public WebApiClientInfo(
         string sessionId = null, LocaleLanguage? language = null,
         string organizationId = null)
      {
         Initialize();
         SetSessionParams(sessionId, language, organizationId);
      }

      public void SetSessionParams(
         string sessionId = null, LocaleLanguage? language = null, 
         string organizationId = null)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         LocaleLanguage lang = language.HasValue ? language.Value :
            Edam.Application.Session.Language;

         Builder.Add(QueryStringTag.SessionId, sid);
         //Builder.Add(QueryStringTag.Language, lang.ToString());
         Builder.Add(QueryStringTag.OrganizationId, oid);
      }

      /// <summary>
      /// Get Web API Client by given Key.
      /// </summary>
      /// <param name="key">(optional) key [default: "Default"] or as configured
      /// in the session and fetched with GetDefaultSessionKey</param>
      /// <returns>requested WebApiClient instance for requested key is returned
      /// </returns>
      public static WebApiClient GetWebApiClient(string key = null)
      {
         string k = String.IsNullOrWhiteSpace(key) ?
            Session.GetDefaultSessionKey() : key;
         var s = ServicesSession.Configurations.FindBaseService(k);
         WebApiClient client = new WebApiClient(s.ServicePathUri);
         return client;
      }

   }

}
