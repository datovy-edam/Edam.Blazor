using System;

// -----------------------------------------------------------------------------
// Copied from Kif Library v5r0

namespace Edam.DataObjects
{
   public interface IDataObjectBase
   {
      String RecordGuid { get; set; }
      String ObjectId { get; set; }
      Labels.LabelInfo Label { get; set; }

      void ClearFields();
   }
}
