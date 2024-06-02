using System;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Documents
{

   public class DocumentClassInfo
   {

      #region -- 1.00 Properties and Fields

      public String GroupId { get; set; }
      public String CodeId { get; set; }
      public String ClassId { get; set; }
      public String CountryCode { get; set; }
      public String StateCode { get; set; }
      public String Description { get; set; }

      #endregion

      public DocumentClassInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         GroupId = String.Empty;
         CodeId = String.Empty;
         ClassId = String.Empty;
         CountryCode = String.Empty;
         StateCode = String.Empty;
         Description = String.Empty;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static DocumentClassInfo ReadData(
         DocumentClassInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new DocumentClassInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.GroupId = DataField.GetString(reader[0]);
         record.CodeId = DataField.GetString(reader[1]);
         record.ClassId = DataField.GetString(reader[2]);
         record.CountryCode = DataField.GetString(reader[3]);
         record.StateCode = DataField.GetString(reader[4]);
         record.Description = DataField.GetString(reader[5]);

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
      /// <returns>instance List of DocumentClassInfo'es</returns>
      public static List<DocumentClassInfo> GetList(
         List<DocumentClassInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         DocumentClassInfo entity;
         if (list == null)
            list = new List<DocumentClassInfo>();
         while (reader.Read())
         {
            entity = new DocumentClassInfo();
            entity.ReadData(reader);
            list.Add(entity);
         }
         return list;
      }

#endif

   }

}
