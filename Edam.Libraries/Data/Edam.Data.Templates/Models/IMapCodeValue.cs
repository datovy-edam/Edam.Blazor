using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{
   /// <summary>
   /// A combo-box will have a single code (CodeId in here) to index all 
   /// possible values, but a map may need multiple matching links of different 
   /// parents to allow the proper selection of a selected code-value.
   /// If you have multiple parents influencing the selection of a code then
   /// get your Values list and assign a "CodeId" starting from 0 -> n to allow
   /// the UI to support a simple selection Code-Value list.
   /// </summary>
   public interface IMapCodeValue
   {
      List<IMapLink> Links { get; set; }
      String CodeId { get; set; }
      String Value { get; set; }
      String HelpText { get; set; }
      Objects.ObjectStatus Status { get; set; }
      Boolean Selected { get; set; }
   }

}
