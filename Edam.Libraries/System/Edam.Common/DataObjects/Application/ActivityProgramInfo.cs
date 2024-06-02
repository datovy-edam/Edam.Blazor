using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Application
{
   public class ActivityProgramInfo
   {

      #region -- 1.00 Properties and Fields

      public DateTime CreatedDate { get; set; }
      public String OrganizationId { get; set; }
      public String ProgramId { get; set; }
      public ActivityProgramType Type { get; set; }
      public ObjectStatus Status { get; set; }
      public ObjectScope Scope { get; set; }
      public String Alias { get; set; }
      public String Abstract { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public String GlAccountId { get; set; }
      public Decimal FeeAmount { get; set; }
      public Int16 Capacity { get; set; }
      public String TemplateId { get; set; }

      #endregion

      public ActivityProgramInfo()
      {
         ClearFields();
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         CreatedDate = Edam.Application.Defaults.NullDate;
         OrganizationId = String.Empty;
         ProgramId = String.Empty;
         Type = ActivityProgramType.Unknown;
         Status = Objects.ObjectStatus.Unknown;
         Scope = Objects.ObjectScope.Public;
         Alias = String.Empty;
         Abstract = String.Empty;
         StartDate = NullDateTime.Value;
         EndDate = NullDateTime.Value;
         GlAccountId = String.Empty;
         FeeAmount = new Decimal(0.0);
         Capacity = 0;
         TemplateId = String.Empty;
      }

      public static void FixNullValues(ActivityProgramInfo record)
      {
         record.OrganizationId =
            Convert.ToNotNullString(record.OrganizationId);
         record.ProgramId =
            Convert.ToNotNullString(record.ProgramId);
         record.Alias =
            Convert.ToNotNullString(record.Alias);
         record.Abstract =
            Convert.ToNotNullString(record.Abstract);
         record.GlAccountId =
            Convert.ToNotNullString(record.GlAccountId);
      }

      /*
      /// <summary>
      /// Read data from reader...
      /// </summary>
      /// <param name="reader">instance of DbDataReader</param>
      public static ActivityProgramInfo ReadData(
         ActivityProgramInfo record, System.Data.Common.DbDataReader reader)
      {
         if (record == null)
            record = new ActivityProgramInfo();

         if (reader == null)
         {
            record.ClearFields();
            return record;
         }

         record.CreatedDate = DataField.GetDateTime(reader[0]);
         record.OrganizationId = DataField.GetString(reader[1]);
         record.ProgramId = DataField.GetString(reader[2]);
         record.Type = (ActivityProgramType)DataField.GetInt16(reader[3]);
         record.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[4]);
         record.Scope = (Objects.ObjectScope)DataField.GetInt16(reader[5]);
         record.Alias = DataField.GetString(reader[6]);
         record.Abstract = DataField.GetString(reader[7]);
         record.StartDate = DataField.GetDateTime(reader[8]);
         record.EndDate = DataField.GetDateTime(reader[9]);
         record.GlAccountId = DataField.GetString(reader[10]);
         record.FeeAmount = DataField.GetDecimal(reader[11]);
         record.Capacity = DataField.GetInt16(reader[12]);
         record.TemplateId = DataField.GetString(reader[13]);

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
      /// <returns>instance List of ActivityProgramInfo'es</returns>
      public static List<ActivityProgramInfo> GetList(
         List<ActivityProgramInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ActivityProgramInfo record;
         if (list == null)
            list = new List<ActivityProgramInfo>();
         while (reader.Read())
         {
            record = new ActivityProgramInfo();
            record.ReadData(reader);
            list.Add(record);
         }
         return list;
      }
      */

   }

}
