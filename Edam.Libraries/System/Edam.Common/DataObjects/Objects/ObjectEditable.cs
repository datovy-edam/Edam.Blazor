using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Objects
{
   public enum ObjectEditable
   {
      Unknown = 0,
      CanInsert = 1,
      CanEdit = 2,
      CanDelete = 3,
      AutoNumber = 4,
      AutoID = 5
   }
}
