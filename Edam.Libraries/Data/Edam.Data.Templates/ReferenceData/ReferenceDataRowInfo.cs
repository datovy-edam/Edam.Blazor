using System;
using System.Text;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Models;
//using System.Data.Common;

namespace Edam.DataObjects.ReferenceData
{

   /// <summary>
   /// 
   /// </summary>
   public class ReferenceDataRowInfo : List<ReferenceDataValueInfo>
   {
      public static readonly String ROW_NUMBER_FIELD = "row_number";
      public static readonly String ROW_NUMBER_TITLE = "Row Number";

      /// <summary>
      /// Covert the row to a JSON string.
      /// </summary>
      /// <returns>a JSON string is returned</returns>
      public String ToJson(Int32 rowCount)
      {
         StringBuilder sb = new StringBuilder();
         Int32 cnt = 0;

         sb.AppendLine("{");
         sb.AppendLine(ReferenceDataValueInfo.GetColumnJson(
            ROW_NUMBER_FIELD, ROW_NUMBER_TITLE, rowCount.ToString()));
         cnt++;

         foreach (var i in this)
         {
            if (i.ElementName != null)
            {
               if (cnt > 0)
                  sb.Append(",");
               sb.AppendLine(i.ToJson());
               cnt++;
            }
         }
         sb.AppendLine("}");
         return sb.ToString();
      }

      public static List<ReferenceDataValueInfo> FromJson(string jsonText)
      {
         List<ReferenceDataValueInfo> r = new List<ReferenceDataValueInfo>();

         return r;
      }

#if DATA_SUPPORT_
      /// <summary>
      /// Read Data from given reader and stuff it into a Reference Data Row 
      /// object instance.  If an element node is given then
      /// </summary>
      /// <param name="node"></param>
      /// <param name="reader"></param>
      /// <returns></returns>
      public static ReferenceDataRowInfo ReadData(ElementNodeInfo node,
         DbDataReader reader)
      {
         ElementItemInfo item;
         ReferenceDataValueInfo value;
         String name;
         ReferenceDataRowInfo row = new ReferenceDataRowInfo();
         for(Int32 i=0; i<reader.FieldCount; i++)
         {
            name = reader.GetName(i);
            item = node?.Items.Find((x) => x.Name == name);
            value = new ReferenceDataValueInfo(item);
            value.Set(name, reader[i], i);
            row.Add(value);
         }
         return row;
      }
#endif

   }

}
