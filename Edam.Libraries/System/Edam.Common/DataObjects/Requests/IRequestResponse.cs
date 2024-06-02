using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.DataObjects.Requests
{

   public interface IRequestBaseResponse
   {
      String SessionId { get; set; }
      String RequestId { get; set; }
      RequestStatus Status { get; set; }
      ResultLog Results { get; set; }
      Boolean Success { get; set; }
      String Message { get; set; }
      Boolean HtmlMessage { get; set; }
   }

   public interface IRequestResponse<T> : IRequestBaseResponse
   {
      T ResponseData { get; set; }
   }

}
