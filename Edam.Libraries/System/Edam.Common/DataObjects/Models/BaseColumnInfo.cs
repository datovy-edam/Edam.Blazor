using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Models
{

   public class BaseColumnInfo
   {
      public string ColumnName { get; set; }
      public string Title { get; set; }
      public int Width { get; set; }
      public virtual ObjectValueType ValueType { get; set; }
      public bool Visible { get; set; }

      public Object EditControl { get; set; }
      public ModelColumnControlType EditControlType { get; set; }
   }

}
