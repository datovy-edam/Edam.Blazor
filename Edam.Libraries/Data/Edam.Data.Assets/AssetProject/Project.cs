using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// -----------------------------------------------------------------------------
using config = Edam.Data.AssetManagement.Helpers.ConfigurationHelper;
using Edam.Data.AssetManagement;
using Edam.Application;
using Edam.Diagnostics;
using Edam.InOut;
using Edam.Data.AssetConsole;

namespace Edam.Data.AssetProject
{

   public class Project
   {

      public const string ARCHIVE = "Archive";
      public const string PROJECTS = "Projects";
      public const string ARGUMENTS = "Arguments";
      public const string DOCUMENTS = "Documents";
      public const string SAMPLES = "Samples";
      public const string FILES = "Files";
      public const string TEXT_MAPS = "TextMaps";
      public const string USE_CASES = "UseCases";
      public const string LIBRARIES = "Libraries";
      public const string NOTEBOOKS = "Notebooks";

      public const string DEFAULT_DATA_TEXT_MAP_KEY =
         "EditorLanguageMapFileName";

      private static bool m_Initialized = false;

      private static string m_ConsoleDefaultPath = null;
      private static string m_ConsolePath = null;
      private static string m_ProjectsPath = null;

      #region -- 4.00 - Console Path Support

      /// <summary>
      /// Set Project Default Full Path using given base path.
      /// </summary>
      /// <param name="basePath">base path</param>
      /// <returns>full path is returned</returns>
      public static string SetDefaultFullPath(string basePath = null)
      {
         if (!String.IsNullOrWhiteSpace(m_ConsoleDefaultPath))
         {
            m_ConsolePath = m_ConsoleDefaultPath;
            return m_ConsolePath;
         }

         string consolePath =
            AppSettings.GetString(config.ASSET_CONSOLE_PATH);
         string fullPath = basePath + 
            (String.IsNullOrWhiteSpace(consolePath) ?
               AppData.GetApplicationDataFolder() : consolePath);
         m_ConsolePath = config.GetAbsoluteAppDataPath(fullPath);
         if (m_ConsoleDefaultPath == null && 
            String.IsNullOrWhiteSpace(m_ConsolePath))
         {
            m_ConsoleDefaultPath = m_ConsolePath;
         }
         return m_ConsolePath;
      }

      /// <summary>
      /// Initialize Project...
      /// </summary>
      public static void InitializeProject()
      {
         m_ProjectsPath = m_ProjectsPath ??
            AppSettings.GetString(config.ASSET_PROJECTS_PATH);

         if (String.IsNullOrWhiteSpace(m_ConsolePath))
         {
            m_ConsolePath = m_ProjectsPath ?? config.GetAbsoluteAppDataPath(
               AppSettings.GetString(config.ASSET_CONSOLE_PATH));
         }

         Directory.SetCurrentDirectory(m_ConsolePath);
         m_Initialized = true;
      }

      /// <summary>
      /// Get Console - Data (absolute) Path, also set the m_ConsolePath as
      /// needed...
      /// </summary>
      /// <returns>path is returned as an absolute path</returns>
      public static string GetConsoleDataPath(bool getRelativePath = false)
      {
         if (!String.IsNullOrWhiteSpace(m_ConsolePath))
         {
            return m_ConsolePath;
         }

         var dataPath = AppSettings.GetString(config.ASSET_DATA_PATH);

         var consolePath = AppSettings.GetString(config.ASSET_CONSOLE_PATH);
         if (String.IsNullOrWhiteSpace(consolePath))
         {
            m_ConsolePath = config.GetAbsoluteAppDataPath("/" + dataPath);
         }
         else
         {
            return consolePath;
         }

         return getRelativePath ?
            dataPath : m_ConsolePath;
      }

      #endregion

      /// <summary>
      /// Get Projects full Path...
      /// </summary>
      /// <returns></returns>
      public static string GetProjectsPath(
         bool getRelativePath = false, bool onlyPath = false)
      {
         if (!onlyPath && !String.IsNullOrWhiteSpace(m_ProjectsPath))
         {
            return m_ProjectsPath;
         }

         if (String.IsNullOrWhiteSpace(m_ConsolePath))
         {
            SetDefaultFullPath();
         }

         string consolePath = String.IsNullOrWhiteSpace(m_ConsolePath) ?
            config.GetAbsoluteAppDataPath(
               AppSettings.GetString(config.ASSET_CONSOLE_PATH)) :
            m_ConsolePath;

         string cpath = consolePath;
         if (onlyPath)
         {
            return cpath;
         }

         m_ProjectsPath = (getRelativePath ? "./" : cpath) + PROJECTS + "/";
         return m_ProjectsPath;
      }

      /// <summary>
      /// Select Projects path.
      /// </summary>
      /// <param name="folderPath">folder path for projects</param>
      public static void SetProjectsPath(string folderPath)
      {
         if (String.IsNullOrWhiteSpace(folderPath))
         {
            SetDefaultFullPath();
         }
         m_ConsolePath = folderPath;
      }

      /// <summary>
      /// Get Projects full Path...
      /// </summary>
      /// <returns></returns>
      public static string GetTextMapPath(bool getRelativePath = false)
      {
         string consolePath = GetProjectsPath();
         return (getRelativePath ? "." : consolePath) + TEXT_MAPS + "/";
      }

