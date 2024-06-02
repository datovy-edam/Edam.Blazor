using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Net.Http;

namespace TigerGraph.Connector
{
   public partial class TigerGraphConnection
   {

      /// <summary>
      /// Returns the list of vertex type names of the graph.
      /// </summary>
      /// <param name="includeUDTs"></param>
      /// <returns></returns>
      public dynamic GetVertexTypes(bool includeUDTs)
      {
         try
         {
            Dictionary<string, object> _Result = GetSchema(includeUDTs);
            JArray values = JArray.Parse(JsonConvert.SerializeObject(_Result["VertexTypes"]));
            List<String> _List = new List<string>();
            foreach (JObject item in values.Children())
            {
               _List.Add(item.GetValue("Name").ToString());
            }
            return _List;
         }
         catch (Exception ex)
         {
            throw GetException("GetVertexTypes", ex);
         }
      }

      /// <summary>
      /// Returns the details of the specified vertex type.
      /// </summary>
      /// <param name="vertexType"></param>
      /// <returns></returns>
      public dynamic GetVertexType(string vertexType)
      {
         try
         {
            Dictionary<string, object> _Result = GetSchema(false);
            JArray values = JArray.Parse(JsonConvert.SerializeObject(_Result["VertexTypes"]));
            List<String> _List = new List<string>();
            foreach (JObject item in values.Children())
            {
               if (item.GetValue("Name").ToString() == vertexType)
                  return item;
            }
            return "";
         }
         catch (Exception ex)
         {
            throw GetException("GetVertexType", ex);
         }
      }

      /// <summary>
      /// Returns the number of vertices.
      /// Endpoint:      GET /graph/{graph_name}/vertices
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-graph-graph_name-vertices
      /// Endpoint:      POST /builtins
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#stat_vertex_number
      /// If WHERE condition is not specified, use /builtins else user /vertices
      /// </summary>
      /// <param name="vertexType">If `vertexType` = "*": vertex count of all vertex types (`where` cannot be specified in this case)
      /// If `vertexType` is specified only: vertex count of the given type
      /// If `vertexType` and `where` are specified: vertex count of the given type after filtered by `where` condition(s)
      /// </param>
      /// <param name="where">For valid values of `where` condition, see https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter</param>
      /// <param name="Count_Only"></param>
      /// <returns></returns>
      public dynamic GetVertexCount(string vertexType, string where, bool Count_Only = true)
      {
         string url = _restppUrl;
         string data;
         dynamic responseContent;
         var _List = new List<Tuple<string, string>>();
         try
         {
            if (string.IsNullOrEmpty(vertexType))
            {
               throw new Exception("No vertex type defined. Either use * for all or specific value.");
            }
            else if (!string.IsNullOrEmpty(where))
            {
               if (string.IsNullOrEmpty(vertexType))
               {
                  throw new Exception("Vertex Type is required when passing where condition.");
               }
               else if (vertexType == "*")
               {
                  throw new Exception("VertexType cannot be \"*\" if where condition is specified.");
               }
               url += "/graph/" + _graphName + "/vertices/" + vertexType + "?count_only=" + Count_Only + "&filter=" + where;
               responseContent = Get(url);
            }
            else
            {
               url += "/builtins/" + _graphName;
               data = "{" + QT + "function" + QT + ":" + QT + "stat_vertex_number" + QT + "," + QT + "type" + QT + ":" + QT + vertexType + QT + "}";
               responseContent = Post(url, AuthMode.Token, null, data);
            }

            foreach (var item in JsonConvert.DeserializeObject(responseContent))
            {
               _List.Add(new Tuple<string, string>(item["v_type"].ToString(), item["count"].ToString()));
            }
            return _List;
         }
         catch (Exception ex)
         {
            throw GetException("GetVertexCount", ex);
         }
      }


