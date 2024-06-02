using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.DataCodes
{

   public interface IDataCode
   {
      String GroupId { get; set; }
      String CodeId { get; set; }
      String Value { get; set; }
      String HelpText { get; set; }
      Objects.ObjectStatus Status { get; set; }
      Boolean Selected { get; set; }
   }

}
