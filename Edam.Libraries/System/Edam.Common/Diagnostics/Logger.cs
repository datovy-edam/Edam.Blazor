using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using app = Edam.Application.Resources;

// -----------------------------------------------------------------------------
// 2012-04-28 The following code was copied verbatum from the Open Knowledge -
// KIFv3r0 Framework Project.

namespace Edam.Diagnostics
{

   public class LoggerEventArgs : EventArgs
   {
      public Object Sender { get; set; }
      public String Message { get; set; }
      public String Source { get; set; }
      public DateTime EventDateTime { get; set; }

      public String DateTimeText
      {
         get
         {
            return EventDateTime.ToString(
               Edam.Application.Resources.Strings.DefaultLoggerDateFormat);
         }
      }

      public LoggerEventArgs() : base()
      {
         EventDateTime = DateTime.Now;
      }
      public LoggerEventArgs(String message, String source) : base()
      {
         Message = message;
         Source = source;
         EventDateTime = DateTime.Now;
      }
   }

   public delegate void LoggerWrite(Object sender, LoggerEventArgs args);

   /// <summary>
   /// Define the Log class used to manage a log that will help to write to a
   /// system log or to a file based on the application configuration section
   /// defined as "LogsConfig".
   /// </summary>
   /// <author>Eduardo Sobrino</author>
   /// <date>Oct/2k3</date>
   public class Logger : ILogger, IDisposable // ,IEventLogger, IFileLogger
   {

      #region -- 1.0 - Define Fields and Properties

      //private static readonly String SystemEventLogExpected =
      //   "EventLog or Source and Log names of instance were expected";
      private static readonly String FilePathNameExpected =
         "File pathname was expected";
      private static readonly String LogSettingsExpected =
         "Log Settings were expected";
      
      private Verbosity m_Verbosity =
         Edam.Diagnostics.Verbosity.ErrorsOnly;

      private IResultsLog m_Results = new ResultLog();
      private LogSettings m_LogSettings = new LogSettings();

      public LogSettings Settings
      {
         get { return m_LogSettings; }
      }

      public Verbosity Verbosity
      {
         get { return m_Verbosity; }
         set { m_Verbosity = value; }
      }
      public IResultsLog Results
      {
         get { return m_Results; }
         set { m_Results = (IResultsLog)value; }
      }

      public IEventLogger EventLog
      {
         get { return (IEventLogger)this; }
      }

      public IFileLogger FileLog
      {
         get { return (IFileLogger)this; }
      }

      public static event LoggerWrite LoggerWriteHandler;

      #endregion
      #region -- 2.0 - Initialize Object (ctor/dtor)

      /// <summary>
      /// Initialize a System Event Log.
      /// </summary>
      /// <param name="sourceName">(your) source name</param>
      /// <param name="logName">target system log name</param>
      /// <param name="log">(optional) log</param>
      public IResultsLog InitializeLog(String sourceName, String logName)
      {
         ResultLog results = new ResultLog();
         return results;
      }

      /// <summary>
      /// Initialize a System Event Log.
      /// </summary>
      /// <param name="sourceName">(your) source name</param>
      /// <param name="logName">target system log name</param>
      public Logger(String sourceName, String logName)
      {
         InitializeLog(sourceName, logName);
      }

      /// <summary>
      /// Initialize a File Log.
      /// </summary>
      /// <param name="filePathName">log file pathname</param>
      public Logger(String filePathName)
      {
         if (String.IsNullOrEmpty(filePathName))
            throw new Exception(FilePathNameExpected);
         m_LogSettings.SendToApplicationLog = true;
         m_LogSettings.ApplicationLogPath = filePathName;
      }

      /// <summary>
      /// Initialize with given log settings.
      /// </summary>
      /// <param name="settings">settings</param>
      public Logger(LogSettings settings)
      {
         if (settings == null)
            throw new Exception(LogSettingsExpected);
         m_LogSettings = settings;
         if (settings.SendToSystemLog)
            InitializeLog(settings.SystemSourceName, settings.SystemLogName);
      }

