using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   public class ElementGroupInfo
   {

      public String Title { get; set; }
      public List<ElementInfo> Items { get; set; }

      public static ElementGroup ToElementGroup(List<ElementInfo> items)
      {
         ElementGroup group = new ElementGroup();
         group.GlobalDictionary = new Dictionary<string, ElementNodeInfo>();
         group.ElementGroupItem = new ElementGroupItem
         {
            Items = new List<ElementGroupItem>()
         };
         foreach (var i in items)
         {
            var eg = new ElementGroupItem
            {
               Title = i.Title,
               Description = i.Description,
               Name = i.Name,
               Type = i.Type,
               Items = new List<ElementGroupItem>()
            };
            group.ElementGroupItem.Items.Add(eg);
            foreach(var c in i.Items)
            {
               var ceg = new ElementGroupItem
               {
                  Title = c.Title,
                  Description = c.Description,
                  Name = c.Name,
                  Type = c.Type,
                  Items = new List<ElementGroupItem>()
               };
               group.GlobalDictionary.Add(c.Name, c);
               eg.Items.Add(ceg);
            }
         }
         return group;
      }
   }

}
