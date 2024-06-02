using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Settings = System.Configuration.ConfigurationManager;

// -----------------------------------------------------------------------------
using Diag = Edam.Diagnostics;
using resource = Edam.Application.ApplicationHelper;
using Edam.DataObjects.Properties;
using Edam.Data;
using app = Edam.Application;

namespace Edam.Application
{

   /// <summary>
   /// Application Session
   /// </summary>
   public class Session
   {

      #region -- define Macros and Labels

      public static IMessageBox MessageBox;
      private static String m_LastSetDataSourceKey = String.Empty;

      public static readonly string DEFAULT_TEXT = "Default";
      public static readonly string DEFAULT_AGENT_ID = "DUMMY";
      public static readonly string DEFAULT_DOMAIN_NAME = "openk.com";
      public static readonly string DEFAULT_APPLICATION_ID = "COMMONS";
      public static readonly string DEFAULT_ORGANIZATION_ID = "COMMONS";

      public static readonly String UnknownDomain = "Unknown";
      public static readonly String DefaultSessionKey = "Default";
      public static readonly String DefaultAgentOrganizationId =
         "DefaultAgentOrganizationId";
      public static readonly String DefaultApplicationId =
         "DefaultApplicationId";
      public static readonly String DefaultDomainId = "DefaultDomainId";
      public static readonly String DefaultAgentId = "DefaultAgentId";
      public static readonly String DefaultTestDbKey = "Test";

      #endregion
      #region -- define BaseSessionInfo properties

      public static String Key
      {
         get { return m_Session.Key; }
         set { m_Session.Key = value; }
      }
      public static RuntimeEnvironment RuntimeEnvironment
      {
         get { return m_Session.RuntimeEnvironment; }
         set { m_Session.RuntimeEnvironment = value; }
      }
      public static Edam.LocaleLanguage Language
      {
         get { return m_Session.Language; }
         set { m_Session.Language = value; }
      }
      public static String ApplicationId
      {
         get { return m_Session.ApplicationId; }
         set { m_Session.ApplicationId = value; }
      }
      public static String SessionId
      {
         get { return m_Session.SessionId; }
         set { m_Session.SessionId = value; }
      }
      public static String DomainId
      {
         get { return m_Session.DomainId; }
         set { m_Session.DomainId = value; }
      }
      public static String UserId
      {
         get { return m_Session.UserId; }
         set { m_Session.UserId = value; }
      }
      public static Boolean UserIsActive
      {
         get { return m_Session.UserIsActive; }
         set { m_Session.UserIsActive = value; }
      }
      public static Boolean UserCanUseApplication
      {
         get { return m_Session.UserCanUseApplication; }
         set { m_Session.UserCanUseApplication = value; }
      }
      public static String ApplicationName
      {
         get { return m_Session.ApplicationName; }
         set { m_Session.ApplicationName = value; }
      }
      public static String EntityId
      {
         get { return m_Session.EntityId; }
         set { m_Session.EntityId = value; }
      }

      public static String OrganizationId
      {
         get { return m_Session.OrganizationId; }
         set { m_Session.OrganizationId = value; }
      }
      public static String AgentId
      {
         get { return m_Session.AgentId; }
         set { m_Session.AgentId = value; }
      }
      public static String AgentName
      {
         get { return m_Session.AgentName; }
         set { m_Session.AgentName = value; }
      }

      public static List<BasePolicyInfo> Policies
      {
         get { return m_Session.Policies; }
         //set { m_Session.Policies = value; }
      }

      public static Dictionary<string,IProperty> Properties
      {
         get { return m_Session.Properties; }
      }

      #endregion
      #region -- define Session specific properties

      private static Int32 m_SessionCounter = 0;
      private static BaseSessionInfo m_Session = null;
      private static List<BaseSessionInfo> m_Sessions =
         new List<BaseSessionInfo>();

      private static BaseSessionInfo BaseSession
      {
         get { return m_Session; }
      }

      private static Data.DataSources m_DataSourceCollection;
      public static Data.DataSources DataSourceCollection
      {
         get { return m_DataSourceCollection; }
      }

      private static Edam.DataObjects.Entities.UserLoggedInfo m_LoggedUser = null;
      public static Edam.DataObjects.Entities.UserLoggedInfo LoggedUser
      {
         get
         {
            if (m_LoggedUser == null)
            {
               var u = new Edam.DataObjects.Entities.UserLoggedInfo();
               return u;
            }
            return m_LoggedUser;
         }
      }
      public static Boolean IsUserLogged
      {
         get
         {
            return (LoggedUser.IsActive);
         }
      }

      #endregion
      #region -- Initialization Constructor / Destructor...

