using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.ReferenceData;

namespace Edam.DataObjects.Models
{

   /// <summary>
   /// This is the [root] node of an Element based model... Below the root you
   /// will find a collection of child nodes (see ElementNodeInfo).
   /// </summary>
   public class ElementInfo : IElementInfo
   {

      public String Title { get; set; }
      public String Description { get; set; }
      public String Name { get; set; }
      public ResourceType Type { get; set; }

      public List<ElementNodeInfo> Items { get; set; }

      public ElementInfo()
      {
         Items = new List<ElementNodeInfo>();
      }

      public Boolean Validate()
      {
         Boolean isvalid = true;
         return isvalid;
      }

   }

}
