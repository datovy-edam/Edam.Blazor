using System;

// -----------------------------------------------------------------------------
// 2012-04-28 The following code was copied verbatum from the Open Knowledge -
// KIFv3r0 Framework Project.

namespace Edam.Application
{

   /// <summary>Provided to support Kif applications exception handling
   /// </summary>
   public class ApplicationException : Exception
   {
      public ApplicationException(String message) : base(message)
      {
      }
   }  // end of ApplicationException

}  // end of Edam.Application

