using System;
using System.IO;
using Microsoft.Extensions.Configuration;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   /// <summary>
   /// Helper to get application configuration settings...
   /// </summary>
   public class AppSettings
   {
      public const String APP_CONFIG_FILE_PATH = "appsettings.json";
      public const String APP_SETTINGS_SECTION_KEY = "AppSettings";
      public const String APP_CONNECTION_STRING_KEY = "ConnectionStrings";

      public const string DEFAULT_ORGANIZATION_ID =
         "AppSettings:DefaultOrganizationID";
      public const string DEFAULT_ORGANIZATION_DOMAIN_URI =
         "AppSettings:DefaultOrganizationDomainUri";

      public static string ApplicationDataFolder { get; private set; }
      public static string ApplicationFolder { get; private set; }
      private static IConfiguration m_Configuration;

      static AppSettings()
      {
         m_Configuration = new ConfigurationBuilder()
            .AddJsonFile(
               APP_CONFIG_FILE_PATH, optional: true, reloadOnChange: true)
            .Build();
         ApplicationFolder = AppDomain.CurrentDomain.BaseDirectory;
         ApplicationDataFolder = AppData.GetApplicationDataLocation();
      }

      /// <summary>
      /// Get default organization id.
      /// </summary>
      /// <returns>organization id is returned if any was configured</returns>
      public static string GetDefaultOrganizationId()
      {
         return GetString(DEFAULT_ORGANIZATION_ID);
      }

      /// <summary>
      /// Get default organization URI.
      /// </summary>
      /// <returns>organization uri is returned if any was configured</returns>
      public static string GetDefaultOrganizationUri()
      {
         return GetString(DEFAULT_ORGANIZATION_DOMAIN_URI);
      }

      /// <summary>
      /// Get JSON based Configuration.
      /// </summary>
      /// <param name="jsonFilePath">JSON app-settings file path. Defaults to
      /// "appsettings.json" if none is provided</param>
      /// <returns>instance of IConfigurationRoot is returned if all is OK.
      /// </returns>
      public static IConfigurationRoot GetJsonConfigurationBuild(
         String jsonFilePath = null)
      {
         if (jsonFilePath == null)
            jsonFilePath = APP_CONFIG_FILE_PATH;

         var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(jsonFilePath, optional: true, reloadOnChange: true);

         return builder.Build();
      }

      /// <summary>
      /// Given a configuration instance, a section and item keys return the
      /// item value as a string.
      /// </summary>
      /// <example>
      /// var csk = AppConfigSettings.GetSectionString("My.Key");
      /// </example>
      /// <param name="itemKey">a leaf child item whos value is requested
      /// </param>
      /// <param name="sectionKey">a section withing the configuration</param>
      /// <returns>the value of the item is returned if found.  If not provided
      /// the default "AppSettings" section key will be used.</returns>
      public static String GetSectionString(
         String key, String sectionKey = null)
      {
         var sk = sectionKey ?? APP_SETTINGS_SECTION_KEY;
         var itemKey = sk + ":" + key;

         String value = m_Configuration[itemKey];
         if (String.IsNullOrEmpty(value))
            value = String.Empty;
         return value;
      }

      /// <summary>
      /// Get Connection Sting related to given key.
      /// </summary>
      /// <param name="key">connection string key</param>
      /// <returns>returns the connection string</returns>
      public static String GetConnectionString(String key)
      {
         return GetSectionString(key, APP_CONNECTION_STRING_KEY);
      }

      /// <summary>
      /// Given a configuration instance, a section and item keys return the
      /// item value as a string.
      /// </summary>
      /// <example>
      /// IConfigurationRoot configuration = 
      ///    AppConfigSettings.GetJsonConfigurationBuild();
      /// var csk = AppConfigSettings.GetString(configuration, "My.Key");
      /// </example>
      /// <param name="configuration">instance of configuration</param>
      /// <param name="itemKey">a leaf child item whos value is requested
      /// </param>
      /// <param name="sectionKey">a section withing the configuration</param>
      /// <returns>the value of the item is returned if found.  If not provided
      /// the default "AppSettings" section key will be used.</returns>
      public static String GetString(
         IConfigurationRoot configuration, String itemKey,
         String sectionKey = null)
      {
         var sk = sectionKey ?? APP_SETTINGS_SECTION_KEY;
         var ap = configuration.GetSection(sk);
         var itm = ap.GetSection(itemKey);
         return itm.Value;
      }

      /// <summary>
      /// Get application configuration settings...
      /// </summary>
      /// <param name="keyName">name of setting</param>
      /// <returns>setting value</returns>
      public static String GetString(String keyName)
      {
         String value = m_Configuration[keyName];
         if (String.IsNullOrEmpty(value))
            value = String.Empty;
         return value;
      }

      /// <summary>
      /// Get application configuration settings as an Int16...
      /// </summary>
      /// <param name="keyName">name of setting</param>
      /// <returns>setting value</returns>
      public static Int16 GetInt16(String keyName)
      {
         String value = m_Configuration[keyName];
         if (String.IsNullOrEmpty(value))
            value = "0";
         return Convert.ToInt16(value);
      }

      /// <summary>
      /// Get application configuration settings as an Int32...
      /// </summary>
      /// <param name="keyName">name of setting</param>
      /// <returns>setting value</returns>
      public static Int32 GetInt32(String keyName)
      {
         String value = m_Configuration[keyName];
         if (String.IsNullOrEmpty(value))
            value = "0";
         return Convert.ToInt32(value);
      }

   }

}

