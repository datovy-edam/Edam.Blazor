using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Linq;
using System.IO;

// -----------------------------------------------------------------------------
using io = Edam.InOut;
using Edam.Application;
using helper = Edam.Data.AssetConsole.ConsoleHelper.AssetConsoleHelper;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;
using Edam.Data.AssetManagement.Helpers;
using prj = Edam.Data.AssetProject;
using Edam.Diagnostics;
using System.Net.Http.Headers;
using Edam.Data.AssetUseCases;
using Edam.Data.Lexicon;

namespace Edam.Data.AssetConsole
{

    /// <summary>
    /// Asset Console Arguments.
    /// </summary>
    public class AssetConsoleArgumentsInfo
   {

      #region -- Fields and Properties Declarations

      private const string CLASS_NAME = "AssetConsoleArgumentsInfo";

      /// <summary>
      /// At runtime the Arguments GUID will uniquely identify the arguments
      /// collection.
      /// </summary>
      [JsonIgnore]
      public Guid ArgumentsGuid { get; } = new Guid();

      [JsonIgnore]
      public AssetUseCaseList UseCases { get; set; }

      public LexiconInfo Lexicon { get; set; } = new LexiconInfo();
      public prj.ProjectInfo Project { get; set; }
      public ProcessInfo Process { get; set; }
      public ReportOptions Report { get; set; }
      public AssetArgumentOptions Options { get; set; }

      [JsonIgnore]
      public String ProjectName
      {
         get { return Project.Name; }
         set { Project.Name = value; }
      }

      [JsonIgnore]
      public String ProjectVersionId
      {
         get { return Project.VersionId; }
         set { Project.VersionId = value; }
      }

      [JsonIgnore]
      public String RecordId
      {
         get { return Process.RecordId; }
         set { Process.RecordId = value; }
      }
      [JsonIgnore]
      public String Name
      {
         get { return Process.Name; }
         set { Process.Name = value; }
      }

      [JsonIgnore]
      public String OrganizationId
      {
         get { return Process.OrganizationId; }
         set { Process.OrganizationId = value; }
      }
      [JsonIgnore]
      public String OrganizationDomainUri
      {
         get { return Process.OrganizationDomainUri; }
         set { Process.OrganizationDomainUri = value; }
      }

      // procedure to execute
      [JsonIgnore]
      public String ProcedureName
      {
         get { return Process.ProcedureName; }
         set { SetProcedure(value); }
      }
      [JsonIgnore]
      public SchemaType? SchemaType
      {
         get { return Process.SchemaType; }
         set { Process.SchemaType = value; }
      }

      [JsonIgnore]
      public AssetConsoleProcedure Procedure
      {
         get { return Process.Procedure; }
         set { SetProcedure(value); }
      }

      // input / output files
      public io.FileInfo InputFile { get; set; }
      public io.FileInfo OutputFile { get; set; }

      [JsonIgnore]
      public String InFilePath
      {
         get { return InputFile.Path; }
         set { InputFile.Path = value; }
      }
      [JsonIgnore]
      public String InFileName
      {
         get { return InputFile.Name; }
         set { InputFile.Name = value; }
      }
      [JsonIgnore]
      public String InFileExtension
      {
         get { return InputFile.Extension; }
         set { InputFile.Extension = value; }
      }
      [JsonIgnore]
      public String InFileFullPath
      {
         get { return InputFile.Full; }
         set { InputFile.FromFullPath(value, null); }
      }

      [JsonIgnore]
      public String OutFilePath
      {
         get { return OutputFile.Path; }
         set { OutputFile.Path = value; }
      }
      [JsonIgnore]
      public String OutFileName
      {
         get { return OutputFile.Name; }
         set { OutputFile.Name = value; }
      }
      [JsonIgnore]
      public String OutFileExtension
      {
         get { return OutputFile.Extension; }
         set { OutputFile.Extension = value; }
      }
      [JsonIgnore]
      public String OutFileFullPath
      {
         get { return OutputFile.Full; }
         set { OutputFile.FromFullPath(value, null); }
      }

