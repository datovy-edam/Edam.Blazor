using System;
using System.Data.Common;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.
// Upra Framework prepared by Eduardo Sobrino, PR.Net

namespace Edam.Data
{

   public struct ReturnCode
   {
      public static readonly Int32 NoCommandAvailable = -999999;
      public static readonly Int32 NoParametersAvailable = -999998;
      public static readonly Int32 NoReturnedValueAvailable = -999997;
      public static readonly Int32 ExecProcedureFailed = -999996;
      public static readonly Int32 IndexOutOfRange = -999995;
   }

   /// <summary>
   /// Manage Provider connection, command and data reader...
   /// </summary>
   public class DataProvider : IDisposable
   {

      private DataConnection m_DataConnection = null;

      private System.Data.Common.DbCommand m_Command;
      private System.Data.Common.DbDataReader m_Reader;
      private DataParameters m_Params;

      public DataParameters Params
      {
         set { m_Params = value; }
         get { return m_Params; }
      }
      public System.Data.Common.DbDataReader Reader
      {
         get { return m_Reader; }
      }

      public Boolean HasReader
      {
         get { return m_Reader != null; }
      }

      public Int32 ReturnValue
      {
         get { return m_Params == null ? -1 : m_Params.GetInt32(0); }
      }

      public DataConnection DataConnection
      {
         get { return m_DataConnection; }
      }

      public DataProvider()
      {

      }

      public DataProvider(DataConnection connection)
      {
         m_DataConnection = connection;
      }

      public DataProvider(string connectionString)
      {
         m_DataConnection =
            DataConnection.CreateDataConnection(connectionString);
      }

      /// <summary>
      /// Create a connection.
      /// </summary>
      /// <param name="connectionString">connection string</param>
      /// <returns>if successful an instance of DbConnection is returned
      /// </returns>
      public System.Data.Common.DbConnection CreateConnection(
         String connectionString = null)
      {
         m_DataConnection =
            DataConnection.CreateDataConnection(connectionString);
         return m_DataConnection.Connection;
      }  // end of CreateConnection

      /// <summary>
      /// Create a connection.
      /// </summary>
      /// <param name="connectionString">connection string</param>
      /// <returns>if successful an instance of DbConnection is returned
      /// </returns>
      public System.Data.Common.DbConnection CreateConnectionByKey(String key)
      {
         m_DataConnection = DataConnection.CreateDataConnectionByKey(key);
         return m_DataConnection.Connection;
      }  // end of CreateConnection

      /// <summary>
      /// Create Command using given command text.
      /// </summary>
      /// <param name="commandText">command Text</param>
      /// <param name="isProcedure">(optional) true if command is a procedure
      /// </param>
      /// <returns>if successful an instance of DbCommand is returned</returns>
      public DbCommand CreateCommand(
         String commandText, Boolean isProcedure = true)
      {
         DbCommand cmd = DataProviderFactory.CreateCommand();
         cmd.Connection = m_DataConnection.Connection;
         cmd.CommandText = commandText;
         cmd.CommandType = isProcedure ?
            System.Data.CommandType.StoredProcedure : 
            System.Data.CommandType.Text;

         if (isProcedure)
         {
            System.Data.Common.DbParameter p =
               DataProviderFactory.CreateParameter();

            p.ParameterName = "@RETURN_VALUE";
            p.DbType = System.Data.DbType.Int32;
            p.Direction = System.Data.ParameterDirection.ReturnValue;

            cmd.Parameters.Add(p);
         }
         return cmd;
      }  // end of CreateCommand

      /// <summary>
      /// Create Command using given command text.
      /// </summary>
      /// <param name="commandText">command Text</param>
      /// <param name="isProcedure">(optional) true if command is a procedure
      /// </param>
      /// <returns>if successful an instance of DbCommand is returned</returns>
      public System.Data.Common.DbCommand CreateCommand(
         String commandText, String dataSourceKey, Boolean isProcedure = true,
         String connectionString = null)
      {
         if (m_DataConnection == null)
         {
            if (!String.IsNullOrEmpty(connectionString))
            {
               CreateConnection(connectionString);
            }
            else if (String.IsNullOrEmpty(dataSourceKey))
               CreateConnection();
            else
               CreateConnectionByKey(dataSourceKey);
         }

         m_Command = CreateCommand(commandText, isProcedure);
         m_Params = new DataParameters(m_Command.Parameters);
         return m_Command;
      }  // end of CreateCommand

      public static DataProvider CreateProcedure(
         String procName, String dataSourceKey = null, 
         string connectionString = null)
      {
         DataProvider p = new DataProvider();
         p.CreateCommand(procName, dataSourceKey, true, connectionString);
         return p;
      }  // end of CreateProcedure

      /// <summary>
      /// Clear All Pools...
      /// </summary>
      public static void ClearAllPools()
      {
         System.Data.SqlClient.SqlConnection.ClearAllPools();
      }

      /// <summary>
      /// Execute None-Query. Always returns -1 (true) for INSERT, UPDATE,
      /// DELETE even if a ROLLBACK was performed...
      /// </summary>
      /// <returns>true is returned if all is ok</returns>
      public Boolean Exec()
      {
         return m_Command.ExecuteNonQuery() == -1;
      }

