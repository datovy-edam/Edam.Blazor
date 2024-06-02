using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;
using Edam.Json.JsonExplore;
using Edam.Json.JsonSchema;

namespace Edam.Json.JsonSchemaReader
{

   public class JsonSchemaInspector : JsonSchemaInstance, IJsonInspector
   {

      #region -- Declarations

      private JsonSchemaSet m_SchemaSet;
      public AssetData Asset { get; set; }
      public NamespaceInfo DefaultNamespace { get; set; }
      public AssetConsoleArgumentsInfo Arguments { get; set; }

      #endregion
      #region -- Constructor / Destructor

      public JsonSchemaInspector(JsonSchemaSet schemaSet,
         NamespaceInfo defaultNamespace)
      {
         if (schemaSet == null || schemaSet.Count == 0)
            throw new Exception(
               "Provided Schema set is empty. Json cannot be generated.");
         m_SchemaSet = schemaSet;

         DefaultNamespace = defaultNamespace;
         m_SchemaSet.Namespace = DefaultNamespace;

         //schemaSet.ValidationEventHandler +=
         //   new ValidationEventHandler(ValidationCallBack);
         Asset = new AssetData(schemaSet.Namespace, AssetType.Schema,
            schemaSet.VersionId);
      }

      #endregion
      #region -- JSON Validation support

      private static void ValidationCallBack(
         object sender, EventArgs args)
      {
         Console.WriteLine("Error in Schema - ");
         //Console.WriteLine(args.Message);
      }

      #endregion
      #region -- Manage Namespaces

      public static NamespaceList SetNamespaces(
         NamespaceList namespaces, JsonSchemaSet schemaSet)
      {
         var list = namespaces ?? new NamespaceList();
         var slist = schemaSet.Schemas;
         foreach (var z in slist)
         {
            var l = ((JsonSchemaInfo)z).Namespaces.ToArray();
            foreach (var i in l)
            {
               var fitem = list.Find((x) => x.Prefix == i.Prefix);
               if (fitem == null)
               {
                  var e = new NamespaceInfo(i.Prefix, i.Uri);
                  list.Add(e);
               }
            }
         }
         return list;
      }

      #endregion
      #region -- Annotations support

      /// <summary>
      /// 
      /// </summary>
      /// <param name="annotation"></param>
      /// <param name="asset"></param>
      public static void AssetSchemaAnnotation(
         string annotation, IAssetElement asset)
      {
         if (string.IsNullOrWhiteSpace(annotation))
            return;
         asset.Annotation.Add(annotation);
      }

      #endregion
      #region -- Schema to DataElement Registration

      public void Inspect()
      {
         NamespaceList namespaces = new NamespaceList();
         namespaces.Add(DefaultNamespace);
         SetNamespaces(namespaces, m_SchemaSet);
         Asset = ToDataAsset(m_SchemaSet, namespaces);
      }

      #endregion

   }

}
