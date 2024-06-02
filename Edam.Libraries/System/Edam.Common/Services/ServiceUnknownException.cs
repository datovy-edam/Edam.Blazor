using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace SccGov.Services
{

   public class ServiceUnknownException : System.Exception
   {
      public ServiceUnknownException(String message) : base(message)
      {

      }
   }

}

