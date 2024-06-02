using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.DataAnnotations;

// -----------------------------------------------------------------------------

//using DbField = Edam.Data.DataField;
using Edam.DataObjects.References;

namespace Edam.DataObjects.Locations
{

   /// <summary>
   /// Provide for a Full Location Address including support for Geo-Coordinates.
   /// </summary>
   public class LocationAddressInfo
   {

      public LocationType Type { get; set; }
      public LocationCategory Category { get; set; }
      public LocationStatus Status { get; set; }

      public String LocationTypeText
      {
         get { return Type.ToString(); }
         set
         {
            LocationType t = LocationType.Unknown;
            if (Enum.TryParse<LocationType>(value, out t))
               Type = t;
            else
               Type = LocationType.Unknown;
         }
      }

      public DateTime CreatedDate { get; set; }

      public String OrganizationId { get; set; }
      public String LocationId { get; set; }
      public String Description { get; set; }
      public String Alias { get; set; }
      public String AttentionTo { get; set; }
      public String StreetNumber { get; set; }
      public String Line1 { get; set; }
      public String Line2 { get; set; }
      public String Country { get; set; }
      public String Region { get; set; }
      public String CityLocality { get; set; }
      public String Neighborhood { get; set; }
      public String CityName { get; set; }
      public String StateCode { get; set; }
      public String PostalCode { get; set; }

      public Int32? CensusLocationNo { get; set; }
      public Decimal? Latitude { get; set; }
      public Decimal? Longitude { get; set; }

      public Boolean HasAssociation { get; set; }

      public LocationAddressInfo() : base()
      {
         ClearFields();
      }

      /// <summary>Clear fields</summary>
      /// <returns>true is returned if address was valid</returns>
      public void ClearFields()
      {
         Type = LocationType.Unknown;
         Category = LocationCategory.Unknown;
         Status = LocationStatus.Unknown;
         LocationId = String.Empty;
         AttentionTo = String.Empty;
         StreetNumber = String.Empty;
         Line1 = String.Empty;
         Line2 = String.Empty;
         Country = String.Empty;
         Region = String.Empty;
         CityLocality = String.Empty;
         Neighborhood = String.Empty;
         CityName = String.Empty;
         StateCode = String.Empty;
         PostalCode = String.Empty;
         Description = String.Empty;
         Alias = String.Empty;
         CensusLocationNo = null;
         Latitude = null;
         Longitude = null;
         HasAssociation = false;
      }

      public void Copy(LocationAddressInfo record)
      {
         if (record == null)
         {
            ClearFields();
            return;
         }

         Type = record.Type;
         Category = record.Category;
         Status = record.Status;
         OrganizationId = record.OrganizationId;
         LocationId = record.LocationId;
         Description = record.Description;
         Alias = record.Alias;
         AttentionTo = record.AttentionTo;
         StreetNumber = record.StreetNumber;
         Line1 = record.Line1;
         Line2 = record.Line2;
         Country = record.Country;
         Region = record.Region;
         CityLocality = record.CityLocality;
         Neighborhood = record.Neighborhood;
         CityName = record.CityName;
         StateCode = record.StateCode;
         PostalCode = record.PostalCode;
         CensusLocationNo = record.CensusLocationNo;
         Latitude = record.Latitude;
         Longitude = record.Longitude;
         HasAssociation = record.HasAssociation;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instace of LocationAddressInfo is returned</returns>
      public static LocationAddressInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         LocationAddressInfo a = new LocationAddressInfo();

         a.OrganizationId = DbField.GetString(reader[0]);
         a.LocationId = DbField.GetString(reader[1]);
         a.Type = (LocationType)DbField.GetInt16(reader[2]);
         a.Category = (LocationCategory)DbField.GetInt16(reader[3]);
         a.Status = (LocationStatus)DbField.GetInt16(reader[4]);
         a.Description = DbField.GetString(reader[5]);
         a.AttentionTo = DbField.GetString(reader[6]);
         a.StreetNumber = DbField.GetString(reader[7]);
         a.Line1 = DbField.GetString(reader[8]);
         a.Line2 = DbField.GetString(reader[9]);
         a.Country = DbField.GetString(reader[10]);
         a.Region = DbField.GetString(reader[11]);
         a.CityLocality = DbField.GetString(reader[12]);
         a.Neighborhood = DbField.GetString(reader[13]);
         a.CityName = DbField.GetString(reader[14]);
         a.StateCode = DbField.GetString(reader[15]);
         a.PostalCode = DbField.GetString(reader[16]);
         a.CensusLocationNo = DbField.GetInt32(reader[17]);
         a.Latitude = DbField.GetDecimal(reader[18]);
         a.Longitude = DbField.GetDecimal(reader[19]);
         a.Alias = DbField.GetString(reader[20]);
         a.HasAssociation = DbField.GetBool(reader[21]);

         return a;
      }

      /// <summary>
      /// Get List of addresses..
      /// </summary>
      /// <param name="reader">data reader to read addresses from</param>
      /// <returns>List of LocationAddressInfoes is returned</returns>
      public static List<LocationAddressInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<LocationAddressInfo> l = new List<LocationAddressInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif 

   }

}
