using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.B2b.Edi
{

   public class EdiDatabaseItem
   {
      public string DatabaseName { get; set; }
      public string SchemaName { get; set; }
      public string TableName { get; set; }
      public string ColumnName { get; set; }
      public string ColumnValue { get; set; }
   }

   public class EdiDatabase
   {

      public void AddSegment(EdiSegmentInfo segment)
      {
         
      }

   }

}
