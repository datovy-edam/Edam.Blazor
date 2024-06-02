using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.DataObjects.References;
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Activities
{

   public class ActivityPeriodRatingInfo
   {
      public const char RECORD_NEW = ' ';
      public const char RECORD_ACEPTED = 'A';
      public const char RECORD_CHANGED = 'C';
      public const char RECORD_DELETED = 'D';
      public const char RECORD_LOADED = 'L';
      public const char RECORD_TOUCHED = 'T';

      public bool ChangedIndicator
      {
         get
         {
            return RecordStatusCode == RECORD_NEW ||
               RecordStatusCode == RECORD_TOUCHED ||
               RecordStatusCode == RECORD_DELETED;
         }
      }

      public string OrganizationId { get; set; }
      public string GroupId { get; set; }
      public string GroupEventId { get; set; }
      public string PeriodId { get; set; }
      public DateTimeOffset ReferenceDate { get; set; }
      public string ProgramId { get; set; }
      public string ContentTemplateId { get; set; }
      public string ContentThreadId { get; set; }
      public int ContentVersionNo { get; set; }
      public string ContentIdentifierId { get; set; }
      public string EntityId { get; set; }
      public long RatingNo { get; set; }
      public short RatingTypeNo { get; set; }
      public decimal RatingWeight { get; set; }
      public string Alias { get; set; }
      public short StatusNo { get; set; }
      public short ResultTypeNo { get; set; }
      public short ParticipationCount { get; set; }
      public short ParticipantRoleNo { get; set; }
      public string ParticipantName { get; set; }
      public decimal RatedValue { get; set; }

      public double RatedValueDouble
      {
         get { return (double)RatedValue; }
         set { RatedValue = (decimal)value; }
      }

      public string RatedValueText
      {
         get { return RatedValue.ToString(); }
         set
         {
            decimal ratedValue;
            if (decimal.TryParse(value, out ratedValue))
            {
               RatedValue = ratedValue;
            }
         }
      }

      public bool ParticipationIndicator
      {
         get { return ParticipationCount > 0; }
         set { ParticipationCount = (short)(value ? 1 : 0); }
      }

      public char RecordStatusCode { get; set; } = ' ';

      public ActivityPeriodRatingInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         OrganizationId = Session.OrganizationId;
         PeriodId = String.Empty;
         ReferenceDate = DateTimeOffset.Now;
         GroupId = String.Empty;
         GroupEventId = String.Empty;
         ProgramId = String.Empty;
         ContentTemplateId = String.Empty;
         ContentThreadId = String.Empty;
         ContentVersionNo = 0;
         ContentIdentifierId = String.Empty;
         EntityId = String.Empty;
         RatedValue = 0;
         RatingTypeNo = 2;
         RatingWeight = 0;
         Alias = String.Empty;
         StatusNo = (short)ObjectStatus.Active;
         ResultTypeNo = 1;
         ParticipationCount = 1;
         ParticipantRoleNo = (short)ReferenceBaseType.Participant;
         ParticipantName = String.Empty;
         RatedValue = 0;
         RecordStatusCode = RECORD_NEW;
      }
   }

}