      public Logger()
      {

      }

      #endregion

#if EVENT_LOG_SUPPORT
      #region -- 1.0 - Define Fields and Properties

      private EventLog m_EventLog = null;

      #endregion
      #region -- 2.0 - Initialize EventLog (ctor/dtor)

      /// <summary>
      /// Initialize a System Event Log.
      /// </summary>
      /// <param name="sourceName">(your) source name</param>
      /// <param name="logName">target system log name</param>
      /// <param name="log">(optional) log</param>
      public IResultsLog InitializeLog(String sourceName, String logName,
         System.Diagnostics.EventLog log)
      {
         ResultLog results = new ResultLog();

         if (String.IsNullOrEmpty(sourceName) || String.IsNullOrEmpty(logName))
            results.Failed(new Exception(SystemEventLogExpected));
         else
         {
            m_LogSettings.SendToSystemLog = true;
            m_LogSettings.SystemLogName = logName;
            m_LogSettings.SystemSourceName = sourceName;
            results = CreateSystemEventLog(sourceName, logName);
            if (results.Success)
               m_EventLog = log == null ? new System.Diagnostics.EventLog(
                  logName, ".", sourceName) : log;
         }
         return results;
      }

      /// <summary>
      /// Initialize a System Event Log with given EventLog details.
      /// </summary>
      /// <param name="log">instance of event log</param>
      public Logger(System.Diagnostics.EventLog log)
      {
         if (log == null)
            throw new Exception(SystemEventLogExpected);
         InitializeLog(log.Source, log.Log, log);
      }
      #endregion
      #region -- 2.0 - Initialize System Event Log Methods

      /// <summary>
      /// Create System Event Log.
      /// </summary>
      /// <remarks>
      /// A System Event log may have many sources therefore specify the source
      /// name for your particular app/process and identify the parent log name.
      /// To create a new Log/Source you need to have Administrative priviliges.
      /// You will find your Log under the "Applications and Services Logs" 
      /// folder in the "Event Viewer".
      /// </remarks>
      /// <param name="sourceName">source name</param>
      /// <param name="logName">log name</param>
      /// <returns>IResultsLog is returned</returns>
      public static ResultLog CreateSystemEventLog(
         String sourceName, String logName)
      {
         // Create the source, if it does not already exist.
         ResultLog results = new ResultLog();
         try
         {
            if (!System.Diagnostics.EventLog.SourceExists(sourceName))
               System.Diagnostics.EventLog.
                  CreateEventSource(sourceName, logName);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }

         return results;
      }

      #endregion
      #region -- 4.0 - Implement Event/File Logger

      /// <summary>
      /// Write message to Event Log.
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="location">location</param>
      /// <param name="level">level</param>
      void IEventLogger.Write(String message, String location,
         SeverityLevel level)
      {
         EventLogEntryType stype = ToLogEntryType(level);

         // Write an informational entry to the event log.
         String outMess = null; //"(" + level.ToString() + ") ";
         if (location != null)
            if (location.Trim().Length > 0)
               outMess += " " + location + ": ";
         outMess = message;

         m_EventLog.WriteEntry(outMess, stype);
      }

      /// <summary>
      /// Write / Log error message.
      /// </summary>
      /// <param name="exception">exception</param>
      void IEventLogger.Write(Exception exception)
      {
         m_EventLog.WriteEntry(GetErrorMessages(exception),
            System.Diagnostics.EventLogEntryType.Error);
      }

