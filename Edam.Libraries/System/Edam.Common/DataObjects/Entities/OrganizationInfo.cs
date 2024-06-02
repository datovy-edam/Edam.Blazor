using System;
using System.Collections.Generic;
//using System.Data.Common;

// -----------------------------------------------------------------------------
//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   public class OrganizationInfo : OrganizationBaseInfo
   {

      public OrganizationInfo() : base()
      {

      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instance of OrganizationInfo is returned</returns>
      public static OrganizationInfo ReadData(DbDataReader reader)
      {
         OrganizationInfo o = new OrganizationInfo();

         o.OrganizationId = DbField.GetString(reader[0]);
         o.OrganizationName = DbField.GetString(reader[1]);

         return o;
      }

      /// <summary>
      /// Read list.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns>list of OrganizationInfo is returned</returns>
      public static List<OrganizationInfo> ReadList(DbDataReader reader)
      {
         List<OrganizationInfo> l = new List<OrganizationInfo>();
         OrganizationInfo o;
         while (reader.Read())
         {
            o = ReadData(reader);
            l.Add(o);
         }
         return l;
      }

#endif

   }

}
