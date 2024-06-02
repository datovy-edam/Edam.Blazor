using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;

namespace Edam.DataObjects.Models
{

   public class ContextInfo
   {
      public NamespaceInfo? Namespace { get; set; }
      public Int16? GroupNo { get; set; }
      public void ClearFields()
      {
         GroupNo = null;
         Namespace = null;
      }
   }

}
