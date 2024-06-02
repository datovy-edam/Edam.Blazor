using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

using Edam.Diagnostics;
using Edam.InOut;

namespace Edam.Application
{

   public class AppData
   {
      public const string APPLICATION_DATA = "ApplicationData";
      public const string ASSET_DATA_PATH = "AppSettings:AssetDataPath";

      private static string m_FolderPath;
      private static bool m_NeedsAppData = false;

      public static string FolderPath
      {
         get { return m_FolderPath; }
      }

      /// <summary>
      /// Initialize the default location in "MyDocuments" folder where the app
      /// data will recide.
      /// </summary>
      /// <param name="applicationNameId">name of the application data folder
      /// within the "MyDocuments" location</param>
      /// <returns>returns true if further initialization is needed</returns>
      public static void InitializeAppData(string applicationNameId)
      {
         // setup decimal precision...
         NumberFormatInfo setPrecision = new NumberFormatInfo();
         setPrecision.NumberDecimalDigits = 8;

         // setup app data folder
         string folderPath = GetApplicationDataLocation() +
            "/" + applicationNameId + "/";
         SetFolderPath(folderPath);

         // if exists we are done...
         if (Directory.Exists(folderPath))
         {
            m_NeedsAppData = false;
            return;
         }
         m_NeedsAppData = true;

         // create directory
         Directory.CreateDirectory(folderPath);

         // move to this directory
         Directory.SetCurrentDirectory(m_FolderPath);
      }

      /// <summary>
      /// Get the "MyDocuments" folder as the AppData folder.
      /// </summary>
      /// <returns>folder full path is returned</returns>
      public static string GetApplicationDataLocation()
      {
         return Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments);
      }

      /// <summary>
      /// Get Application Data Folder.
      /// </summary>
      /// <returns>folder path is returned</returns>
      public static string GetApplicationDataFolder()
      {
         return GetApplicationDataLocation() + "/" +
            AppSettings.GetString(ASSET_DATA_PATH);
      }

      /// <summary>
      /// Get Application Data Folder.
      /// </summary>
      /// <returns>folder path is returned</returns>
      public static string GetApplicationDataResourcesFolder()
      {
         return GetApplicationDataLocation() + "/" +
            AppSettings.GetString(ASSET_DATA_PATH) + "Resources";
      }

      /// <summary>
      /// Set Folder Path.
      /// </summary>
      /// <param name="folderPath">folder path</param>
      public static void SetFolderPath(string folderPath)
      {
         m_FolderPath = m_FolderPath ?? folderPath;
      }

      /// <summary>
      /// Copy given source folder into the AppData folder.
      /// </summary>
      /// <param name="sourceFolderPath">source folder path</param>
      /// <returns>result log instance is returned</returns>
      public static ResultLog CopyFolder(string sourceFolderPath)
      {
         ResultLog resultLog = new ResultLog();
         if (!Directory.Exists(sourceFolderPath))
         {
            resultLog.Failed(EventCode.Failed);
            return resultLog;
         }

         try
         {
            FolderHelper.FolderCopy(sourceFolderPath, m_FolderPath, true);
            m_NeedsAppData = false;
            resultLog.Succeeded();
         }
         catch (Exception ex)
         {
            resultLog.Failed(ex);
         }

         return resultLog;
      }

      /// <summary>
      /// Initialize a Application Data Copy if needed.
      /// </summary>
      /// <param name="sourceFolderPath">source folder path</param>
      public static void InitializeAppDataCopy(string sourceFolderPath)
      {
         if (m_NeedsAppData)
         {
            CopyFolder(sourceFolderPath);
         }
      }

   }

}
