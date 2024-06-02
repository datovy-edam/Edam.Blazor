using System;

// -----------------------------------------------------------------------------
// Copied from Kif Library v5r0

namespace Edam.DataObjects.References
{

   /// <summary>
   /// Reference Info to hold the type and it's ID.
   /// </summary>
   public class ReferenceInfo
   {
      public References.ReferenceType ReferenceType { get; set; }
      public DateTime? ReferenceDate { get; set; }
      public String ReferenceId { get; set; }
      public String ReferenceDescription { get; set; }

      public ReferenceInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         ReferenceType = References.ReferenceType.Unknown;
         ReferenceId = String.Empty;
         ReferenceDescription = String.Empty;
         ReferenceDate = DateTime.Now;
      }
   }

   public class ReferenceItem<T> : ReferenceInfo
   {
      public T Item { get; set; }
      public ReferenceItem(ReferenceInfo reference = null) : base()
      {
         ClearFields();
         Copy(reference);
      }
      public void Copy(ReferenceInfo reference)
      {
         if (reference == null)
            return;
         ReferenceType = reference.ReferenceType;
         ReferenceId = reference.ReferenceId;
         ReferenceDescription = reference.ReferenceDescription;
         ReferenceDate = reference.ReferenceDate;
      }
      public new void ClearFields()
      {
         base.ClearFields();
      }
   }

}
