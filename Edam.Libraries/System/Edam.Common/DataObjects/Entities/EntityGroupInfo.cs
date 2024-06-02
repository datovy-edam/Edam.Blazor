using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
//using DataField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Entity Group Info.
   /// </summary>
   public class EntityGroupInfo
   {

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String EntityId { get; set; }
      public String GroupId { get; set; }
      public String GroupName { get; set; }
      public String AliasId { get; set; }
      public References.ReferenceGroupType Type { get; set; }
      public Objects.ObjectStatus  Status { get; set; }

      public EntityGroupInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields.
      /// </summary>
      public void ClearFields()
      {
         CreatedDate = Edam.NullDateTime.Value;
         OrganizationId = String.Empty;
         EntityId = String.Empty;
         GroupId = String.Empty;
         GroupName = String.Empty;
         AliasId = String.Empty;
         Type = References.ReferenceGroupType.Unknown;
         Status = Objects.ObjectStatus.Unknown;
      }

      public void Copy(EntityGroupInfo group)
      {
         CreatedDate = group.CreatedDate;
         OrganizationId = group.OrganizationId;
         EntityId = group.EntityId;
         GroupId = group.GroupId;
         GroupName = group.GroupName;
         AliasId = group.AliasId;
         Type = group.Type;
         Status = group.Status;
      }

      public static void FixNullValues(EntityGroupInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.EntityId =
            Edam.Convert.ToNotNullString(record.EntityId);
         record.GroupId =
            Edam.Convert.ToNotNullString(record.GroupId);
         record.AliasId =
            Edam.Convert.ToNotNullString(record.AliasId);
         record.GroupName =
            Edam.Convert.ToNotNullString(record.GroupName);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="group">group data</param>
      /// <param name="reader">instance of DbDataReader</param>
      public static EntityGroupInfo ReadData(
         EntityGroupInfo group,
         global::System.Data.Common.DbDataReader reader)
      {
         if (group == null)
            group = new EntityGroupInfo();

         if (reader == null)
         {
            group.ClearFields();
            return group;
         }

         group.CreatedDate = DataField.GetDateTime(reader[0]);
         group.OrganizationId = DataField.GetString(reader[1]);
         group.EntityId = DataField.GetString(reader[2]);
         group.GroupId = DataField.GetString(reader[3]);
         group.GroupName = DataField.GetString(reader[4]);
         group.AliasId = DataField.GetString(reader[5]);
         group.Type = (References.ReferenceGroupType)
            DataField.GetInt16(reader[6]);
         group.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[7]);

         return group;
      }

      public void ReadData(global::System.Data.Common.DbDataReader reader)
      {
         ReadData(this, reader);
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of NoteInfo'es</returns>
      public static List<EntityGroupInfo> ReadData(
         List<EntityGroupInfo> list,
         global::System.Data.Common.DbDataReader reader)
      {
         EntityGroupInfo entity;
         if (list == null)
            list = new List<EntityGroupInfo>();
         while (reader.Read())
         {
            entity = new EntityGroupInfo();
            entity.ReadData(reader);
            list.Add(entity);
         }
         return list;
      }

      /// <summary>
      /// Get List...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>list of entity groups is returned</returns>
      public static List<EntityGroupInfo> GetList(
         global::System.Data.Common.DbDataReader reader)
      {
         return ReadData(null as List<EntityGroupInfo>, reader);
      }

#endif

   }

}
