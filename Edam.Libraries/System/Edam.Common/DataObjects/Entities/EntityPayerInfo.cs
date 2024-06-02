using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
//using Edam.Data;
//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.Entities
{

   public class EntityPayerInfo
   {

      public String OrganizationId { get; set; }
      public String PayerId { get; set; }
      public String PayerEntityId { get; set; }
      public String RepresentativeId { get; set; }
      public String MemberEntityId { get; set; }
      public String MemberId { get; set; }
      public String Alias { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public DateTime? ExpirationDate { get; set; }
      public String GlAccountId { get; set; }
      public String PayeeAccountId { get; set; }
      public Decimal? CommittedAmount { get; set; }
      public Decimal? PaidAmount { get; set; }
      public Objects.ObjectStatus Status { get; set; }

      public void ClearFields()
      {
         OrganizationId = String.Empty;
         PayerId = String.Empty;
         PayerEntityId = String.Empty;
         RepresentativeId = String.Empty;
         MemberEntityId = String.Empty;
         MemberId = String.Empty;
         Alias = String.Empty;
         ReferenceDate = null;
         ExpirationDate = null;
         GlAccountId = String.Empty;
         PayeeAccountId = String.Empty;
         CommittedAmount = 0.0M;
         PaidAmount = 0.0M;
         Status = Objects.ObjectStatus.Active;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instace of EntityPayerInfo is returned</returns>
      public static EntityPayerInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         EntityPayerInfo p = new EntityPayerInfo();

         p.OrganizationId = DbField.GetString(reader[0]);
         p.PayerId = DbField.GetString(reader[1]);
         p.PayerEntityId = DbField.GetString(reader[2]);
         p.RepresentativeId = DbField.GetString(reader[3]);
         p.MemberEntityId = DbField.GetString(reader[4]);
         p.MemberId = DbField.GetString(reader[5]);
         p.Alias = DbField.GetString(reader[6]);
         p.ReferenceDate = DbField.GetNullableDateTime(reader[7]);
         p.ExpirationDate = DbField.GetNullableDateTime(reader[8]);
         p.GlAccountId = DbField.GetString(reader[9]);
         p.PayeeAccountId = DbField.GetString(reader[10]);
         p.CommittedAmount = DbField.GetNullableDecimal(reader[11]);
         p.PaidAmount = DbField.GetNullableDecimal(reader[12]);
         p.Status = (Objects.ObjectStatus)DbField.GetInt16(reader[13]);

         return p;
      }

      /// <summary>
      /// Get List of payers..
      /// </summary>
      /// <param name="reader">data reader to read addresses from</param>
      /// <returns>List of EntityPayerInfo is returned</returns>
      public static List<EntityPayerInfo> GetList(
         System.Data.Common.DbDataReader reader)
      {
         List<EntityPayerInfo> l = new List<EntityPayerInfo>();
         while (reader.Read())
         {
            l.Add(ReadData(reader));
         }
         return l;
      }

#endif

   }

}
