using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
//using Newtonsoft.Json;

// -----------------------------------------------------------------------------
using app = Edam.Application.Resources;
using Edam.Serialization;
using Edam.DataObjects.Entities;
using System.Net.Http.Json;

namespace Edam.Net.Web
{

   /// <summary>
   /// Helper for Web-Api / RESTfull (GET, POST, PUT DELETE) calls.
   /// </summary>
   public class WebApiClient : IDisposable
   {

      private Edam.Diagnostics.ResultLog m_Results =
         new Diagnostics.ResultLog();

      private String m_ServerUri;
      private HttpResponseMessage m_Response;
      private HttpClient m_Client;

      private WebApiContentType m_ContentType = WebApiContentType.Unknown;

      public String Data { get; set; }
      public Edam.Diagnostics.ResultLog Results
      {
         get { return m_Results; }
      }

      public HttpResponseMessage Response
      {
         get { return m_Response; }
      }

      public HttpRequestHeaders Headers
      {
         get { return m_Client.DefaultRequestHeaders; }
      }

      public bool Success
      {
         get { return m_Response.StatusCode == System.Net.HttpStatusCode.OK; }
      }

      /// <summary>
      /// Initialize the HTTP Client resources... using given Request details.
      /// </summary>
      /// <param name="request">request info</param>
      /// <param name="client">(optional) HTTP client instance</param>
      private void Initialize(HttpRequestInfo request, HttpClient client = null)
      {
         m_Results = new Diagnostics.ResultLog();
         m_ServerUri = request.BaseUri;
         m_ContentType = request.ContentType;

         //HttpClientHandler chandler = new HttpClientHandler
         //{
         //   ClientCertificateOptions = ClientCertificateOption.Manual
         //};

         m_Client = client ?? new HttpClient();
         m_Client.BaseAddress = new Uri(m_ServerUri);

         if (client == null)
            m_Client.DefaultRequestHeaders.Accept.Clear();

         if (m_ContentType == WebApiContentType.ApplicationJson)
         {
            m_Client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
         }
         else
         if (m_ContentType == WebApiContentType.TextPlain)
            m_Client.DefaultRequestHeaders.Add("Accept", "text/plain");

         if (!String.IsNullOrWhiteSpace(request.UserID) &&
            !String.IsNullOrWhiteSpace(request.UserSecret))
         {
            m_Client.DefaultRequestHeaders.Authorization =
               PrepareBasicAuthentication(request.UserID, request.UserSecret);
         }
      }

      /// <summary>
      /// Initilize client.
      /// </summary>
      /// <param name="uri">URI</param>
      /// <param name="client">prepared HttpClient</param>
      /// <param name="accept">accept Content Type</param>
      /// <param name="contentType">Content Type</param>
      /// <param name="clientId">client ID</param>
      /// <param name="clientSecret">client Secret</param>
      private void Initialize(String uri, HttpClient client,
         WebApiContentType accept = WebApiContentType.ApplicationJson,
         WebApiContentType contentType = WebApiContentType.ApplicationJson,
         string clientId = null, string clientSecret = null)
      {
         HttpRequestInfo request = new HttpRequestInfo();
         request.BaseUri = uri;
         request.ContentType = contentType;
         request.UserID = clientId;
         request.UserSecret = clientSecret;

         Initialize(request);
      }

      /// <summary>
      /// Initialize HttpClient with given server URI.
      /// </summary>
      /// <param name="uri">server URI (e.g. http://localhost:8080/) </param>
      public WebApiClient(String uri,
         WebApiContentType accept = WebApiContentType.ApplicationJson,
         WebApiContentType contentType = WebApiContentType.ApplicationJson,
         string clientId = null, string clientSecret = null)
      {
         Initialize(uri, null, accept, contentType, clientId, clientSecret);
      }

      public WebApiClient(String uri, HttpClient client,
         WebApiContentType accept = WebApiContentType.ApplicationJson,
         WebApiContentType contentType = WebApiContentType.ApplicationJson,
         string clientId = null, string clientSecret = null)
      {
         Initialize(uri, client, accept, contentType, clientId, clientSecret);
      }

      public WebApiClient(HttpRequestInfo request, HttpClient client)
      {
         Initialize(request, client);
      }

      public WebApiClient(HttpRequestInfo request)
      {
         Initialize(request);
      }

