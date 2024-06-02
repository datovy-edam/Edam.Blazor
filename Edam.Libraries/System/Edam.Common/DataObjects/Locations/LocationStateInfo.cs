using System;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
//using Edam.Data;

namespace Edam.DataObjects.Locations
{

   public class LocationStateInfo
   {

      #region -- 1.00 Properties and Fields

      public String CountryCode { get; set; }
      public String StateCode { get; set; }
      public String StateName { get; set; }

      #endregion

      public LocationStateInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         CountryCode = String.Empty;
         StateCode = String.Empty;
         StateName = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static LocationStateInfo ReadData(
         LocationStateInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new LocationStateInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CountryCode = DataField.GetString(reader[0]);
         record.StateCode = DataField.GetString(reader[1]);
         record.StateName = DataField.GetString(reader[2]);

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
      /// <returns>instance List of LocationStateInfo'es</returns>
      public static List<LocationStateInfo> GetList(
         List<LocationStateInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         LocationStateInfo entity;
         if (list == null)
            list = new List<LocationStateInfo>();
         while (reader.Read())
         {
            entity = new LocationStateInfo();
            entity.ReadData(reader);
            list.Add(entity);
         }
         return list;
      }

#endif

   }

}
