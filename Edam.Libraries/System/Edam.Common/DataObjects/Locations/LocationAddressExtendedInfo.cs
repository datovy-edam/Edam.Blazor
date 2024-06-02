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
   /// Location Address Extended...
   /// </summary>
   public class LocationAddressExtendedInfo : LocationAddressInfo
   {

      public String TypeDescription { get; set; }
      public String CategoryDescription { get; set; }
      public String StatusDescription { get; set; }

      public LocationAddressExtendedInfo()
         : base()
      {
      }

      public new void ClearFields()
      {
         base.ClearFields();
         TypeDescription = String.Empty;
         CategoryDescription = String.Empty;
         StatusDescription = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// 
      /// </summary>
      /// <param name="reader"></param>
      /// <returns></returns>
      public static new LocationAddressExtendedInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         LocationAddressExtendedInfo a = new LocationAddressExtendedInfo();
         LocationAddressInfo.ReadData(reader);
         Int32 i = reader.FieldCount - 3;
         a.TypeDescription = DbField.GetString(reader[i]);
         a.CategoryDescription = DbField.GetString(reader[i + 1]);
         a.StatusDescription = DbField.GetString(reader[i + 2]);

         return a;
      }

      /// <summary>
      /// Get List...
      /// </summary>
      /// <param name="reader"></param>
      /// <returns></returns>
      public static new List<LocationAddressExtendedInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<LocationAddressExtendedInfo> l = new List<LocationAddressExtendedInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif

   }
   
}
