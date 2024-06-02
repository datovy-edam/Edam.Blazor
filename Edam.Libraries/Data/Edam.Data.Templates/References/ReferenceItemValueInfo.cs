using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
//using Edam.Data;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Models;

namespace Edam.DataObjects.References
{

   public class ReferenceItemValueInfo : ObjectValueBase
   {
      private String m_ItemId;

      public Int64 SerialNo { get; set; }
      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public String Name { get; set; }

      public String ItemId
      {
         get { return m_ItemId; }
         set
         {
            m_ItemId = (String.IsNullOrWhiteSpace(value)) ? Name : value;
         }
      }

      public String EntryId { get; set; }
      public String NumericId { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public ObjectCheckable Checked { get; set; }
      public ObjectStatus Status { get; set; }
      public ObjectScope Scope { get; set; }
      public ObjectRequirable Required { get; set; }
      public String ValueId { get; set; }
      public DateTime? ExpirationDate { get; set; }
      public String Description { get; set; }

      public ReferenceItemValueInfo() : base()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         SerialNo = 0;
         CreatedDate = Edam.NullDateTime.Value;
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         ItemId = String.Empty;
         NumericId = String.Empty;
         ReferenceDate = null;
         Checked = ObjectCheckable.Unknown;
         Status = ObjectStatus.Unknown;
         Scope = ObjectScope.Unknown;
         Required = ObjectRequirable.Unknown;
         ValueId = String.Empty;
         ExpirationDate = null;
         Description = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ReferenceItemValueInfo ReadData(
         ReferenceItemValueInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ReferenceItemValueInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.ReferenceId = DataField.GetString(reader[2]);
         record.ItemId = DataField.GetString(reader[3]);
         record.EntryId = DataField.GetString(reader[4]);
         record.NumericId = DataField.GetString(reader[5]);
         record.ReferenceDate = DataField.GetNullableDateTime(reader[6]);
         record.Checked = (ObjectCheckable)DataField.GetInt16(reader[7]);
         record.Status = (ObjectStatus)DataField.GetInt16(reader[8]);
         record.Scope = (ObjectScope)DataField.GetInt16(reader[9]);
         record.Required = (ObjectRequirable)DataField.GetInt16(reader[10]);
         record.ValueId = DataField.GetString(reader[11]);
         record.ValueText = DataField.GetString(reader[12]);
         record.ExpirationDate = DataField.GetNullableDateTime(reader[13]);
         record.ValueType = (ObjectValueType)DataField.GetInt16(reader[14]);
         record.Description = DataField.GetString(reader[15]);

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
      public static List<ReferenceItemValueInfo> GetList(
         List<ReferenceItemValueInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ReferenceItemValueInfo record;
         if (list == null)
            list = new List<ReferenceItemValueInfo>();
         while (reader.Read())
         {
            record = new ReferenceItemValueInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}
