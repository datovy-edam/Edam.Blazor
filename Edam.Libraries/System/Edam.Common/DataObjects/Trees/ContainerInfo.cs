using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Trees
{

   public class ContainerInfo : ITreeContainer
   {
      public string ContainerId { get; set; } = String.Empty;
      public string Description { get; set; }
      public string ContentType { get; set; }
      public string Catalog { get; set; }
      public string StatusCode { get; set; }
   }

}
