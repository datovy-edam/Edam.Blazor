using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TigerGraphComponents
{
    public class Edge
    {

        Dictionary<string, object> _attributes;

        [JsonProperty("e_type")]
        public string EdgeType { get; set; }

        [JsonProperty("from_id")]
        public object FromId { get; set; }

        [JsonProperty("from_type")]
        public string FromVertexType { get; set; }

        [JsonProperty("to_id")]
        public object ToId { get; set; }

        [JsonProperty("to_type")]
        public string ToVertexType { get; set; }

        [JsonProperty("directed")]
        public bool Directed { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, object> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new Dictionary<string, object>();
                }
                return _attributes;
            }
            set { _attributes = value; }
        }


        public string ToJsonForSingleEdgeUpsert()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"edges\": {");
            sb.Append("\"" + FromVertexType + "\": {\"" + FromId + "\": {");
            sb.Append("\"" + EdgeType + "\": {");
            sb.Append("\"" + ToVertexType + "\": {\"" + ToId + "\": {");
            if (_attributes == null || _attributes.Count == 0)
            {
                sb.Append("}}}");
            }
            else
            {
                StringBuilder sbAtts = new StringBuilder();
                foreach (KeyValuePair<string, object> att in Attributes)
                {
                    if (sbAtts.Length == 0)
                    {
                        sbAtts.Append("\"" + att.Key + "\": {\"value\": " + JsonHelper.FormatAttributeValue(att.Value) + "}");
                    }
                    else
                    {
                        sbAtts.Append(", \"" + att.Key + "\": {\"value\": " + JsonHelper.FormatAttributeValue(att.Value) + "}");
                    }
                }
                sb.Append(sbAtts + "}}}");
            }
            sb.Append("}}}}");
            return sb.ToString();
        }

    }

    public class EdgeList : List<Edge>
    {
        public EdgeList() { }

        public string ToJsonForUpsert()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"edges\": {");
            foreach (Edge edge in this)
            {
                sb.Append("\"" + edge.FromVertexType + "\": {\"" + edge.FromId + "\": {");
                sb.Append("\"" + edge.EdgeType + "\": {");
                sb.Append("\"" + edge.ToVertexType + "\": {\"" + edge.ToId + "\": {");
                if (edge.Attributes == null || edge.Attributes.Count == 0)
                {
                    sb.Append("}}}");
                }
                else
                {
                    StringBuilder sbAtts = new StringBuilder();
                    foreach (KeyValuePair<string, object> att in edge.Attributes)
                    {
                        if (sbAtts.Length == 0)
                        {
                            sbAtts.Append("\"" + att.Key + "\": {\"value\": " + FormatAttributeValue(att.Value) + "}");
                        }
                        else
                        {
                            sbAtts.Append(", \"" + att.Key + "\": {\"value\": " + FormatAttributeValue(att.Value) + "}");
                        }
                    }
                    sb.Append(sbAtts + "}}}");
                }
                sb.Append("}},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}}");
            return sb.ToString();
        }

        object FormatAttributeValue(object val)
        {
            object newVal;
            if (val is string || val is DateTime)// || val is bool)
            {
                newVal = "\"" + val + "\"";
            }
            else if (val is bool)
            {
                newVal = "\"" + val.ToString().ToLower() + "\"";
            }
            else
            {
                newVal = val;
            }
            return newVal;
        }
    }



    //



}

