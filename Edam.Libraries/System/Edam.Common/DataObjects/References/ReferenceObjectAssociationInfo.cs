using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Copied from Kif Library v5r0

namespace Edam.DataObjects.References
{

   public class ReferenceObjectAssociationInfo<T> : ReferenceObjectInfo
   {
      public T AssociationType { get; set; }
   }

}
