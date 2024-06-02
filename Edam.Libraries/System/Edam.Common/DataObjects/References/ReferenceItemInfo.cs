using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// using Edam.Data;
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.References
{

   public class ReferenceItemInfo
   {

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public Int32? VersionNo { get; set; }
      public Int32? GroupNo { get; set; }
      public String ReferenceId { get; set; }
      public String ItemsGroupId { get; set; }
      public ObjectStatus Status { get; set; }
      public ObjectScope Scope { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public String Alias { get; set; }

      public ReferenceItemInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         CreatedDate = Edam.NullDateTime.Value;
         OrganizationId = String.Empty;
         VersionNo = null;
         GroupNo = 0;
         ReferenceId = String.Empty;
         ItemsGroupId = String.Empty;
         Status = ObjectStatus.Unknown;
         Scope = ObjectScope.Unknown;
         ReferenceDate = null;
         Alias = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ReferenceItemInfo ReadData(
         ReferenceItemInfo record,
         System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ReferenceItemInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[2]);
         record.VersionNo = DataField.GetInt32(reader[4]);
         record.GroupNo = DataField.GetInt32(reader[5]);
         record.ReferenceId = DataField.GetString(reader[6]);
         record.ItemsGroupId = DataField.GetString(reader[7]);
         record.Status = (ObjectStatus)DataField.GetInt16(reader[8]);
         record.Scope = (ObjectScope)DataField.GetInt16(reader[9]);
         record.ReferenceDate = DataField.GetNullableDateTime(reader[10]);
         record.Alias = DataField.GetString(reader[11]);

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
      public static List<ReferenceItemInfo> GetList(
         List<ReferenceItemInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ReferenceItemInfo record;
         if (list == null)
            list = new List<ReferenceItemInfo>();
         while (reader.Read())
         {
            record = new ReferenceItemInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}
