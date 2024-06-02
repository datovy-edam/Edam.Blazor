using Edam.Data.Asset;
using Edam.DataObjects.Objects;
using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
   public partial class ObjectValueTypes
   {
      public ObjectValueTypes()
      {
         DataElement = new HashSet<DataElement>();
      }

      public short IdNo { get; set; }
      public string Description { get; set; }
      //public string UpdateSessionId { get; set; }
      //public DateTime CreatedDate { get; set; }
      //public DateTime LastUpdateDate { get; set; }
      //public string RecordStatusCode { get; set; }

      public virtual ICollection<DataElement> DataElement { get; set; }

      public static ObjectValueType GetValueType(string dataType)
      {
         string t = dataType;
         // remove prefix if any
         int indx = dataType.IndexOf(':');
         if (indx != -1)
         {
            t = t.Substring(indx + 1, t.Length - indx - 1);
         }
         switch(t.ToLower())
         {
            case ElementBaseTypeInfo.DECIMAL:
               return ObjectValueType.Decimal;
            case ElementBaseTypeInfo.STRING:
               return ObjectValueType.String;
            case ElementBaseTypeInfo.DATE:
               return ObjectValueType.Date;
            case ElementBaseTypeInfo.DATETIME:
               return ObjectValueType.DateTime;
            case ElementBaseTypeInfo.INT:
            case ElementBaseTypeInfo.INTEGER:
               return ObjectValueType.Integer;
            case ElementBaseTypeInfo.LONG:
               return ObjectValueType.Int64;
            case ElementBaseTypeInfo.DOUBLE:
               return ObjectValueType.Double;
            case ElementBaseTypeInfo.TIME:
               return ObjectValueType.Time;
            case ElementBaseTypeInfo.TIMESTAMP:
               return ObjectValueType.Time;
            case ElementBaseTypeInfo.BOOLEAN:
               return ObjectValueType.Bit;
            case ElementBaseTypeInfo.CHAR:
               return ObjectValueType.Char;
            case ElementBaseTypeInfo.SHORT:
               return ObjectValueType.SmallInt;
            default:
               return ObjectValueType.String;
         }
      }

   }

}
