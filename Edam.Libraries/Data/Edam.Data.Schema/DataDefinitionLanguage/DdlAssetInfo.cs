using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.Schema.SchemaObject;
using Edam.DataObjects.Objects;

namespace Edam.Data.Schema.DataDefinitionLanguage
{

   public class DdlAssetInfo : SchemaInfo
   {
      public const string SQL_STRING = "varchar";
      public const string SQL_DATETIME2 = "datetime2";
      public const string SQL_DATETIME = "datetime";
      public const string SQL_INT = "int";
      public const string SQL_INTEGER = "integer";
      public const string SQL_DECIMAL = "decimal";
      public const string SQL_BOOL = "bit";
      public const string SQL_CHAR = "char";
      public const string SQL_SMALLINT = "smallint";
      public const string SQL_BIGINT = "bigint";
      public const string SQL_UNIQUEIDENTIFIER = "uniqueidentifier";

      public AssetData Assets { get; set; }
      public NamespaceList Namespaces { get; set; }

      public DdlAssetInfo(NamespaceInfo ns, AssetType type, string versionId) 
         : base(versionId)
      {
         Assets = new AssetData(ns, type, versionId);
         Namespaces = new NamespaceList();
      }

      public List<NamespaceInfo> Add(List<NamespaceInfo> list)
      {
         if (list == null)
            return null;
         foreach (var n in list)
         {
            Namespaces.Add(n);
         }
         return list;
      }

      public static ObjectValueType GetType(string valueTypeText)
      {
         ObjectValueType v = ObjectValueType.Unknown;
         switch (valueTypeText.ToLower())
         {
            case SQL_STRING:
               v = ObjectValueType.String;
               break;
            case SQL_DECIMAL:
               v = ObjectValueType.Decimal;
               break;
            case SQL_BOOL:
               v = ObjectValueType.Bit;
               break;
            case SQL_DATETIME2:
            case SQL_DATETIME:
               v = ObjectValueType.DateTime;
               break;
            case SQL_SMALLINT:
               v = ObjectValueType.Int16;
               break;
            case SQL_INTEGER:
            case SQL_INT:
               v = ObjectValueType.Int32;
               break;
            case SQL_CHAR:
               v = ObjectValueType.Char;
               break;
            case SQL_BIGINT:
               v = ObjectValueType.Int64;
               break;
            case SQL_UNIQUEIDENTIFIER:
               v = ObjectValueType.GUID;
               break;
            default:
               v = ObjectValueType.String;
               break;
         }
         return v;
      }

   }

}