      /// <summary>
      /// Write To file.
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="location">location</param>
      /// <param name="level">severity level</param>
      void IFileLogger.Write(String message, String location,
         SeverityLevel level)
      {
         String fpath = GetFilePath(m_LogSettings);

         // create file if needed, else open to append
         StreamWriter sw;
         if (!File.Exists(fpath))
            sw = File.CreateText(fpath);
         else
            sw = File.AppendText(fpath);

         // investigate if we do have permission to write to disk
         //Edam.Security.ResourceAccessRights.AccessRights accr =
         //   new Edam.Security.ResourceAccessRights.AccessRights(fpath);
         //if (!(accr.CanWrite && accr.CanModify))
         //   return(false);

         if (sw != null)
         {
            try
            {
               switch (m_LogSettings.Format)
               {
                  case LogFormat.XML:
                     sw.WriteLine(ToXmlMessage(message, location, level));
                     break;
                  case LogFormat.PlainText:
                  default:
                     sw.WriteLine(ToPlainText(
                        message, location, level, DateTime.Now));
                     break;
               }
               sw.Flush();
               sw.Close();
            }
            finally
            {
               sw.Dispose();
               sw = null;
            }
         }
      }

      /// <summary>
      /// Write exception message to file.
      /// </summary>
      /// <param name="exception">exception</param>
      void IFileLogger.Write(Exception exception)
      {
         String message = GetErrorMessages(exception);
         ((IFileLogger)this).Write(message, null, SeverityLevel.Critical);
      }

      /// <summary>
      /// Write message to file and/or Event Log based on configuration...
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="location">location</param>
      /// <param name="level">level</param>
      void ILoggerWriter.Write(
         String message, String location, SeverityLevel level)
      {
         if (m_LogSettings.SendToApplicationLog)
            ((IFileLogger)this).Write(message, null, level);
         if (m_LogSettings.SendToSystemLog)
            ((IEventLogger)this).Write(message, null, level);
      }

      /// <summary>
      /// Write Exception to file and/or Event Log based on configuration...
      /// </summary>
      /// <param name="exception">exception</param>
      void ILoggerWriter.Write(Exception exception)
      {
         String message = GetErrorMessages(exception);
         if (m_LogSettings.SendToApplicationLog)
            ((IFileLogger)this).Write(message, null, SeverityLevel.Critical);
         if (m_LogSettings.SendToSystemLog)
            ((IEventLogger)this).Write(message, null, SeverityLevel.Critical);
      }

      /// <summary>
      /// Map a severity level into an Log Entry type.
      /// </summary>
      /// <param name="level">severity level</param>
      /// <returns>corresponding EvengLogEntryType is returned</returns>
      static public System.Diagnostics.EventLogEntryType ToLogEntryType(
         SeverityLevel level)
      {
         // map severity to log entry type ...
         System.Diagnostics.EventLogEntryType stype;
         switch (level)
         { //log.Severity) {
            case SeverityLevel.Fatal:
               stype = System.Diagnostics.EventLogEntryType.Error;
               break;
            case SeverityLevel.Critical:
               stype = System.Diagnostics.EventLogEntryType.Error;
               break;
            case SeverityLevel.Warning:
               stype = System.Diagnostics.EventLogEntryType.Warning;
               break;
            case SeverityLevel.Info:
               stype = System.Diagnostics.EventLogEntryType.Information;
               break;
            case SeverityLevel.Debug:
               stype = System.Diagnostics.EventLogEntryType.Information;
               break;
            default:
               stype = System.Diagnostics.EventLogEntryType.Information;
               break;
         }
         return stype;
      }

      #endregion
#endif

      /// <summary>
      /// Write message to file and/or Event Log based on configuration...
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="location">location</param>
      /// <param name="level">level</param>
      void ILoggerWriter.Write(
         String message, String location, SeverityLevel level)
      {
         if (m_LogSettings.SendToApplicationLog)
            ((IFileLogger)this).Write(message, null, level);
         if (m_LogSettings.SendToSystemLog)
            ((IEventLogger)this).Write(message, null, level);
      }