      /// <summary>
      /// Initialize base (default) sesion and load configurations...
      /// </summary>
      static Session()
      {
         MessageBox = null;
         BaseSessionInfo session = new BaseSessionInfo();
         session.Key = DefaultSessionKey;
         Add(session);

         //Edam.Services.ServicesSession.LoadConfiguration();
         m_DataSourceCollection = new Data.DataSources();
#if DATA_SUPPORT_
         m_DataSourceCollection.RegisterDefaultDataSource();
#endif

         InitAppSettings();
      }

      public static void ClearFields()
      {
         if (m_Session == null)
            return;
         m_Session.ClearFields();
      }

      /// <summary>
      /// Add default Application Runtime Properties...
      /// </summary>
      public static void InitAppSettings()
      {
         string k = AppSettings.GetString("DefaultUrlSource");
         string v = AppSettings.GetString(k);
         Properties.Add(k, new Property { KeyId = k, Name = k, Value = v });
      }

      #endregion
      #region -- Get Configured defaults

      public static String GetDefaultSessionKey()
      {
         return DefaultSessionKey;
      }

      /// <summary>
      /// Get default agent ID if none is given.
      /// </summary>
      /// <param name="agentId">agent ID. to use</param>
      /// <returns>configured agent ID. is returned if given value is
      /// null or empty</returns>
      public static String GetDefaultAgentId(String agentId = null)
      {
         if (!String.IsNullOrEmpty(agentId))
            return agentId;
         return DEFAULT_AGENT_ID;
         //return Settings.AppSettings.Get(DefaultAgentId);
      }

      /// <summary>
      /// Get default organization ID if none is given.
      /// </summary>
      /// <param name="organizationId">organization to use</param>
      /// <returns>configured organization is returned if given value is
      /// null or empty</returns>
      public static String GetDefaultAgentOrganization(
         String organizationId = null)
      {
         if (!String.IsNullOrEmpty(organizationId))
            return organizationId;
         return DEFAULT_ORGANIZATION_ID;
         //return Settings.AppSettings.Get(DefaultAgentOrganizationId);
      }

      /// <summary>
      /// Get default application ID. if none is given.
      /// </summary>
      /// <param name="applicationId">application to use</param>
      /// <returns>configured application is returned if given value is
      /// null or empty</returns>
      public static String GetDefaultApplicationId(String applicationId = null)
      {
         if (!String.IsNullOrEmpty(applicationId))
            return applicationId;
         return DEFAULT_APPLICATION_ID;
         //return Settings.AppSettings.Get(DefaultApplicationId);
      }

      /// <summary>
      /// Get default domain ID. if none is given.
      /// </summary>
      /// <param name="domainId">application to use</param>
      /// <returns>configured domain is returned if given value is
      /// null or empty</returns>
      public static String GetDefaultDomain(String domainId = null)
      {
         if (!String.IsNullOrEmpty(domainId))
            return domainId;
         String domain = Edam.Application.
            AppSettings.GetString(DefaultDomainId);
         if (String.IsNullOrEmpty(domain))
            domain = DEFAULT_DOMAIN_NAME; //System.Environment.UserDomainName;
         if (String.IsNullOrEmpty(domain))
            domain = UnknownDomain;
         return domain;
      }
      
      /// <summary>
      /// Get Default application purposes if none where given...
      /// </summary>
      /// <param name="purposes">string array of purposes</param>
      /// <returns>purposes as string array</returns>
      public static String[] GetPurposes(String[] purposes)
      {
         if (purposes != null && purposes.Length > 0)
            return purposes;
         String[] p = new String[3];
         
         String org = GetDefaultAgentOrganization();
         String aid = GetDefaultApplicationId();
         String did = GetDefaultDomain();

         p[0] = org;
         p[1] = aid;
         p[2] = did;
         return p;
      }

      #endregion
      #region -- Default Connection Strings support...

      public static Data.DataSourceInfo GetApplicationDefaultDataSource()
      {
         if (m_DataSourceCollection != null)
         {
            return m_DataSourceCollection.SelectedDataSource;
         }
         Data.DataSourceInfo source = new Data.DataSourceInfo();
         return source;
      }

      public static String GetApplicationDefaultConnectionString()
      {
         return DataSources.GetDefaultConnectionString(m_DataSourceCollection);
      }

      /// <summary>
      /// Get DataSource for given Key.
      /// </summary>
      /// <param name="keyId">data source key</param>
      /// <returns>instance of Data Source...</returns>
      public static Data.DataSourceInfo GetDataSource(String keyId)
      {
         Data.DataSourceInfo s = m_DataSourceCollection.Find(keyId);
         if (s == null)
         {
            s = m_DataSourceCollection.SelectedDataSource;
         }
         if (s == null)
         {
            s = DataSources.GetDataSource(keyId);
         }
         return s;
      }

