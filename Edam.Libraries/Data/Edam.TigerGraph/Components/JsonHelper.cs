using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TigerGraphConnector;

namespace TigerGraphComponents
{
   public class JsonHelper
    {

        public static Vertex[] DeserializeVertexes(string jsonString)
        {
            try
            {
                if (jsonString == null) { return null; }
                JArray jsonObject = (JArray)JsonConvert.DeserializeObject(jsonString);
                if (jsonObject.Children().Count() == 1)
                {
                    if (jsonObject.First is Array)
                    {
                        Vertex vertex = jsonObject[0].First.ToObject<Vertex>();
                        return new Vertex[] { vertex };
                    }
                    else
                    {
                        JObject vertexes = (JObject)jsonObject.First;
                        JProperty prop1 = (JProperty)vertexes.First;
                        string name = prop1.Name;
                        Vertex[] vertexArray2 = prop1.Value.ToObject<Vertex[]>();
                        return vertexArray2;
                    }

                }
                else if (jsonObject.Children().Count() > 1)
                {
                    if (jsonObject.First is JObject)
                    {
                        try
                        {
                            Vertex[] vertexArray1 = JsonConvert.DeserializeObject<Vertex[]>(jsonString);
                            if (vertexArray1[0].VertexType == null)
                            {
                                JObject vertexes = (JObject)jsonObject.First;
                                JProperty prop1 = (JProperty)vertexes.First;
                                string name = prop1.Name;
                                Vertex[] vertexArray2 = prop1.Value.ToObject<Vertex[]>();
                                return vertexArray2;
                            }
                            return vertexArray1;
                        }
                        catch (Exception e)
                        {

                        }

                    }
                }
                //Vertex vertexArray = jsonObject[0].First.ToObject<Vertex>();
                //Vertex[] vertexArray = JsonConvert.DeserializeObject<Vertex[]>(jsonString);
                //JArray jsonObject = (JArray)JsonConvert.DeserializeObject(jsonString);

                //JObject vertexes = (JObject)jsonObject.First;
                //JProperty prop1 = (JProperty)vertexes.First;
                //string name = prop1.Name;
                //Vertex[] vertexArray = prop1.Value.ToObject<Vertex[]>();

                return new Vertex[1];
                //return vertexArray;
            }
            catch (Exception e)
            {
                Vertex[] vertexArray = JsonConvert.DeserializeObject<Vertex[]>(jsonString);
                return vertexArray;
            }

        }

        public static Edge[] DeserializeEdges(string jsonString)
        {
            JArray jsonObject = (JArray)JsonConvert.DeserializeObject(jsonString);

            if (jsonObject.Count == 2)
            {
                JObject eddges = (JObject)jsonObject.Last;
                JProperty prop2 = (JProperty)eddges.First;
                string name2 = prop2.Name;
                Edge[] edgeArray = prop2.Value.ToObject<Edge[]>();
                return edgeArray;
            }

            return new Edge[0];
        }

        public static string FormatAttributeValue(object val)
        {
            string newVal;
            if(val == null)
            {
                newVal = "";
            }
            else if (val is string || val is DateTime)// || val is bool)
            {
                newVal = "\"" + val.ToString() + "\"";
            }
            else if (val is bool)
            {
                newVal = val.ToString().ToLower();
            }
            else
            {
                newVal = val.ToString();
            }
            return newVal;
        }


    }
}