      // transform a text into another...
      public String TextMapFilePath { get; set; }
      public String TextMapFolderPath { get; set; }

      // domain information
      public DataDomain Domain { get; set; }

      [JsonIgnore]
      public String DomainId
      {
         get { return Domain.DomainId; }
         set { Domain.DomainId = value; }
      }
      [JsonIgnore]
      public String DomainPrefix
      {
         get { return Domain.Prefix; }
         set { Domain.Prefix = value; }
      }
      [JsonIgnore]
      public String DomainUri
      {
         get { return Domain.DomainUri; }
         set { Domain.DomainUri = value; }
      }
      [JsonIgnore]
      public String DomainName
      {
         get { return Domain.DomainName; }
         set { Domain.DomainName = value; }
      }
      [JsonIgnore]
      public String DomainDescription
      {
         get { return Domain.Description; }
         set { Domain.Description = value; }
      }

      [JsonIgnore]
      public String DomainBusiness
      {
         get { return Domain.Business; }
         set { Domain.Business = value; }
      }
      [JsonIgnore]
      public String DomainBusinessId
      {
         get { return Domain.BusinessId; }
         set { Domain.BusinessId = value; }
      }

      // namespace details
      public NamespaceInfo Namespace { get; set; }

      [JsonIgnore]
      public String UriPrefix
      {
         get { return Namespace.Prefix; }
         set { Namespace.Prefix = value; }
      }
      [JsonIgnore]
      public String NamespaceUri
      {
         get { return Namespace.Uri.AbsoluteUri; }
         set { Namespace.Uri = new Uri(value); }
      }
      [JsonIgnore]
      public String RootElementName
      {
         get { return Namespace.RootElementName; }
         set { Namespace.RootElementName = value; }
      }

      // use to generate XML sample
      public InspectDepthArguments InspectArguments { get; set; }

      [JsonIgnore]
      public String MaxThreshold
      {
         get { return InspectArguments.MaxThreshold; }
         set { InspectArguments.MaxThreshold = value; }
      }
      [JsonIgnore]
      public String ListLength
      {
         get { return InspectArguments.ListLength; }
         set { InspectArguments.ListLength = value; }
      }

      [JsonIgnore]
      public Boolean ToFile
      {
         get
         {
            return Procedure == AssetConsoleProcedure.XsdToFile ||
               Procedure == AssetConsoleProcedure.JsdToFile ||
               Procedure == AssetConsoleProcedure.DdlToFile ||
               Procedure == AssetConsoleProcedure.DbToFile ||
               Procedure == AssetConsoleProcedure.DbDdlToFile ||
               Procedure == AssetConsoleProcedure.ToFile ||
               Procedure == AssetConsoleProcedure.XsdToAssetsReport;
         }
      }

      [JsonIgnore]
      public Boolean ToDatabase
      {
         get
         {
            return Procedure == AssetConsoleProcedure.XsdToDb ||
               Procedure == AssetConsoleProcedure.JsdToDb ||
               Procedure == AssetConsoleProcedure.DdlToDb ||
               Procedure == AssetConsoleProcedure.DbDdlToDb;
         }
      }

      [JsonIgnore]
      public Boolean IsRetrieveAssetFromSource
      {
         get
         {
            return Procedure == AssetConsoleProcedure.DbToAssets ||
               Procedure == AssetConsoleProcedure.DdlToAssets ||
               Procedure == AssetConsoleProcedure.DdlImportToAssets;
         }
      }

      [JsonIgnore]
      public Boolean FromDatabase
      {
         get
         {
            return Procedure == AssetConsoleProcedure.XsdFromDb ||
               Procedure == AssetConsoleProcedure.JsdFromDb ||
               Procedure == AssetConsoleProcedure.DdlFromDb ||
               Procedure == AssetConsoleProcedure.DbToFile ||
               Procedure == AssetConsoleProcedure.DbToAssets ||
               Procedure == AssetConsoleProcedure.DbDdlToAssets ||
               Procedure == AssetConsoleProcedure.DbDdlToFile ||
               Procedure == AssetConsoleProcedure.DbDdlToDb ||
               Procedure == AssetConsoleProcedure.DbDdlToGsql;
         }
      }

