using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SQLite;
using SQLitePCL;

// -----------------------------------------------------------------------------
// https://msdn.microsoft.com/en-us/magazine/mt736454.aspx
using Edam.Diagnostics;
using Edam.InOut;

namespace Edam.Data
{

   public class LDbConnection : IDisposable
   {
      public const long HRESULT_TABLE_NOT_EXISTS = -2146233088;

      private static readonly string DEFAULT_DB_NAME = "Edam.db3";
      private static LDbConnection m_Instance;
      public static IResultsLog ResultsLog = new ResultLog();

      private SQLiteAsyncConnection m_Connection = null;

      public static SQLiteAsyncConnection Connection
      {
         get
         {
            if (m_Instance == null)
               InitializeDb();
            return m_Instance.m_Connection;
         }
      }

      public static void InitializeDb()
      {
         if (m_Instance == null)
            m_Instance = new LDbConnection();
         m_Instance.SetConnection();
      }

      public LDbConnection()
      {
         if (m_Instance == null)
            m_Instance = this;
         m_Instance.SetConnection();
      }

      public bool SetConnection(string filePath = null)
      {
         if (m_Connection == null)
         {
            String fpath = (filePath == null) ? DEFAULT_DB_NAME : filePath;
            var path = FileHelper.GetWorkingDirectoryFullPath(fpath);
            if (m_Instance == null)
               m_Instance = this;
            try
            {
               m_Instance.m_Connection = new SQLiteAsyncConnection(path);
            }
            catch(Exception e)
            {
               ResultsLog.Failed(e);
            }
         }
         ResultsLog.Succeeded();
         return ResultsLog.Success;
      }

      public Task<CreateTableResult> CreateTableAsync<T>() 
         where T : ILDbObject, new()
      {
         return m_Instance.m_Connection.CreateTableAsync<T>();
      }

      public Task<List<T>> GetItemsAsync<T>() where T : new()
      {
         return m_Instance.m_Connection.Table<T>().ToListAsync();
      }

      public Task<int> DeleteItemAsync<T>(T table) where T : new()
      {
         return m_Instance.m_Connection.DeleteAsync(table);
      }

      public Task<int> InsertAsync<T>(T table) where T : new()
      {
         return m_Instance.m_Connection.InsertAsync(table);
      }

      public Task<int> UpdateAsync<T>(T table) where T : new()
      {
         return m_Instance.m_Connection.UpdateAsync(table);
      }

      public Task<List<T>> GetAsync<T>(
         string tableName, string columnName, string param) 
         where T : ILDbIntIdObject, ILDbObject, new()
      {
         return m_Instance.m_Connection.QueryAsync<T>(
            "SELECT * FROM " + tableName + " WHERE " + columnName + "=?", 
            new string[1] { param });
      }

      public Task<List<T>> GetAsync<T>(
         string tableName, string queryTail, string[] parameters)
         where T : ILDbIntIdObject, ILDbObject, new()
      {
         return m_Instance.m_Connection.QueryAsync<T>(
            "SELECT * FROM " + tableName + " " + queryTail,
            parameters);
      }

      public Task<T> GetIntIdObjectAsync<T>(int id) 
         where T : ILDbIntIdObject, ILDbObject, new()
      {
         return m_Instance.m_Connection.Table<T>().
            Where(i => i.IdNo == id).FirstOrDefaultAsync();
      }

      public Task<int> DeleteAsync<T>(int idNo)
         where T : ILDbIntIdObject, ILDbObject, new()
      {
         return m_Instance.m_Connection.DeleteAsync<T>(idNo);
      }

      public Task<int> SaveIntIdObjectAsync<T>(T table)
         where T : ILDbIntIdObject, ILDbObject, new()
      {
         if (table.IdNo != 0)
         {
            return m_Instance.m_Connection.UpdateAsync(table);
         }
         else
         {
            return m_Instance.m_Connection.InsertAsync(table);
         }
      }

      public static bool TableExists<T>(SQLiteConnectionWithLock connection)
      {
         const string cmdText = 
            "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
         SQLiteCommand cmd = connection.CreateCommand(cmdText, typeof(T).Name);
         return cmd.ExecuteScalar<string>() != null;
      }

      public bool TableExists<T>()
      {
         return TableExists<T>(m_Instance.m_Connection.GetConnection());
      }

      public void Disconnect()
      {
      }

      public void Dispose()
      {
      }

   }

}