      ///// <summary>
      ///// If vertex is not yet present in graph, it will be created.
      ///// If it's already in the graph, its attributes are updated with the values specified in the request. An optional operator controls how the attributes are updated.
      ///// For valid values of <operator> see: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#post-graph-graph_name-upsert-the-given-data
      ///// Returns a single number of accepted (successfully upserted) vertices (0 or 1).
      ///// Endpoint:      POST /graph
      ///// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#post-graph-graph_name-upsert-the-given-data
      ///// </summary>
      ///// <param name="vertexType"></param>
      ///// <param name="vertexId"></param>
      ///// <param name="_Attributes_Json">The `attributes` argument is expected to be a dictionary in this format:
      ///// {<attribute_name>: <attribute_value>|(<attribute_name>, <operator>), …}
      ///// </param>
      ///// <returns></returns>
      //public dynamic UpsertVertex(string vertexType, string vertexId, string _Attributes_Json)
      //{
      //    try
      //    {
      //        if (string.IsNullOrEmpty(vertexId) || string.IsNullOrEmpty(vertexType))
      //        {
      //            return "";
      //        }
      //        else
      //        {
      //            string url = _restppUrl + "/graph/" + _graphName;
      //            string data = "{ \"vertices\" : { \"" + vertexType + "\": { \"" + vertexId + "\" : " + _Attributes_Json + " } } }";
      //            Dictionary<string, string> headers = new Dictionary<string, string>();
      //            headers.Add("Accept", "application/json");
      //            return Post(url, AuthMode.Token, headers, data);
      //        }
      //    }
      //    catch (Exception ex)
      //    {
      //        throw GetException("UpsertVertex", ex);
      //    }
      //}

      //public dynamic UpsertVertex(string vertexJaon)
      //{
      //    try
      //    {
      //            string url = _restppUrl + "/graph/" + _graphName;
      //            string data = vertexJaon;
      //            Dictionary<string, string> headers = new Dictionary<string, string>();
      //            headers.Add("Accept", "application/json");
      //            return Post(url, AuthMode.Token, headers, data);
      //    }
      //    catch (Exception ex)
      //    {
      //        throw GetException("UpsertVertex", ex);
      //    }
      //}

      /// <summary>
      /// Retrieves vertices of the given vertex type.
      /// NOTE:          The primary ID of a vertex instance is NOT an attribute, thus cannot be used in above arguments.
      ///                Use `getVerticesById` if you need to retrieve by vertex ID.
      /// Endpoint:      GET /graph/{graph_name}/vertices
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-graph-graph_name-vertices
      /// </summary>
      /// <param name="vertexTypes"></param>
      /// <param name="select">Comma separated list of vertex attributes to be retrieved or omitted.
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#select
      /// </param>
      /// <param name="where">Comma separated list of conditions that are all applied on each vertex' attributes.
      /// The conditions are in logical conjunction (i.e. they are "AND'ed" together).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter
      /// </param>
      /// <param name="limit">Maximum number of vertex instances to be returned (after sorting).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#limit
      /// </param>
      /// <param name="sort">Comma separated list of attributes the results should be sorted by.
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#sort
      /// </param>
      /// <param name="timeout"></param>
      /// <returns></returns>
      public dynamic GetVertices(string vertexTypes, string select = "", string where = "", string limit = "", string sort = "", int timeout = 500)
      {
         try
         {
            string url = _restppUrl + "/graph/" + _graphName + "/vertices/" + vertexTypes;
            url += Create_URL_Extended(select, where, limit, sort, timeout);
            dynamic responseContent = Get(url);
            return responseContent;
         }
         catch (Exception ex)
         {
            throw GetException("GetVertices", ex);
         }
      }

      /// <summary>
      /// Retrieves vertices of the given vertex type, identified by their ID.
      /// Endpoint:      GET /graph/{graph_name}/vertices
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-graph-graph_name-vertices
      /// </summary>
      /// <param name="vertexType"></param>
      /// <param name="vertexIds">A single vertex ID or a list of vertex IDs.</param>
      /// <returns></returns>
      public dynamic GetVerticesById(string vertexType, object vertexIds)
      {
         try
         {
            if (vertexIds == null || vertexIds.ToString().Trim().Length == 0)
            {
               throw new Exception("No vertex IDs were specified.");
            }

            List<object> ids = new List<object>();
            if (vertexIds is int || vertexIds is long || vertexIds is string || vertexIds is Guid)
            {
               ids.Add(vertexIds);
            }
            else if (vertexIds is List<object>)
            {
               foreach (var item in vertexIds as List<object>)
               {
                  ids.Add(item);
               }
            }
            else if (vertexIds is object[])
            {
               foreach (var item in vertexIds as object[])
               {
                  ids.Add(item);
               }
            }
            List<dynamic> results = new List<dynamic>();
            foreach (var id in ids)
            {
               results.Add(Get(_restppUrl + "/graph/" + _graphName + "/vertices/" + vertexType + "/" + id.ToString()));
            }
            return results;
         }
         catch (Exception ex)
         {
            throw GetException("GetVerticesById", ex);
         }
      }

