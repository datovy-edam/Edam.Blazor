using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement;
using Edam.Application;
using Edam.InOut;

namespace Edam.Data.AssetManagement.Helpers
{

   public class ConfigurationHelper
   {
      public const String ASSET_MANAGEMENT_DB_KEY = "AM.DB.Key";
      public const string ASSET_PROJECTS_PATH = "AppSettings:AssetProjectsPath";
      public const string ASSET_CONSOLE_PATH = "AppSettings:AssetConsolePath";
      public const string ASSET_DATA_PATH = "AppSettings:AssetDataPath";
      public const string ASSET_ARGUMENTS_TEMPLATE_PATH =
         "AppSettings:AssetArgumentsTemplatePath";

      public const string ABSOLUTE_FILE_URI_HEADER = "file:///";

      public const String TYPE_NAME_SUFFIX = "Type";
      public const string DEFAULT_IN_PATH = "DefaultInPath";
      public const string DEFAULT_OUT_PATH = "DefaultOutPath";
      public const String KEY = "key";

      /// <summary>
      /// Get Connection String.
      /// </summary>
      /// <param name="configuration">optional configuration, defaults to 
      /// appsettings.json if not given</param>
      /// <param name="connectionKey">connection key, defaults to "AM.DB.Key"
      /// </param>
      /// <returns>the connection string is returned if configurations resources
      /// contains it</returns>
      public static String GetConnectionString(
         IConfigurationRoot configuration = null, String connectionKey = null)
      {
         IConfigurationRoot config = configuration ??
            AppSettings.GetJsonConfigurationBuild();
         var csk = AppSettings.GetString(config, 
            connectionKey ?? ASSET_MANAGEMENT_DB_KEY);
         return config.GetConnectionString(csk);
      }

      /// <summary>
      /// If path is not an absolute path make it..
      /// </summary>
      /// <param name="path">path to examine</param>
      /// <returns>absolute path is returned</returns>
      public static string GetAbsoluteAppDataPath(string path)
      {
         // is an absolute path?  if not find application path...
         if (path.IndexOf(':') == -1)
         {
            path = AppSettings.ApplicationDataFolder + path;
         }
         return path;
      }

      /// <summary>
      /// If path is not an absolute path make it..
      /// </summary>
      /// <param name="path">path to examine</param>
      /// <returns>absolute path is returned</returns>
      public static string GetAbsoluteFileUri(string path)
      {
         // is an absolute path?  if not find application path...
         if (path.IndexOf(ABSOLUTE_FILE_URI_HEADER) == -1)
         {
            path = ABSOLUTE_FILE_URI_HEADER + GetAbsoluteAppDataPath(path);
         }
         return path;
      }

      public static String GetDefaultFolderInputhPath()
      {
         return GetAbsoluteAppDataPath(AppSettings.GetString(DEFAULT_IN_PATH));
      }

      public static String GetDefaultFolderOutputPath()
      {
         return GetAbsoluteAppDataPath(AppSettings.GetString(DEFAULT_OUT_PATH));
      }

   }

}
