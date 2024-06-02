using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

// -----------------------------------------------------------------------------

//using Ldap = Edam.Net.Ldap;
using Diag = Edam.Diagnostics;
//using Edam.Net;

namespace Edam.Services.Configuration
{

   public enum TraceLevel
   {
      Unknown = 0,
      Normal = 1,
      Full = 99
   }

   /// <summary>
   /// 
   /// </summary>
   [XmlRoot("ServiceConfigurations")]
   public class ServicesConfiguration
   {
      public static readonly String ConfigFileKey = "DefaultServiceConfigFile";
      public static readonly String ServicesConfigFileKey =
         "DefaultServiceConfigFile";
      public static readonly String TraceLevelKey = "DefaultTraceLevel";
      public static readonly String TraceLevelNormal = "normal";
      public static readonly String TraceLevelFull = "full";

      [XmlArray("BaseServices")]
      [XmlArrayItem("Service")]
      public List<Edam.Services.ServiceBaseInfo> BaseServices { get; set; }

      [XmlArray("LogSettings")]
      [XmlArrayItem("Setting")]
      public List<Diag.LogSettings> LogSettings { get; set; }

      [XmlArray("Assemblies")]
      [XmlArrayItem("Type")]
      public List<Edam.Services.TypeInfo> Assemblies { get; set; }

      /// <summary>
      /// Get Trace Level by reading config file.
      /// </summary>
      /// <returns></returns>
      public static TraceLevel GetTraceLevel()
      {
         TraceLevel traceLevel = TraceLevel.Unknown;
         String levelText = Edam.Application.
            AppSettings.GetString(TraceLevelKey);
         if (String.IsNullOrEmpty(levelText))
            return traceLevel;
         
         levelText = levelText.ToLower();
         if (levelText == TraceLevelNormal)
            traceLevel = TraceLevel.Normal;
         else
         if (levelText == TraceLevelFull)
            traceLevel = TraceLevel.Full;
         else
            traceLevel = TraceLevel.Unknown;
         return traceLevel;
      }

      /// <summary>
      /// Add an SMTP service configuration item...
      /// </summary>
      /// <param name="config"></param>
      public void Add(Edam.Services.ServiceBaseInfo config)
      {
         if (config == null)
            return;
         if (BaseServices == null)
            BaseServices = new List<ServiceBaseInfo>();

         BaseServices.Add(config);
      }

      /// <summary>
      /// Add a new Settings entry with given info...
      /// </summary>
      /// <param name="settings">instance of data source to add</param>
      public void Add(Diag.LogSettings settings)
      {
         if (settings == null)
            return;
         if (LogSettings == null)
            LogSettings = new List<Diag.LogSettings>();
         LogSettings.Add(settings);
      }  // end of Add

      /// <summary>
      /// Add a LDAP configuration item...
      /// </summary>
      /// <param name="config">configuration to be added</param>
      public Edam.Services.ServiceBaseInfo FindBaseService(String key)
      {
         return BaseServices.Find(x => x.Key == key);
      }

      /// <summary>
      /// Find Type.
      /// </summary>
      /// <param name="key">key</param>
      /// <returns>TypeInfo instance is returned if found, null if not</returns>
      public Edam.Services.TypeInfo FindType(String key)
      {
         if (String.IsNullOrEmpty(key))
            return null;
         if (Assemblies.Count <= 0)
            return null;
         return Assemblies.Find(x => x.Key == key);
      }

      /// <summary>
      /// Find a settings item by key...
      /// </summary>
      /// <param name="keyId">Key Id.</param>
      /// <returns>if found an instance of LogSetting is returned
      /// </returns>
      public Diag.LogSettings FindLogSettings(String keyId)
      {
         if (String.IsNullOrEmpty(keyId))
            return null;
         if (LogSettings.Count <= 0)
            return null;
         return LogSettings.Find(x => x.KeyId == keyId);
      }  // end of Find

      /// <summary>
      /// Find a settings item by severity...
      /// </summary>
      /// <param name="severity">Severity</param>
      /// <returns>if found an instance of LogSetting is returned
      /// </returns>
      public Diag.LogSettings FindLogSettings(Diag.SeverityLevel severity)
      {
         if (severity == Diag.SeverityLevel.Unknown)
            return null;
         if (LogSettings.Count <= 0)
            return null;
         return LogSettings.Find(x => x.Severity == severity);
      }  // end of Find
      
   }

}

