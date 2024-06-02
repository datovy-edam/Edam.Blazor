using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.DataObjects.Requests
{

   /// <summary>
   /// Request Response...
   /// </summary>
   public class RequestResponseInfo : IRequestBaseResponse
   {

      private ResultLog m_Results =
         new Diagnostics.ResultLog();
      public String SessionId { get; set; }
      public String RequestId { get; set; }
      public RequestStatus Status { get; set; }
      public ResultLog Results
      {
         get { return m_Results; }
         set
         {
            if (value == null)
            {
               m_Results.Clear();
               return;
            }
            m_Results = value;
         }
      }
      public String Message { get; set; }
      public Boolean HtmlMessage { get; set; }

      public Boolean Success
      {
         get { return m_Results.Success; }
         set
         {
            if (value)
               m_Results.Succeeded();
         }
      }

      /// <summary>
      /// Initialize object using given resutls.
      /// </summary>
      /// <param name="results">results to initialize object with</param>
      public RequestResponseInfo(ResultLog results = null)
      {
         if (results != null)
            m_Results.Copy(results);
         if (m_Results != null)
         {
            m_Results.Succeeded();
            Success = m_Results.Success;
            Status = m_Results.Success ? RequestStatus.Completed :
               RequestStatus.Failed;
         }
      }

      /// <summary>
      /// Set fields/properties to their default values...
      /// </summary>
      public void ClearFields()
      {
         SessionId = String.Empty;
         RequestId = String.Empty;
         Status = RequestStatus.Unknown;
         Message = String.Empty;
         HtmlMessage = false;
         m_Results.Clear();
      }

      public static RequestResponseInfo FailedResponse(Exception ex)
      {
         RequestResponseInfo r = new RequestResponseInfo();
         r.Status = RequestStatus.Failed;
         r.Success = false;
         r.Results = new ResultLog();
         if (ex != null)
            r.Results.Failed(ex);
         else
            r.Results.Failed("Failed with unknown Exception");
         return r;
      }

      public static RequestResponseInfo FailedResponse(String message)
      {
         RequestResponseInfo r = new RequestResponseInfo();
         r.Status = RequestStatus.Failed;
         r.Success = false;
         r.Results = new ResultLog();
         if (message != null)
            r.Results.Failed(message);
         else
            r.Results.Failed("Failed with unknown Message");
         return r;
      }

      public static RequestResponseInfo<T> FailedResponse<T>(Exception ex)
      {
         RequestResponseInfo<T> r = new RequestResponseInfo<T>();
         r.Status = RequestStatus.Failed;
         r.Success = false;
         r.Results = new ResultLog();
         if (ex != null)
            r.Results.Failed(ex);
         else
            r.Results.Failed("Failed with unknown Exception");
         return r;
      }

      public static RequestResponseInfo<T> FailedResponse<T>(String message)
      {
         RequestResponseInfo<T> r = new RequestResponseInfo<T>();
         r.Status = RequestStatus.Failed;
         r.Success = false;
         r.Results = new ResultLog();
         if (message != null)
            r.Results.Failed(message);
         else
            r.Results.Failed("Failed with unknown Message");
         return r;
      }

   }

   /// <summary>
   /// Request Response with additional (T) data as specified by caller.
   /// </summary>
   /// <typeparam name="T">type of additional data</typeparam>
   public class RequestResponseInfo<T> :
      RequestResponseInfo, IRequestResponse<T>
   {
      public T ResponseData { get; set; }

      public RequestResponseInfo(ResultLog results = null) :
         base(results)
      {

      }

      public void Failed(IResultsLog result)
      {
         Success = false;
         ResponseData = default(T);
         Status = RequestStatus.Failed;
         Results.Copy(result);
      }

      public void Failed(String message)
      {
         Success = false;
         ResponseData = default(T);
         Status = RequestStatus.Failed;
         Results.Failed(message);
      }

      public new void ClearFields()
      {
         base.ClearFields();
      }
   }

}
