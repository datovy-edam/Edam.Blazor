using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
//using DataField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Entity Group Info.
   /// </summary>
   public class EntityGroupMemberInfo
   {

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String EntityId { get; set; }
      public String GroupId { get; set; }
      public String MemberId { get; set; }
      public String MemberReferenceId { get; set; }
      public References.ReferenceBaseType Role { get; set; }
      public String Alias { get; set; }
      public Objects.ObjectStatus Status { get; set; }

      public String PayersSummaryText { get; set; }

      public EntityGroupMemberInfo()
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
         MemberId = String.Empty;
         MemberReferenceId = String.Empty;
         Role = References.ReferenceBaseType.Unknown;
         Alias = String.Empty;
         Status = Objects.ObjectStatus.Unknown;
         PayersSummaryText = "";
      }

      public static void FixNullValues(EntityGroupMemberInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.EntityId =
            Edam.Convert.ToNotNullString(record.EntityId);
         record.GroupId =
            Edam.Convert.ToNotNullString(record.GroupId);
         record.MemberId =
            Edam.Convert.ToNotNullString(record.MemberId);
         record.Alias =
            Edam.Convert.ToNotNullString(record.Alias);
         record.MemberReferenceId =
            Edam.Convert.ToNotNullString(record.MemberReferenceId);
      }

      public void Copy(EntityGroupMemberInfo member)
      {
         OrganizationId = member.OrganizationId;
         EntityId = member.EntityId;
         GroupId = member.GroupId;
         MemberId = member.MemberId;
         MemberReferenceId = member.MemberReferenceId;
         Role = member.Role;
         Alias = member.Alias;
         Status = member.Status;
         PayersSummaryText = member.PayersSummaryText;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="group">group data</param>
      /// <param name="reader">instance of DbDataReader</param>
      public static EntityGroupMemberInfo ReadData(
         EntityGroupMemberInfo group,
         global::System.Data.Common.DbDataReader reader)
      {
         if (group == null)
            group = new EntityGroupMemberInfo();

         if (reader == null)
         {
            group.ClearFields();
            return group;
         }

         group.CreatedDate = DataField.GetDateTime(reader[0]);
         group.OrganizationId = DataField.GetString(reader[1]);
         group.EntityId = DataField.GetString(reader[2]);
         group.GroupId = DataField.GetString(reader[3]);
         group.MemberId = DataField.GetString(reader[4]);
         group.MemberReferenceId = DataField.GetString(reader[5]);
         group.Role = (References.ReferenceBaseType)
            DataField.GetInt16(reader[6]);
         group.Alias = DataField.GetString(reader[7]);
         group.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[8]);
         group.PayersSummaryText = DataField.GetString(reader[9]);

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
      public static List<EntityGroupMemberInfo> ReadData(
         List<EntityGroupMemberInfo> list,
         global::System.Data.Common.DbDataReader reader)
      {
         EntityGroupMemberInfo entity;
         if (list == null)
            list = new List<EntityGroupMemberInfo>();
         while (reader.Read())
         {
            entity = new EntityGroupMemberInfo();
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
      public static List<EntityGroupMemberInfo> GetList(
         global::System.Data.Common.DbDataReader reader)
      {
         return ReadData(null as List<EntityGroupMemberInfo>, reader);
      }

#endif

   }

}
