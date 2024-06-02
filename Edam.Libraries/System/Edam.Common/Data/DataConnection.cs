using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.
// "Data Source=openk10;Initial Catalog=Cjisv1r0;Integrated Security=false;User Id=KifDbAdmin;Password=KifDb@dmin";// connectionString;

using Edam.Diagnostics;

namespace Edam.Data
{

   /// <summary>
   /// Helper to manage database connectivity and transaction/s.
   /// </summary>
   public class DataConnection : IDisposable
   {

      public IResultsLog ResultsLog { get; set; }
      public DataSourceInfo DataSource { get; set; }
      public System.Data.Common.DbTransaction Transaction { get; set; }
      public System.Data.Common.DbConnection Connection { get; set; }

      public DataConnection()
      {
         ResultsLog = null;
         DataSource = null;
         Transaction = null;
         Connection = null;
      }

      /// <summary>
      /// Get Connection string.
      /// </summary>
      /// <param name="connectionString">(optional provided) connection string
      /// </param>
      /// <returns>the connection string is returned</returns>
      public static String GetConnectionString(String connectionString = null)
      {
         if (!String.IsNullOrEmpty(connectionString))
            return connectionString;

         String connString = null;
         DataSourceInfo source =
            Application.Session.GetApplicationDefaultDataSource();
         if (source != null)
            connString = source.ConnectionString;
         return connString;
      }

      /// <summary>
      /// Create Results Log.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="log">(optional) log to use</param>
      /// <returns>returns instance of log</returns>
      public IResultsLog CreateResultsLog<T>(
         IResultsLog log = null)
      {
         if (ResultsLog != null)
            return ResultsLog;
         return log == null ? new ResultsLog<T>() : log;
      }

      /// <summary>
      /// Create Results Log.
      /// </summary>
      /// <param name="log">(optional) log to use</param>
      /// <returns>returns instance of log</returns>
      public IResultsLog CreateResultsLog(
         IResultsLog log = null)
      {
         if (ResultsLog != null)
            return ResultsLog;
         return log == null ? new ResultLog() : log;
      }

      /// <summary>
      /// Create a connection.
      /// </summary>
      /// <param name="connectionString">connection string</param>
      /// <returns>if successful an instance of DbConnection is returned
      /// </returns>
      public DataConnection CreateConnection(String connectionString = null)
      {
         if (String.IsNullOrEmpty(connectionString))
            connectionString = Application.Session.
               GetApplicationDefaultConnectionString();

         if (Connection == null)
            Connection = DataProviderFactory.CreateConnection();

         if (Connection.State == System.Data.ConnectionState.Open)
            return this;

         Connection.ConnectionString = GetConnectionString(connectionString);
         Connection.Open();
         return this;
      }  // end of CreateConnection

      /// <summary>
      /// Create a DataConnection object based on current datasource settings
      /// and make the connection to the database.
      /// </summary>
      /// <param name="connectionString">(optional) connection string, if given
      /// it will be used </param>
      /// <returns>instance of DataConnection is returned</returns>
      public static DataConnection CreateDataConnection(
         String connectionString = null)
      {
         DataConnection c = null;
         DataSourceInfo source = null;
         if (String.IsNullOrEmpty(connectionString))
            source = Application.Session.GetApplicationDefaultDataSource();
         c = (source == null) ? new DataConnection() : source.DefaultConnection;
         if (c == null)
            c = new DataConnection();
         c.CreateConnection(connectionString);
         return c;
      }

      /// <summary>
      /// Create Data Connection by using given Data Source key.
      /// </summary>
      /// <param name="key">data source key</param>
      /// <returns>instance of a DataConnection is returned</returns>
      public static DataConnection CreateDataConnectionByKey(String key)
      {
         DataConnection c = new DataConnection();
         DataSourceInfo source = Application.Session.GetDataSource(key);
         c.CreateConnection(source.GetConnectionString());
         return c;
      }

      /// <summary>
      /// Begin Transaction
      /// </summary>
      /// <returns>transaction object is returned if transaction begined
      /// </returns>
      public System.Data.Common.DbTransaction BeginTransaction()
      {
         Transaction = Connection.BeginTransaction();
         return Transaction;
      }

      /// <summary>
      /// Release allocated resources.
      /// </summary>
      public void Dispose()
      {
         CloseAll();
      }

      /// <summary>
      /// Close all resources including transaction(s) and connection.
      /// </summary>
      public void CloseAll()
      {
         // don't close Default Connections since they will be reused later...
         if (DataSource != null)
         {
            if (DataSource.DefaultConnection != null)
               return;
         }
         Disconnect();
      }

      public void Disconnect()
      {
         if (Transaction != null)
            Transaction.Dispose();
         Transaction = null;
         if (Connection != null)
            Connection.Dispose();
         Connection = null;
      }

   }

}
