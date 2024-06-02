using System;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Data;

namespace Edam.DataObjects.Documents
{

   public class DocumentInfo
   {

      #region -- 1.00 Properties and Fields
      
      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public DocumentType DocumentType { get; set; }
      public String DocumentId { get; set; }
      public String DocumentNumber { get; set; }
      public String AliasId { get; set; }
      public DocumentClass Class { get; set; }
      public String ClassText { get; set; }
      public Objects.ObjectScope Scope { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public String IssuingAuthorityText { get; set; }
      public String IssuingAuthorityId { get; set; }
      public String CountryCode { get; set; }
      public String StateCode { get; set; }
      public DateTime ExpirationDate { get; set; }
      public String Annotations { get; set; }
      public References.ReferenceType ReferenceType { get; set; }
      public DocumentAssociationType AssociationType { get; set; }

      #endregion

      public DocumentInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         CreatedDate = Edam.Application.Defaults.NullDate;
         OrganizationId = Edam.Application.Resources.Strings.DefaultAgencyId;
         ReferenceId = String.Empty;
         DocumentType = DocumentType.Unknown;
         DocumentId = String.Empty;
         DocumentNumber = String.Empty;
         AliasId = String.Empty;
         Class = DocumentClass.Unknown;
         ClassText = String.Empty;
         Status = Objects.ObjectStatus.Unknown;
         Scope = Objects.ObjectScope.Unknown;
         IssuingAuthorityText = String.Empty;
         IssuingAuthorityId = String.Empty;
         CountryCode = String.Empty;
         StateCode = String.Empty;
         ExpirationDate = Edam.NullDateTime.Value;
         Annotations = String.Empty;
         ReferenceType = References.ReferenceType.Unknown;
         AssociationType = DocumentAssociationType.Unknown;
      }

      public static void FixNullValues(DocumentInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.ReferenceId =
            Edam.Convert.ToNotNullString(record.ReferenceId);
         record.DocumentId =
            Edam.Convert.ToNotNullString(record.DocumentId);
         record.DocumentNumber =
            Edam.Convert.ToNotNullString(record.DocumentNumber);
         record.ClassText =
            Edam.Convert.ToNotNullString(record.ClassText);
         record.AliasId =
            Edam.Convert.ToNotNullString(record.AliasId);
         record.CountryCode =
            Edam.Convert.ToNotNullString(record.CountryCode);
         record.StateCode =
            Edam.Convert.ToNotNullString(record.StateCode);
         record.IssuingAuthorityText =
            Edam.Convert.ToNotNullString(record.IssuingAuthorityText);
         record.IssuingAuthorityId =
            Edam.Convert.ToNotNullString(record.IssuingAuthorityId);
         record.Annotations = Edam.Convert.ToNotNullString(record.Annotations);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static DocumentInfo ReadData(
         DocumentInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new DocumentInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.ReferenceId = DataField.GetString(reader[2]);
         record.DocumentType = (DocumentType)DataField.GetInt16(reader[3]);
         record.DocumentId = DataField.GetString(reader[4]);
         record.DocumentNumber = DataField.GetString(reader[5]);
         record.AliasId = DataField.GetString(reader[6]);
         record.Class = (DocumentClass)DataField.GetInt16(reader[7]);
         record.ClassText = DataField.GetString(reader[8]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[9]);
         record.Scope = (Objects.ObjectScope)DataField.GetInt16(reader[10]);
         record.IssuingAuthorityText = DataField.GetString(reader[11]);
         record.IssuingAuthorityId = DataField.GetString(reader[12]);
         record.CountryCode = DataField.GetString(reader[13]);
         record.StateCode = DataField.GetString(reader[14]);
         record.ExpirationDate = DataField.GetDateTime(reader[15]);
         record.Annotations = DataField.GetString(reader[16]);
         record.ReferenceType = (References.ReferenceType)
            DataField.GetInt16(reader[17]);
         record.AssociationType = (DocumentAssociationType)
            DataField.GetInt16(reader[18]);

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
      /// <returns>instance List of DocumentInfo'es</returns>
      public static List<DocumentInfo> GetList(
         List<DocumentInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         DocumentInfo record;
         if (list == null)
            list = new List<DocumentInfo>();
         while(reader.Read())
         {
            record = new DocumentInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}
