using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_WEB_HTTP
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Security.Cryptography.Certificates;

//[assembly: Xamarin.Forms.Dependency(typeof(Edam.UWP.Net.Web.WebApiClientHelper))]

namespace Edam.Net.Web
{
   public class WebApiClientHelper
   {

      public void SetHttpBaseProtocolFilters()
      {
         HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.Untrusted);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.Expired);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.IncompleteChain);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.WrongUsage);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.InvalidName);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.RevocationInformationMissing);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.RevocationFailure);
      }

      public HttpClient PreparedClient()
      {
         var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();

         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.Expired);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.Untrusted);
         filter.IgnorableServerCertificateErrors.Add(
            ChainValidationResult.InvalidName);

         HttpClient client = new HttpClient(filter);

         // I also handle other stuff here 
         // (client certificate, authentification), but the lines above should 
         // allow the Httpclient to accept all certificates

         return client;
      }

   }
}

#endif
