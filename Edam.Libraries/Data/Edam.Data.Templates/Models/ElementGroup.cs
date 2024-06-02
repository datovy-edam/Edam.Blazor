using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.Models
{

   /// <summary>
   /// Manage the Collection of (Group) of Element Nodes
   /// </summary>
   public class ElementGroup
   {
      public Dictionary<string,ElementNodeInfo> GlobalDictionary { get; set; }
      public ElementGroupItem ElementGroupItem { get; set; }

      public ElementNodeInfo GetNode(string name)
      {
         if (GlobalDictionary.TryGetValue(name, out ElementNodeInfo value))
         {
            return value;
         }
         return null;
      }

      public ElementNodeInfo GetNode(long TemplateNo)
      {
         foreach(var i in GlobalDictionary)
         {
            if (i.Value.TemplateNo == TemplateNo)
            {
               return i.Value;
            }
         }
         return null;
      }

      public ElementNodeInfo GetNode(ElementGroupItem item)
      {
         ElementNodeInfo node = null;
         if (item == null || item.Name == null ||
            !GlobalDictionary.TryGetValue(item.Name, out node))
            return null;
         return node;
      }

   }

}