      /// <summary>
      /// Prepare Basic authentication for given request.
      /// </summary>
      /// <param name="request"></param>
      /// <param name="clientId"></param>
      /// <param name="clientSecret"></param>
      public static AuthenticationHeaderValue PrepareBasicAuthentication(
         string clientId, string clientSecret)
      {
         var authenticationString = $"{clientId}:{clientSecret}";
         var base64EncodedAuthenticationString =
            System.Convert.ToBase64String(
               System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
         return new AuthenticationHeaderValue(
            "Basic", base64EncodedAuthenticationString);
      }

      /// <summary>
      /// Prepare Basic authentication for given request.
      /// </summary>
      /// <param name="request"></param>
      /// <param name="clientId"></param>
      /// <param name="clientSecret"></param>
      public static void AddBasicAuthentication(
         HttpRequestMessage request, string clientId, string clientSecret)
      {
         var authenticationString = $"{clientId}:{clientSecret}";
         var base64EncodedAuthenticationString =
            System.Convert.ToBase64String(
               System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
         request.Headers.Authorization = 
            PrepareBasicAuthentication(clientId, clientSecret);
      }

      /// <summary>
      /// Get Data Object from JSON.
      /// </summary>
      /// <typeparam name="T">type of data-object to return</typeparam>
      /// <param name="jsonData">JSON data</param>
      /// <returns>instance of type T is returned</returns>
      public T GetDataObjectFromJson<T>(String jsonData = null)
      {
         if (string.IsNullOrWhiteSpace(jsonData))
            jsonData = Data;
         T data =  JsonSerializer.Deserialize<T>(jsonData);
         return data;
      }

      /// <summary>
      /// GET data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products/1)</param>
      /// <returns>Task is returned</returns>
      public async Task GetAsync(String requestUri)
      {
         m_Response = await m_Client.GetAsync(requestUri).ConfigureAwait(
            continueOnCapturedContext: false);
         Data = await m_Response.Content.ReadAsStringAsync();
      }

      /// <summary>
      /// GET JSON data asynchronously and return the instance of T. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products/1)</param>
      /// <returns>T is returned, null if some issue was found</returns>
      public T GetDataFromJson<T>(String requestUri)
      {
         m_Results.Clear();
         T data = default(T);
         try
         {
            GetAsync(requestUri).Wait();
            data = GetDataObjectFromJson<T>(Data);
            m_Results.Succeeded();
         }
         catch (Exception ex)
         {
            m_Results.Failed(ex);
         }
         return data;
      }

      /// <summary>
      /// Get data as Text...
      /// </summary>
      /// <param name="requestUri">(optional) request URI (e.g. api/products)
      /// </param>
      /// <returns>result as Text</returns>
      public String GetDataAsText(String requestUri = null)
      {
         String uri = (requestUri == null) ? String.Empty : requestUri;
         m_Results.Clear();
         try
         {
            GetAsync(uri).Wait();
            m_Results.Succeeded();
         }
         catch (Exception ex)
         {
            m_Results.Failed(ex);
         }
         return Data;
      }

      /// <summary>
      /// Get HTTP Content
      /// </summary>
      /// <typeparam name="T">object type</typeparam>
      /// <param name="content">content instance of T</param>
      /// <returns>HttpContent is returned</returns>
      public static HttpContent GetHttpContent<T>(T content)
      {
         String jsonString = JsonSerializer.Serialize(content);
         HttpContent httpContent = new StringContent(jsonString,
            UTF8Encoding.UTF8, app.Strings.HttpContentTypes.ApplicationJson);
         return httpContent;
      }

      /// <summary>
      /// POST data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products)</param>
      /// <returns>Task is returned</returns>
      public async Task PostAsync(String requestUri)
      {
         if (m_ContentType == WebApiContentType.ApplicationJson)
         {
            m_Response = await m_Client.PostAsync(requestUri, null);
            Data = await m_Response.Content.ReadAsStringAsync();
         }
         else
         if (m_ContentType == WebApiContentType.ApplicationFormUrlEncoded)
         {
            HttpRequestMessage req =
               new HttpRequestMessage(HttpMethod.Post, requestUri);
            //m_Response = await m_Client.SendAsync(req);
            //Data = await m_Response.Content.ReadAsStringAsync();
            var task = m_Client.SendAsync(req).ContinueWith((taskAndMessage) =>
            {
               m_Response = taskAndMessage.Result;
               var dataTask = m_Response.Content.ReadAsStringAsync();
               dataTask.Wait();
               Data = dataTask.Result;
            });
            task.Wait();
         }
      }

      /// <summary>
      /// POST data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products)</param>
      /// <param name="payload">data to post</param>
      /// <returns>Task is returned</returns>
      public async Task PostAsync<T>(String requestUri, T payload)
      {
         HttpContent content = GetHttpContent<T>(payload);
         if (m_ContentType == WebApiContentType.ApplicationJson)
         {
            m_Response = await m_Client.PostAsync(requestUri, content);
            Data = await m_Response.Content.ReadAsStringAsync();
         }
         else
         if (m_ContentType == WebApiContentType.ApplicationFormUrlEncoded)
         {
            HttpRequestMessage req =
               new HttpRequestMessage(HttpMethod.Post, requestUri);
            req.Content = new StringContent(payload.ToString(),
               Encoding.UTF8, "application/x-www-form-urlencoded");
            //m_Response = await m_Client.SendAsync(req);
            //Data = await m_Response.Content.ReadAsStringAsync();
            var task = m_Client.SendAsync(req).ContinueWith((taskAndMessage) =>
               {
                  m_Response = taskAndMessage.Result;
                  var dataTask = m_Response.Content.ReadAsStringAsync();
                  dataTask.Wait();
                  Data = dataTask.Result;
               });
            task.Wait();
         }
      }

      /// <summary>
      /// POST data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products)</param>
      /// <param name="payload">data to post</param>
      /// <returns>Task is returned</returns>
      public void Post<T>(String requestUri, T payload)
      {
         HttpContent content = GetHttpContent<T>(payload);
         if (m_ContentType == WebApiContentType.ApplicationJson)
         {
            var t = m_Client.PostAsync(requestUri, content);
            t.Wait();
            if (t.Status == TaskStatus.RanToCompletion)
            {
               m_Response = t.Result;
               var tread = m_Response.Content.ReadAsStringAsync();
               tread.Wait();
               if (tread.Status == TaskStatus.RanToCompletion)
               {
                  Data = tread.Result;
               }
            }
         }
      }

      public void PostAsJson(String requestUri, object payload)
      {
         var t = m_Client.PostAsJsonAsync(requestUri, payload);
         t.Wait();
         if (t.Status == TaskStatus.RanToCompletion)
         {
            m_Response = t.Result;
            var tread = m_Response.Content.ReadAsStringAsync();
            tread.Wait();
            if (tread.Status == TaskStatus.RanToCompletion)
            {
               Data = tread.Result;
            }
         }
      }

      /// <summary>
      /// POST data and get JSON data asynchronously and return the 
      /// instance of T. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products/1)</param>
      /// <param name="requestData">request Data as string</param>
      /// <returns>T is returned, null if some issue was found</returns>
      public T PostAndGetDataFromJson<T>(String requestUri, String requestData)
      {
         m_Results.Clear();
         T data = default(T);
         try
         {
            PostAsync<String>(requestUri, requestData).Wait();
            data = GetDataObjectFromJson<T>(Data);
            m_Results.Succeeded();
         }
         catch (Exception ex)
         {
            m_Results.Failed(ex);
         }
         return data;
      }

      /// <summary>
      /// PUT data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products/1)</param>
      /// <param name="payload">data to put (update)</param>
      /// <returns>Task is returned</returns>
      public async Task PutAsync<T>(String requestUri, T payload)
      {
         HttpContent content = GetHttpContent<T>(payload);
         m_Response = await m_Client.PutAsync(requestUri, content);
      }

      /// <summary>
      /// PUT data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products/1)</param>
      /// <param name="payload">data to put (update)</param>
      /// <returns>Task is returned</returns>
      public void Put<T>(String requestUri, T payload)
      {
         HttpContent content = GetHttpContent<T>(payload);
         var t = m_Client.PutAsync(requestUri, content);
         t.Wait();
         if (t.Status == TaskStatus.RanToCompletion)
         {
            m_Response = t.Result;
            var tread = m_Response.Content.ReadAsStringAsync();
            tread.Wait();
            if (tread.Status == TaskStatus.RanToCompletion)
            {
               Data = tread.Result;
            }
         }
      }

      /// <summary>
      /// DELETE data asynchronously. 
      /// </summary>
      /// <param name="requestUri">request URI (e.g. api/products/1)</param>
      /// <param name="payload">data to delete</param>
      /// <returns>Task is returned</returns>
      public async Task DeleteAsync(String requestUri)
      {
         m_Response = await m_Client.DeleteAsync(requestUri);
      }
      
      /// <summary>
      /// Clean up resources...
      /// </summary>
      public void Dispose()
      {
         if (m_Client != null)
            m_Client.Dispose();
         m_Client = null;
      }

   }

}
