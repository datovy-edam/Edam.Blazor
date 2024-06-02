using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.Data.Schema.SchemaObject;
using Edam.Diagnostics;

// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-schema-collections

namespace Edam.Data.Schema
{

   /// <summary>
   /// Read database schema.
   /// </summary>
   public class SchemaReader : IDisposable
   {
      public const string TABLE_CATALOG = "TABLE_CATALOG";
      public const string TABLE_SCHEMA = "TABLE_SCHEMA";
      public const string TABLE_NAME = "TABLE_NAME";
      public const string COLUMN_NAME = "COLUMN_NAME";
      public const string DATA_TYPE = "DATA_TYPE";
      public const string ORDINAL_POSITION = "ORDINAL_POSITION";
      public const string COLUMN_DEFAULT = "COLUMN_DEFAULT";
      public const string IS_NULLABLE = "IS_NULLABLE";
      public const string CHARACTER_MAXIMUM_LENGTH =
         "CHARACTER_MAXIMUM_LENGTH";

      public const string XCONSTRAINT_CATALOG = "constraint_catalog";
      public const string XCONSTRAINT_SCHEMA = "constraint_schema";
      public const string XCONSTRAINT_NAME = "constraint_name";
      public const string XTABLE_CATALOG = "table_catalog";
      public const string XTABLE_SCHEMA = "table_schema";
      public const string XTABLE_NAME = "table_name";
      public const string XCOLUMN_NAME = "column_name";
      public const string XCONSTRAINT_TYPE = "constraint_type";
      public const string XORDINAL_PIOSITION = "ordinal_position";
      public const string XKEY_TYPE = "KeyType";
      public const string XINDEX_NAME = "index_name";
      public const string XIS_DEFERRABLE = "is_deferrable";
      public const string XINITIALLY_DEFERRED = "initially_deferred";

      private DataProvider m_Provider;
      private DataTable m_Tables;
      private DataTable m_Indexes;
      public string VersionId { get; set; }

      public SchemaReader(String connectionString)
      {
         m_Provider = new DataProvider(connectionString);
      }

      public List<SchemaObject.SchemaResource> GetTables()
      {
         m_Tables = 
            m_Provider.DataConnection.Connection.GetSchema("Columns");
         return GetTables(m_Tables);
      }

      public List<SchemaObject.SchemaResource> GetIndexes(
         List<SchemaObject.SchemaResource> resources)
      {
         m_Indexes =
            m_Provider.DataConnection.Connection.GetSchema("IndexColumns");
         return GetIndexes(m_Indexes, resources);
      }

      public List<SchemaResourceConstraint> GetForeignKeys()
      {
         ResultsLog<List<SchemaResourceConstraint>> results =
            SchemaDataObject.SchemaConstraintsGet(
               m_Provider.DataConnection.Connection.ConnectionString);
         return results.Data;
      }

