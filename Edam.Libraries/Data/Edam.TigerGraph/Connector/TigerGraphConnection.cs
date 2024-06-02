using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TigerGraph.Connector
{

   public partial class TigerGraphConnection
   {

      #region -- Initialize Variables

      private static string _as = "TigerGraphConnector";
      private static string _cn = "TgConnector";

      public event TokenExpirationChangedEventHandler TokenExpirationChanged;

      string _host = "http://localhost";
      string _userName = "tigergraph";
      string _passWord = "tigergraph";
      string _graphName = "MyGraph";
      int _restppPort = 9000;
      string _restppUrl;
      int _gsPort = 14240;
      string _gsUrl;
      string _secret;
      string _apiToken = "";
      bool _authenticationIsEnabled = true;
      long _tokenExpiration = 0;  //In Unix Time
      int _lifetime = 2592000;
      int _timeout = -1;
      HttpClient _initialClient = null;
      HttpResponseMessage _initialResponse = null;
      bool _useToken = true;
      bool _debug = false;
      static string QT = "\"";

      public enum AuthMode
      {
         Password,
         Token
      };

      AuthMode _curAuthMode = AuthMode.Token;

      #endregion Initialize Variables
      #region -- Properties

      public string Host
      {
         //get => _host; set => _host = value;
         get { return _host; }
         set { _host = value; }
      }

      public string UserName
      {
         //get => _userName; set => _userName = value;
         get { return _userName; }
         set { _userName = value; }
      }

      public string PassWord
      {
         //get => _passWord; set => _passWord = value;
         get { return _passWord; }
         set { _passWord = value; }
      }

      public string GraphName
      {
         //get => _graphName; set => _graphName = value; 
         get { return _graphName; }
         set { _graphName = value; }
      }

      public int RestPort
      {
         //get => _restppPort; set => _restppPort = value; 
         get { return _restppPort; }
         set { _restppPort = value; }
      }

      public string RestppUrl
      {
         //get => _restppUrl; set => _restppUrl = value; 
         get { return _restppUrl; }
         set { _restppUrl = value; }
      }

      public int GsPort
      {
         //get => _gsPort; set => _gsPort = value; 
         get { return _gsPort; }
         set { _gsPort = value; }
      }

      public string GsUrl
      {
         //get => _gsUrl; set => _gsUrl = value; 
         get { return _gsUrl; }
         set { _gsUrl = value; }
      }

      public string ApiToken
      {
         //get => _apiToken; set => _apiToken = value; 
         get { return _apiToken; }
         set { _apiToken = value; }
      }

      //public Dictionary<string, string> AuthHeader { get => _authHeader; set => _authHeader = value; }

      public string Secret
      {
         //get => _secret; set => _secret = value; 
         get { return _secret; }
         set { _secret = value; }
      }

      public int Lifetime
      {
         //get => _lifetime; set => _lifetime = value; 
         get { return _lifetime; }
         set { _lifetime = value; }
      }

      public int Timeout
      {
         //get => _timeout; set => _timeout = value; 
         get { return _timeout; }
         set { _timeout = value; }
      }

      public HttpClient Client
      {
         //get => _initialClient; set => _initialClient = value; 
         get { return _initialClient; }
         set { _initialClient = value; }
      }

      public HttpResponseMessage InitialResponse
      {
         //get => _initialResponse; set => _initialResponse = value; 
         get { return _initialResponse; }
         set { _initialResponse = value; }
      }

      #endregion Properties
      #region -- Constructors

      public TigerGraphConnection(
         string host, int restppPort, int gsPort, string graphName,
         bool authenticationIsEnabled,
         bool useToken,
         string tokenSecret,
         string apiToken,
         long tokenExpiration,
         int lifetime,
         string userName,
         string password)
      {
         try
         {
            _host = host;
            _restppPort = restppPort;
            _gsPort = gsPort;
            _restppUrl = _host + ":" + _restppPort;
            _gsUrl = _host + ":" + _gsPort;
            _graphName = graphName;
            _authenticationIsEnabled = authenticationIsEnabled;
            _useToken = useToken;
            _secret = tokenSecret;
            _apiToken = apiToken;
            _tokenExpiration = tokenExpiration;
            _lifetime = lifetime;
            _userName = userName;
            _passWord = password;

            if (_host == null)
            {
               throw new Exception("Host URL is null.");
            }
            if (_graphName == null)
            {
               throw new Exception("Graph Name is null.");
            }

            if (_useToken)
            {
               IsTokenValid();
               //If token expires with in an hour, refresh it
               if (_tokenExpiration < DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600)
               {
                  RefreshToken();
               }
            }
         }
         catch (Exception ex)
         {
            throw GetException("TigerGraphConnection", ex);
         }

      }

      #endregion Constructors
      #region -- Private Methods

      /// <summary>
      /// Checks if the JSON document returned by an endpoint has contains 
      /// error: true; if so, it raises an exception.
      /// </summary>
      /// <param name="response"></param>
      void ErrorCheck(dynamic response)
      {
         try
         {
            if (response.error == true)
            {
               throw new TigerGraphException(response.message, response.code);
            }
         }
         catch (Exception ex)
         {
            throw GetException("ErrorCheck", ex);
         }
      }

      /// <summary>
      /// Generic REST++ API request
      /// </summary>
      /// <param name="method">HTTP method, currently 
      /// one of GET, POST, DELETE or PUT</param>
      /// <param name="url">Complete REST++ API URL including 
      /// path and parameters</param>
      /// <param name="authMode">Authentication mode, one of 'token' 
      /// (default) or 'pwd'</param>
      /// <param name="headers">Standard HTTP request headers (dict)</param>
      /// <param name="resKey">the JSON subdocument to be returned, 
      /// default is 'result'</param>
      /// <param name="skipCheck">Skip error checking? Some endpoints 
      /// return error to indicate that the requested action is not applicable;
      /// a problem, but not really an error.</param>
      /// <param name="data">Request payload, typically a JSON document</param>
      /// <param name="parms">Request URL parameters.</param>
      /// <returns>Dictionary in most cases</returns>
      dynamic Req(HttpMethod method, string url, 
         AuthMode authMode = AuthMode.Token, 
         Dictionary<string, string> headers = null, string resKey = "results",
         bool skipCheck = false, string data = null, 
         Dictionary<string, object> parms = null)
      {
         try
         {
            if (_debug)
            {
               string msg = method + " " + url +
                  (string.IsNullOrEmpty(data) ? "" : " => " + data);
               Console.WriteLine(msg);
            }
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            AddDefaultHeader(ref client, headers, authMode);
            client.Timeout = new TimeSpan(-1);

            RequestParameters rparams = new RequestParameters();
            if (parms != null)
            {
               foreach (var item in parms)
               {
                  rparams.Add(item.Key, item.Value.ToString());
               }
            }

            HttpRequestMessage request = 
               new HttpRequestMessage(method, rparams.ToString());
            request.Method = method;

            if (authMode == AuthMode.Password)
            {
               var authString = $"{_userName}:{_passWord}";
               var base64EncodedAuthenticationString =
                  System.Convert.ToBase64String(
                     System.Text.ASCIIEncoding.UTF8.GetBytes(authString));
               request.Headers.Authorization = new AuthenticationHeaderValue(
                  "Basic", base64EncodedAuthenticationString);
            }

            if (method == HttpMethod.Post && !string.IsNullOrEmpty(data))
            {
               request.Content = new StringContent(
                  data, Encoding.UTF8, "application/text");
            }

            HttpResponseMessage response = client.Send(request);
            string responseData = response.Content.ReadAsStringAsync().Result;

            dynamic responseContent = 
               JsonConvert.DeserializeObject(responseData);
            if (responseContent == null)
            {
               throw new Exception("URL: " + url + "; response.StatusCode = " + 
                  response.StatusCode.ToString());
            }
            if (!skipCheck)
            {
               ErrorCheck(responseContent);
            }
            return responseContent[resKey];
         }
         catch (Exception ex)
         {
            throw GetException("Req", ex);
         }
      }

      /// <summary>
      /// Generic GET method
      /// </summary>
      /// <param name="url">Complete REST++ API URL including 
      /// path and parameters</param>
      /// <param name="authMode">Authentication mode, one of 'token' 
      /// (default) or 'pwd'</param>
      /// <param name="headers">Standard HTTP request headers (dict)</param>
      /// <param name="resKey">the JSON subdocument to be returned, 
      /// default is 'result'</param>
      /// <param name="skipCheck">Skip error checking? Some endpoints 
      /// return error to indicate that the requested action is not applicable;
      /// a problem, but not really an error.</param>
      /// <param name="parms">Request URL parameters.</param>
      /// <returns>Dictionary in most cases</returns>
      dynamic Get(string url, AuthMode authMode = AuthMode.Token, 
         Dictionary<string, string> headers = null, string resKey = "results",
         bool skipCheck = false, Dictionary<string, object> parms = null)
      {
         try
         {
            return Req(HttpMethod.Get, url, authMode, headers, resKey, 
               skipCheck, null, parms);
         }
         catch (Exception ex)
         {
            throw GetException("Get", ex);
         }
      }

      /// <summary>
      /// Generic Post
      /// </summary>
      /// <param name="url">Complete REST++ API URL including path and parameters</param>
      /// <param name="authMode">Authentication mode, one of 'token' (default) or 'pwd'</param>
      /// <param name="headers">Standard HTTP request headers (dict)</param>
      /// <param name="data">Request payload, typically a JSON document</param>
      /// <param name="resKey">the JSON subdocument to be returned, default is 'result'</param>
      /// <param name="skipCheck">Skip error checking? Some endpoints return error to indicate that the requested action is not applicable; a problem, but not really an error.</param>
      /// <param name="parms">Request URL parameters.</param>
      /// <returns></returns>
      dynamic Post(string url, AuthMode authMode, Dictionary<string, string> headers = null, string data = null, string resKey = "results", bool skipCheck = false, string parms = null)
      {
         try
         {
            return Req(HttpMethod.Post, url, authMode, headers, resKey, skipCheck, data);
         }
         catch (Exception ex)
         {
            throw GetException("Post", ex);
         }
      }

      /// <summary>
      /// Generic delete method
      /// </summary>
      /// <param name="url"></param>
      /// <returns></returns>
      dynamic Delete(string url)
      {
         try
         {
            return Req(HttpMethod.Delete, url);
         }
         catch (Exception ex)
         {
            throw GetException("Delete", ex);
         }
      }

      #endregion Private Methods
      #region -- Schema related methods

      /// <summary>
      /// Retrieves the schema (all vertex and edge type and - if not disabled -
      /// the User Defined Type details) of the graph.
      /// Endpoint:      GET /gsqlserver/gsql/schema.
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/
      ///    built-in-endpoints#get-the-graph-schema-get-gsql-schema
      /// </summary>
      /// <param name="includeUdts">Calls GetUDTs if includeUdts=
      /// true (default).</param>
      /// <returns></returns>
      public Dictionary<string, object> GetSchema(bool includeUdts)
      {
         try
         {
            dynamic responseContent = Get(_gsUrl + 
               "/gsqlserver/gsql/schema?graph=" + _graphName, 
               AuthMode.Password);
            var values = ((Newtonsoft.Json.Linq.JObject)responseContent).
               ToObject<Dictionary<string, object>>();
            Dictionary<string, object> returnData = 
               new Dictionary<string, object>();
            returnData.Add("error", responseContent.error);
            returnData.Add("message", responseContent.message);
            foreach (var item in values)
            {
               returnData.Add(item.Key, item.Value);
            }
            if (includeUdts)
            {
               dynamic udts = GetUDTs();
               if (udts.Count > 0)
               {
                  returnData.Add("UDTs", udts);
               }
            }
            return returnData;
         }
         catch (Exception ex)
         {
            throw GetException("GetSchema", ex);
         }
      }

      /// <summary>
      /// Returns the details of a specific User Defined Type.
      /// </summary>
      /// <param name="udtName">Name of the UDT</param>
      /// <returns><Dictionary<string, object></returns>
      public dynamic GetUDT(string udtName)
      {
         try
         {
            dynamic responseContent = GetUDTs();
            if (responseContent != null)
            {
               foreach (var item in responseContent)
               {
                  if (item["name"] == udtName)
                  {
                     return item;
                  }
               }
            }
            return null;
         }
         catch (Exception ex)
         {
            throw GetException("GetUDT", ex);
         }
      }

      /// <summary>
      /// Returns the list of User Defined Types as dictionaries
      /// </summary>
      /// <returns>List<Dictionary<string, object>></returns>
      public List<Dictionary<string, object>> GetUDTs()
      {
         try
         {
            string url = _gsUrl + "/gsqlserver/gsql/udtlist?graph=" + 
               _graphName;
            dynamic responseContent = Get(url, AuthMode.Password);
            List<Dictionary<string, object>> udtList = 
               new List<Dictionary<string, object>>();
            if (responseContent != null && responseContent.Count > 0)
            {
               foreach (var item in responseContent)
               {
                  var values = ((Newtonsoft.Json.Linq.JObject)item).
                     ToObject<Dictionary<string, object>>();
                  udtList.Add(values);
               }
            }
            return udtList;
         }
         catch (Exception ex)
         {
            throw GetException("GetUDTs", ex);
         }
      }


      /// <summary>
      /// Upsdert Json for a single vertex, a list of vertices, and/or a single
      /// edge or a list of edges.
      /// https://docs.tigergraph.com/dev/restpp-api/
      ///    built-in-endpoints#post-graph-graph_name-upsert-the-given-data
      /// </summary>
      /// <param name="json">Fully formatted Json.  No additional formatting 
      /// will be applied.</param>
      /// <param name="ack">"all": request will return after all GPE instances 
      /// have acknowledged the POST.  "none": request will return immediately 
      /// after RESTPP processed the POST.</param>
      /// <param name="new_vertex_only">"false": Upsert vertices.  "true": 
      /// Treat vertices as insert-only. If the input data refers to a vertex 
      /// which already exists, do not update it.</param>
      /// <param name="vertex_must_exist">"false": Always insert new edges. 
      /// If a new edge refers to an endpoint vertex which does not exist, 
      /// create the necessary vertices, using given id and default values for 
      /// other parameters.  "true": Only insert or update an edge If both 
      /// endpoint vertices already exist.</param>
      /// <returns>String count of upserts</returns>
      public dynamic UpsertData(string json, string ack = "all", 
         bool new_vertex_only = false, bool vertex_must_exist = false)
      {
         Dictionary<string, string> Header = new Dictionary<string, string>();
         try
         {
            //Endpoint parameters:
            string parms = "ack=" + ack + "& new_vertex_only=" + 
               new_vertex_only.ToString() + "& vertex_must_exist=" + 
               new_vertex_only.ToString();
            string url = _restppUrl + "/graph/" + _graphName + "?" + parms;
            Header.Add("Accept", "application/json");
            dynamic responseContent = Post(url, AuthMode.Token, Header, json, 
               "results", false);
            JArray values = JArray.Parse(
               JsonConvert.SerializeObject(responseContent));
            return values;
         }
         catch (Exception ex)
         {
            throw GetException("UpsertData", ex);
         }
      }

      #endregion Schema related methods
      #region -- Query related methods

      /// <summary>
      /// The query must be already created and installed in the graph.
      /// Use `getEndpoints(dynamic=True)` or GraphStudio to find out the 
      /// generated endpoint URL of the query, but only the query name 
      /// needs to be specified here.
      /// Endpoint:      POST /query/{graph_name}/<query_name>
      /// Documentation: https://docs.tigergraph.com/dev/gsql-ref/
      ///    querying/query-operations#running-a-query
      /// </summary>
      /// <param name="queryName"></param>
      /// <param name="parms">A string of param1=value1&param2=value2 
      /// format or a dictionary.</param>
      /// <param name="timeout">Maximum duration for successful 
      /// query execution.</param>
      /// <param name="sizeLimit">Maximum size of response (in bytes).</param>
      /// <returns>Json</returns>
      public dynamic RunInstalledQuery(string queryName,
         Dictionary<string, object> parms = null, int timeout = 16000, 
         int sizeLimit = 32000000)
      {
         try
         {
            Dictionary<string, string> headers = 
               new Dictionary<string, string>();
            headers.Add("RESPONSE-LIMIT", sizeLimit.ToString());
            headers.Add("GSQL-TIMEOUT", timeout.ToString());
            return Get(_restppUrl + "/query/" + _graphName + "/" + queryName, 
               AuthMode.Token, headers, "results", false, parms);
         }
         catch (Exception ex)
         {
            throw GetException("RunInstalledQuery", ex);
         }
      }

      /// <summary>
      /// Runs an interpreted query.
      /// You must provide the query text in this format:
      /// INTERPRET QUERY (<params>) FOR GRAPH <graph_name> {<statements>}
      /// Endpoint:      POST /gsqlserver/interpreted_query
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/
      ///    built-in-endpoints#post-gsqlserver-
      ///    interpreted_query-run-an-interpreted-query
      /// </summary>
      /// <param name="queryText"></param>
      /// <param name="parms">A string of param1=value1&param2=value2 format 
      /// or a dictionary.</param>
      /// <returns>Json as string</returns>
      public dynamic RunInterpretedQuery(string queryText, string parms)
      {
         try
         {
            return Post(_gsUrl + "/gsqlserver/interpreted_query", 
               AuthMode.Password, null, queryText, "results", false, parms);
         }
         catch (Exception ex)
         {
            throw GetException("RunInterpretedQuery", ex);
         }
      }

      #endregion Query related methods
      #region -- Token Management 

      /// <summary>
      /// Test to see if the token is valid (does it exist or has it expired).
      /// If not it will try to refresh the token.
      /// If the token is refreshed, an event will be raised sending back the 
      /// new expiration date.
      /// </summary>
      /// <returns>True if valid, otherwise it will throw an exception</returns>
      bool IsTokenValid()
      {
         try
         {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_restppUrl + "/echo/" + _graphName);
            client.DefaultRequestHeaders.Add(
               "Authorization", "Bearer " + _apiToken);
            client.Timeout = new TimeSpan(-1);
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = client.Send(request);
            string responseData = response.Content.ReadAsStringAsync().Result;
            dynamic responseContent = 
               JsonConvert.DeserializeObject(responseData);
            if (responseContent.message == "Hello GSQL")
            {
               return true;
            }
            else if (((string)responseContent.message).Contains("is invalid"))
            {
               RefreshToken();
               //Test again
               response = client.Send(request);
               string testData = 
                  response.Content.ReadAsStringAsync().Result;
               responseContent = JsonConvert.DeserializeObject(testData);
               if (responseContent.message == "Hello GSQL")
               {
                  return true;
               }
               else
               {
                  throw new TigerGraphException(
                     responseContent.message, responseContent.code);
               }
            }
            return true;
         }
         catch (Exception ex)
         {
            throw GetException("IsTokenValid", ex);
         }
      }

      /// <summary>
      /// If authentication is enabled, add the authentication header
      /// Add any other headers
      /// </summary>
      /// <param name="client">Pass in an instance of RestClient and add header
      /// s to it.</param>
      /// <param name="headers"></param>
      /// <param name="authMode">Specify the authentication mode if different 
      /// than the default</param>
      void AddDefaultHeader(ref HttpClient client,
         Dictionary<string, string> headers, AuthMode authMode = AuthMode.Token)
      {
         try
         {
            Dictionary<string, string> defaultHeaders = 
               new Dictionary<string, string>();
            if (_curAuthMode == AuthMode.Token && authMode == AuthMode.Token)
            {
               defaultHeaders.Add("Authorization", "Bearer " + _apiToken);
            }
            if (headers != null && headers.Count > 0)
            {
               foreach (var item in headers)
               {
                  defaultHeaders.Add(item.Key, item.Value);
               }
            }
            if (defaultHeaders.Count > 0)
            {
               foreach (var item in defaultHeaders)
               {
                  client.DefaultRequestHeaders.Add(item.Key, item.Value);
               }
            }
         }
         catch (Exception e)
         {
            throw GetException("AddDefaultHeader", e);
         }
      }

      /// <summary>
      /// Get the token saved in this instance of TigerGraphConnection.
      /// </summary>
      /// <param name=""></param>
      /// <returns></returns>
      public object[] GetTokenFromThis()
      {
         try
         {
            object[] returnData = new object[2];
            returnData[0] = _graphName;
            returnData[1] = _tokenExpiration;
            return returnData;
         }
         catch (Exception e)
         {
            throw GetException("GetTokenFromThis", e);
         }
      }


      /// <summary>
      /// Refresh the token using the lifetime saved in the instance of this
      /// class.
      /// An event will be raised passing back the token and it's new expiration
      /// date.
      /// And the token and it's new expiration date will also be returned from 
      /// this method.
      /// </summary>
      /// <returns>object[3]:  Graph Name, Token, Token Expiration</returns>
      public object[] RefreshToken()
      {
         try
         {
            object[] returnData = new object[3];
            var client = new HttpClient();
            client.BaseAddress = new Uri(_restppUrl + "/requesttoken?secret=" + 
               _secret + "&token=" + _apiToken + "&lifetime=" + _lifetime);
            client.Timeout = new TimeSpan(-1);

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Put;

            var response = client.Send(request);

            string responseData = response.Content.ReadAsStringAsync().Result;
            dynamic responseContent = 
               JsonConvert.DeserializeObject(responseData);
            if (responseContent.error == false)
            {
               _tokenExpiration = 
                  DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _lifetime;
               returnData[0] = _graphName;
               returnData[1] = _apiToken;
               returnData[2] = _tokenExpiration;
               RaiseEvent_TokenExpirationChanged();
               return returnData;
            }
            else
            {
               if (((string)responseContent.message).Contains(
                  "Endpoint is not found from url"))
               {
                  throw new TigerGraphException(
                     "REST++ authentication is not enabled," + 
                     " can't refresh token.");
               }
               else
               {
                  throw new TigerGraphException(
                     responseContent.message, responseContent.code);
               }
            }
         }
         catch (Exception e)
         {
            throw GetException("RefreshToken", e);
         }
      }

      /// <summary>
      /// An event 'Will Not' be raised returning the new token as this is a 
      /// static method.
      /// </summary>
      /// <param name="url">http://localhost:9000</param>
      /// <param name="secret"></param>
      /// <param name="lifeTime"></param>
      /// <returns>object[]:  Token, Expiration</returns>
      public static object[] GetNewToken(
         string url, string secret, long lifeTime)
      {
         try
         {
            object[] returnData = new object[2];
            var client = new HttpClient();
            client.BaseAddress = new Uri(url + "/requesttoken?secret=" + 
               secret + "&lifetime=" + lifeTime);
            client.Timeout = new TimeSpan(-1);
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            var response = client.Send(request);
            string responseData = response.Content.ReadAsStringAsync().Result;
            dynamic responseContent =
               JsonConvert.DeserializeObject(responseData);
            if (responseContent.error == false)
            {
               returnData[0] = responseContent.token;
               returnData[1] = responseContent.expiration;
               return returnData;
            }
            else
            {
               throw new TigerGraphException(
                  responseContent.message, responseContent.code);
            }
         }
         catch (Exception e)
         {
            throw e;
         }
      }

      /// <summary>
      /// Note:  If token is null, the token saved in the instance of this class
      /// will be deleted.
      /// </summary>
      /// <param name="token"></param>
      /// <param name="isSkipException"></param>
      /// <returns></returns>
      public bool DeleteToken(string token = null, bool isSkipException = true)
      {
         try
         {
            if (token == null)
            {
               token = _apiToken;
            }
            string url = _restppUrl + "/requesttoken?secret=" + _secret + 
               "&token=" + token;
            dynamic responseContent = Delete(url);
            if (responseContent.error == false)
            {
               return true;
            }
            else if (responseContent.code == "REST-3300" && 
               isSkipException == true)
            {
               return true;
            }
            else if (((string)responseContent.message).Contains(
               "Endpoint is not found from url = /requesttoken"))
            {
               throw new TigerGraphException(
                  "REST++ authentication is not enabled, can't delete token.",
                  responseContent.code);
            }
            else
            {
               throw new TigerGraphException((string)responseContent.message, 
                  (string)responseContent.code);
            }
         }
         catch (Exception e)
         {
            throw GetException("DeleteToken", e);
         }
      }

      #endregion Token management
      #region -- Other methods

      /// <summary>
      /// Pings the database.
      /// Expected return value is "Hello GSQL"
      /// Endpoint:   GET /echo  and  POST /echo
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-echo-and-post-echo
      /// </summary>
      /// <returns></returns>
      public dynamic GetEcho()
      {
         try
         {
            return Get(_restppUrl + "/echo/" + _graphName, resKey: "message", skipCheck: false);
         }
         catch (Exception ex)
         {
            throw GetException("Get_Echo", ex);
         }
      }

      /// <summary>
      /// Lists the REST++ endpoints and their parameters.
      /// If no parameters are specified, all endpoints are listed.
      /// Endpoint:   GET /endpoints
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-endpoints
      /// </summary>
      /// <param name="getBuiltin">TigerGraph provided REST++ endpoints.</param>
      /// <param name="getDynamic">Endpoints for user installed queries.</param>
      /// <param name="getStatic">Static endpoints.</param>
      /// <returns></returns>
      public dynamic GetEndPoints(bool getBuiltin = false, bool getDynamic = false, bool getStatic = false)
      {
         try
         {
            string url = _restppUrl + "/endpoints?builtin=" + getBuiltin + "&dynamic=" + getDynamic + "&static=" + getStatic;
            return Get(url);
         }
         catch (Exception ex)
         {
            throw GetException("GetEndPoints", ex);
         }
      }

      /// <summary>
      /// Retrieves real-time query performance statistics over the given time period.
      /// Endpoint:   GET /statistics
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-statistics
      /// </summary>
      /// <param name="seconds">The duration of statistic collection period (the last n seconds before the function call).</param>
      /// <param name="segments">The number of segments of the latency distribution (shown in results as LatencyPercentile).
      /// By default, segments is 10, meaning the percentile range 0-100% will be divided into ten equal segments: 0%-10%, 11%-20%, etc.
      /// Segments must be [1, 100].
      /// </param>
      /// <returns></returns>
      public dynamic GetStatistics(int seconds = 10, int segments = 10)
      {
         try
         {
            string url = _restppUrl + "/statistics?seconds=" + seconds + "&segment=" + segments;
            return Get(url);
         }
         catch (Exception ex)
         {
            throw GetException("GetStatistics", ex);
         }
      }

      /// <summary>
      /// Retrieves the git versions of all components of the system.
      /// Endpoint:   GET /version
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-version
      /// </summary>
      /// <returns></returns>
      public dynamic GetVersion()
      {
         try
         {
            string url = _restppUrl + "/version/" + _graphName;
            dynamic result = Get(url, AuthMode.Token, null, "message");
            return result;
         }
         catch (Exception ex)
         {
            throw GetException("GetVersion", ex);
         }
      }

      /// <summary>
      /// Gets the version information of specific component
      /// </summary>
      /// <param name="component">One of TigerGraph's components (e.g. product, gpe, gse).</param>
      /// <param name="isFull"></param>
      /// <returns>List<Dictionary<string, string>></returns>
      public dynamic GetVer(string component = "product", bool isFull = false)
      {
         try
         {
            dynamic result = GetVersion();
            string data = result.ToString();
            List<Dictionary<string, string>> info = new List<Dictionary<string, string>>();
            string[] lines = data.Split(Environment.NewLine.ToCharArray());
            foreach (string item in lines)
            {
               string[] segments = item.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
               if (segments.Length == 6)
               {
                  Dictionary<string, string> comp = new Dictionary<string, string>();
                  comp.Add("name", segments[0]);
                  comp.Add("version", segments[1]);
                  comp.Add("hash", segments[2]);
                  comp.Add("date", segments[3]);
                  comp.Add("time", segments[4]);
                  comp.Add("code", segments[5]);
                  info.Add(comp);
                  if (segments[0] == component)
                  {
                     info.Clear();
                     info.Add(comp);
                     return info;
                  }
               }
            }
            return info;
         }
         catch (Exception ex)
         {
            throw GetException("GetVer", ex);
         }
      }

      //THIS DOES NOT WORK!
      //Its better to do this from the termnal window as it requires a path to the license file.
      public dynamic GetLicenseInfo()
      {
         try
         {
            return null;
         }
         catch (Exception ex)
         {
            throw GetException("Get_LicenseInfo", ex);
         }
      }


      #endregion Other methods
      #region -- Common Helper Methods

      // Function to create URL with all conditions to take load off
      // each function
      public string Create_URL_Extended(string select, string where, 
         string limit, string sort, int timeout)
      {
         try
         {
            string url = "";
            bool isFirst = true;
            if (!string.IsNullOrEmpty(select))
            {
               url += "?select=" + select;
               isFirst = false;
            }
            if (!string.IsNullOrEmpty(where))
            {
               url += (isFirst ? "?" : "&") + "where=" + where;
               isFirst = false;
            }
            if (!string.IsNullOrEmpty(limit))
            {
               url += (isFirst ? "?" : "&") + "limit=" + limit;
               isFirst = false;
            }
            if (!string.IsNullOrEmpty(sort))
            {
               url += (isFirst ? "?" : "&") + "sort=" + sort;
               isFirst = false;
            }
            if (timeout > 0)
            {
               url += (isFirst ? "?" : "&") + "timeout=" + timeout.ToString();
               isFirst = false;
            }
            return url;
         }
         catch (Exception ex)
         {
            throw GetException("Create_URL_Extended", ex);
         }
      }

      public List<string> GetVertexNames(object vertexTypes)
      {
         try
         {
            List<string> vertextNames = new List<string>();

            if (vertexTypes is string)
            {
               vertextNames.Add((string)vertexTypes);
            }
            else if (vertexTypes is List<string>)
            {
               foreach (var item in (List<string>)vertexTypes)
               {
                  vertextNames.Add(item);
               }
            }
            else
            {
               //do something
               throw new Exception("This part has not been implemented yet. " +
                  "vertexttypes was not a string or list. We need to " +
                  "implement an option for an object[].");
            }

            return vertextNames;
         }
         catch (Exception ex)
         {
            throw GetException("GetVertexNames", ex);
         }
      }

      #endregion
      #region -- Events

      void RaiseEvent_TokenExpirationChanged()
      {
         try
         {
            TokenExpirationChanged?.Invoke(this, 
               new TokenExpirationChangedArgs(_graphName, _apiToken, 
                  _tokenExpiration));
         }
         catch (Exception ex)
         {
            throw GetException("RaiseEvent_TokenExpirationChanged", ex);
         }
      }

      #endregion Events
      #region -- Exception Handling

      Exception GetException(string methodName, Exception e)
      {
         string msg = "Assembly: " + _as + ", Class: " + _cn + 
            ", Method: " + methodName;
         if (_debug)
         {
            Console.WriteLine(msg + System.Environment.NewLine + e.ToString());
         }
         return new Exception(msg, e);
      }

      Exception GetException(string methodName, string info, Exception e)
      {
         string msg = "Assembly: " + _as + ", Class: " + _cn + 
            ", Method: " + methodName + System.Environment.NewLine + 
            "Information: " + info;
         if (_debug)
         {
            Console.WriteLine(msg + System.Environment.NewLine + e.ToString());
         }
         return new Exception(msg, e);
      }

      Exception GetException(string methodName, string exceptionInfo)
      {
         string msg = "Assembly: " + _as + ", Class: " + _cn + 
            ", Method: " + methodName + System.Environment.NewLine + 
            "Exception Info: " + exceptionInfo;
         if (_debug)
         {
            Console.WriteLine(msg);
         }
         return new Exception("Assembly: " + _as + ", Class: " + _cn + 
            ", Method: " + methodName, new Exception(exceptionInfo));
      }

      Exception GetException(
         string methodName, string info, string exceptionInfo)
      {
         string msg = "Assembly: " + _as + ", Class: " + _cn + 
            ", Method: " + methodName + System.Environment.NewLine + 
            "Information: " + info;
         if (_debug)
         {
            Console.WriteLine(msg);
         }

         return new Exception(msg, new Exception(exceptionInfo));
      }

      #endregion Exception Handling

   }

}
