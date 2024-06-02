using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.

using CSProperties = Edam.Data.ConnectionStringProperties;
using ConfigKeys = Edam.Data.DatabaseConfigKeys;

namespace Edam.Data
{

   /// <summary>
   /// Connection String properties.
   /// </summary>
   public struct ConnectionStringProperties
   {
      public static readonly String DataSourceLabel = "data source";
      public static readonly String InitialCatalogLabel = "initial catalog";
      public static readonly String UserIdLabel = "user id";
      public static readonly String PasswordLabel = "password";
      public static readonly String IntegratedSecurityLabel =
         "integrated security";
   }
   
   public struct DatabaseServers
   {
      public static readonly String ServerAzureExtention =
         ".database.windows.net";
      public static readonly String ServerAzureAdditionalProperties = 
         ";Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
   }

   /// <summary>
   /// Database Configuration Keys
   /// </summary>
   public struct DatabaseConfigKeys
   {
      public static readonly String DefaultDbKeyLabel = "DefaultDbKey";
   }

   /// <summary>
   /// Data Source Info...
   /// </summary>
   //[ Serializable ]
   public class DataSourceInfo : Edam.Services.IServiceInfo
   {

      public static readonly string DEFAULT_KEY = "Default";

      private String m_ConnectionString;

      public String KeyId { get; set; }
      public String Alias { get; set; }
      public String HostName { get; set; }
      public String CatalogName { get; set; }
      public String UserId { get; set; }
      public Edam.Security.Password Password { get; set; }
      public String AccountId { get; set; }
      public bool IsActive { get; set; }
      public String Version { get; set; }
      public String ConnectionString
      {
         get
         {
            if (String.IsNullOrEmpty(m_ConnectionString))
               m_ConnectionString = GetConnectionString();
            return m_ConnectionString;
         }
         set { m_ConnectionString = value; }
      }
      public DbAuthenticationMethod AuthenticationMethod { get; set; }
      public String AdditionalConnectionProperties { get; set; }

      #region -- 1.0 Implement IServiceInfo

      [XmlIgnoreAttribute]
      public String Key
      {
         get { return KeyId; }
         set { KeyId = value; }
      }

      /// <summary>
      /// This will be the Server DNS entry name or the IP address. (e.g.
      /// MyADServerName or 10.72.76.134)
      /// </summary>
      [XmlIgnoreAttribute]
      public String ServicePathUri
      {
         get { return HostName; }
         set { HostName = value; }
      }

      public String ServiceLogFileFolderPath { get; set; }    
      public String ServiceLogFileName { get; set; }


      [XmlIgnoreAttribute]
      public String AccountUserId
      {
         get { return UserId; }
         set { UserId = value; }
      }

      [XmlIgnoreAttribute]
      public Edam.Security.Password AccountPassword
      {
         get { return Password; }
         set { Password = value; }
      }

      [XmlIgnoreAttribute]
      public String AccountDomain { get; set; }

      [XmlIgnoreAttribute]
      public Boolean IsValid
      {
         get { return true; }
      }
      
      #endregion

      /// <summary>
      /// Default connection to allow for multiple provider calls using the same
      /// connection usefull in those cases where a transaction should be used
      /// over those sets of database requests.  Try to keep it null if it is 
      /// not being used.
      /// </summary>
      //[XmlIgnore]
      public DataConnection DefaultConnection { get; set; }

      /// <summary>
      /// Initialize a Data Source...
      /// </summary>
      public DataSourceInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields.
      /// </summary>
      public void ClearFields()
      {
         KeyId = String.Empty;
         Alias = String.Empty;
         HostName = String.Empty;
         CatalogName = String.Empty;
         UserId = String.Empty;
         Password = new Security.Password(String.Empty);
         ConnectionString = String.Empty;
         AuthenticationMethod = DbAuthenticationMethod.Unknown;
         AdditionalConnectionProperties = String.Empty;
      }

      /// <summary>
      /// Get a connection string build with current data source details.
      /// </summary>
      /// <returns>connection string is returned</returns>
      public String GetConnectionString()
      {
         if (!String.IsNullOrWhiteSpace(ConnectionString))
         {
            return ConnectionString;
         }
         return GetConnectionString(
            HostName, CatalogName, UserId, Password,
            AdditionalConnectionProperties, AuthenticationMethod);
      }

      /// <summary>
      /// Get the data-source for the given key...
      /// </summary>
      /// <param name="configurationKey">configuration Key</param>
      /// <returns>The default DataSource instance is returned</returns>
      public static DataSourceInfo GetConnectionString(String configurationKey)
      {
         DataSourceInfo d = Edam.Application.Session.
            DataSourceCollection.Find(configurationKey);
         if (d != null)
            return d;

         d = new DataSourceInfo();
         String key = (String.IsNullOrWhiteSpace(configurationKey)) ?
            ConfigKeys.DefaultDbKeyLabel : configurationKey;

         d.KeyId = configurationKey;
         if (!String.IsNullOrEmpty(configurationKey))
            d.ConnectionString = DEFAULT_KEY;
            // System.Configuration.ConfigurationManager.
            //    ConnectionStrings[configurationKey].ConnectionString;
         else
            d.ConnectionString = String.Empty;
         if (!String.IsNullOrEmpty(key))
            Edam.Application.Session.DataSourceCollection.Add(key, d);
         return d;
      }