      /// <summary>
      /// Write Exception to file and/or Event Log based on configuration...
      /// </summary>
      /// <param name="exception">exception</param>
      void ILoggerWriter.Write(Exception exception)
      {
         String message = GetErrorMessages(exception);
         if (m_LogSettings.SendToApplicationLog)
            ((IFileLogger)this).Write(message, null, SeverityLevel.Critical);
         if (m_LogSettings.SendToSystemLog)
            ((IEventLogger)this).Write(message, null, SeverityLevel.Critical);
      }

      #region -- 4.0 - Write to log Methods

      /// <summary>
      /// Write to System Event Log.
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="location">location</param>
      /// <param name="level">level</param>
      public void WriteToEventLog(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info)
      {
         ((IEventLogger)this).Write(message, location, level);
      }

      /// <summary>
      /// Write to System Event Log.
      /// </summary>
      /// <param name="exception">exception</param>
      public void WriteToEventLog(Exception exception)
      {
         ((IEventLogger)this).Write(exception);
      }

      /// <summary>
      /// Write To File.
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="location">location</param>
      /// <param name="level">level</param>
      /// <returns>true is returned if message was successfully written
      /// </returns>
      public void WriteToFile(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info)
      {
         ((IFileLogger)this).Write(message, location, level);
      }

      /// <summary>
      /// Write To File.
      /// </summary>
      /// <param name="exception">exception</param>
      public void WriteToFile(Exception exception)
      {
         ((IFileLogger)this).Write(exception);
      }

      /// <summary>
      /// Add message to results log.
      /// </summary>
      /// <param name="message">mesage</param>
      /// <param name="location">(optional) location</param>
      /// <param name="level">(optional) level</param>
      public void ToResults(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info)
      {
         m_Results.Add(message);
      }

      /// <summary>
      /// Add exception to results log.
      /// </summary>
      /// <param name="exception">exception</param>
      public void ToResults(Exception exception)
      {
         m_Results.Failed(exception);
      }

      /// <summary>
      /// Write message using given settings.
      /// </summary>
      /// <param name="settings">log settings</param>
      /// <param name="message">message</param>
      /// <param name="location">(optional)location</param>
      /// <param name="level">(optional)level</param>
      static public void Write(LogSettings settings, String message,
         String location = null, SeverityLevel level = SeverityLevel.Info)
      {
         Logger l = new Logger(settings);
         if (settings.SendToSystemLog)
            ((IEventLogger)l).Write(message, location, level);

         if (settings.SendToApplicationLog)
            ((IFileLogger)l).Write(message, location, level);

         l.Dispose();
      }

      /// <summary>
      /// Write exception using given settings.
      /// </summary>
      /// <param name="settings">log settings</param>
      /// <param name="exception">exception</param>
      /// <param name="location">(optional)location</param>
      static public void Write(LogSettings settings, Exception exception,
         String location = null)
      {
         Logger l = new Logger(settings);
         if (settings.SendToSystemLog)
            ((IEventLogger)l).Write(exception);

         if (settings.SendToApplicationLog)
            ((IFileLogger)l).Write(exception);

         l.Dispose();
      }

      /// <summary>
      /// If defined, write to the console or designated writer(s)...
      /// </summary>
      /// <param name="message">message</param>
      /// <param name="source">source</param>
      public void ConsoleWrite(String message, String source = null)
      {
         m_Results.Add(source, message);
         if (LoggerWriteHandler == null)
            return;
         LoggerWriteHandler(this, new LoggerEventArgs(message, source));
      }

      /// <summary>
      /// If defined, write to the console or designated writer(s)...
      /// </summary>
      /// <param name="exception">exception</param>
      /// <param name="source">source</param>
      public void ConsoleWrite(Exception exception, String source = null)
      {
         ResultLog r = new ResultLog();
         r.Failed(exception);
         m_Results.Copy(r);
         if (LoggerWriteHandler == null)
         {
            return;
         }
         LoggerWriteHandler(this, new LoggerEventArgs(r.MessageText, source));
      }

      #endregion
      #region -- 4.0 - Error Messages Support

