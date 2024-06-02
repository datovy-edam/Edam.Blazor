using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Notes
{

   public class NoteInfo
   {

      #region -- 1.00 Properties and Fields
      
      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String ReferenceId { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public String NoteId { get; set; }
      public Objects.ObjectScope Scope { get; set; }
      public Objects.ObjectStatus Status { get; set; }

      public NoteType Type { get; set; }
      public Int16 TypeNo
      {
         get { return (Int16)Type; }
         set { Type = (NoteType)value; }
      }

      public String Alias { get; set; }
      public String NoteText { get; set; }

      public String TypeDescription
      {
         get
         {
            return TypeCode == null ? String.Empty : TypeCode.Description;
         }
      }
      private DataCodeInfo m_TypeCode;
      public DataCodeInfo TypeCode
      {
         get { return m_TypeCode; }
         set
         {
            if (value == null)
               return;
            m_TypeCode = value;
            var no = value.CodeNo;
            if (no.HasValue)
               TypeNo = no.Value;
         }
      }

      #endregion

      public NoteInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         CreatedDate = DateTime.Now;
         OrganizationId = String.Empty;
         ReferenceId = String.Empty;
         NoteId = String.Empty;
         ReferenceDate = DateTime.Now;
         Scope = Objects.ObjectScope.Private;
         Status = Objects.ObjectStatus.Active;
         Type = NoteType.Unknown;
         Alias = String.Empty;
         NoteText = String.Empty;
         TypeCode = new DataCodeInfo();
      }

      public Boolean Validate()
      {
         return !String.IsNullOrWhiteSpace(NoteText) &&
            !String.IsNullOrWhiteSpace(ReferenceId) &&
            !String.IsNullOrWhiteSpace(OrganizationId);
      }

      public static void FixNullValues(NoteInfo record)
      {
         record.OrganizationId =
            Edam.Convert.ToNotNullString(record.OrganizationId);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
         record.NoteId = Edam.Convert.ToNotNullString(record.NoteId);
         record.Alias = Edam.Convert.ToNotNullString(record.Alias);
         record.NoteText = Edam.Convert.ToNotNullString(record.NoteText);
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static NoteInfo ReadData(
         NoteInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new NoteInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.ReferenceId = DataField.GetString(reader[2]);
         record.NoteId = DataField.GetString(reader[3]);
         record.ReferenceDate = DataField.GetDateTime(reader[4]);
         record.Scope = (Objects.ObjectScope)DataField.GetInt16(reader[5]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[6]);
         record.Type = (NoteType)DataField.GetInt16(reader[7]);
         record.Alias = DataField.GetString(reader[8]);
         record.NoteText = DataField.GetString(reader[9]);

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
      /// <returns>instance List of NoteInfo'es</returns>
      public static List<NoteInfo> GetList(
         List<NoteInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         NoteInfo record;
         if (list == null)
            list = new List<NoteInfo>();
         while(reader.Read())
         {
            record = new NoteInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}