      [JsonIgnore]
      public Boolean FromDatabaseSchema
      {
         get
         {
            return Procedure == AssetConsoleProcedure.DbDdlToFile ||
               Procedure == AssetConsoleProcedure.DbDdlToAssets ||
               Procedure == AssetConsoleProcedure.DbDdlToDb ||
               Procedure == AssetConsoleProcedure.DbDdlToGsql;
         }
      }

      [JsonIgnore]
      public Boolean FromAssets
      {
         get
         {
            return Procedure != AssetConsoleProcedure.EdiToDb;
         }
      }

      public Boolean HasAssets
      {
         get
         {
            return AssetDataItems != null && this.AssetDataItems.Count > 0;
         }
      }

      [JsonIgnore]
      public String DefaultFolderInputPath
      {
         get { return ConfigurationHelper.GetDefaultFolderInputhPath(); }
      }

      [JsonIgnore]
      public String DefaultFolderOutputPath
      {
         get { return ConfigurationHelper.GetDefaultFolderOutputPath(); }
      }

      [JsonIgnore]
      public AssetDataList AssetDataItems { get; set; } = new AssetDataList();

      // connection string
      public String ConnectionString { get; set; }

      /// <summary>
      /// Related to an enumeration with the same name... needed to map a type
      /// (see ...Data.Asset.ElementTransform enum. apply to DB schemas.
      /// </summary>
      public string ElementTransform { get; set; }

      public List<Uri> UriList { get; set; }

      #endregion
      #region -- Constructor/Destructor

      /// <summary>
      /// 
      /// </summary>
      public AssetConsoleArgumentsInfo()
      {
         Process = new ProcessInfo();

         AssetDataItems = null;
         Options = new AssetArgumentOptions();
         UriList = new List<Uri>();
         InputFile = new io.FileInfo();
         OutputFile = new io.FileInfo();
         Project = new prj.ProjectInfo();
         Domain = new DataDomain();
         Namespace = new NamespaceInfo("", "http://tempuri", "");
         InspectArguments = new InspectDepthArguments();
         Procedure = AssetConsoleProcedure.Unknown;
      }

      #endregion
      #region -- Other Methods

      /// <summary>
      /// Add file info to list of input files.
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="filePath"></param>
      public void AddFileName(String fileName, String filePath)
      {
         if (!String.IsNullOrWhiteSpace(fileName))
         {
            var fn = UriList.Find((x) => x.LocalPath == fileName);
            if (fn == null)
               UriList.Add(new Uri("file:///" + filePath + fileName));
         }
         if (String.IsNullOrWhiteSpace(InFilePath) &&
            !String.IsNullOrWhiteSpace(filePath))
            InFilePath = filePath;
         if (String.IsNullOrWhiteSpace(InFileFullPath))
            InFileFullPath = filePath + fileName;
      }

      public void SetProcedure(String name)
      {
         Process.Procedure = (AssetConsoleProcedure)
            Enum.Parse(typeof(AssetConsoleProcedure), name);
         Process.ProcedureName = name;
      }

      public void SetProcedure(AssetConsoleProcedure procedure)
      {
         Process.Procedure = procedure;
         Process.ProcedureName = procedure.ToString();
      }

      public NamespaceInfo GetNamespace()
      {
         if (Namespace.NamePath == null)
         {
            Namespace.NamePath = new NamespacePath();
            Namespace.NamePath.SetUri(Namespace.Uri);
         }
         return Namespace;
      }

      public void Add(AssetDataElementList items)
      {
         AssetData data = new AssetData(items);
         data.Items = items;
         AssetDataItems.Add(data);
      }

      public List<AssetData> AssetDataMerge(AssetType type)
      {
         var assets = AssetData.Merge(AssetDataItems, Namespace, type,
            Project.VersionId);
         AssetDataItems.Clear();
         AssetDataItems.Add(assets);
         return AssetDataItems;
      }

