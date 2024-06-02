using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using reader = Edam.Text.StringReader;

namespace Edam.Data.Schema.ImportExport
{

   public class ImportItemInfo
   {
      public const string NULL = "null";

      public string Dbms { get; set; }
      public string TableCatalog { get; set; }
      public string TableSchema { get; set; }
      public string ObjectName { get; set; }
      public string ColumnName { get; set; }
      public long? OrdinalPosition { get; set; }
      public string DataType { get; set; }
      public decimal? CharacterMaximumLength { get; set; }
      public string ConstraintType { get; set; }
      public string ConstraintTableSchema { get; set; }
      public string ConstraintTableName { get; set; }
      public string ConstraintColumnName { get; set; }

      public int? Precision { get; set; } = 0;
      public int? Scale { get; set; } = 0;
      public bool IsOutput { get; set; } = false;
      public bool IsReadOnly { get; set; } = false;
      public bool IsNullable { get; set; } = false;
      public bool IsIdentity { get; set; } = false;

      public string Tags { get; set; }
      public string ColumnDescription { get; set; }

      public string MetadataBag { get; set; }

      public string TableName
      {
         get { return ObjectName; }
         set { ObjectName = value; }
      }

      public ElementType ObjectType { get; set; } = ElementType.type;

      public NamespaceInfo SchemaNamespace { get; set; }
      public int OrdinalNo { get; set; }

      public bool IsPrimaryKey
      {
         get
         { 
            return ConstraintType != NULL && ConstraintType != null &&
               ConstraintType.ToUpper() == 
               AssetSchema.AssetElementConstraintInfo.PRIMARY_KEY; 
         }
      }

      public bool HasForeignKey
      {
         get
         {
            return ConstraintType != NULL && ConstraintType != null &&
               ConstraintType.ToUpper() == 
               AssetSchema.AssetElementConstraintInfo.FOREIGN_KEY;
         }
      }

   }

}