      /// <summary>
      /// Given an expection error messages by building a full message going
      /// through all inner exceptions...
      /// </summary>
      /// <param name="exception">original exection</param>
      /// <returns>A string message is returned.</returns>
      static public String GetErrorMessages(Exception exception)
      {
         if (exception == null)
            return String.Empty;
         StringBuilder sb = new StringBuilder();
         sb.Append(exception.Message);
         while (exception.InnerException != null)
         {
            exception = exception.InnerException;
            sb.AppendLine(exception.Message);
         }
         return sb.ToString();
      }  // end of GetErrorMessages

      #endregion
      #region -- 4.0 - Log File Path Support Methods

      /// <summary>
      /// Get the application log path.
      /// </summary>
      /// <returns>the path is returned</returns>
      public static String GetApplicationLogPath()
      {
         return Edam.Application.AppSettings.GetString(
            Edam.Application.Resources.Strings.DefaultApplicationLogPath);
      }

      /// <summary>
      /// Get file path.
      /// </summary>
      /// <param name="log">(optional) log settings</param>
      /// <returns>the file path is returned</returns>
      public static String GetFilePath(LogSettings log = null)
      {
         if (log == null)
            log = new LogSettings();

         // let's check the AppLogPath and see if one was given
         String logPath = log.ApplicationLogPath;

         // if no path was given try to use the defaualt diagnostics
         // directory path if any is available...
         if (String.IsNullOrEmpty(logPath))
         {
            String ddir = Edam.Application.
               Defaults.DefaultDiagnosticsDirectoryPath;
            if (!String.IsNullOrEmpty(ddir))
               logPath = ddir;
         }

         // if log path is still null or empty assign the current directory
         // as default
         if (String.IsNullOrEmpty(logPath))
         {
            //String cdir = Directory.GetCurrentDirectory();
            //logPath = cdir + "/";
            logPath = String.Empty;
         }

         // prepare the file name to be used for the log
         string fname = "."
         + DateTime.Today.Year.ToString("d4")
         + DateTime.Today.Month.ToString("d2")
         + DateTime.Today.Day.ToString("d2")
         + ((log.Format == LogFormat.XML) ? ".xml" : ".txt");
         string fpath = logPath + fname;

         return fpath;
      }

      /// <summary>
      /// Get Application Log Full Path.
      /// </summary>
      /// <returns>returns the full path</returns>
      public static String GetApplicationLogFullPath()
      {
         LogSettings s = new LogSettings();
         s.ApplicationLogPath = GetApplicationLogPath();
         s.Format = LogFormat.PlainText;
         return GetFilePath(s);
      }

      #endregion
      #region -- 4.0 - Log Support Methods

      /// <summary>
      /// Get default verbosity level as configured...
      /// </summary>
      /// <returns>Verbosity level is returned</returns>
      static public Verbosity GetDefaultVerbosityLevel()
      {
         String v = Edam.Application.AppSettings.GetString(
            Edam.Application.Resources.Strings.DefaultVerbosityLevel);
         if (String.IsNullOrWhiteSpace(v))
            return Verbosity.ErrorsOnly;
         else if (v.ToLower() == "trace")
            return Verbosity.Trace;
         else if (v.ToLower() == "debugging")
            return Verbosity.Debugging;
         else if (v.ToLower() == "errorsonly")
            return Verbosity.ErrorsOnly;
         else if (v.ToLower() == "none")
            return Verbosity.None;
         return Verbosity.ErrorsOnly;
      }

