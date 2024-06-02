using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{
   public interface IElementGroupItem: IElementInfo
   {
      List<ElementGroupItem> Items { get; set; }
   }
   public class ElementGroupItem : IElementGroupItem
   {

      public List<ElementGroupItem> Items { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public string Name { get; set; }
      public ResourceType Type { get; set; }

      public bool Validate()
      {
         return true;
      }
   }
}
