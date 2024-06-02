using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Json.JsonSchemaReader;
using Edam.Json.JsonInstanceReader;
using Edam.Data.AssetUseCases;
using Edam.Json.JsonSchema;

namespace Edam.Json.JsonExplore
{

   public class JsonInspector
   {

      #region -- 1.00 - Properties and Fields

      private JsonSchemaSet m_SchemaSet = null;
      private AssetConsoleArgumentsInfo m_Arguments;

      public bool ToAssets
      {
         get { return m_Arguments.Procedure != AssetConsoleProcedure.Unknown; }
      }

      public bool ToAssetsReport
      {
         get
         {
            return m_Arguments.Procedure ==
               AssetConsoleProcedure.XsdToAssetsReport;
         }
      }

      #endregion
      #region -- 1.50 - Constructor

      public JsonInspector(AssetConsoleArgumentsInfo arguments)
      {
         m_Arguments = arguments;
         if (arguments.Procedure == AssetConsoleProcedure.JsdToAssets)
         {
            PrepareSchemaSet();
         }
      }

      #endregion
      #region -- 4.00 - Prepare JSON Schema support

      /// <summary>
      /// Given arguments prepare schema set.
      /// </summary>
      /// <returns>instance of schema set is returned</returns>
      private JsonSchemaSet PrepareSchemaSet()
      {
         if (String.IsNullOrWhiteSpace(m_Arguments.OutFileExtension))
            m_Arguments.OutFileExtension = "csv";

         NamespaceList namespaces = new NamespaceList();
         namespaces.Add(m_Arguments.Namespace);

         m_SchemaSet = new JsonSchemaSet(namespaces);

         // no URI list, try using arguments input-file if any
         if (m_Arguments.UriList.Count == 0)
         {
            var fresults = JsonSchemaInstance.FromFile(
               m_Arguments.InputFile.Full, null, m_SchemaSet);
            if (fresults.Success)
            {
               m_SchemaSet = fresults.Data;
            }
         }

         // URI list was given, scan all XSD's
         var uriList = UriResourceInfo.GetUriList(
            m_Arguments.UriList, UriResourceType.jsdjson);

         foreach (var i in uriList)
         {
            var fresults = JsonSchemaInstance.FromFile(
               i, namespaces, m_SchemaSet);
         }

         // prepare use cases
         var useCases =
            PrepareUseCases(UriResourceInfo.GetUriList(
               m_Arguments.UriList, UriResourceType.json));

         if (m_Arguments.UseCases == null)
         {
            m_Arguments.UseCases = new AssetUseCaseList();
         }

         m_Arguments.UseCases.AddRange(useCases);

         return m_SchemaSet;
      }

      #endregion
      #region -- 4.00 - Prepare Use Case support

      /// <summary>
      /// Prepare Use Cases.
      /// </summary>
      /// <param name="items">list of XML instance files</param>
      /// <returns></returns>
      private List<AssetUseCase> PrepareUseCases(List<string> items)
      {
         List<AssetUseCase> cases = new List<AssetUseCase>();
         foreach (var i in items)
         {
            var jsonText = System.IO.File.ReadAllText(i);
            var uc = new JsonHelper.JsonAssetUseCase(m_Arguments, jsonText);
            if (uc.Success)
               cases.Add(uc.UseCase);
         }
         return cases;
      }

      #endregion
      #region -- 4.00 - Manage Asset to Report support

      /// <summary>
      /// To Ouput / Report.
      /// </summary>
      /// <param name="asset">AssetData instance</param>
      /// <param name="toOutput">true to output</param>
      /// <returns>instance of AssetData is returned</returns>
      private AssetData ToOutput(AssetData asset, bool toOutput)
      {
         if (asset == null)
         {
            return null;
         }

         asset.MapDataTypes(m_Arguments.TextMapFilePath);
         asset.SetUseCases(m_Arguments);

         if (toOutput)
            asset.ToOutput(null, m_Arguments);

         return asset;
      }

      /// <summary>
      /// Output to Report.
      /// </summary>
      public void ToOutput()
      {
         if (ToAssetsReport)
            InspectSchema(true);
      }

      #endregion
      #region -- 4.00 - Inspect Schema or Instance support

      /// <summary>
      /// Inspect Schema and return AssetData.
      /// </summary>
      /// <param name="toOutput">true to output to report</param>
      /// <returns>AssetData instance is returned</returns>
      private AssetData InspectSchema(bool toOutput)
      {
         if (m_SchemaSet.Count == 0)
            return null;

         m_SchemaSet.Namespace = m_Arguments.Namespace;
         IJsonInspector i = new JsonSchemaInspector(
            m_SchemaSet, m_Arguments.Namespace);
         i.Inspect();

         m_Arguments.AssetDataItems = 
            m_Arguments.AssetDataItems ?? new AssetDataList();
         m_Arguments.AssetDataItems.Add(i.Asset);

         return ToOutput(i.Asset, toOutput);
      }

      /// <summary>
      /// Inspect Instance and return AssetData.
      /// </summary>
      /// <param name="toOutput">true to output to report</param>
      /// <returns>AssetData instance is returned</returns>
      public AssetData InspectInstance(bool toOutput)
      {
         IJsonInspector i = new JsonInstanceInspector(m_Arguments);

         i.Inspect();

         return ToOutput(i.Asset, toOutput);
      }

      /// <summary>
      /// Inspect JSON Schema or Instance (based on arguments)
      /// </summary>
      /// <param name="arguments">arguments</param>
      /// <param name="toOutput">true to output a Report</param>
      public AssetData Inspect(
         AssetConsoleArgumentsInfo arguments, bool toOutput = false)
      {
         AssetData assetData;
         switch(arguments.Procedure)
         {
            case AssetConsoleProcedure.JsdToAssets:
               assetData = InspectSchema(toOutput);
               break;
            case AssetConsoleProcedure.JsonToAssets:
               assetData = InspectInstance(toOutput);
               break;
            default:
               assetData = null;
               break;
         }
         return assetData;
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="arguments">list of string arguments</param>
      public static AssetData ToAssetList(AssetConsoleArgumentsInfo arguments)
      {
         JsonInspector i = new JsonInspector(arguments);
         return i.Inspect(arguments, false);
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="args">list of string arguments</param>
      public static AssetData ToAssetList(String[] args)
      {
         var arguments = AssetConsoleArgumentsInfo.FromArguments(args);
         return ToAssetList(arguments);
      }

      #endregion
      #region -- 4.00 - To File / Report support

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="arguments">list of string arguments</param>
      public static void ToFile(AssetConsoleArgumentsInfo arguments)
      {
         var i = new JsonInspector(arguments);
         i.ToOutput();
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="args">list of string arguments</param>
      public static void ToFile(String[] args)
      {
         var arguments = AssetConsoleArgumentsInfo.FromArguments(args);
         ToFile(arguments);
      }

      #endregion

   }

}
