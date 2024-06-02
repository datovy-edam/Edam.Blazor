using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
//using DbField = Edam.Data.DataField;
using Edam.DataObjects.Objects;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.ReferenceData
{

   public class ReferenceDataEditTemplateInfo
   {

      public IDataCode? Group { get; set; }

      public DateTime? CreatedDate { get; set; }
      public DateTime? LastUpdateDate { get; set; }
      public String? OrganizationId { get; set; }
      public Int64 TemplateNo { get; set; }
      public String? TemplateVersionId { get; set; }
      public String? ResourceName { get; set; }
      public Int64? TemplateDefaultNo { get; set; }
      public Int16 TemplateTypeNo { get; set; }
      public String? TemplateData { get; set; }
      public String? MapData { get; set; }
      public String? GroupData { get; set; }
      public String? Title { get; set; }
      public Int64 GroupNo { get; set; }
      public Int16 ScopeNo { get; set; }
      public Int16 StatusNo { get; set; }
      public String? PostUpdateScript { get; set; }

      public ReferenceDataEditTemplate TemplateType
      {
         get { return (ReferenceDataEditTemplate)TemplateTypeNo; }
         set { TemplateTypeNo = (Int16)value; }
      }

      public ObjectScope Scope
      {
         get { return (ObjectScope)ScopeNo; }
         set { ScopeNo = (Int16)value; }
      }

      public ObjectStatus Status
      {
         get { return (ObjectStatus)StatusNo; }
         set { StatusNo = (Int16)value; }
      }

      public void CleaarFields()
      {
         Group = null;
         CreatedDate = DateTime.Now;
         LastUpdateDate = DateTime.Now;
         OrganizationId = String.Empty;
         TemplateNo = 0;
         TemplateVersionId = "v1r0";
         ResourceName = String.Empty;
         TemplateDefaultNo = null;
         TemplateTypeNo = 0;
         TemplateData = null;
         MapData = null;
         GroupData = null;
         Title = String.Empty;
         GroupNo = 0;
         ScopeNo = 1;
         StatusNo = 0;
         PostUpdateScript = String.Empty;
      }

#if DATA_SUPPORT_
      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instace of ReferenceListGroupInfo is returned</returns>
      public static ReferenceDataEditTemplateInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         ReferenceDataEditTemplateInfo p = new ReferenceDataEditTemplateInfo();

         p.CreatedDate = DbField.GetNullableDateTime(reader[0]);
         p.LastUpdateDate = DbField.GetNullableDateTime(reader[1]);
         p.OrganizationId = DbField.GetString(reader[2]);
         p.TemplateNo = DbField.GetInt64(reader[3]);
         p.TemplateVersionId = DbField.GetString(reader[4]);
         p.ResourceName = DbField.GetString(reader[5]);
         p.TemplateDefaultNo = DbField.GetNullableInt64(reader[6]);
         p.TemplateTypeNo = DbField.GetInt16(reader[7]);
         p.TemplateData = DbField.GetString(reader[8]);
         p.MapData = DbField.GetString(reader[9]);
         p.GroupData = DbField.GetString(reader[10]);
         p.Title = DbField.GetString(reader[11]);
         p.GroupNo = DbField.GetInt64(reader[12]);
         p.ScopeNo = DbField.GetInt16(reader[13]);
         p.StatusNo = DbField.GetInt16(reader[14]);
         p.PostUpdateScript = DbField.GetString(reader[15]);

         return p;
      }

      /// <summary>
      /// Get List of reference list groups..
      /// </summary>
      /// <param name="reader">data reader to read addresses from</param>
      /// <returns>List of ReferenceListGroupInfo is returned</returns>
      public static List<ReferenceDataEditTemplateInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<ReferenceDataEditTemplateInfo> l = 
            new List<ReferenceDataEditTemplateInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif

   }

}