      /// <summary>
      /// Get a connection string for NT-Authentication for the following host
      /// and catalog (database).
      /// </summary>
      /// <param name="host">host</param>
      /// <param name="catalog">catalog</param>
      /// <returns>prepared connection string is returned</returns>
      public static String GetConnectionString(String host, String catalog)
      {
         return GetConnectionString(host, catalog, null, null, String.Empty,
            DbAuthenticationMethod.NtAuthentication);
      }

      /// <summary>
      /// Get/prepare the default data-source...
      /// </summary>
      /// <returns>The default DataSource instance is returned</returns>
      public static DataSourceInfo GetDefaultDataSource()
      {
         DataSourceInfo d = new DataSourceInfo();

         d.KeyId = "Default";
         d.HostName = Edam.Application.AppSettings.
            GetString("DefaultDbHost");
         d.CatalogName = Edam.Application.AppSettings.
            GetString("DefaultDbCatalog");
         d.UserId = Edam.Application.AppSettings.
            GetString("DefaultDbUserId");
         d.Password = new Security.Password(Edam.Application.AppSettings.
            GetString("DefaultDbPassword"));

         String a = Edam.Application.AppSettings.
            GetString("DefaultDbAuthentication");
         d.AuthenticationMethod = (a.ToLower() == "sqlauthentication") ?
            DbAuthenticationMethod.SqlAuthentication :
            DbAuthenticationMethod.NtAuthentication;

         d.ConnectionString = d.AuthenticationMethod ==
            DbAuthenticationMethod.NtAuthentication ?
               GetConnectionString(d.HostName, d.CatalogName) :
               GetConnectionString(d.HostName, d.CatalogName,
                  d.UserId, d.Password, d.AdditionalConnectionProperties);

         return d;
      }

      /// <summary>
      /// Prepare default data source list...
      /// </summary>
      /// <returns>prepared list is returned</returns>
      public static List<DataSourceInfo> PrepareDataSources()
      {
         List<DataSourceInfo> l = new List<DataSourceInfo>();
         DataSourceInfo d = GetDefaultDataSource();
         l.Add(d);
         return l;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Prepare a connection string using provided info.
      /// </summary>
      /// <param name="host">host</param>
      /// <param name="catalog">catalog</param>
      /// <param name="userId">user Id</param>
      /// <param name="password">password</param>
      /// <param name="additionalConnectionProperties"></param>
      /// <param name="authenticationMethod">authentication Method</param>
      /// <returns>connection string is returned</returns>
      public static String GetConnectionString(String host, String catalog,
         String userId, Edam.Security.Password password,
         String additionalConnectionProperties = "",
         DbAuthenticationMethod authenticationMethod =
            DbAuthenticationMethod.SqlAuthentication)
      {
         System.Data.Common.DbConnectionStringBuilder b =
            new System.Data.Common.DbConnectionStringBuilder();

         b.Add(CSProperties.DataSourceLabel, host);
         b.Add(CSProperties.InitialCatalogLabel, catalog);

         switch (authenticationMethod)
         {
            case DbAuthenticationMethod.SqlAuthentication:
               if (!String.IsNullOrEmpty(userId))
                  b.Add(CSProperties.UserIdLabel, userId);
               if (!String.IsNullOrEmpty(password.ClearText))
                  b.Add(CSProperties.PasswordLabel, password.ClearText);
               break;
            case DbAuthenticationMethod.NtAuthentication:
               b.Add(CSProperties.IntegratedSecurityLabel, true);
               break;
         }

         // is this an azure instance?
         if (!String.IsNullOrEmpty(additionalConnectionProperties))
         {
            if (additionalConnectionProperties.Substring(0, 1) != ";")
               additionalConnectionProperties = 
                  ";" + additionalConnectionProperties;
            b.ConnectionString += additionalConnectionProperties;
         }

         return b.ConnectionString;

         //System.Text.StringBuilder sb = new System.Text.StringBuilder();
         //sb.Append("user id=");
         //sb.Append(userId);
         //sb.Append(";password=");
         //sb.Append(password.ClearText);
         //sb.Append(";" + InitialCatalogLabel + "=");
         //sb.Append(catalog);
         //sb.Append(";" + InitialCatalogLabel + "=");
         //sb.Append(host);

         //return sb.ToString();
      }

#else

      public static String GetConnectionString(String host, String catalog,
         String userId, Edam.Security.Password password,
         String additionalConnectionProperties = "",
         DbAuthenticationMethod authenticationMethod =
         DbAuthenticationMethod.SqlAuthentication)
      {
         return string.Empty;
      }

#endif

   }

}