      public NamespaceInfo GetDefaultNamespace()
      {
         return Namespace;
      }

      /// <summary>
      /// Check the consistency of the arguments.
      /// </summary>
      /// <param name="arguments">arguments to check</param>
      /// <exception cref="Exception"></exception>
      public static void CheckArguments(AssetConsoleArgumentsInfo arguments)
      {
         string func = CLASS_NAME + "CheckArguments";
         if (arguments == null)
         {
            throw new Exception(func + ": Arguments instance expected.");
         }

         string name = String.IsNullOrWhiteSpace(
            arguments.Process.ProcedureName) ?
               AppAssembly.GetKeyId(arguments.Process.ProcedureTag) :
               arguments.Process.ProcedureName;

         string tag;
         if (String.IsNullOrWhiteSpace(arguments.Process.ProcedureTag))
         {
            var pname = AppAssembly.GetAlias(arguments.Process.ProcedureName);
            tag = String.IsNullOrWhiteSpace(pname) ? name : pname;
         }
         else
         {
            tag = arguments.Process.ProcedureTag;
         }

         if (String.IsNullOrWhiteSpace(name) ||
             String.IsNullOrWhiteSpace(tag))
         {
            throw new Exception(func + ": no Procedure Name can be resolved.");
         }
         
         arguments.Process.ProcedureName = name;
         arguments.Process.ProcedureTag = tag;
      }

      /// <summary>
      /// Make a duplicate of args... some sections should never change at
      /// runtime...
      /// </summary>
      /// <param name="args"></param>
      /// <returns>A duplicate is returned</returns>
      public static AssetConsoleArgumentsInfo Duplicate(
         AssetConsoleArgumentsInfo args)
      {
         AssetConsoleArgumentsInfo copy = new AssetConsoleArgumentsInfo();

         // the following should never change...
         copy.AssetDataItems = args.AssetDataItems;
         copy.Domain = args.Domain;
         copy.UseCases = args.UseCases;
         copy.Namespace = args.Namespace;
         copy.Options = args.Options;
         copy.UriList = args.UriList;

         // the following may change at runtime...
         copy.TextMapFilePath = args.TextMapFilePath;
         copy.TextMapFolderPath = args.TextMapFolderPath;
         copy.ConnectionString = args.ConnectionString;

         // ... Input File
         copy.InFileExtension = args.InFileExtension;
         copy.InFileFullPath = args.InFileFullPath;
         copy.InFileName = args.InFileName;
         copy.InFilePath = args.InFilePath;

         // ... Output File
         copy.OutFileExtension = args.OutFileExtension;
         copy.OutFileFullPath = args.OutFileFullPath;
         copy.OutFileName = args.OutFileName;
         copy.OutFilePath = args.OutFilePath;

         // ... Process
         copy.Name = args.Name;
         copy.OrganizationDomainUri = args.OrganizationDomainUri;
         copy.OrganizationId = args.OrganizationId;
         copy.Procedure = args.Procedure;
         copy.ProcedureName = args.ProcedureName;
         copy.RecordId = args.RecordId;
         copy.SchemaType = args.SchemaType;

         // ... Project
         copy.ProjectName = args.ProjectName;

         // ... Report
         copy.Report = args.Report;

         return copy;
      }

      /// <summary>
      /// Get project arguments input file name.
      /// </summary>
      /// <returns>partial file path to the file is returned</returns>
      public string GetInputFileName()
      {
         // TODO: remove hardcoded text...
         return "./files/" + InputFile.Name + "." + InputFile.Extension;
      }

      /// <summary>
      /// Get Data Domain based on project arguments.
      /// </summary>
      /// <returns>instance of DataDomain is returned</returns>
      public DataDomain GetDataDomain()
      {
         DataDomain dmain = new DataDomain();
         dmain.OrganizationId = Session.OrganizationId;
         dmain.DataOwnerId = Namespace.OrganizationDomainId;
         dmain.DomainId = Domain.DomainId;
         dmain.DomainName =
            String.IsNullOrWhiteSpace(DomainName) ?
               DomainDescription : DomainName;
         dmain.Prefix = Namespace.Prefix;
         dmain.DomainUri = Namespace.UriText;
         dmain.Root = Namespace.NamePath.Root;
         dmain.Domain = Namespace.NamePath.Domain;
         dmain.Description = Domain.Description;
         dmain.Type = DomainType.Asset;
         return dmain;
      }

