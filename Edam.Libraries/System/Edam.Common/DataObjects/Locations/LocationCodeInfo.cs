using System;
using System.Collections.Generic;
//using System.Data.Common;

// -----------------------------------------------------------------------------
//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Locations
{

   /// <summary>
   /// Location Code Info...
   /// </summary>
   public class LocationUnitInfo
   {

      public String CodeId { get; set; }
      public String CountryId { get; set; }
      public String CountyName { get; set; }
      public String StateCode { get; set; }
      public String GeographicalUnitName { get; set; }
      public String PostalCode { get; set; }

      public LocationUnitInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         CodeId = String.Empty;
         CountryId = String.Empty;
         CountyName = String.Empty;
         StateCode = String.Empty;
         GeographicalUnitName = String.Empty;
         PostalCode = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read a location code from given data reader.
      /// </summary>
      /// <param name="reader">reader</param>
      /// <returns>instance of LocationCodeInfo is returned</returns>
      public static LocationUnitInfo GetData(DbDataReader reader)
      {
         LocationUnitInfo l = new LocationUnitInfo();
         l.CodeId = DbField.GetString(reader[0]);
         l.CountryId = DbField.GetString(reader[1]);
         l.CountyName = DbField.GetString(reader[2]);
         l.StateCode = DbField.GetString(reader[3]);
         l.GeographicalUnitName = DbField.GetString(reader[4]);
         l.PostalCode = DbField.GetString(reader[5]);
         return l;
      }

      /// <summary>
      /// Get the list of locations from given data reader.
      /// </summary>
      /// <param name="reader">reader</param>
      /// <returns>list of LocationCodeInfoes is returned</returns>
      public static List<LocationUnitInfo> GetList(DbDataReader reader)
      {
         List<LocationUnitInfo> l = new List<LocationUnitInfo>();
         while (reader.Read())
         {
            l.Add(GetData(reader));
         }
         return l;
      }

#endif

   }

}
