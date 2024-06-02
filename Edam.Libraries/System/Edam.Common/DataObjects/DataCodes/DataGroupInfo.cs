using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.DataCodes
{
   public class DataGroupInfo
   {
      public int GroupNo { get; set; }
      public string GroupName { get; set; }
      public List<object> Items { get; set; }
      public DataGroupInfo()
      {
         Items = new List<object>();
      }
   }
}
