using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;
using Edam.DataObjects.Models;
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.ReferenceData
{

   /// <summary>
   /// Define Reference Data Code sets, lookups or value set validation...
   /// </summary>
   public class ReferenceDataCodeInfo
   {
      public ResourceType Type { get; set; }
      public String Name { get; set; }
      public Object Data { get; set; }

      public ReferenceDataCodeInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         Type = ResourceType.Unknown;
         Name = String.Empty;
         Data = null;
      }

   }

}
