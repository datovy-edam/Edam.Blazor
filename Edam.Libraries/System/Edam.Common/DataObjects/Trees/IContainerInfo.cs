using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Trees
{

   public interface ITreeContainer
   {
      string ContainerId { get; set; }
      string Description { get; set; }
      string ContentType { get; set; }
      string Catalog { get; set; }
      string StatusCode { get; set; }
   }

}