      #endregion
      #region -- To/From JSON Support

      /// <summary>
      /// Get instance of AssetconsoleArgumentsInfo from corresponding json
      /// document instance.
      /// </summary>
      /// <param name="jsonDocument">JSON document instance</param>
      /// <returns>instance of AssetConsoleArgumentsInfo is returned</returns>
      public static AssetConsoleArgumentsInfo FromJson(string jsonDocument)
      {
         var options = new JsonSerializerOptions
         {
            Converters =
            {
               new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
         };
         var args =
            JsonSerializer.Deserialize<AssetConsoleArgumentsInfo>(
               jsonDocument, options);
         CheckArguments(args);
         return args;
      }

      /// <summary>
      /// Get instance of AssetconsoleArgumentsInfo from corresponding json
      /// document instance.
      /// </summary>
      /// <param name="jsonDocument">JSON document instance</param>
      /// <returns>instance of results with inner AssetConsoleArgumentsInfo
      /// is returned</returns>
      public static ResultsLog<AssetConsoleArgumentsInfo> TryFromJson(
         string jsonText)
      {
         ResultsLog<AssetConsoleArgumentsInfo> results = 
            new ResultsLog<AssetConsoleArgumentsInfo>();
         if (string.IsNullOrEmpty(jsonText))
         {
            results.Data = null;
            results.Failed("JSON is null or empty");
            return results;
         }

         try
         {
            results.Data = FromJson(jsonText);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Data = null;
            results.Failed(ex);
         }
         return results;
      }

      /// <summary>
      /// Read JSON arguments file using given file path.
      /// </summary>
      /// <param name="filePath">JSON file path</param>
      /// <returns>instance of AssetConsoleArgumentsInfo is returned</returns>
      public static AssetConsoleArgumentsInfo FromJsonFilePath(string filePath)
      {
         if (File.Exists(filePath))
         {
            string jtext = File.ReadAllText(filePath);
            if (!String.IsNullOrWhiteSpace(jtext))
            {
               return FromJson(jtext);
            }
         }
         return null;
      }

      /// <summary>
      /// Get arguments info by reading a JSON file or given ProcessName.
      /// </summary>
      /// <param name="relativeFilePathOrProcessName">relative file path or
      /// process name</param>
      /// <param name="arguments">current gathered arguments until now... that 
      /// at least should contain the Project Name to use</param>
      /// <returns>instance of arguments info is returned</returns>
      public static AssetConsoleArgumentsInfo FromJsonFile(
         string relativeFilePathOrProcessName, 
         AssetConsoleArgumentsInfo arguments = null)
      {
         string rpath = "./" + prj.Project.ARGUMENTS + "/" 
            + relativeFilePathOrProcessName;
         if (File.Exists(rpath))
         {
            string jtext = File.ReadAllText(rpath);
            return FromJson(jtext);
         }

         // file don't exist try to get file by "Process.Name"
         if (arguments == null ||
            string.IsNullOrWhiteSpace(arguments.ProjectName))
         {
            return null;
         }

         // there is a project name so scan all arguments files to find the 
         // given project name...
         string pname = prj.Project.GetProjectsPath(true) + 
            arguments.ProjectName + "/" + prj.Project.ARGUMENTS;
         io.FolderFileReader reader = new io.FolderFileReader(pname);
         foreach(var i in reader.Items)
         {
            var txt = System.IO.File.ReadAllText(i);
            AssetConsoleArgumentsInfo args = FromJson(txt);
            if (args.Process.Name == relativeFilePathOrProcessName)
            {
               args.ProjectName = arguments.ProjectName;
               return args;
            }
         }
         return null;
      }

      public static String ToJson(AssetConsoleArgumentsInfo arguments)
      {
         var args =
            JsonSerializer.Serialize<AssetConsoleArgumentsInfo>(arguments);
         return args;
      }

      #endregion
      #region -- To/From Arguments List Support

      /// <summary>
      /// Given an argument-value pairs list convert it into an
      /// AssetConsoleArguments object.
      /// </summary>
      /// <param name="args"></param>
      /// <returns></returns>
      public static AssetConsoleArgumentsInfo FromArguments(String[] args)
      {
         AssetConsoleArgumentsInfo a = new AssetConsoleArgumentsInfo();
         AssetConsoleArgument argument;

         int index = 0;
         while(index < args.Length)
         {
            argument = AssetConsoleArgument.Parse(args,ref index);
            if (argument == null)
               break;

            if (!argument.IsArgument)
               continue;

            switch(argument.ArgumentName)
            {
               case helper.NEW:
                  a.ProjectName = argument.Value;
                  a.Procedure = AssetConsoleProcedure.CreateProject;
                  return a;
               case helper.USE:
                  a.ProjectName = argument.Value;
                  a.Procedure = AssetConsoleProcedure.CreateProject;
                  break;
               case helper.USE_ARGS_FILE:
                  if (a.Procedure == AssetConsoleProcedure.CreateProject)
                  {
                     prj.Project.CreateProject(a.ProjectName, true);
                  }
                  return(FromJsonFile(argument.Value));
               case helper.SCHEMA_TYPE:
                  switch(argument.Value)
                  {
                     case "DDL":
                        a.SchemaType = AssetConsole.SchemaType.DDL;
                        break;
                     case "JML":
                        a.SchemaType = AssetConsole.SchemaType.JML;
                        break;
                     case "JSON":
                        a.SchemaType = AssetConsole.SchemaType.JSON;
                        break;
                     case "XSD":
                        a.SchemaType = AssetConsole.SchemaType.XSD;
                        break;
                  }
                  break;
               case helper.PROCEDURE_NAME:
                  a.ProcedureName = argument.Value;
                  break;
               case helper.ELEMENT_TRANSFORM:
                  a.ElementTransform = argument.Value;
                  break;
               case helper.ORG_URI:
                  a.OrganizationDomainUri = argument.Value;
                  break;

               case helper.IN_FILE_PATH:
                  a.InFilePath = argument.Value;
                  break;
               case helper.IN_FILE_NAME:
                  a.InFileName = argument.Value;
                  break;
               case helper.IN_FILE_EXT:
                  a.InFileExtension = argument.Value;
                  break;
               case helper.IN_FILE_FULL_PATH:
                  a.InFileFullPath = argument.Value;
                  break;

               case helper.OUT_FILE_PATH:
                  a.OutFilePath = argument.Value;
                  break;
               case helper.OUT_FILE_NAME:
                  a.OutFileName = argument.Value;
                  break;
               case helper.OUT_FILE_EXT:
                  a.InFileExtension = argument.Value;
                  break;
               case helper.OUT_FILE_FULL_PATH:
                  a.InFileFullPath = argument.Value;
                  break;

               case helper.TEXT_MAP_FILE_PATH:
                  a.TextMapFilePath = argument.Value;
                  break;
               case helper.TEXT_MAP_FOLDER_PATH:
                  a.TextMapFolderPath = argument.Value;
                  break;
               case helper.ROOT_ELEMENT_NAME:
                  a.RootElementName = argument.Value;
                  break;
               case helper.URI_PREFIX:
                  a.UriPrefix = argument.Value;
                  break;
               case helper.URI_NAMESPACE:
                  a.NamespaceUri = argument.Value;
                  break;
               case helper.MAX_THRESHOLD:
                  a.MaxThreshold = argument.Value;
                  break;
               case helper.LIST_LENGTH:
                  a.ListLength = argument.Value;
                  break;
               case helper.CONNECTION_STRING:
                  a.ConnectionString = argument.Value;
                  break;
               default:
                  throw new Exception("Unexpected Argument(" +
                     argument.ArgumentName + ")");
            }
         }

         return a;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="arguments"></param>
      /// <returns></returns>
      public static String[] ToArguments(AssetConsoleArgumentsInfo arguments)
      {
         List<String> list = new List<string>();

         if (arguments.SchemaType.HasValue)
            list.Add(helper.PrepareParam(
               helper.SCHEMA_TYPE, arguments.SchemaType.ToString()));

         if (!String.IsNullOrWhiteSpace(arguments.OrganizationDomainUri))
            list.Add(helper.PrepareParam(
               helper.ORG_URI, arguments.OrganizationDomainUri));

         if (!String.IsNullOrWhiteSpace(arguments.ElementTransform))
            list.Add(helper.PrepareParam(
               helper.ELEMENT_TRANSFORM, arguments.ElementTransform));

         if (!String.IsNullOrWhiteSpace(arguments.InFilePath))
            list.Add(helper.PrepareParam(
               helper.IN_FILE_PATH, arguments.InFilePath));

         if (!String.IsNullOrWhiteSpace(arguments.InFileName))
            list.Add(helper.PrepareParam(
               helper.IN_FILE_NAME, arguments.InFileName));

         if (!String.IsNullOrWhiteSpace(arguments.ProcedureName))
            list.Add(helper.PrepareParam(
               helper.PROCEDURE_NAME, arguments.ProcedureName));

         foreach (var fname in arguments.UriList)
         {
            list.Add(helper.PrepareParam(
               helper.IN_FILE_NAME, fname.IsAbsoluteUri ? 
                  fname.LocalPath : fname.OriginalString));
         }

         if (!String.IsNullOrWhiteSpace(arguments.InFileFullPath))
            list.Add(helper.PrepareParam(
               helper.IN_FILE_FULL_PATH, arguments.InFileFullPath));

         if (!String.IsNullOrWhiteSpace(arguments.TextMapFilePath))
            list.Add(helper.PrepareParam(
               helper.TEXT_MAP_FILE_PATH, arguments.TextMapFilePath));

         if (!String.IsNullOrWhiteSpace(arguments.TextMapFolderPath))
            list.Add(helper.PrepareParam(
               helper.TEXT_MAP_FILE_PATH, arguments.TextMapFolderPath));

         if (!String.IsNullOrWhiteSpace(arguments.InFileExtension))
            list.Add(helper.PrepareParam(
               helper.IN_FILE_EXT, arguments.InFileExtension));

         if (!String.IsNullOrWhiteSpace(arguments.RootElementName))
            list.Add(helper.PrepareParam(
               helper.ROOT_ELEMENT_NAME, arguments.RootElementName));

         if (!String.IsNullOrWhiteSpace(arguments.UriPrefix))
            list.Add(helper.PrepareParam(
               helper.URI_PREFIX, arguments.UriPrefix));

         if (!String.IsNullOrWhiteSpace(arguments.NamespaceUri))
            list.Add(helper.PrepareParam(
               helper.URI_NAMESPACE, arguments.NamespaceUri));

         if (!String.IsNullOrWhiteSpace(arguments.MaxThreshold))
            list.Add(helper.PrepareParam(
               helper.MAX_THRESHOLD, arguments.MaxThreshold));

         if (!String.IsNullOrWhiteSpace(arguments.ListLength))
            list.Add(helper.PrepareParam(
               helper.LIST_LENGTH, arguments.ListLength));

         if (!String.IsNullOrWhiteSpace(arguments.OutFilePath))
            list.Add(helper.PrepareParam(
               helper.OUT_FILE_PATH, arguments.OutFilePath));

         if (!String.IsNullOrWhiteSpace(arguments.ConnectionString))
            list.Add(helper.PrepareParam(
               helper.CONNECTION_STRING, arguments.ConnectionString));

         return list.ToArray();
      }

      public String ToLine()
      {
         string[] args = ToArguments(this);
         var sb = new StringBuilder();
         foreach (var i in args)
         {
            sb.Append(i + " ");
         }
         return sb.ToString().TrimEnd();
      }

      #endregion

   }

}
