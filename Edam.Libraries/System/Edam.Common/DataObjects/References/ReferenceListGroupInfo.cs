using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

#if DATA_SUPPORT_
using DbField = Edam.Data.DataField;
#endif

using Edam.DataObjects;

namespace Edam.DataObjects.References
{

   public class ReferenceListGroupInfo
   {

      public Int64 SerialNo { get; set; }
      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public Int16? GroupNo { get; set; }
      public Int16? ListNo { get; set; }
      public String Name { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public String Comment { get; set; }

      public ReferenceListGroupInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         SerialNo = -1;
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         GroupNo = null;
         ListNo = null;
         Name = String.Empty;
         ReferenceDate = null;
         Comment = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instace of ReferenceListGroupInfo is returned</returns>
      public static ReferenceListGroupInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         ReferenceListGroupInfo p = new ReferenceListGroupInfo();

         p.OrganizationId = DbField.GetString(reader[0]);
         p.ReferenceId = DbField.GetString(reader[1]);
         p.GroupNo = DbField.GetInt16(reader[2]);
         p.ListNo = DbField.GetInt16(reader[3]);
         p.Name = DbField.GetString(reader[4]);
         p.ReferenceDate = DbField.GetDateTime(reader[5]);
         p.Comment = DbField.GetString(reader[6]);

         return p;
      }

      /// <summary>
      /// Get List of reference list groups..
      /// </summary>
      /// <param name="reader">data reader to read addresses from</param>
      /// <returns>List of ReferenceListGroupInfo is returned</returns>
      public static List<ReferenceListGroupInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<ReferenceListGroupInfo> l = new List<ReferenceListGroupInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif

   }

}
