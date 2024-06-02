using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// using Edam.Data;
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.References
{

   public class ReferenceItemGroupValueInfo
   {

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public Int32 VersionNo { get; set; }
      public Int32 GroupNo { get; set; }
      public String ItemId { get; set; }
      public String NumericId { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public Objects.ObjectCheckable DefaultChecked { get; set; }
      public Objects.ObjectStatus DefaultStatus { get; set; }
      public Objects.ObjectRequirable DefaultRequired { get; set; }
      public Objects.ObjectValueType ValueType { get; set; }
      public String DefaultValueId { get; set; }
      public String DefaultValueText { get; set; }
      public String Description { get; set; }
      public String PeriodToComply { get; set; }

      public ReferenceItemGroupValueInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         CreatedDate = Edam.NullDateTime.Value;
         OrganizationId = String.Empty;
         VersionNo = 0;
         GroupNo = 0;
         ItemId = String.Empty;
         NumericId = String.Empty;
         ReferenceDate = null;
         DefaultChecked = Objects.ObjectCheckable.Unknown;
         DefaultStatus = Objects.ObjectStatus.Unknown;
         DefaultRequired = Objects.ObjectRequirable.Unknown;
         ValueType = Objects.ObjectValueType.String;
         DefaultValueId = String.Empty;
         DefaultValueText = String.Empty;
         Description = String.Empty;
         PeriodToComply = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ReferenceItemGroupValueInfo ReadData(
         ReferenceItemGroupValueInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ReferenceItemGroupValueInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[2]);
         record.VersionNo = DataField.GetInt32(reader[4]);
         record.GroupNo = DataField.GetInt32(reader[5]);
         record.ItemId = DataField.GetString(reader[6]);
         record.NumericId = DataField.GetString(reader[7]);
         record.ReferenceDate = DataField.GetNullableDateTime(reader[8]);
         record.DefaultChecked = (ObjectCheckable) DataField.GetInt16(reader[9]);
         record.DefaultStatus = (ObjectStatus) DataField.GetInt16(reader[10]);
         record.DefaultRequired = (ObjectRequirable) DataField.GetInt16(reader[11]);
         record.ValueType = (ObjectValueType) DataField.GetInt16(reader[11]);
         record.DefaultValueId = DataField.GetString(reader[11]);
         record.DefaultValueText = DataField.GetString(reader[11]);
         record.Description = DataField.GetString(reader[11]);
         record.PeriodToComply = DataField.GetString(reader[11]);

         return record;
      }

      public void ReadData(System.Data.Common.DbDataReader reader)
      {
         ReadData(this, reader);
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of ActivityProgramInfo'es</returns>
      public static List<ReferenceItemGroupValueInfo> GetList(
         List<ReferenceItemGroupValueInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ReferenceItemGroupValueInfo record;
         if (list == null)
            list = new List<ReferenceItemGroupValueInfo>();
         while (reader.Read())
         {
            record = new ReferenceItemGroupValueInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}