      /// <summary>
      /// Returns vertex attribute statistics.
      /// Endpoint:      POST /builtins
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#stat_vertex_attr
      /// </summary>
      /// <param name="vertexTypes">A single vertex type name or a list of vertex types names or '*' for all vertex types.</param>
      /// <param name="skipNA">Skip those non-applicable vertices that do not have attributes or none of their attributes have statistics gathered.</param>
      /// <returns></returns>
      public dynamic GetVertexStats(object vertexTypes, bool skipNA)
      {
         try
         {
            List<string> vertextNames = new List<string>();
            Dictionary<String, object> _List = new Dictionary<string, object>();

            vertextNames = GetVertexNames(vertexTypes);

            if (vertextNames.Count == 0)
            {
               throw new Exception("Input parameter 'vertexTypes' was not: A vertext name, '*' or List<string>.");
            }

            var url = _restppUrl + "/builtins/" + _graphName;

            foreach (var vtName in vertextNames)
            {
               string Params = "{" + QT + "function" + QT + ":" + QT + "stat_vertex_attr" + QT + "," + QT + "type" + QT + ":" + QT + vtName + QT + "}";
               dynamic responseContent = Post(url, AuthMode.Token, null, Params);

               JArray values = JArray.Parse(JsonConvert.SerializeObject(responseContent));

               foreach (JObject item in values)
               {
                  _List.Add(item["v_type"].ToString(), item["attributes"]);
               }
            }
            return JsonConvert.SerializeObject(_List);
         }
         catch (Exception ex)
         {
            throw GetException("GetVertices", ex);
         }
      }

      /// <summary>
      /// Deletes vertices from graph.
      /// </summary>
      /// <param name="vertexTypes"></param>
      /// <param name="where">Comma separated list of conditions that are all applied on each vertex' attributes.
      /// The conditions are in logical conjunction (i.e. they are "AND'ed" together).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter
      /// </param>
      /// <param name="limit">Maximum number of vertex instances to be returned (after sorting).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#limit
      /// Must be used with `sort`.
      /// </param>
      /// <param name="sort">Comma separated list of attributes the results should be sorted by.
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#sort
      /// Must be used with `limit`.
      /// </param>
      /// <param name="permanent">If true, the deleted vertex IDs can never be inserted back, unless the graph is dropped or the graph store is cleared.</param>
      /// <param name="timeout">Time allowed for successful execution (0 = no limit, default).</param>
      /// <returns></returns>
      public dynamic DeleteVertices(string vertexTypes, string where = "", string limit = "", string sort = "", bool permanent = false, int timeout = 0)
      {
         try
         {
            if (string.IsNullOrEmpty(where))
            {
               throw new Exception("Attempt to delete all records. Attempt foiled.");
            }
            string url = _restppUrl + "/graph/" + _graphName + "/vertices/" + vertexTypes;
            url += Create_URL_Extended("", where, limit, sort, timeout);
            return Delete(url);
         }
         catch (Exception ex)
         {
            throw GetException("DeleteVertices", ex.InnerException.ToString());
         }
      }

      /// <summary>
      /// Deletes vertices from graph identified by their ID.
      /// Returns a single number of vertices deleted.
      /// Endpoint:      DELETE /graph/{graph_name}/vertices
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#delete-graph-graph_name-vertices
      /// </summary>
      /// <param name="vertexType"></param>
      /// <param name="vertexIds">A single vertex ID or a list of vertex IDs</param>
      /// <param name="permanent">If true, the deleted vertex IDs can never be inserted back, unless the graph is dropped or the graph store is cleared.</param>
      /// <param name="timeout">Time allowed for successful execution (0 = no limit, default).</param>
      /// <returns></returns>
      public dynamic DeleteVerticesById(string vertexType, string vertexIds, bool permanent = false, int timeout = 0)
      {
         try
         {
            if (string.IsNullOrEmpty(vertexType) || string.IsNullOrEmpty(vertexIds))
            {
               throw new Exception("Attempt to delete wihtout VertexType or VertexID.");
            }
            string url = _restppUrl + "/graph/" + _graphName + "/vertices/" + vertexType + "/" + vertexIds;
            url += "?&permanent=" + permanent + "&timeout=" + timeout;
            return Delete(url);
         }
         catch (Exception ex)
         {
            throw GetException("DeleteVerticesById", ex);
         }
      }


   }
}
