using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using JsonLD.Core;

namespace Edam.Json.LinkData
{

   /// <summary>
   /// Provide access to an JSON-LD Processor...
   /// </summary>
   public class JsonLdHelper
   {

      public string JsonText { get; set; }
      public string Context { get; set; }

      /// <summary>
      /// Using JsonText replate URI with namespace alias
      /// </summary>
      /// <returns>JObject is returned</returns>
      public JObject Compact()
      {
         var doc = JObject.Parse(JsonText);
         var context = JObject.Parse(Context);
         var opts = new JsonLdOptions();
         var compacted = JsonLdProcessor.Compact(doc, context, opts);
         return compacted;
      }

      public JArray Expand(JObject jobject)
      {
         var expanded = JsonLdProcessor.Expand(jobject);
         return expanded;
      }

      public JToken Flatten()
      {
         var doc = JObject.Parse(JsonText);
         var context = JObject.Parse(Context);
         var opts = new JsonLdOptions();
         var flatten = JsonLdProcessor.Flatten(doc, context, opts);
         return flatten;
      }

      public JToken Frame(
         string frameTextFilePath = null, string frameJsonText = null)
      {
         if (frameTextFilePath == null && frameJsonText == null)
         {
            return null;
         }

         var fjText = String.IsNullOrWhiteSpace(frameTextFilePath) ?
            File.ReadAllText(frameTextFilePath) : frameJsonText;

         var doc = JObject.Parse(JsonText);
         var context = JObject.Parse(fjText);
         var opts = new JsonLdOptions();
         var flatten = JsonLdProcessor.Frame(doc, context, opts);
         return flatten;
      }

      public Object Normalize()
      {
         var doc = JObject.Parse(JsonText);
         var opts = new JsonLdOptions();
         var normalized = JsonLdProcessor.Normalize(doc, opts);
         return normalized;
      }

      /// <summary>
      /// Prepare an RDF triplets document given a JSON document.
      /// </summary>
      /// <returns>return as RDF text</returns>
      public String ToRdf()
      {
         var doc = JObject.Parse(JsonText);
         var opts = new JsonLdOptions();
         var rdf = (RDFDataset)JsonLdProcessor.ToRDF(doc, opts);
         var rdfText = RDFDatasetUtils.ToNQuads(rdf);
         return rdfText;
      }

      /// <summary>
      /// Prepare a JSON-LD object from given RDF text.
      /// </summary>
      /// <param name="rdfText">RDF text as returned from FromRdf (above)
      /// </param>
      /// <returns></returns>
      public JToken FromRdf(string rdfText)
      {
         var opts = new JsonLdOptions();
         var jsonld = JsonLdProcessor.FromRDF(rdfText, opts);
         return jsonld;
      }

      /// <summary>
      /// Prepare a JsonLdHelper using given file paths
      /// </summary>
      /// <param name="jsonFilePath">json file path</param>
      /// <param name="jsonLdContextFilePath">context file path</param>
      /// <returns>instance of JsonLdHelper is returned</returns>
      public static JsonLdHelper FromFile(
         string jsonFilePath, string jsonLdContextFilePath)
      {
         JsonLdHelper jld = new JsonLdHelper
         {
            JsonText = File.ReadAllText(jsonFilePath),
            Context = File.ReadAllText(jsonLdContextFilePath)
         };
         return jld;
      }

      /// <summary>
      /// Prepare a JsonLdHelper using given text strings
      /// </summary>
      /// <param name="jsonText">json document text</param>
      /// <param name="jsonContextText">context text</param>
      /// <returns>instance of JsonLdHelper is returned</returns>
      public static JsonLdHelper FromText(
         string jsonText, string jsonContextText)
      {
         JsonLdHelper jld = new JsonLdHelper
         {
            JsonText = jsonText,
            Context = jsonContextText
         };
         return jld;
      }

   }

}
