using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.Diagnostics
{

   public interface ILoggerWriter
   {
      void Write(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info);
      void Write(Exception exception);
   }

   public interface IEventLogger: ILoggerWriter
   {
      new void Write(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info);
      new void Write(Exception exception);
   }

   public interface IFileLogger: ILoggerWriter
   {
      new void Write(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info);
      new void Write(Exception exception);
   }

   public interface ILogger: ILoggerWriter
   {

      Verbosity Verbosity { get; set; }
      IResultsLog Results { get; set; }

      IEventLogger EventLog { get; }
      IFileLogger FileLog { get; }

      void ToResults(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info);
      void ToResults(Exception exception);

      void WriteToEventLog(String message, String location = null,
         SeverityLevel level = SeverityLevel.Info);
      void WriteToEventLog(Exception exception);

      void WriteToFile(String message, String location,
         SeverityLevel level = SeverityLevel.Info);
      void WriteToFile(Exception exception);

      void ConsoleWrite(String message, String location = null);
      void ConsoleWrite(Exception exception, String location = null);

   }

}