      /// <summary>
      /// Convert a string to a SeverityLevel enumerator.
      /// </summary>
      /// <param name="LevelStr">String representation of severity level.
      /// </param>
      /// <returns>The SeverityLevel enumerator is returned else
      /// "SeverityLevel.Unknown" is returned.</returns>
      static public SeverityLevel ToSeverityLevel(string LevelStr)
      {
         SeverityLevel level;
         if (LevelStr.ToLower().CompareTo("fatal") == 0)
            level = SeverityLevel.Fatal;
         else
         if (LevelStr.ToLower().CompareTo("critical") == 0)
            level = SeverityLevel.Critical;
         else
         if (LevelStr.ToLower().CompareTo("warning") == 0)
            level = SeverityLevel.Warning;
         else
         if (LevelStr.ToLower().CompareTo("info") == 0)
            level = SeverityLevel.Info;
         else
         if (LevelStr.ToLower().CompareTo("debug") == 0)
            level = SeverityLevel.Debug;
         else
            level = SeverityLevel.Unknown;
         return (level);
      }

      /// <summary>
      /// Given a caller location, severity level and a message get an XML
      /// representation of the messag.
      /// </summary>
      /// <param name="message">message to log</param>
      /// <param name="location">caller location (if any)</param>
      /// <param name="level">seveirty leve</param>
      /// <param name="dateTime">log date time</param>
      /// <returns>an XML message is returned...</returns>
      static public String ToPlainText(String message,
         String location, SeverityLevel level, DateTime dateTime)
      {
         if (String.IsNullOrEmpty(message))
            message = String.Empty;

         string format = app.Strings.DefaultDateTimeShortFormat;
         string outMess = dateTime.ToString(format) + ": "
            + "(" + level.ToString() + ") " + message.Trim();

         if (!String.IsNullOrEmpty(location))
            outMess += " Location:" + location;

         return outMess;
      }

      #endregion
      #region -- 4.0 - XML Log Support Methods

      /// <summary>
      /// Given a caller location, severity level and a message get an XML
      /// representation of the messag.
      /// </summary>
      /// <param name="message">message to log</param>
      /// <param name="location">caller location (if any)</param>
      /// <param name="level">seveirty leve</param>
      /// <param name="dateTime">log date time</param>
      /// <returns>an XML message is returned...</returns>
      static public String ToXmlMessage(String message,
         String location, SeverityLevel level, DateTime dateTime)
      {
         if (String.IsNullOrEmpty(message))
            message = String.Empty;

         string outMess =
            "<Entry>"
          + "<Date>" + dateTime.ToString() + "</Date>"
          + "<Message>" + message.Trim() + "</Message>"
          + "<Severity>" + level.ToString() + "</Severity>";

         if (!String.IsNullOrEmpty(location))
            outMess += "<Location>" + location + "</Location>";

         outMess += "</Entry>";

         return outMess;
      }  // end of ToXmlMessage

      /// <summary>
      /// Given a caller location, severity level and a message get an XML
      /// representation of the messag.
      /// </summary>
      /// <param name="message">message to log</param>
      /// <param name="location">caller location (if any)</param>
      /// <param name="level">seveirty leve</param>
      /// <returns>an XML message is returned...</returns>
      static public String ToXmlMessage(String message, String location,
         SeverityLevel level)
      {
         DateTime dt = DateTime.Now;
         return ToXmlMessage(message, location, level, dt);
      }  // end of ToXmlMessage

      #endregion
      #region -- 4.0 - IDisposable Support

      private bool m_DisposedValue = false; // To detect redundant calls

      /// <summary>
      /// Do not call directly, call Dispose() insted.
      /// </summary>
      /// <param name="disposing"></param>
      protected virtual void Dispose(bool disposing)
      {
         if (!m_DisposedValue)
         {
            if (disposing)
            {
               m_Results = null;
               m_LogSettings = null;
            }

            //if (m_EventLog != null)
            //   m_EventLog.Dispose();
            //m_EventLog = null;

            m_DisposedValue = true;
         }
      }

      ~Logger()
      {
         Dispose(false);
      }

      /// <summary>
      /// Dispose and cleanup allocated resources.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         // TODO: uncomment the next line if the finalizer is overridden above.
         // GC.SuppressFinalize(this);
      }

      #endregion

   }  // end of Log class

}  // end of Edam.Diagnostics
