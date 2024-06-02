using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// using Edam.Data;

namespace Edam.DataObjects.Application
{

   public class ApplicationEntityGroupInfo
   {

      public String EntityId { get; set; }
      public String GroupId { get; set; }
      public String GroupName { get; set; }
      public String MemberId { get; set; }
      public References.ReferenceBaseType Role { get; set; }
      public String RoleText { get; set; }
      public String Alias { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public String StatusText { get; set; }

      public ApplicationEntityGroupInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         EntityId = String.Empty;
         GroupId = String.Empty;
         GroupName = String.Empty;
         MemberId = String.Empty;
         Role = References.ReferenceBaseType.Unknown;
         RoleText = String.Empty;
         Alias = String.Empty;
         Status = Objects.ObjectStatus.Unknown;
         StatusText = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="group">group data</param>
      /// <param name="reader">instance of DbDataReader</param>
      public static ApplicationEntityGroupInfo ReadData(
         ApplicationEntityGroupInfo group,
         global::System.Data.Common.DbDataReader reader)
      {
         if (group == null)
            group = new ApplicationEntityGroupInfo();

         if (reader == null)
         {
            group.ClearFields();
            return group;
         }

         group.EntityId = DataField.GetString(reader[0]);
         group.GroupId = DataField.GetString(reader[1]);
         group.GroupName = DataField.GetString(reader[2]);
         group.MemberId = DataField.GetString(reader[3]);
         group.Role = (References.ReferenceBaseType) 
            DataField.GetInt16(reader[4]);
         group.RoleText = DataField.GetString(reader[5]);
         group.Alias = DataField.GetString(reader[6]);
         group.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[7]);
         group.StatusText = DataField.GetString(reader[8]);

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
      public static List<ApplicationEntityGroupInfo> ReadData(
         List<ApplicationEntityGroupInfo> list,
         global::System.Data.Common.DbDataReader reader)
      {
         ApplicationEntityGroupInfo entity;
         if (list == null)
            list = new List<ApplicationEntityGroupInfo>();
         while (reader.Read())
         {
            entity = new ApplicationEntityGroupInfo();
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
      public static List<ApplicationEntityGroupInfo> GetList(
         global::System.Data.Common.DbDataReader reader)
      {
         return ReadData(null as List<ApplicationEntityGroupInfo>, reader);
      }

#endif

   }

}
