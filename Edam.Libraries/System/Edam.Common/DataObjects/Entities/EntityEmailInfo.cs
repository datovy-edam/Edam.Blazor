using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// using DataField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   public class EntityEmailInfo
   {

      public String EntityId { get; set; }
      public String OrganizationId { get; set; }
      public String DisplayName { get; set; }
      public String EmailUrlId { get; set; }
      public String Email { get; set; }

      public EntityEmailInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         EntityId = String.Empty;
         OrganizationId = String.Empty;
         DisplayName = String.Empty;
         EmailUrlId = String.Empty;
         Email = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="group">group data</param>
      /// <param name="reader">instance of DbDataReader</param>
      public static EntityEmailInfo ReadData(
         EntityEmailInfo email,
         global::System.Data.Common.DbDataReader reader)
      {
         if (email == null)
            email = new EntityEmailInfo();

         if (reader == null)
         {
            email.ClearFields();
            return email;
         }

         email.EntityId = DataField.GetString(reader[0]);
         email.OrganizationId = DataField.GetString(reader[1]);
         email.DisplayName = DataField.GetString(reader[2]);
         email.EmailUrlId = DataField.GetString(reader[3]);
         email.Email = DataField.GetString(reader[4]);

         return email;
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
      public static List<EntityEmailInfo> ReadData(
         List<EntityEmailInfo> list,
         global::System.Data.Common.DbDataReader reader)
      {
         EntityEmailInfo email;
         if (list == null)
            list = new List<EntityEmailInfo>();
         while (reader.Read())
         {
            email = new EntityEmailInfo();
            email.ReadData(reader);
            list.Add(email);
         }
         return list;
      }

      /// <summary>
      /// Get List...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>list of entity grous is returned</returns>
      public static List<EntityEmailInfo> GetList(
         global::System.Data.Common.DbDataReader reader)
      {
         return ReadData(null as List<EntityEmailInfo>, reader);
      }

#endif

   }

}
