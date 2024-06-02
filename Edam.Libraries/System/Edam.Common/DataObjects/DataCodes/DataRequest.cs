using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.DataCodes
{

   public enum DataRequest
   {
      Unknown = 0,
      CheckDuplicates = 1,
      Exists = 2,
      Select = 3,
      InsertUpdate = 4,
      Delete = 5,
      Execute = 6,
      UpdateRecordStatus = 7
   }

}
