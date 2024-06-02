using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Net.Http;

namespace TigerGraph.Connector
{
   public partial class TigerGraphConnection
   {

      /// <summary>
      /// Returns the list of edge type names of the graph.
      /// </summary>
      /// <returns></returns>
      public dynamic GetEdgeTypes()
      {
         try
         {
            dynamic responseContent = GetSchema(false);
            return responseContent["EdgeTypes"];
         }
         catch (Exception ex)
         {
            throw GetException("GetEdgeTypes", ex);
         }
      }

      /// <summary>
      /// Returns the details of vertex type.
      /// </summary>
      /// <param name="typeName"></param>
      /// <returns></returns>
      public dynamic GetEdgeType(string typeName)
      {
         try
         {
            dynamic responseContent = GetEdgeTypes();
            foreach (var item in responseContent)
            {
               if (item.Name == typeName)
               {
                  return item;
               }
            }
            return null;
         }
         catch (Exception ex)
         {
            throw GetException("GetEdgeType", ex);
         }
      }


      /// <summary>
      /// Return the number of edges.
      /// Uses:
      /// If `edgeType` = "*": edge count of all edge types (no other arguments can be specified in this case).
      /// If `edgeType` is specified only: edge count of the given edge type.
      /// If `sourceVertexType`, `edgeType`, `targetVertexType` are specified: edge count of the given edge type between source and target vertex types.
      /// If `sourceVertexType`, `sourceVertexId` are specified: edge count of all edge types from the given vertex instance.
      /// If `sourceVertexType`, `sourceVertexId`, `edgeType` are specified: edge count of all edge types from the given vertex instance.
      /// If `sourceVertexType`, `sourceVertexId`, `edgeType`, `where` are specified: the edge count of the given edge type after filtered by `where` condition.
      ///
      /// If `targetVertexId` is specified, then `targetVertexType` must also be specified.
      /// If `targetVertexType` is specified, then `edgeType` must also be specified.
      ///
      /// For valid values of `where` condition, see https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter
      ///
      /// Returns a dictionary of <edge_type>: <edge_count> pairs.
      ///
      /// Endpoint:      GET /graph/{graph_name}/edges
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-graph-graph_name-edges
      /// Endpoint:      POST /builtins
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#stat_edge_number        /// 
      /// 
      /// </summary>
      /// <param name="sourceVertexType"></param>
      /// <param name="sourceVertexId"></param>
      /// <param name="edgeType"></param>
      /// <param name="targetVertexType"></param>
      /// <param name="targetVertexId"></param>
      /// <param name="where">Comma separated list of conditions that are all applied on each edge's attributes.
      /// The conditions are in logical conjunction (i.e. they are "AND'ed" together).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter
      /// </param>
      /// <returns></returns>
      public dynamic GetEdgeCount(string sourceVertexType, string sourceVertexId, string edgeType, string targetVertexType, string targetVertexId, string where)
      {
         try
         {
            dynamic responseContent;
            string data = "";
            if (!string.IsNullOrEmpty(where) || (!string.IsNullOrEmpty(sourceVertexType) && !string.IsNullOrEmpty(sourceVertexId)))
            {
               string url = _restppUrl + "/graph/" + _graphName + "/edges/";

               if (string.IsNullOrEmpty(sourceVertexType) || string.IsNullOrEmpty(sourceVertexId))
               {
                  throw new Exception("sourceVertexType or sourceVertexId were null or empty.  They are required.");
               }

               url += sourceVertexType + "/" + sourceVertexId;

               if (edgeType != "*")
               {
                  url += "/" + edgeType;

                  if (!string.IsNullOrEmpty(targetVertexType))
                  {
                     url += "/" + targetVertexType;

                     if (!string.IsNullOrEmpty(targetVertexId))
                     {
                        url += "/" + targetVertexId;
                     }

                     if (!string.IsNullOrEmpty(where))
                     {
                        url += "&filter=" + where;
                     }
                  }
               }
               url += "?count_only=true";
               return Get(url);
            }
            else
            {
               string url = _restppUrl + "/builtins/" + _graphName;
               data = "{\"function\":\"stat_edge_number\",\"type\":\"" + edgeType + "\"";
               if (!string.IsNullOrEmpty(sourceVertexType))
               {
                  data += ",\"from_type\":\"" + sourceVertexType + "\"";
               }

               if (!string.IsNullOrEmpty(targetVertexType))
               {
                  data += ",\"to_type\":\"" + targetVertexType + "\"";
               }
               data += "}";
               responseContent = Post(url, AuthMode.Token, null, data);
            }

            Dictionary<string, string> _Result_Dic = new Dictionary<string, string>();
            foreach (var item in responseContent)
            {
               _Result_Dic.Add(Convert.ToString(item.e_type), Convert.ToString(item.count));
            }
            return _Result_Dic;
         }
         catch (Exception ex)
         {
            throw GetException("GetEdgeCount", ex);
         }
      }

