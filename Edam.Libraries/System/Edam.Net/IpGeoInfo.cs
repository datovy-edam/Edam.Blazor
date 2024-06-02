using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// -----------------------------------------------------------------------------

namespace Kif.Net
{

   [XmlRoot("Response")]
   public class IpGeoInfo
   {

      public String IP { get; set; }
      public String CountryCode { get; set; }
      public String CountryName { get; set; }
      public String RegionCode { get; set; }
      public String RegionName { get; set; }
      public String City { get; set; }
      public String ZipCode { get; set; }
      public String TimeZone { get; set; }
      public String Latitude { get; set; }
      public String Longitude { get; set; }
      public String MetroCode { get; set; }

      public IpGeoInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         IP = String.Empty;
         CountryCode = String.Empty;
         CountryName = String.Empty;
         RegionCode = String.Empty;
         RegionName = String.Empty;
         City = String.Empty;
         ZipCode = String.Empty;
         TimeZone = String.Empty;
         Latitude = String.Empty;
         Longitude = String.Empty;
         MetroCode = String.Empty;
      }

   }

}