      /// <summary>
      /// Set Data Source by its KeyId.
      /// </summary>
      /// <param name="keyId">key id</param>
      /// <returns>true is returned if set</returns>
      public static Boolean SetDataSource(String keyId)
      {
         Data.DataSourceInfo s = m_DataSourceCollection.Find(keyId);
         if (s != null)
         {
            m_DataSourceCollection.SelectedDataSource = s;
            m_LastSetDataSourceKey = keyId;
         }
         return (s != null);
      }

      #endregion
      #region -- Default Session support...

      #endregion
      #region -- Exception and Error Messages Login

      private static Diag.LogSettings m_LogSettings =
         new Diag.LogSettings();
      public static Diag.LogSettings LogSettings
      {
         get
         {
            return(m_LogSettings);
         }
      }  // end of ApplicationLogs

      /// <summary>
      /// Get Log Settings...
      /// </summary>
      /// <param name="severity">severity</param>
      /// <returns>log entry found is returned, else an empty one is returned
      /// </returns>
      public static Diag.LogSettings GetLogSetting(Diag.SeverityLevel severity)
      {
         Diag.LogSettings entry = null;
            //Edam.Services.ServicesSession.
            //   Configurations.FindLogSettings(severity);
         if (entry == null)
            return new Diag.LogSettings();
         return entry;
      }  // end of GetLogSetting

      /// <summary>Log message in default message log</summary>
      /// <date>Oct/2k4 (ESob)</date>
      public static Boolean LogMessage(global::System.Exception exception)
      {
         Diag.LogSettings l = GetLogSetting(
            Diag.SeverityLevel.Fatal);
         return (Diag.Log.Write(l, String.Empty, exception));
      }  // end of LogMessage

      /// <summary>Log message in default message log</summary>
      /// <param name="Level">SeverityLevel</param>
      /// <param name="Location">Location identifying where the error occured
      /// </param>
      /// <param name="Message">Message to log</param>
      /// <date>Dec/2k5 (ESob)</date>
      public static Boolean LogMessage(
         Edam.Diagnostics.SeverityLevel level,
         String location, String message)
      {
         Diag.LogSettings l = GetLogSetting(level);
         return (Diag.Log.Write(l, level, location, message));
      }  // end of LogMessage

      /// <summary>Log message in default message log</summary>
      /// <param name="Location">Location identifying where the error occured
      /// </param>
      /// <param name="Exception">Exception to log</param>
      /// <date>Dec/2k5 (ESob)</date>
      public static Boolean LogMessage(
         String location, System.Exception exception)
      {
         Diag.LogSettings l = GetLogSetting(
            Diagnostics.SeverityLevel.Fatal);
         return (Diag.Log.Write(l, location, exception));
      }  // end of LogMessage

      public static Boolean LogMessage(
         String Location, String Message)
      {  return(LogMessage(Edam.Diagnostics.SeverityLevel.Info,
            Location, Message)); }

      #endregion
      #region -- User / System User

      public static String SystemUserName
      {
         get
         {
            return DEFAULT_AGENT_ID; // System.Environment.UserName;
         }
      }

      public static String SystemUserDomainName
      {
         get
         {
            return DEFAULT_DOMAIN_NAME; // System.Environment.UserDomainName;
         }
      }