      /// <summary>
      /// Retrieves edges of the given edge type.
      /// Only `sourceVertexType` and `sourceVertexId` are required.
      /// If `targetVertexId` is specified, then `targetVertexType` must also be specified.
      /// If `targetVertexType` is specified, then `edgeType` must also be specified.
      /// Endpoint:      GET /graph/{graph_name}/vertices
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#get-graph-graph_name-vertices
      /// </summary>
      /// <param name="sourceVertexType"></param>
      /// <param name="sourceVertexId"></param>
      /// <param name="edgeType"></param>
      /// <param name="targetVertexType"></param>
      /// <param name="targetVertexId"></param>
      /// <param name="select">Comma separated list of edge attributes to be retrieved or omitted.
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#select
      /// </param>
      /// <param name="where">Comma separated list of conditions that are all applied on each edge's attributes.
      /// The conditions are in logical conjunction (i.e. they are "AND'ed" together).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter
      /// </param>
      /// <param name="limit">Maximum number of edge instances to be returned (after sorting).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#limit
      /// </param>
      /// <param name="sort">Comma separated list of attributes the results should be sorted by.
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#sort
      /// </param>
      /// <param name="timeout"></param>
      /// <returns></returns>
      public dynamic GetEdges(string sourceVertexType, string sourceVertexId, string edgeType, string targetVertexType, string targetVertexId, string select, string where, string limit, string sort, int timeout)
      {
         try
         {
            if (string.IsNullOrEmpty(sourceVertexType) || string.IsNullOrEmpty(sourceVertexId))
            {
               throw new Exception("Source Vertex Type or Source Vertex Id were not specified.  Both are required.");
            }

            string url = _restppUrl + "/graph/" + _graphName + "/edges/" + sourceVertexType + "/" + sourceVertexId;

            if (!string.IsNullOrEmpty(edgeType))
            {
               url += "/" + edgeType;
            }

            if (!string.IsNullOrEmpty(targetVertexType) && !string.IsNullOrEmpty(targetVertexId))
            {
               url += "/" + targetVertexType + "/" + targetVertexId;
            }

            url += Create_URL_Extended(select, where, limit, sort, timeout);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Add(
               "Authorization", "Bearer " + _apiToken);
            client.Timeout = new TimeSpan(-1);
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;

            var response = client.Send(request);

            string responseData = response.Content.ReadAsStringAsync().Result;
            dynamic responseContent = 
               JsonConvert.DeserializeObject(responseData);
            return responseContent;
         }
         catch (Exception ex)
         {
            throw GetException("GetEdges", ex);
         }
      }

      /// <summary>
      /// Returns edge attribute statistics.
      /// Endpoint:      POST /builtins
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#stat_edge_attr
      /// </summary>
      /// <param name="edgeTypes">A single edge type name or a list of edges types names or '*' for all edges types</param>
      /// <param name="skipNA">Skip those edges that do not have attributes or none of their attributes have statistics gathered</param>
      /// <returns></returns>
      public dynamic GetEdgeStats(string edgeTypes, bool skipNA)
      {
         try
         {
            string url = _restppUrl + "/builtins/" + _graphName;
            string data = "{" + QT + "function" + QT + ":" + QT + "stat_edge_attr" + QT + "," + QT + "type" + QT + ":" + QT + edgeTypes + QT + "}";
            return Post(url, AuthMode.Token, null, data);
         }
         catch (Exception ex)
         {
            throw GetException("GetEdgeStats", ex);
         }
      }

      /// <summary>
      /// Deletes edges from the graph.
      /// Only `sourceVertexType` and `sourceVertexId` are required.
      /// If `targetVertexId` is specified, then `targetVertexType` must also be specified.
      /// If `targetVertexType` is specified, then `edgeType` must also be specified.
      /// Endpoint:      DELETE /graph/{/graph_name}/edges
      /// Documentation: https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#delete-graph-graph_name-edges
      /// </summary>
      /// <param name="sourceVertexType"></param>
      /// <param name="sourceVertexId"></param>
      /// <param name="edgeType"></param>
      /// <param name="targetVertexType"></param>
      /// <param name="targetVertexId"></param>
      /// <param name="where">Comma separated list of conditions that are all applied on each edge's attributes.
      /// The conditions are in logical conjunction (i.e. they are "AND'ed" together).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#filter
      /// </param>
      /// <param name="limit">Maximum number of edge instances to be returned (after sorting).
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#limit
      /// </param>
      /// <param name="sort">Comma separated list of attributes the results should be sorted by.
      /// See https://docs.tigergraph.com/dev/restpp-api/built-in-endpoints#sort
      /// </param>
      /// <param name="timeout">Time allowed for successful execution (0 = no limit, default).</param>
      /// <returns>Returns a dictionary of <edge_type>: <deleted_edge_count> pairs.</returns>
      public dynamic DeleteEdges(string sourceVertexType, string sourceVertexId, string edgeType = null, string targetVertexType = null,
          string targetVertexId = null, string where = "", string limit = "", string sort = "", int timeout = 0)
      {
         try
         {
            if (string.IsNullOrEmpty(sourceVertexType) || string.IsNullOrEmpty(sourceVertexId))
            {
               throw new Exception("Source Vertex Type or Source Vertex Id both should be specified");
            }
            string url = _restppUrl + "/graph/" + _graphName + "/edges/" + sourceVertexType + "/" + sourceVertexId;
            if (!string.IsNullOrEmpty(edgeType))
            {
               url += "/" + edgeType;
            }
            if (!string.IsNullOrEmpty(targetVertexType) && !string.IsNullOrEmpty(targetVertexId))
            {
               url += "/" + targetVertexType + "/" + targetVertexId;
            }
            url += Create_URL_Extended("", where, limit, sort, timeout);
            dynamic responseContent = Delete(url);
            Dictionary<string, string> result_Dic = new Dictionary<string, string>();
            foreach (var item in responseContent)
            {
               result_Dic.Add(Convert.ToString(item.e_type), Convert.ToString(item.count));
            }
            //return JsonConvert.SerializeObject(result_Dic);
            return result_Dic;
         }
         catch (Exception ex)
         {
            throw GetException("DeleteEdges", ex);
         }
      }

   }
}