      /// <summary>
      /// Set Projects Directory with given Path, if no path is specified
      /// </summary>
      /// <param name="projectsPath">project path to set directory too, if none
      /// is provided then the GetProjectsPath() will be used instead</param>
      /// <returns>results log is returned.  the results.Data contains the 
      /// setted current directory</returns>
      public static ResultsLog<string> SetProjectsDirectory(
         string projectsPath = null)
      {
         ResultsLog<string> results = new Diagnostics.ResultsLog<string>();
         try
         {
            // if requested path is empty then return to Default
            if (String.IsNullOrWhiteSpace(projectsPath))
            {
               m_ProjectsPath = String.Empty;
               SetDefaultFullPath();
            }

            // get new path
            string ppath = String.IsNullOrWhiteSpace(projectsPath) ?
               GetProjectsPath() : projectsPath;

            if (!ppath.EndsWith(PROJECTS + "/"))
            {
               ppath += PROJECTS + "/";
            }

            Directory.SetCurrentDirectory(ppath);
            m_ProjectsPath = ppath;

            results.Data = Directory.GetCurrentDirectory();
            results.Succeeded();
         }
         catch(Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      /// <summary>
      /// Get Projects Path.
      /// </summary>
      /// <param name="projectName">project name</param>
      public static string GetProjectPath(string projectName)
      {
         string projectname = m_ProjectsPath.Replace("\\","/");
         if (projectname.EndsWith("/"))
         {
            projectname = projectname.Substring(0, projectname.Length - 1);
         }
         string [] l = projectname.Split("/");
         if (l.Length > 1)
         {
            string pname = l[l.Length - 1].Replace("/","");
            if (pname == "Projects")
            {
               return projectname + "/" + projectName + "/";
            }
         }
         return projectname;
      }

      /// <summary>
      /// Set the project directory before working on a project.
      /// </summary>
      /// <param name="arguments"></param>
      public static void GotoProject(AssetConsoleArgumentsInfo arguments)
      {
         string path = GetProjectPath(arguments.ProjectName);
         Directory.SetCurrentDirectory(path);
      }

      /// <summary>
      /// Create Project using given name.
      /// </summary>
      /// <param name="name">project name</param>
      /// <param name="useProject">true to move to the directory if it exists,
      /// else create directory and move there</param>
      /// <returns>results are returned.  The results.ResultValueObject contains
      /// the file-path of the newly created project</returns>
      public static ResultLog CreateProject(
         string name, bool useProject = false)
      {
         if (!m_Initialized)
         {
            InitializeProject();
         }

         ResultLog results = new ResultLog();
         string currentDirectory = null;
         try
         {
            currentDirectory = Directory.GetCurrentDirectory();
            string pname = Text.Convert.ToTitleCase(name).Replace(" ", "");

            // if project names exists just return...
            string fpath = GetProjectsPath() + name;
            if (Directory.Exists(fpath))
            {
               if (useProject)
               {
                  Directory.SetCurrentDirectory(fpath);
               }
               else
               {
                  results.Failed(EventCode.ObjectExists);
               }

               return results;
            }

            // create directories
            Directory.CreateDirectory(fpath);
            Directory.SetCurrentDirectory(fpath);

            Directory.CreateDirectory(ARCHIVE);
            Directory.CreateDirectory(ARGUMENTS);
            Directory.CreateDirectory(DOCUMENTS);
            Directory.CreateDirectory(FILES);
            Directory.CreateDirectory(SAMPLES);
            Directory.CreateDirectory(USE_CASES);
            Directory.CreateDirectory(LIBRARIES);

            // copy start-up template...
            string src = AppSettings.GetString(
               config.ASSET_ARGUMENTS_TEMPLATE_PATH);
            string templateFineName = Path.GetFileName(src);
            string targetFileName = 
               fpath + "/" + ARGUMENTS + "/" + pname + "." + templateFineName;
            File.Copy(currentDirectory + "/" + src, targetFileName);

            results.ResultValueObject = fpath;
            results.Succeeded();
         }
         catch(Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (!useProject)
            {
               Directory.SetCurrentDirectory(currentDirectory);
            }
         }

         return results;
      }

      /// <summary>
      /// Get all Projects as found in the Projects default folder.
      /// </summary>
      /// <returns>instance of FolderFileItemInfo is returned with all 
      /// projects and file/folder artifacts</returns>
      public static FolderFileItemInfo GetProjectItems()
      {
         ResultsLog<string> results = SetProjectsDirectory();
         if (!results.Success)
         {
            return null;
         }
         return FolderFileReader.GetFolderFileInfo(results.Data);
      }

      /// <summary>
      /// Get a single project file/folder artifacts...
      /// </summary>
      /// <param name="rootFilePath">name of project</param>
      /// <returns>instance of FolderFileItemInfo is returned with requested 
      /// project and file/folder artifacts</returns>
      public static FolderFileItemInfo GetProjectItems(string rootFilePath)
      {
         string cdir = Directory.GetCurrentDirectory();

         ResultsLog<string> results = SetProjectsDirectory(rootFilePath);
         if (!results.Success)
         {
            return null;
         }

         FolderFileItemInfo finfo = null;
         try
         {
            finfo = FolderFileReader.GetFolderFileInfo(results.Data);
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            Directory.SetCurrentDirectory(cdir);
         }

         return finfo;
      }

      /// <summary>
      /// Get Data Text Map...
      /// </summary>
      /// <param name="mapKey">configured map-key (check appSettings)</param>
      /// <returns>instance of DataTextMap is returrned</returns>
      public static DataTextMap GetDataTextMapByKey(string mapKey = null)
      {
         string key = mapKey == null ? DEFAULT_DATA_TEXT_MAP_KEY : mapKey;
         string fileName = AppSettings.GetSectionString(key);
         if (fileName == null)
         {
            return new DataTextMap();
         }
         DataTextMap map = null;
         try
         {
            string filePath = GetTextMapPath() + fileName;
            map = DataTextMap.FromFile(filePath);
         }
         catch(Exception ex)
         {
            
         }
         finally
         {
            if (map == null)
            {
               map = new DataTextMap();
            }
         }
         return map;
      }

   }

}
