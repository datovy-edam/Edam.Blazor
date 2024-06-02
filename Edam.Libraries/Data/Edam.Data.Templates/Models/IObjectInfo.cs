using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Models
{

   interface IObjectInfo
   {
      ResourceType Type { get; set; }
      Int64? ObjectNo { get; set; }
      String Name { get; set; }
      String Description { get; set; }
      String Help { get; set; }
      String Sample { get; set; }
   }

}
