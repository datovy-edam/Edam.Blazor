using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Objects;
using Edam.Serialization;

namespace Edam.Text
{

   public class TextValue
   {

      public static int IntSampleValueCount = 0;

      public string Value { get; set; }
      public ObjectValueType ValueType { get; set; }
      public string Description { get; set; }

      public short? AsInt16
      {
         get
         {
            if (short.TryParse(Value, out short value))
            {
               return value;
            }
            return null;
         }
      }

      public int? AsInt32
      {
         get
         {
            if (int.TryParse(Value, out int value))
            {
               return value;
            }
            return null;
         }
      }

      public TextValue()
      {

      }

      public TextValue(
         string value, ObjectValueType valueType, string description)
      {
         Value = value;
         ValueType = valueType;
         Description = description;
      }

      public static string ToTextValue(string value, string description,
         ObjectValueType type = ObjectValueType.String)
      {
         JsonBuilder b = new JsonBuilder();
         b.StartDocument();
         b.AddPropertyValue("Value", value);
         b.AddPropertyValue("Type", type.ToString());
         b.AddPropertyValue("Description", description);
         b.EndDocument();
         return b.ToString();
      }

      public static TextValue FromTextValue(string jsonText)
      {
         return JsonSerializer.Deserialize<TextValue>(jsonText);
      }

      public static string GenerateValue(
         ObjectValueType type = ObjectValueType.String)
      {
         IntSampleValueCount++;
         switch(type)
         {
            case ObjectValueType.SecretText:
               return "**********";
            case ObjectValueType.Text:
            case ObjectValueType.String:
               return "String" + IntSampleValueCount.ToString();
            case ObjectValueType.Char:
               return "C";
            case ObjectValueType.Bit:
               return "1";
            case ObjectValueType.SByte:
               return "1010";
            case ObjectValueType.Byte:
               return "10101010";
            case ObjectValueType.GUID:
               return Guid.NewGuid().ToString();
            case ObjectValueType.SmallInt:
            case ObjectValueType.Integer:
            case ObjectValueType.BigInt:
            case ObjectValueType.Int16:
            case ObjectValueType.Int32:
            case ObjectValueType.Int64:
            case ObjectValueType.UInt16:
            case ObjectValueType.UInt32:
            case ObjectValueType.UInt64:
               return IntSampleValueCount.ToString();
            case ObjectValueType.Decimal:
            case ObjectValueType.Double:
            case ObjectValueType.Money:
               return IntSampleValueCount.ToString() + "." 
                  + IntSampleValueCount.ToString();
            case ObjectValueType.Time:
               return DateTime.Now.ToString("HH:mm:ss");
            case ObjectValueType.Date:
               return DateTime.Now.ToString("yyyy-MM-dd");
            case ObjectValueType.DateTime:
               return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            default:
               return String.Empty;
         }
      }

   }

}