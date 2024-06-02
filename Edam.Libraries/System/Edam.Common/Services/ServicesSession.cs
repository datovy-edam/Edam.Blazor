using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Copied from Kif v5r0
using Edam.Application;
using app = Edam.Application.Resources;

namespace Edam.Services
{

   public class InvalidServiceConfigurationException : Exception
   {
      public InvalidServiceConfigurationException(
         String message, Exception exception = null) :
            base(message, exception)
      {

      }
   }

   /// <summary>
   /// Manage Services Configuration and Runtime session.
   /// </summary>
   public class ServicesSession
   {
        
//#if DEBUG
//      public static readonly string 
//         DEFAULT_SERVICE_API_URI = "http://localhost:59475/api/";
//#else
//      public static readonly string 
//         DEFAULT_SERVICE_API_URI = "https://www.cdlway.com/api/";
//#endif

      private static Configuration.ServicesConfiguration m_Configurations;
      public static Configuration.ServicesConfiguration Configurations
      {
         get { return m_Configurations; }
      }
      
      private static String m_ConfigurationFilePath { get; set; }

//#if PORTABLE_SUPPORT_

      /// <summary>
      /// Initialize the Services Session.
      /// </summary>
      static ServicesSession()
      {
         if (m_Configurations != null)
            return;

         m_Configurations = new Configuration.ServicesConfiguration();

         var serviceUri = AppSettings.GetString(app.Strings.DefaultUrlSource);

         ServiceBaseInfo b = new ServiceBaseInfo()
         {
            Key = "Default",
            AccountUserId = "",
            AccountPassword = new Security.Password(""),
            ServicePathUri = serviceUri, // DEFAULT_SERVICE_API_URI,
            IsActive = true
         };

         // Key is the "Default" value while creating a new ServiceBaseInfo...
         m_Configurations.Add(b);
      }

      /*
#else

      /// <summary>
      /// Initialize the Services Session.
      /// </summary>
      static ServicesSession()
      {
         String configFilePath =
            Configuration.ServicesConfiguration.GetConfigFilePath();
         if (String.IsNullOrEmpty(configFilePath))
            return;

         if (System.IO.File.Exists(configFilePath))
            LoadConfiguration(configFilePath);
      }

      /// <summary>
      /// Get Default Empty Services Configuration...
      /// </summary>
      /// <returns>Instance of Default Empty ServicesConfig is returned
      /// </returns>
      public static Configuration.ServicesConfiguration GetDefaultEmptyConfig()
      {
         var config = new Configuration.ServicesConfiguration();
         config.DataSources = new List<Data.DataSourceInfo>();
         config.DataSources.Add(new Data.DataSourceInfo()
         {
            Key = ApplicationStrings.DefaultDatabaseKey,
            Alias = ApplicationStrings.DefaultDatabaseKey,
            HostName = ApplicationStrings.DefaultApplicationHost,
            AuthenticationMethod =
               Data.DbAuthenticationMethod.NtAuthentication
         });
         return config;
      }

      /// <summary>
      /// Load the Services Configuration File providing a file path.  This is
      /// useful for ASP.Net applications since you need to map the file path
      /// to the actual project/web directory as follows:
      /// 
      ///   System.String configFilePath =
      ///      SantaClara.Net.Configuration.ServicesConfiguration.
      ///         GetConfigFileToBeMappedPath();
      ///   System.String filePath = this.Server.MapPath(configFilePath);
      ///   SantaClara.Net.ServicesSession.LoadConfiguration(filePath);
      ///   
      /// For ASP.Net Apps. make sure that your config file has:
      /// 
      ///    -add key="DefaultServiceConfigFile" value=""/-
      ///    -add key="DefaultServiceConfigFileToBeMapped" value="ServiceConfigurations.xml"/-
      ///    
      /// </summary>
      /// <remarks>If you need to provide the location of the file mapped by
      /// your application to a particular directory make sure to call this 
      /// method before anything else.</remarks>
      /// <param name="configurationFilePath"></param>
      public static Boolean LoadConfiguration(
         String configurationFilePath = null)
      {
         if (m_Configurations != null)
            return false;

         if (String.IsNullOrEmpty(configurationFilePath))
            configurationFilePath =
               Configuration.ServicesConfiguration.GetConfigFilePath();

         // if we don't have a config file till this point setup a corresponding
         // application services session...
         if (String.IsNullOrEmpty(configurationFilePath))
         {
            m_Configurations = GetDefaultEmptyConfig();
            return true;
         }

         try
         {
            if (!System.IO.File.Exists(configurationFilePath))
               return false;
            m_ConfigurationFilePath = configurationFilePath;
            m_Configurations = Configuration.ServicesConfiguration.FromFile(
               configurationFilePath);
         }
         catch(Exception ex)
         {
            Diagnostics.ResultLog r = new Diagnostics.ResultLog();
            r.Failed(ex);
            throw new InvalidServiceConfigurationException(
               "Service Configuration was expected but not found.", ex);
         }

         return true;
      }

#endif
      */

   }

}
