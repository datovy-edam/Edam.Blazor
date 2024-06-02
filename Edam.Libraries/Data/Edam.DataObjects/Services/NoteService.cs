using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Text;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.DataObjects.Notes;
using Edam.DataObjects.Requests;
using Edam.DataObjects.References;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Services;
using Edam.DataObjects.Dashboards;

namespace Edam.DataObjects.Services
{

   public class NoteService
   {

      public static readonly string NOTE = "Note";

      #region -- 4.00 - Notes Find

      public static async Task<RequestResponseInfo<List<NoteInfo>>>
         GetNotes(
            String sessionId, LocaleLanguage? language, 
            ReferenceType? referenceType, String organizationId,
            String referenceId, String alias, String description,
            DateTime? referenceDate)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         LocaleLanguage lang = language.HasValue ? language.Value :
            Edam.Application.Session.Language;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.Language, lang.ToString());
         b.Add(QueryStringTag.ReferenceType, (Int16)referenceType);
         b.Add(QueryStringTag.OrganizationId, oid);
         b.Add(QueryStringTag.ReferenceId, referenceId);
         b.Add(QueryStringTag.Alias, alias);
         b.Add(QueryStringTag.Description, description);
         b.Add(QueryStringTag.ReferenceDate, referenceDate);

         RequestResponseInfo<List<NoteInfo>> data = 
            new RequestResponseInfo<List<NoteInfo>>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = NOTE + "/GetNote" + b.ToString();
            await client.GetAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<List<NoteInfo>>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      public static async Task<RequestResponseInfo<List<NoteInfo>>>
         GetNotesRecord(
            String sessionId, LocaleLanguage? language,
            ReferenceType? referenceType, String organizationId,
            String referenceId, String alias, String description,
            DateTime? referenceDate)
      {
         return await GetNotes(sessionId, language, referenceType,
            organizationId, referenceId, alias, description, referenceDate);
      }

      public static async Task<RequestResponseInfo<List<NoteInfo>>>
         GetNotesRecord(
            ReferenceType? referenceType,
            String referenceId, String alias, String description,
            DateTime? referenceDate)
      {
         return await GetNotes(null, null, referenceType,
            null, referenceId, alias, description, referenceDate);
      }

      #endregion
      #region -- 4.00 - Note Post

      public static async Task<RequestResponseInfo<String>> PostNote(
         String sessionId, ReferenceType referenceType, NoteInfo note)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(note.OrganizationId)) ?
            Edam.Application.Session.OrganizationId : 
            note.OrganizationId;
         note.OrganizationId = oid;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.ReferenceType, (Int16)referenceType);

         RequestResponseInfo<String> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = NOTE + "/PostNote" + b.ToString();
            await client.PostAsync<NoteInfo>(url, note);
            data.ResponseData = client.GetDataObjectFromJson<String>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      #endregion
      #region -- 4.00 - Note Delete

      public static async Task<RequestResponseInfo<String>> DeleteNote(
         String sessionId, ReferenceType referenceType, String organizationId, 
         String referenceId, String noteId, ObjectStatus status, 
         NoteOption option)
      {
         String sid = (String.IsNullOrWhiteSpace(sessionId)) ?
            Edam.Application.Session.SessionId : sessionId;
         String oid = (String.IsNullOrWhiteSpace(organizationId)) ?
            Edam.Application.Session.OrganizationId : organizationId;
         String rid = (String.IsNullOrWhiteSpace(referenceId)) ?
            String.Empty : referenceId;
         String nid = (String.IsNullOrWhiteSpace(noteId)) ?
            String.Empty : noteId;

         QueryStringBuilder b = new QueryStringBuilder();
         b.Add(QueryStringTag.SessionId, sid);
         b.Add(QueryStringTag.ReferenceType, (Int16)referenceType);
         b.Add(QueryStringTag.OrganizationId, organizationId);
         b.Add(QueryStringTag.ReferenceId, referenceId);
         b.Add(QueryStringTag.NoteId, noteId);
         b.Add(QueryStringTag.Status, (Int16)status);
         b.Add(QueryStringTag.Option, (Int16)option);

         RequestResponseInfo<String> data = new RequestResponseInfo<string>();
         WebApiClient client = WebApiClientInfo.GetWebApiClient();
         ResultLog results = client.Results;
         try
         {
            var url = NOTE + "/PostNoteStatus" + b.ToString();
            await client.PostAsync(url);
            data.ResponseData = client.GetDataObjectFromJson<String>();
            results.Succeeded();
            if (results.Success)
            {

            }
         }
         catch (Exception ex)
         {
            client.Results.Failed(ex);
            data.Results.Failed(ex);
         }
         finally
         {
            client.Dispose();
         }
         return data;
      }

      #endregion

   }

}
