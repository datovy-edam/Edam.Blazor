using System;
using System.Data.Common;
// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2015.  Kifv3r0.
// Upra Framework prepared by Eduardo Sobrino, PR.Net

namespace Edam.Data
{

   /// <summary>
   /// Helper class to manage parameters.
   /// </summary>
   public class DataParameters
   {

      private DbParameterCollection m_Params;

      public Int32 Count
      {
         get
         {
            if (m_Params == null)
               return 0;
            return m_Params.Count;
         }
      }

      public DataParameters(DbParameterCollection dbParams)
      {
         m_Params = dbParams;
      }

      public Int16 GetInt16(Int32 index)
      {
         return (Int16)m_Params[index].Value;
      }

      public Int32 GetInt32(Int32 index)
      {
         return (Int32)m_Params[index].Value;
      }

      public String GetString(Int32 index)
      {
         return (String)m_Params[index].Value;
      }

      public DbParameter AddWithValue(
         String paramName, String value, Int32 length = 0,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.String;
         p.Size = length;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Int64 value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Int64;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Int32? value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Int32;
         p.Value = value.HasValue ? (object)value.Value : DBNull.Value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Int32 value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Int32;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Int16 value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Int16;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Int16? value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Int16;
         p.Value = value.HasValue ? (object)value.Value : DBNull.Value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Double value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Double;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Decimal value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Decimal;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, DateTime value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.DateTime;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, DateTime? value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.DateTime;
         p.Value = value.HasValue ? (Object)value.Value : DBNull.Value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Guid value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Guid;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Byte[] value, Int32 length = 0,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Size = length;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Binary;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, TimeSpan value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Time;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Boolean value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Boolean;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithValue(
         String paramName, Boolean? value,
         Boolean outputParam = false)
      {
         DbParameter p =
            DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = outputParam ?
            System.Data.ParameterDirection.Output :
            System.Data.ParameterDirection.Input;
         p.DbType = System.Data.DbType.Boolean;
         p.Value = value.HasValue ? value.Value : false;
         m_Params.Add(p);
         return p;
      }

      public DbParameter AddWithStructuredValue<T>(
         String paramName, T value)
      {
         System.Data.SqlClient.SqlParameter sqlParam =
            new System.Data.SqlClient.SqlParameter();
         sqlParam.SqlDbType = System.Data.SqlDbType.Structured;
         DbParameter p = sqlParam;
         //   DataProviderFactory.CreateParameter();
         p.ParameterName = paramName;
         p.Direction = System.Data.ParameterDirection.Input;
         p.Value = value;
         m_Params.Add(p);
         return p;
      }

   }  // end of DataParameters

}