      /// <summary>
      /// Get Catalog Information
      /// </summary>
      /// <param name="connectionString">Connection string.</param>
      /// <param name="list"></param>
      /// <returns></returns>
      public static Diagnostics.ResultsLog<SchemaObject.SchemaSet> GetCatalogs(
         string connectionString, string versionId,
         List<SchemaObject.CatalogInfo> list = null)
      {
         //_ = list ?? new List<SchemaObject.CatalogInfo>();
         Diagnostics.ResultsLog<SchemaObject.SchemaSet> results = 
            new Diagnostics.ResultsLog<SchemaObject.SchemaSet>();
         SchemaReader r = null;
         try
         {
            r = new SchemaReader(connectionString);
            var t = r.GetTables();
            var i = r.GetIndexes(t);
            var k = r.GetForeignKeys();
            var h = new SchemaObject.SchemaSet();
            h.Add(t, k, versionId);
            results.Data = h;
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         finally
         {
            if (r != null)
            {
               r.Dispose();
            }
         }
         return results;
      }

      /// <summary>
      /// Get Table schema metadata information for each column.
      /// </summary>
      /// <param name="resourceName">(table) resource name</param>
      /// <returns>list of resource metadata</returns>
      private List<ResourceMetadataInfo> GetTableSchemaMetadata(
         string resourceName)
      {
         DbCommand cmd = m_Provider.CreateCommand(
            "SELECT * FROM " + resourceName, false);
         DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
         DataTable dataTable = reader.GetSchemaTable();
         reader.Close();
         cmd.Dispose();

         var selectedRows =
            from info in dataTable.AsEnumerable()
            select new
            {
               AllowDBNull = info["AllowDBNull"],
               ColumnOrdinal = info["ColumnOrdinal"],
               ColumnName = info["ColumnName"],
               ColumnSize = info["ColumnSize"],
               DataTypeName = info["DataTypeName"],
               IsAutoIncrement = info["IsAutoIncrement"],
               IsIdentity = info["IsIdentity"],
               IsKey = info["IsKey"],
               IsBlob = info["IsLong"],
               IsUniqueTimestamp = info["IsUnique"]
            };

         List<ResourceMetadataInfo> columns = new List<ResourceMetadataInfo>();
         foreach(var i in selectedRows)
         {
            ResourceMetadataInfo c = new ResourceMetadataInfo
            {
               AllowDBNull = (bool)i.AllowDBNull,
               ColumnOrdinal = (int)i.ColumnOrdinal,
               ColumnName = i.ColumnName.ToString(),
               ColumnSize = (int)i.ColumnSize,
               DataTypeName = i.DataTypeName.ToString(),
               IsAutoIncrement = (bool)i.IsAutoIncrement,
               IsIdentity = (bool)i.IsIdentity,
               IsKey = (bool)(i.IsKey == System.DBNull.Value ?
                  false : (bool)i.IsKey),
               IsBlob = (bool)i.IsBlob,
               IsUniqueTimestamp = (bool)i.IsUniqueTimestamp
            };
            columns.Add(c);
         }
         dataTable.Dispose();
         return columns;
      }

      /// <summary>
      /// Get Table / Column details and metadata.
      /// </summary>
      /// <param name="collection">DataTable with collection of tables</param>
      /// <returns>list of (Table/Column) resources</returns>
      private List<SchemaObject.SchemaResource>
         GetTables(DataTable collection)
      {
         List<SchemaObject.SchemaResource> l =
            new List<SchemaObject.SchemaResource>();
         SchemaObject.SchemaResource t;
         var selectedRows =
            from info in collection.AsEnumerable()
            select new
            {
               TableCatalog = info[TABLE_CATALOG],
               TableSchema = info[TABLE_SCHEMA],
               TableName = info[TABLE_NAME],
               ColumnName = info[COLUMN_NAME],
               DataType = info[DATA_TYPE],
               DataSize = info[CHARACTER_MAXIMUM_LENGTH],
               OrdinalPosition = info[ORDINAL_POSITION],
               ColumnDefault = info[COLUMN_DEFAULT],
               IsNullable = info[IS_NULLABLE]
            };

         string fname = null;
         string cname = null;
         List<ResourceMetadataInfo> metadata = null;
         foreach (var row in selectedRows)
         {
            cname = row.TableSchema + "." + row.TableName;
            if (fname != cname)
            {
               metadata = GetTableSchemaMetadata(cname);
               fname = cname;
            }

            t = new SchemaObject.SchemaResource
            {
               CatalogName = row.TableCatalog.ToString(),
               SchemaName = row.TableSchema.ToString(),
               ResourceName = row.TableName.ToString(),
               ElementName = row.ColumnName.ToString(),
               DataType = row.DataType.ToString(),
               DataSize = row.DataSize.ToString(),
               OrdinalPosition = row.OrdinalPosition.ToString(),
               ColumnDefault = row.ColumnDefault.ToString(),
               IsNullable = row.IsNullable.ToString()
            };

            var f = metadata.Find((x) => x.ColumnName == t.ElementName);
            if (f != null)
            {
               t.Metadata = f;
            }

            l.Add(t);
         }

         return l;
      }

      /// <summary>
      /// Get Indexes information.
      /// </summary>
      /// <param name="collection">DataTable with table details</param>
      /// <param name="resources">SchemaResources (Tables and Columns)</param>
      /// <returns>resources update with index info (if any found)</returns>
      private List<SchemaObject.SchemaResource>
         GetIndexes(DataTable collection,
         List<SchemaObject.SchemaResource> resources)
      {
         SchemaObject.SchemaResourceIndex t;
         var selectedRows =
            from info in collection.AsEnumerable()
            select new
            {
               ConstraintCatalog = info[XCONSTRAINT_CATALOG],
               ConstraintSchema = info[XCONSTRAINT_SCHEMA],
               ConstraintName = info[XCONSTRAINT_NAME],
               TableCatalog = info[XTABLE_CATALOG],
               TableSchema = info[XTABLE_SCHEMA],
               TableName = info[XTABLE_NAME],
               ColumnName = info[XCOLUMN_NAME],
               OrdinalPosition = info[XORDINAL_PIOSITION],
               KeyType = info[XKEY_TYPE],
               IndexName = info[XINDEX_NAME]
            };

         foreach (var row in selectedRows)
         {
            t = new SchemaObject.SchemaResourceIndex
            {
               CatalogName = row.TableCatalog.ToString(),
               SchemaName = row.TableSchema.ToString(),
               ResourceName = row.TableName.ToString(),
               ElementName = row.ColumnName.ToString(),
               OrdinalPosition = row.OrdinalPosition.ToString(),
               KeyType = row.KeyType.ToString(),
               IndexName = row.IndexName.ToString()
            };
            var i = resources.Find((x) =>
            {
               return x.CatalogName == t.CatalogName &&
                      x.SchemaName == t.SchemaName &&
                      x.ResourceName == t.ResourceName &&
                      x.ElementName == t.ElementName;
            });
            if (i == null)
            {
               continue;
            }

            i.Indexes.Add(t);
         }
         return resources;
      }

      /// <summary>
      /// Dispose of resources.
      /// </summary>
      public void Dispose()
      {
         if (m_Tables != null)
         {
            m_Tables.Dispose();
            m_Tables = null;
         }
         if (m_Indexes != null)
         {
            m_Indexes.Dispose();
            m_Indexes = null;
         }
         if (m_Provider != null)
         {
            m_Provider.Dispose();
            m_Provider = null;
         }
      }

   }

}