      /// <summary>
      /// Get Returned Value...
      /// </summary>
      /// <returns></returns>
      public Int32 GetReturnedValue()
      {
         if (m_Command == null)
            return ReturnCode.NoCommandAvailable;
         if (m_Command.Parameters.Count <= 0)
            return ReturnCode.NoParametersAvailable;
         if (m_Command.Parameters[0].Direction !=
            System.Data.ParameterDirection.ReturnValue)
            return ReturnCode.NoReturnedValueAvailable;
         return m_Command.Parameters[0].Value == null ?
            0 : (Int32)m_Command.Parameters[0].Value;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="index"></param>
      /// <returns></returns>
      public Object GetOutputValue(Int32 index)
      {
         if (m_Command == null)
            return ReturnCode.NoCommandAvailable;
         if (m_Command.Parameters.Count <= 0)
            return ReturnCode.NoParametersAvailable;
         if ((index >= m_Command.Parameters.Count) ||
             (index < 0))
            return ReturnCode.IndexOutOfRange;
         Object v = m_Command.Parameters[index].Value;
         if (v == System.DBNull.Value)
            v = null;
         return v;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="index"></param>
      /// <returns></returns>
      public Int64 GetOutputValue(Int32 index, Int64 defaultValue = 0)
      {
         if (m_Command == null)
            return ReturnCode.NoCommandAvailable;
         if (m_Command.Parameters.Count <= 0)
            return ReturnCode.NoParametersAvailable;
         if ((index >= m_Command.Parameters.Count) ||
             (index < 0))
            return ReturnCode.IndexOutOfRange;

         if (m_Command.Parameters[index].Value.GetType() == typeof(Double))
         {
            Double val = (Double)m_Command.Parameters[index].Value;
            return (Int64)Math.Truncate(val);
         }
         if (m_Command.Parameters[index].Value.GetType() == typeof(Decimal))
         {
            Decimal val = (Decimal)m_Command.Parameters[index].Value;
            return (Int64)Math.Truncate(val);
         }
         return (Int64)defaultValue;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="index"></param>
      /// <returns></returns>
      public Int16 GetOutputValue(Int32 index, Int16 defaultValue = 0)
      {
         if (m_Command == null)
            return defaultValue;
         if (m_Command.Parameters.Count <= 0)
            return defaultValue;
         if ((index >= m_Command.Parameters.Count) ||
             (index < 0))
            return defaultValue;

         if (m_Command.Parameters[index].Value.GetType() == typeof(Int16))
         {
            Int16 val = (Int16)m_Command.Parameters[index].Value;
            return val;
         }
         return (Int16)defaultValue;
      }

      /// <summary>
      /// Fetch the reader of current command.
      /// </summary>
      /// <returns>true is returned if successful</returns>
      public Boolean OpenReader()
      {
         if (m_Command == null)
            return false;
         if (m_Reader != null)
            return true;
         m_Reader = m_Command.ExecuteReader();
         return true;
      }  // end of OpenReader

      /// <summary>Open query / command for data reading.</summary>
      /// <date>Nov/2k3 ESob</date>
      /// <returns>Return a Data Reader if successful else null.</returns>
      public System.Data.Common.DbDataReader OpenReaderForSequentialAccess()
      {
         // make sure we free previous allocated resources before getting more
         if (m_Reader != null)
         {
            m_Reader.Close();
            m_Reader.Dispose();
         }

         m_Reader = m_Command.ExecuteReader(
            System.Data.CommandBehavior.SequentialAccess);

         return (m_Reader);
      }  // end of prepare command with given string

      /// <summary>
      /// Dispose / release all allocated resources.
      /// </summary>
      public void Close()
      {
         if (m_Reader != null)
            m_Reader.Dispose();
         m_Reader = null;
         if (m_Command != null)
            m_Command.Dispose();
         m_Command = null;
      }  // end of Close

      /// <summary>
      /// Close all resources including transaction(s) and connection.
      /// </summary>
      public void CloseAll()
      {
         Close();
         m_DataConnection.Dispose();
      }

      /// <summary>
      /// Dispose all allocated resources (this is the same as calling CloseAll).
      /// </summary>
      public void Dispose()
      {
         CloseAll();
      }

      /// <summary>
      /// Using current DataReader fill in a DataTable...
      /// </summary>
      /// <param name="tableName">table name (optional)</param>
      /// <returns>an instance of a DataTale is returned</returns>
      public System.Data.DataTable ToDataTable(System.Data.DataTable table)
      {
         if (m_Reader == null)
            OpenReader();
         if (table == null)
            table = new System.Data.DataTable("Items");

         Int32 i;
         if (table.Columns.Count == 0)
         {
            for (i = 0; i < m_Reader.FieldCount; i++)
               table.Columns.Add(m_Reader.GetName(i), m_Reader.GetFieldType(i));
         }
         System.Data.DataRow r;
         while (m_Reader.Read())
         {
            r = table.NewRow();
            for (i = 0; i < m_Reader.FieldCount; i++)
               r[i] = m_Reader[i];
            table.Rows.Add(r);
         }
         return table;
      }  // end of ToDataTable

      /// <summary>
      /// Using current DataReader fill in a DataTable...
      /// </summary>
      /// <param name="table">table to be used so don't create a new one</param>
      /// <param name="tableName">(optional) table name</param>
      /// <returns>an instance of a DataTale is returned</returns>
      public System.Data.DataTable ToDataTable(
         System.Data.DataTable table, String tableName = null)
      {
         if (String.IsNullOrEmpty(tableName))
            tableName = "Items";

         if (table == null)
            table = new System.Data.DataTable(tableName);
         return ToDataTable(table);
      }  // end of ToDataTable

      /// <summary>
      /// Using current DataReader fill in a DataTable and return a datase...
      /// </summary>
      /// <param name="tableName">table name (optional)</param>
      /// <returns>an instance of a DataSet is returned</returns>
      public System.Data.DataSet ToDataSet(System.Data.DataTable table)
      {
         System.Data.DataSet ds = new System.Data.DataSet();
         ds.Tables.Add(ToDataTable(table));
         return ds;
      }  // end of ToDataSet

   }  // end of DataProvider

}  // end of Kif.Data


