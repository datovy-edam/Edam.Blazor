using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

// -----------------------------------------------------------------------------
using am = Edam.Data.AssetManagement;
using Edam.Data.AssetManagement;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetManagement.Writers.Ddl;
using Edam.Data.AssetManagement.Writers.Jsd;
using Edam.Data.AssetManagement.Writers.Xsd;
using Edam.Data.AssetManagement.Writers.Gsql;
using Edam.Data.Schema;
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Data.AssetProject;
using Edam.Application;
using Edam.Diagnostics;
using Edam.InOut;

using keys = Edam.Data.AssetSchema.AssetResourceHelper;

using Edam.Xml.XmlExplore;
using Edam.Json.JsonExplore;
using Edam.Data.Schema.DataDefinitionLanguage;
using Edam.DataObjects.ReferenceData;
using Edam.DataObjects.Documents;
using ddl = Edam.Data.Schema.SchemaObject;
using Edam.Data.AssetDb.Readers;
using Edam.Json.JsonSchema;
using Edam.Data.Lexicon;

namespace Edam.Data.AssetConsole.Services
{

    public class ArgumentSet
   {
      public List<String> Arguments { get; set; }
      public ArgumentSet()
      {
         Arguments = new List<string>();
      }
   }

   public class AssetServiceHelper
   {

      #region -- Procedure Registry Support

      public const string CLASS_NAME = "AssetConsoleHelper";

      //public static readonly string SAMPLES_FILEPATH =
      //  "C:/prjs/Edam/Edam.Wab/Edam.Consoles/Edam.AmConsole/Samples/";

      public const string USE_ARGS_FILE = "-useargsfile";
      public const string USE_ARGS_FOLDER = "-useargsfolder";

      private static IConfiguration m_Configuration;

      private static ProcedureRegistry m_Registry =
         new ProcedureRegistry();

