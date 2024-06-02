using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace SccGov.Services
{
   
   /// <summary>
   /// Service details, configuration or its identity are unknown and can't be
   /// resolved.
   /// </summary>
   public class ServiceConfigurationException : System.Exception
   {
      public ServiceConfigurationException(String message) : base(message)
      {

      }
   }

}
