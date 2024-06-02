using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using ReqResp = Edam.DataObjects.Requests;
using Activity = Edam.DataObjects.Activities;
//using Edam.DataObjects.Notifications;
using Edam.DataObjects;

namespace Edam.Help.Notifications
{
   /*
   public class NotifyRecipientHelper
   {

      #region -- 1.0 Fields and Properties

      #endregion
      #region -- 4.0 Notify Activity Rating Participants

      /// <summary>
      /// Notify about Activity Rating...
      /// </summary>
      /// <param name="sessionId">session id</param>
      /// <param name="language">notify using given local</param>
      /// <param name="requestId">request id</param>
      /// <param name="record">artivity rating participant info</param>
      /// <param name="request">request</param>
      /// <param name="roles">roles</param>
      /// <returns>request response with updated record id (that was modified) 
      /// as a string is returned</returns>
      public static ReqResp.RequestResponseInfo<String> Notify(
         String sessionId, LocaleLanguage language, String requestId,
         Activity.ActivityRatingParticipantsInfo record,
         NotificationType request,
         List<Edam.DataObjects.References.ReferenceBaseType> roles)
      {
         ReqResp.RequestResponseInfo<String> 
            response = null;

         try
         {
            response = Activity.ActivityProgramRecord.UpdateResponse(
               sessionId, record);

            if (!response.Success || request == NotificationType.Unknown)
               return response;
            if (request == NotificationType.RequestAcceptance &&
                String.IsNullOrEmpty(record.DocumentSignature.SignatorEmail))
               return response;

            // do we need to prepare a request and notify via email? if so doit.
            NotificationInfo n = new NotificationInfo();

            n.CreatedDate = DateTime.Now;
            n.OrganizationId = record.Rating.OrganizationId;
            n.ReferenceId = record.Rating.ReferenceId;
            n.ReferenceDate = record.Rating.ReferenceDate;
            n.Alias = record.Rating.Alias;
            n.ExpirationDate = DateTime.Now.AddDays(10);
            n.RecipientsId = record.Rating.ReferenceId;
            n.RecipientsRole =
               DataObjects.References.ReferenceBaseType.Participant;
            n.ShouldEmail = true;
            n.ShouldRead = true;
            n.ShouldText = false;
            n.State = DataObjects.Objects.ObjectState.Submitted;
            n.Type = request;

            NotificationMessageInfo m = new NotificationMessageInfo();

            m.OrganizationId = n.OrganizationId;
            m.ReferenceId = n.ReferenceId;
            m.Relevance = DataObjects.Objects.ObjectRelevance.Important;
            m.Subject = Application.Resources.
               ApplicationStrings.NotifyActivityRatingEvent;
            m.MessageText = Requests.RequestProcessor.
               GetActivityEvaluationReviewRequestText();

            List<NotificationRecipientInfo> recipients;
            NotificationRecipientInfo recipient = null;

            if (request == NotificationType.RequestAcceptance)
            {
               recipient = new NotificationRecipientInfo();
               recipient.Email = record.DocumentSignature.SignatorEmail;
               recipient.Name = record.DocumentSignature.SignatorName;
               recipient.EmailedDate = DateTime.Now;
               n.Recipients.Add(recipient);
            }
            else
            {
               recipients = NotificationInfo.GetRecipients(
                  record.Participants, roles);
               n.Recipients = recipients;
            }

            n.Messages.Add(m);

            var notifyResponse =
               NotificationRecord.UpdateNotificationRecord(
                  sessionId, n, NotifyRequestOption.Unknown);
            if (notifyResponse.Success)
            {
               String email = String.Empty;
               if (recipient != null)
                  email = recipient.Email;

               if (!String.IsNullOrEmpty(email))
               {
                  Requests.RequestProcessor p = new Requests.RequestProcessor(
                     String.Empty, email);
                  if (!p.SendEMail(n))
                     response.Results.Add(
                        Diagnostics.EventCode.RequestProcessorEmailSendFailed,
                        "Failed to send Email for Evaluation Review Request (" +
                        record.Rating.EvaluationId + ").");
               }
            }
            else
               response.Results.Add(
                     Diagnostics.EventCode.FailedToInsertNotification,
                     "Failed to insert Notification for Evaluation Review Request (" +
                     record.Rating.EvaluationId + ").");

         }
         catch(Exception ex)
         {
            if (response == null)
               response = new ReqResp.RequestResponseInfo<string>();
            response.Results.Failed(ex);
         }

         return response;
      }

      #endregion
      #region -- 4.0 Notify Joining Event

      /// <summary>
      /// Insert notification and send email...
      /// </summary>
      /// <param name="sessionId">session Id.</param>
      /// <param name="participant">participant details</param>
      /// <returns>results log is returned</returns>
      public static Diagnostics.ResultLog Notify(
         String sessionId,
         Activity.ActivityParticipantsInfo activity)
      {
         Diagnostics.ResultLog results = new Diagnostics.ResultLog();

         try
         {
            NotificationInfo n = new NotificationInfo();

            n.CreatedDate = DateTime.Now;
            n.OrganizationId = activity.OrganizationId;
            n.ReferenceId = activity.ReferenceId;
            n.ReferenceDate = DateTime.Now;
            n.Alias = String.Empty;
            n.ExpirationDate = DateTime.Now.AddDays(10);
            n.RecipientsId = String.Empty;
            n.RecipientsRole =
               DataObjects.References.ReferenceBaseType.Participant;
            n.ShouldEmail = true;
            n.ShouldRead = true;
            n.ShouldText = false;
            n.State = DataObjects.Objects.ObjectState.Submitted;
            n.Type = NotificationType.JoinedActivity;

            NotificationMessageInfo m = new NotificationMessageInfo();

            m.OrganizationId = n.OrganizationId;
            m.ReferenceId = n.ReferenceId;
            m.Relevance = DataObjects.Objects.ObjectRelevance.Informative;
            m.Subject = Application.Resources.
               ApplicationStrings.NotifyActivityJoinEvent;
            m.MessageText = Requests.RequestProcessor.
               GetActivityJoinedNotificationText(n.OrganizationId);

            Edam.DataObjects.Entities.ParticipantInfo participant;
            if (activity.Participants != null &&
                activity.Participants.Count > 0)
               participant = activity.Participants[0];
            else
               participant = new DataObjects.Entities.ParticipantInfo();

            NotificationRecipientInfo recipient = 
               new NotificationRecipientInfo();
            recipient.Email = participant.Email;
            recipient.EmailedDate = DateTime.Now;
            recipient.EntityId = participant.EntityId;
            recipient.ParticipantId = participant.ParticipantId;
            recipient.PhoneNumber = participant.PhoneNumber;
            recipient.WasEmailed = true;

            List<NotificationRecipientInfo> recipients =
               new List<NotificationRecipientInfo>();
            n.Recipients.Add(recipient);
            n.Messages.Add(m);

            var notifyResponse =
               NotificationRecord.UpdateNotificationRecord(sessionId, n,
                  NotifyRequestOption.ReturnRecipients);
            if (notifyResponse.Success)
            {
               String email = String.Empty;
               if (notifyResponse.ResponseData.Items.Count > 0)
                  email = notifyResponse.ResponseData.Items[0].Email;
               else
                  email = recipient.Email;
               if (!String.IsNullOrEmpty(email))
               {
                  Requests.RequestProcessor p = new Requests.RequestProcessor(
                     String.Empty, email);

                  if (!p.SendEMail(n))
                     results.Add(
                        Diagnostics.EventCode.RequestProcessorEmailSendFailed,
                        "Failed to send Email for Joining an Event (" +
                        participant.EntityId + ").");
                  else
                     results.Succeeded();
               }
            }
            else
               results.Add(Diagnostics.EventCode.FailedToInsertNotification,
                  "Failed inserting notificaiton while Joining an Activity (" +
                  participant.ReferenceId + "," + 
                  participant.ParticipantId + ")");

         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }

         return results;
      }

      #endregion

   }
   */
}