      /// <summary>
      /// Prepare Registry.
      /// </summary>
      public static void PrepareProceduresRegistry()
      {
         if (m_Registry.List.Count > 0)
         {
            return;
         }

         ExecutorHandler callBack =
            new ExecutorHandler(DdlFromDb);
         m_Registry.Add(AssetConsoleProcedure.DdlFromDb, callBack);

         callBack = new ExecutorHandler(JsdFromDb);
         m_Registry.Add(AssetConsoleProcedure.JsdFromDb, callBack);

         callBack = new ExecutorHandler(AssetsToGsql);
         m_Registry.Add(AssetConsoleProcedure.AssetsToGql, callBack);
         m_Registry.Add(AssetConsoleProcedure.DbDdlToGsql, callBack);

         callBack = new ExecutorHandler(AssetToJsd);
         m_Registry.Add(AssetConsoleProcedure.AssetsToJsd, callBack);

         callBack = new ExecutorHandler(ToDb);
         m_Registry.Add(AssetConsoleProcedure.ToDb, callBack);

         callBack = new ExecutorHandler(XsdToDb);
         m_Registry.Add(AssetConsoleProcedure.XsdToDb, callBack);

         callBack = new ExecutorHandler(ToDb);
         m_Registry.Add(AssetConsoleProcedure.DbDdlToDb, callBack);

         callBack = new ExecutorHandler(DdlToFile);
         m_Registry.Add(AssetConsoleProcedure.DdlToFile, callBack);

         callBack = new ExecutorHandler(AssetsToDdl);
         m_Registry.Add(AssetConsoleProcedure.AssetsToDdl, callBack);

         callBack = new ExecutorHandler(DbToFile);
         m_Registry.Add(AssetConsoleProcedure.DbToFile, callBack);

         callBack = new ExecutorHandler(JsdToFile);
         m_Registry.Add(AssetConsoleProcedure.JsdToFile, callBack);

         callBack = new ExecutorHandler(XsdToFile);
         m_Registry.Add(AssetConsoleProcedure.XsdToFile, callBack);

         callBack = new ExecutorHandler(XsdToFile);
         m_Registry.Add(AssetConsoleProcedure.XsdToAssetsReport, callBack);

         callBack = new ExecutorHandler(XsdToAssets);
         m_Registry.Add(AssetConsoleProcedure.XsdToAssets, callBack);

         callBack = new ExecutorHandler(ToXsd);
         m_Registry.Add(AssetConsoleProcedure.ToXsd, callBack);
         m_Registry.Add(AssetConsoleProcedure.AssetsToXsd, callBack);

         callBack = new ExecutorHandler(JsdToAssets);
         m_Registry.Add(AssetConsoleProcedure.JsdToAssets, callBack);
         m_Registry.Add(AssetConsoleProcedure.JsonToAssets, callBack);

         callBack = new ExecutorHandler(DdlToAssets);
         m_Registry.Add(AssetConsoleProcedure.DdlToAssets, callBack);

         callBack = new ExecutorHandler(DbDdlToAssets);
         m_Registry.Add(AssetConsoleProcedure.DbDdlToAssets, callBack);

         callBack = new ExecutorHandler(ToFile);
         m_Registry.Add(AssetConsoleProcedure.ToFile, callBack);

         callBack = new ExecutorHandler(CreateProject);
         m_Registry.Add(AssetConsoleProcedure.CreateProject, callBack);

         callBack = new ExecutorHandler(AssetsToDataTemplate);
         m_Registry.Add(AssetConsoleProcedure.AssetsToDataTemplate, callBack);

         callBack = new ExecutorHandler(AssetsToLexiconDatabase);
         m_Registry.Add(
            AssetConsoleProcedure.AssetsToLexiconDatabase, callBack);

         callBack = new ExecutorHandler(AssetsToLexiconWorkbook);
         m_Registry.Add(
            AssetConsoleProcedure.AssetsToLexiconWorkbook, callBack);

         callBack = new ExecutorHandler(EdiToDb);
         m_Registry.Add(AssetConsoleProcedure.EdiToDb, callBack); ;

         callBack = new ExecutorHandler(ToAssets);
         m_Registry.Add(AssetConsoleProcedure.DdlImportToAssets, callBack);

         callBack = new ExecutorHandler(ToAssets);
         m_Registry.Add(AssetConsoleProcedure.EdiToAssets, callBack);

         callBack = new ExecutorHandler(EdiToFile);
         m_Registry.Add(AssetConsoleProcedure.EdiToFile, callBack);

         callBack = new ExecutorHandler(EdiToXsd);
         m_Registry.Add(AssetConsoleProcedure.EdiToXsd, callBack);

         callBack = new ExecutorHandler(UseCaseToFile);
         m_Registry.Add(AssetConsoleProcedure.UseCaseToFile, callBack);

         callBack = new ExecutorHandler(ToJsonLd);
         m_Registry.Add(AssetConsoleProcedure.ToJsonLd, callBack);
         m_Registry.Add(AssetConsoleProcedure.AssetsToJld, callBack);

         callBack = new ExecutorHandler(FileToCodeSet);
         m_Registry.Add(AssetConsoleProcedure.FileToCodeSet, callBack);
      }

      public static NamespaceList GetNamespaces(NamespaceInfo ns = null)
      {
         NamespaceList l = new NamespaceList();

         if (ns == null)
            return l;

         NamespaceInfo i =
            new NamespaceInfo(
               "nc", "http://release.niem.gov/niem/niem-core/3.0/");
         l.Add(i);
         return l;
      }

      #endregion
      #region -- Procedure Registry Delegates

      public static IResultsLog CreateProject(
         AssetConsoleArgumentsInfo arguments)
      {
         return Project.CreateProject(arguments.ProjectName);
      }

      public static IResultsLog UseProject(
         AssetConsoleArgumentsInfo arguments)
      {
         return Project.CreateProject(arguments.ProjectName, true);
      }