      public static String SystemUserAndDomainName
      {
         get { return SystemUserDomainName + "\\" + SystemUserName; }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="policy"></param>
      /// <returns></returns>
      public static Boolean UserHasPolicy(PolicyType policy)
      {
         Boolean found = false;
         foreach(var p in Policies)
         {
            if (p.PolicyNo == (Int32)policy)
            {
               if (p.Value != null && p.Value.ToLower() == 
                  Edam.Application.Resources.Strings.True)
               {
                  found = true;
                  break;
               }
            }
         }
         return found;
      }

      /// <summary>
      /// Set or reset application logged user.
      /// </summary>
      /// <param name="user">user login info, pass it as null to reset</param>
      public static void SetUser(Edam.DataObjects.Entities.UserLoggedInfo user)
      {
         m_LoggedUser = user;
         if (user == null)
         {
            UserCanUseApplication = false;
            UserIsActive = false;
            AgentName = String.Empty;
            AgentId = String.Empty;
            OrganizationId = String.Empty;
            EntityId = String.Empty;
            SessionId = String.Empty;
            m_Session.Policies = new List<BasePolicyInfo>();
            UserId = String.Empty;
            return;
         }

         Session.AgentName = user.Alias;
         Session.AgentId = user.ReferenceId;
         Session.OrganizationId = user.OrganizationId;
         //Session.ApplicationId = String.Empty;
         //Session.ApplicationName = String.Empty;
         //Session.DomainId = String.Empty;
         Session.EntityId = user.ReferenceId;
         Session.Language = LocaleLanguage.English;
         Session.m_Session.Policies = user.Policies;
         //Session.RuntimeEnvironment = RuntimeEnvironment.Production;
         Session.SessionId = user.SessionId;
         Session.UserId = user.ReferenceId;
         Session.UserCanUseApplication =
            UserHasPolicy(PolicyType.CanUseApplication);
         Session.UserIsActive = user.IsActive;
      }

      /// <summary>
      /// Reset Logged user (if any)
      /// </summary>
      public static void LogoutUser()
      {
         SetUser(null);
      }

      #endregion
      #region -- Session Management

      /// <summary>
      /// Set session as Current.
      /// </summary>
      /// <param name="session">session to set.</param>
      public static void SetSession(String key)
      {
         BaseSessionInfo s = Find(key);
         if (s == null)
            return;
         m_Session = s;
      }

      /// <summary>
      /// Given a session key, try to find it and make it current.
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public static BaseSessionInfo Find(String key)
      {
         BaseSessionInfo foundSession = null;
         foreach (BaseSessionInfo s in m_Sessions)
         {
            if (s.Key == key)
            {
               foundSession = s;
               m_Session = s;
               break;
            }
         }
         return foundSession;
      }

      /// <summary>
      /// Add session and make it current.
      /// </summary>
      /// <param name="session"></param>
      public static void Add(BaseSessionInfo session)
      {
         if (session == null)
            return;
         if (String.IsNullOrEmpty(session.Key))
            session.Key = "Session." + m_SessionCounter.ToString();
         BaseSessionInfo s = Find(session.Key);
         if (s != null)
            return;

         m_SessionCounter++;
         m_Sessions.Add(session);
         SetSession(session.Key);
      }

      /// <summary>
      /// Reset the Database Source to the default and initial source as
      /// configured.
      /// </summary>
      public static void ResetDatabaseSourceToDefault()
      {
         Data.DataSourceInfo s;
         if (!String.IsNullOrEmpty(m_LastSetDataSourceKey))
         {
            s = m_DataSourceCollection.Find(m_LastSetDataSourceKey);
            if (s != null)
               m_DataSourceCollection.SelectedDataSource = s;
         }
      }

      /// <summary>
      /// Check session, organization Id and reference Id's, if anyone of those
      /// is empty then switch the database to use the Test or public database.
      /// </summary>
      /// <param name="sessionId">session Id</param>
      /// <param name="organizationId">organization Id</param>
      /// <param name="referenceId">reference Id</param>
      public static void CheckSessionAndReference(
         String sessionId, 
         String organizationId,
         String referenceId)
      {
         Data.DataSourceInfo s;
         if (String.IsNullOrEmpty(sessionId) ||
             String.IsNullOrEmpty(organizationId) ||
             String.IsNullOrEmpty(referenceId) ||
            (referenceId.ToLower() ==
             Edam.Application.Resources.Strings.DefaultPublicId.ToLower()))
         {
            s = m_DataSourceCollection.Find(DefaultTestDbKey);
            if (s != null)
            {
               // recall last set data source...
               Data.DataSourceInfo currentDs =
                  m_DataSourceCollection.SelectedDataSource;
               if (currentDs != null)
               {
                  if (currentDs.KeyId != DefaultTestDbKey)
                     m_LastSetDataSourceKey = currentDs.KeyId;
               }

               m_DataSourceCollection.SelectedDataSource = s;
            }
         }
         else
            ResetDatabaseSourceToDefault();
      }

      #endregion
      #region -- 4.00 - Message Box

      /// <summary>
      /// Show Message Box if implemented...
      /// </summary>
      /// <param name="prompt">Window banner message</param>
      /// <param name="message">Window inner message</param>
      /// <param name="action"></param>
      /// <param name="type"></param>
      public static void ShowMessageBox(String prompt, String message,
         Action<bool> action = null, MessageBoxType type = MessageBoxType.Done)
      {
         if (MessageBox == null)
            return;
         MessageBox.ShowMessageBox(prompt, message, action, type);
      }

      #endregion
      #region -- 4.00 - application folder - file paths support

      /// <summary>
      /// Get Application folder - file full path.
      /// </summary>
      /// <param name="folderName">folder name</param>
      /// <param name="fileName">file name</param>
      /// <param name="extension">(optional) file extension without the dot
      /// </param>
      /// <returns>the application full path is returned</returns>
      public static string GetApplicationFullPath(
         string folderName, string fileName, string extension = null)
      {
         return app.AppSettings.ApplicationFolder +
            folderName + "/" + fileName +
            (String.IsNullOrWhiteSpace(extension) ?
               String.Empty : "." + extension);
      }

      #endregion

   }  // end of Session

}  // end of Edam.Application

