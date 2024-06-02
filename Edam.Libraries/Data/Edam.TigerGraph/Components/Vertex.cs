using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TigerGraphComponents
{
    public class Vertex
    {

        Dictionary<string, object> _attributes;

        [JsonProperty("v_id")]
        public object Id { get; set; }

        [JsonProperty("v_type")]
        public string VertexType { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, object> Attributes
        {
            get
            {
                if(_attributes==null)
                {
                    _attributes = new Dictionary<string, object>();
                }
               return _attributes; 
            }
            set { _attributes = value; } 
        }

        public override string ToString()
        {
            string str = "";
            if (Attributes != null)
            {
                foreach (KeyValuePair<string, object> pair in Attributes)
                {
                    if(pair.Key=="id") continue;
                    str += string.Format("{0}", pair.Value);
                }
            }
            else
            {
                return string.Format("{0} {1}", VertexType, Id);
            }

            return str;
        }
       
        public string ToJson()
        {
           return JsonConvert.SerializeObject(this);
        }

        public string ToJsonForSingleVertexUpsert()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"vertices\":  {\"" + VertexType);
            sb.Append("\": {\"" + Id + "\":{");

            if (Attributes.Count() > 0)
            {                
                StringBuilder sbAtts = new StringBuilder();
                foreach (KeyValuePair<string, object> att in Attributes)
                {
                    if(sbAtts.Length == 0)
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
            else
            {
                sb.Append("}}}");
            }
            sb.Append("}");
            return sb.ToString();
        }

        public string ToJsonForVertexListUpsert()
        {
            StringBuilder sb = new StringBuilder(); 
            if (Attributes.Count() > 0)
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
            return sb.ToString();        }

        public string AttributesToJson()
        {
            return JsonConvert.SerializeObject(Attributes);
        }

    }

    public class VertexList : List<Vertex>
    {
        public VertexList() { }

        public string ToJsonForUpsert()
        {
            string vertexType = this.First().VertexType;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"vertices\":  {\"" + vertexType + "\": {");
            foreach (Vertex vert in this)
            {
                if (vert.Attributes.Count() > 0)
                {
                    sb.Append("\"" + vert.Id + "\":{");
                    StringBuilder sbAtts = new StringBuilder();
                    foreach (KeyValuePair<string, object> att in vert.Attributes)
                    {
                        if (sbAtts.Length == 0)
                        {
                            sbAtts.Append("\"" + att.Key + "\": {\"value\": \"" + att.Value + "\"}");
                        }
                        else
                        {
                            sbAtts.Append(", \"" + att.Key + "\": {\"value\": \"" + att.Value + "\"}");
                        }
                    }
                    sb.Append(sbAtts);
                }
                else
                {
                    sb.Append("\": \"" + vert.Id + "\"}");
                }
                sb.Append("}, ");
            }
            sb.Remove(sb.Length - 2, 1);
            sb.Append("}}}");
            return sb.ToString();        
        }
    }

}