      public static IResultsLog DdlFromDb(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         DdlWriter.WriteSchema(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog AssetsToDdl(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         var assets = AssetData.Merge(
            arguments.AssetDataItems, arguments.Namespace, AssetType.Instance,
            arguments.Project.VersionId);
         AssetData.ToDataElement(assets.Items);
         arguments.AssetDataItems.Clear();
         arguments.AssetDataItems.Add(assets);
         DdlWriter.WriteSchema(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog JsdFromDb(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         JsdWriter.WriteSchema(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog AssetsToGsql(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         arguments.AssetDataMerge(AssetType.Schema);
         GsqlWriter.WriteSchema(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog AssetToJsd(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         JsdWriter.WriteSchema(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog ToDb(AssetConsoleArgumentsInfo arguments)
      {
         var assets = AssetData.Merge(
            arguments.AssetDataItems, arguments.Namespace, AssetType.Instance,
            arguments.Project.VersionId);
         ResourceContext context = ResourceContext.GetContext(arguments);
         var results = AssetData.ToDatabase(context,
            assets.Items, arguments.Namespace, arguments.Process.Name,
            AssetType.Asset);
         return results;
      }

      public static IResultsLog XsdToDb(AssetConsoleArgumentsInfo arguments)
      {
         AssetData assets = XmlInspector.ToAssetList(arguments);
         var massets = AssetData.Merge(
            arguments.AssetDataItems, arguments.Namespace, AssetType.Schema, 
            arguments.Project.VersionId);
         ResourceContext context = ResourceContext.GetContext(arguments);
         var results = AssetData.ToDatabase(context,
            massets.Items, arguments.Namespace, arguments.Process.Name,
            AssetType.Asset);
         return results;
      }

      public static IResultsLog ToXsd(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         ResourceContext context = ResourceContext.GetContext(
            arguments.AssetDataItems, arguments.Project.VersionId);
         XsdWriter.WriteSchema(context, arguments, null);
         results.Succeeded();
         return results;
      }

      public static IResultsLog EdiToDb(AssetConsoleArgumentsInfo arguments)
      {
         var reader = AppAssembly.FetchInstance<IDataAssets>(
            AssetResourceHelper.ASSET_B2B_EDI_FILE_READER);
         return reader.ToDatabase(arguments);
      }

      public static IResultsLog ToAssets(
         AssetConsoleArgumentsInfo arguments)
      {
         var reader = AppAssembly.FetchInstance<IDataAssets>(
            arguments.Process.ProcedureName);
         if (reader == null)
         {
            var results = new Diagnostics.ResultLog();
            results.Failed("Reader (" + arguments.Process.ProcedureName
               + ") not found.");
            return results;
         }
         return reader.ToAsset(arguments);
      }

      public static IResultsLog EdiToFile(
         AssetConsoleArgumentsInfo arguments)
      {
         var reader = AppAssembly.FetchInstance<IDataAssets>(
            AssetResourceHelper.ASSET_B2B_EDI_FILE_READER);
         var results = reader.ToAsset(arguments);

         if (arguments.AssetDataItems.Count > 0)
         {
            AssetData asset = new AssetData(arguments.AssetDataItems[0].Items);
            var proc = arguments.Procedure;
            arguments.Procedure = AssetConsoleProcedure.ToFile;
            asset.ToOutput(null, arguments);
            arguments.Procedure = proc;
         }
         return results;
      }

      public static IResultsLog EdiToXsd(
         AssetConsoleArgumentsInfo arguments)
      {
         var reader = AppAssembly.FetchInstance<IDataAssets>(
            AssetResourceHelper.ASSET_B2B_EDI_FILE_READER);
         var results = reader.ToAsset(arguments);

         if (arguments.AssetDataItems.Count > 0)
         {
            var r = ToXsd(arguments);
         }
         return results;
      }

      /// <summary>
      /// Prepare Asset Use Cases Subsets - XSD's.
      /// </summary>
      /// <param name="arguments">Arguments containing the Asset Data for the
      /// Use Case subset</param>
      /// <returns>instance of results log is returned</returns>
      public static IResultsLog UseCaseToFile(
         AssetConsoleArgumentsInfo arguments)
      {
         IResultsLog results = new ResultLog();
         if (arguments.AssetDataItems.Count > 0)
         {
            var report = arguments.AssetDataItems[0].ReconcileUseCases();
            AssetDataElementList l = new AssetDataElementList(
               arguments.Namespace, AssetType.UseCase,
               arguments.Project.VersionId);

            foreach(var i in report.UseCasesMergedItems[0].UseCase.Items)
            {
               AssetDataElement.CompleteElementUpdate(i);
               l.Add(i);
            }

            ResourceContext context = ResourceContext.GetContext(
               arguments.AssetDataItems, arguments.Project.VersionId);

            XsdWriter.WriteSchema(context, arguments, l);
         }
         return results;
      }
      
      public static IResultsLog ToJsonLd(
         AssetConsoleArgumentsInfo arguments)
      {
         // prepare JSON-LD resources...
         var results = Json.LinkData.LinkDataAsset.ToLinkData(arguments);

         // now prepare OpenAPI components...
         FileInfo outFile = new FileInfo
         {
            Path = arguments.OutputFile.Path,
            Name = arguments.UriPrefix + "." + "dam-json-ld-components",
            Extension = "jsd.json"
         };

         JsdWriter.WriteSchema(arguments, outFile);
         return null;// results;
      }

      public static IResultsLog DbToFile(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<AssetDataElementList> results = 
            ResourceContext.GetAssets(arguments.Namespace.NamePath.Root, null,
               DataObjects.Assets.DataReferenceOption.Elements);
         if (results.Success)
         {
            AssetData asset = new AssetData(results.Data);
            asset.MapDataTypes(arguments.TextMapFilePath);
            asset.ToOutput(null, arguments);
            results.Succeeded();
         }
         return results;
      }

      public static IResultsLog DdlToFile(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         
         DdlSchemaInstance.ToFile(arguments);
         results.Succeeded();
         return results;
      }

      /// <summary>
      /// Given a collection of Use-Cases and JSD files write generated
      /// dictionary to a file.
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public static IResultsLog JsdToFile(AssetConsoleArgumentsInfo arguments)
      {
         return JsonSchemaInstance.ToFile(arguments);
      }

      public static IResultsLog XsdToFile(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         XmlInspector.ToFile(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog XsdToAssets(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<AssetData> results = new ResultsLog<AssetData>();
         AssetData assets = XmlInspector.ToAssetList(arguments);
         results.Data = assets;
         results.Succeeded();
         return results;
      }

      public static IResultsLog JsdToAssets(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<AssetData> results = new ResultsLog<AssetData>();
         AssetData assets = JsonInspector.ToAssetList(arguments);
         results.Data = assets;
         results.Succeeded();
         return results;
      }

      public static IResultsLog DdlToAssets(AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         DdlSchemaInstance.ToFile(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog DbDdlToAssets(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<EventCode> results = new ResultsLog<EventCode>();
         DdlSchemaInstance.ToFile(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog AssetsToLexiconDatabase(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<object> results = new ResultsLog<object>();
         var lexiconService = LexiconHelper.GetLexiconDataInstance();
         lexiconService.Delete(arguments.Lexicon.LexiconId);
         var result = lexiconService.ToDatabase(arguments);
         results.Data = result;
         results.Succeeded();
         return results;
      }

      public static IResultsLog AssetsToLexiconWorkbook(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultLog results = new ResultLog();
         ILexiconData lexiconService = LexiconHelper.GetLexiconDataInstance();
         results.ResultValueObject = lexiconService.Export(arguments);
         results.Succeeded();
         return results;
      }

      public static IResultsLog AssetsToDataTemplate(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<object> results = new ResultsLog<object>();
         var templates = ReferenceDataTemplateBaseInfo.
            FromAssetData(arguments.AssetDataItems);
         results.Data = templates;
         results.Succeeded();
         return results;
      }
      
      public static IResultsLog FileToCodeSet(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<object> results = new ResultsLog<object>();
         object codeSet = FileReader.Reader(arguments);
         results.Data = codeSet;
         results.Succeeded();
         return results;
      }

      /// <summary>
      /// Send information to a File base on file extension...
      /// </summary>
      /// <param name="arguments"></param>
      /// <param name="context"></param>
      /// <returns></returns>
      public static IResultsLog ToFile(AssetConsoleArgumentsInfo arguments)
      {
         if (arguments == null || arguments.AssetDataItems == null ||
            arguments.AssetDataItems.Count == 0)
         {
            IResultsLog r = new ResultLog();
            r.Failed(CLASS_NAME + "::ToFile", 
               "Asset Data Items expected but not found");
            return r;
         }

         AssetData asset = AssetData.Merge(
            arguments.AssetDataItems, arguments.Namespace, AssetType.Schema,
            arguments.Project.VersionId);

         if (arguments.OutputFile.Extension == "xsd")
         {
            var r = ToXsd(arguments);
         }
         else if (arguments.OutputFile.Extension == FileExtension.OPEN_XML)
         {
            if (arguments.ToFile)
               asset.ToOutput(null, arguments);
         }

         var results = new ResultsLog<EventCode>();
         results.Succeeded();
         return results;
      }

      #endregion
      #region -- Console Initialization & Registration

      /// <summary>
      /// Register types...
      /// </summary>
      public static void RegisterTypes()
      {

         // ASSET_ROW_BUILDER_NAME is used to register Asset Row Builders that
         // can prepare a row of data to be inserted into a CSV or Open XML file
         var result = Edam.Application.AppAssembly.RegisterType(
            keys.ASSET_ROW_BUILDER_NAME,
            typeof(Edam.Xml.OpenXml.ExcelRowBuilder));
         if (result == RegistryType.Unknown)
         {
            // TODO: register the label elsewhere...
            throw new Exception("ProgramMain:RegisterTypes::" +
               "Asset Table Type failed to register");
         }

         // ASSET_RESOURCE_DATA_NAME is used to read/write data into a given
         // ContextDb

      }

      public static void Initialize()
      {
         Session.SessionId = Guid.NewGuid().ToString();
         IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
         m_Configuration = config;

         // TODO: check about DbContext although it may not be needed
         // AssetDbContextConfiguration.SetConfiguration(config);
         RegisterTypes();
         PrepareProceduresRegistry();
      }

      #endregion
      #region -- Console Source Data Support

      /// <summary>
      /// Scan File Folder.
      /// </summary>
      /// <param name="arguments">arguments</param>
      /// <param name="argumentsFilePath">arguments file folder</param>
      /// <returns>IResultsLog is returned</returns>
      private static IResultsLog FromFolder(AssetConsoleArgumentsInfo arguments,
         String argumentsFilePath = null)
      {
         ResultLog tresults = new ResultLog();
         AssetDataList items = new AssetDataList();
         IResultsLog results = null;

         if (!arguments.Process.ScanFilesFolder)
         {
            tresults.Succeeded();
            return tresults;
         }

         // go through different file or directory URI lists and get assets...
         List<UriResourceType> resources = UriResourceInfo.GetExtensionTypes();
         foreach (var i in resources)
         {
            results = null;
            switch (i)
            {
               case UriResourceType.xsd:
                  results = Execute(arguments, argumentsFilePath,
                     AssetConsoleProcedure.XsdToAssets);
                  break;
               case UriResourceType.jsdjson:
                  results = Execute(arguments, argumentsFilePath,
                     AssetConsoleProcedure.JsdToAssets);
                  break;
            }
            if (results != null)
            {
               if (results.Success)
               {
                  if (results.DataObject is AssetData asset)
                     items.Add(asset);
               }
               tresults.Copy(results);
            }
         }

         // finally complete the requested service... (output... or other)
         arguments.AssetDataItems = items;
         tresults.Succeeded();
         return tresults;
      }

      public static IResultsLog FromContext(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<AssetDataElementList> results =
            ResourceContext.GetAssets(arguments.Namespace.NamePath.Root, null,
               DataObjects.Assets.DataReferenceOption.Elements);
         if (results.Success)
         {
            AssetData asset = new AssetData(results.Data);
            arguments.AssetDataItems = new AssetDataList();
            arguments.AssetDataItems.Add(asset);
            results.Succeeded();
         }
         return results;
      }

      public static IResultsLog FromDbSchema(
         AssetConsoleArgumentsInfo arguments)
      {
         ResultsLog<AssetData> results = DdlSchemaInstance.ToDataAsset(
            arguments);
         return results;
      }

      public static IResultsLog FromDatabase(
         AssetConsoleArgumentsInfo arguments)
      {
         IResultsLog results = null;
         if (arguments.FromDatabaseSchema)
         {
            results = FromDbSchema(arguments);
         }
         else
         {
            results = FromContext(arguments);
         }
         return results;
      }

      #endregion
      #region -- Console Procedure Execution Support

      /// <summary>
      /// 
      /// </summary>
      /// <param name="submissionJson"></param>
      private static void InsertSubmission(String submissionJson)
      {
         DataDocumentItem.SaveItem<string>("",submissionJson);
         /*
          * Mongo DB
         MongoDbClient c = new MongoDbClient(null);
         c.GetDatabase("AssetConsole");
         c.GetCollection("ConsoleSubmissions");
         c.InsertDocument(submissionJson);
          */
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="arguments"></param>
      /// <param name="results"></param>
      /// <param name="argumentsFilePath"></param>
      private static async void ExecuteLogResults(
         AssetConsoleArgumentsInfo arguments,
         IResultsLog results, string argumentsFilePath)
      {
         AssetConsoleSubmission submission = new AssetConsoleSubmission();
         submission.Arguments = arguments;

         // setup result message
         submission.Results.Message = String.Empty;
         submission.Results.Source = "ExecuteLogResults";
         submission.SessionId = Application.Session.SessionId;
         if (!results.Success)
         {
            submission.Results.ResultCode = EventCode.ExceptionFound;
            submission.Results.Severity = SeverityLevel.Fatal;

            String argumentsText = String.Empty;
            if (String.IsNullOrWhiteSpace(argumentsFilePath))
            {
               argumentsText = arguments.ToLine();
            }
            else
            {
               argumentsText = argumentsFilePath;
            }

            submission.Results.Message =
               "Execution using (" + argumentsText + ") failed.";

            if (!String.IsNullOrWhiteSpace(results.MessageText))
               submission.Results.Message += " (" + results.MessageText + ")";
         }
         else
         {
            submission.Results.ResultCode = EventCode.Success;
            submission.Results.Message = null;
            submission.Results.Severity = SeverityLevel.Info;
         }

         String message;
         try
         {
            var submissionJson = AssetConsoleSubmission.ToJson(submission);
            //InsertSubmission(submissionJson);
            await DataDocumentItem.SaveItem<string>(
               "AssetConsoleArguments", "Submission", arguments.Name);
            message = "DONE";
         }
         catch (Exception ex)
         {
            message = ex.Message;
         }

         System.Console.WriteLine(message);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="arguments"></param>
      /// <param name="argumentsFilePath"></param>
      public static IResultsLog Execute(AssetConsoleArgumentsInfo arguments,
         String argumentsFilePath, AssetConsoleProcedure procedure)
      {
         const string METHOD_NAME = CLASS_NAME + "Execute";

         var proc = m_Registry.Find(procedure);
         if (proc == null)
         {
            ResultLog r = new ResultLog();
            r.Failed(METHOD_NAME,
               "Procedure (" + procedure.ToString() + ") not found.");
            System.Console.WriteLine(r.Message);
            return r;
         }

         return proc.Execute(arguments);
      }

      /// <summary>
      /// Execute and Log Results
      /// </summary>
      /// <param name="arguments">instance of AssetConsoleArgumentsInfo</param>
      /// <param name="argumentsFilePath">arguments file path</param>
      /// <param name="procedure"></param>
      /// <param name="results"></param>
      public static void ExecuteAndLogResults(
         AssetConsoleArgumentsInfo arguments, String argumentsFilePath,
         AssetConsoleProcedure procedure, IResultsLog results)
      {
         var r = Execute(arguments, argumentsFilePath, procedure);
         ExecuteLogResults(arguments, results, argumentsFilePath);
      }
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="arguments"></param>
      /// <param name="argumentsFilePath"></param>
      public static IResultsLog Execute(AssetConsoleArgumentsInfo arguments,
         String argumentsFilePath)
      {
         IResultsLog results = null;
         if (arguments.FromAssets && !arguments.HasAssets)
         {
            if (arguments.FromDatabase)
            {
               results = FromDatabase(arguments);
            }
            else
            {
               results = FromFolder(arguments, argumentsFilePath);
            }
         }

         // finally complete the requested service... (output... or other)
         AssetConsoleProcedure procedure =
            arguments.ToFile ? AssetConsoleProcedure.ToFile :
            arguments.Procedure;
         return Execute(arguments, argumentsFilePath, procedure);
      }

      /// <summary>
      /// Given a JSON file with the arguments execute requested procedure.
      /// </summary>
      /// <param name="argumentsFilePath"></param>
      public static ProcessArgumentsContext 
         GetArgsContext(String argumentsFilePath)
      {
         string func = "AssetServiceHelper::Execute";
         ProcessArgumentsContext context = new ProcessArgumentsContext(func);

         if (String.IsNullOrWhiteSpace(argumentsFilePath))
         {
            System.Console.WriteLine("No arguments provided!");
            System.Console.WriteLine("usage: app -useargsfile <filepath.json");
            return null;
         }

         if (!System.IO.File.Exists(argumentsFilePath))
         {
            System.Console.WriteLine(
               "File (" + argumentsFilePath + ") don't exists.");
            return null;
         }

         var jsonDoc = System.IO.File.ReadAllText(argumentsFilePath);

         context.Arguments = AssetConsoleArgumentsInfo.FromJson(jsonDoc);
         context.Results.Succeeded();
         return context;
      }

      /// <summary>
      /// Get Asset Data Items...  NOT USED FOR NOW...
      /// </summary>
      /// <param name="argumentsFilePath"></param>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public static ResultsLog<AssetDataList> GetAssetDataItems(
         String argumentsFilePath,
         AssetConsoleArgumentsInfo arguments)
      {
         // TODO: manage the issue of saving items -> parent and reloading...
         // try to read from a file...
         var results = AssetDataList.FromFile(arguments);

         if (results.Success)
         {
            arguments.AssetDataItems = results.Data;
         }
         else
         {
            Execute(arguments, argumentsFilePath);

            // save results to a file if needed
            AssetDataList.ToFile(arguments);
         }

         return results;
      }

      /// <summary>
      /// Given a JSON file with the arguments execute requested procedure.
      /// </summary>
      /// <param name="argumentsFilePath"></param>
      public static ProcessArgumentsContext Execute(String argumentsFilePath,
         AssetConsoleProcedure procedure = AssetConsoleProcedure.Unknown)
      {
         ProcessArgumentsContext context = GetArgsContext(argumentsFilePath);

         if (context == null)
         {
            context = new ProcessArgumentsContext(null);
            context.Results.ResultValueObject = null;
            context.Results.Failed(EventCode.Failed);
            return context;
         }

         // if a procedure had been specified then switch the procedure
         if (procedure != AssetConsoleProcedure.Unknown)
         {
            context.Arguments.Procedure = procedure;
         }

         // get/generate assets from a source?
         /*
         ResultsLog<AssetDataItems> results = null;
         if (context.Arguments.IsRetrieveAssetFromSource)
         {
            results = GetAssetDataItems(argumentsFilePath, context.Arguments);
         }
         else
         {
            Execute(context.Arguments, argumentsFilePath);
         }
          */

         Execute(context.Arguments, argumentsFilePath);

         // setup context - succeed and return context
         context.Results.ResultValueObject = context.Arguments;
         context.Results.Succeeded();

         return context;
      }

      #endregion
      #region -- Console Print Results

      public static void ConsolePrint(string message)
      {
         System.Console.WriteLine(message);
      }

      #endregion
      #region -- Console Arguments Processing Support

      public static ResultsLog<AssetConsoleArgumentsInfo> ProcessArguments(
         AssetConsoleArgumentsInfo arguments, bool outputToConsole = true)
      {
         string func = "AssetServiceHelper::ProcessArguments";
         ProcessArgumentsContext context = new ProcessArgumentsContext(func);

         if (arguments == null)
         {
            context.Results.Add(func, SeverityLevel.Fatal,
               "Project or Arguements can't be found!");
            if (outputToConsole)
            {
               foreach (var m in context.Results.Messages)
               {
                  ConsolePrint(m.Message);
               }
            }
            return null;
         }

         if (String.IsNullOrWhiteSpace(arguments.Process.OrganizationId))
            arguments.Process.OrganizationId =
               AppSettings.GetDefaultOrganizationId();
         if (String.IsNullOrWhiteSpace(arguments.Process.OrganizationDomainUri))
            arguments.Process.OrganizationDomainUri =
               AppSettings.GetDefaultOrganizationUri();

         Application.Session.OrganizationId =
            arguments.Process.OrganizationId;
         Execute(arguments, null);

         return context.Results;
      }

      /// <summary>
      /// Process Application Arguments.
      /// </summary>
      /// <param name="arguments">Array of arguments</param>
      public static ResultsLog<AssetConsoleArgumentsInfo> ProcessArguments(
         String[] arguments, bool outputToConsole = true)
      {
         string func = "AssetServiceHelper::ProcessArguments";
         ProcessArgumentsContext context = new ProcessArgumentsContext(func);

         ArgumentSet set = new ArgumentSet();
         string key = null;

         context.Results.Add(
            func, SeverityLevel.Info, "Started Processing Arguments");

         // expecting key->value, get key first followed by value
         foreach (var i in arguments)
         {
            // remember the key...
            if (key == null)
            {
               key = i;
            }

            // this will manage the values...
            else
            {
               set.Arguments.Add(key);
               set.Arguments.Add(i);

               // prepare for next key -> value...
               key = null;
            }
         }

         // if any argument was added to the list process those...
         AssetConsoleArgumentsInfo args = null;
         if (set.Arguments.Count > 0)
         {
            args =
               AssetConsoleArgumentsInfo.FromArguments(set.Arguments.ToArray());
            ProcessArguments(args);
         }

         context.Done(args);
         return context.Results;
      }

      #endregion

   }

   public class ProcessArgumentsContext
   {
      public AssetConsoleArgumentsInfo Arguments { get; set; }
      public string LocationText { get; set; }
      public DateTime Start { get;set; }
      public bool OutputToConsole { get; set; }

      private ResultsLog<AssetConsoleArgumentsInfo> results =
         new ResultsLog<AssetConsoleArgumentsInfo>();
      public ResultsLog<AssetConsoleArgumentsInfo> Results
      {
         get { return results; }
         set { results = value; }
      }

      public ProcessArgumentsContext(string locationText)
      {
         OutputToConsole = false;
         LocationText = locationText;
         Start = DateTime.Now;
      }

      public void Done(object resultValueObject)
      {
         DateTime end = DateTime.Now;
         TimeSpan s = end - Start;

         results.Add(LocationText, SeverityLevel.Info, "Ended Processing Arguments (" +
            (s.TotalSeconds.ToString() + " seconds.)"));
         results.ResultValueObject = resultValueObject;
         results.ReturnValue = (int)EventCode.Success;
         results.Succeeded();

         //ConsolePrint(s.TotalSeconds.ToString() + " seconds.");

         if (OutputToConsole)
         {
            foreach (var m in results.Messages)
            {
               AssetServiceHelper.ConsolePrint(m.Message);
            }
         }
      }
   }

}
