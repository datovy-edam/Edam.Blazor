using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.Models
{

   public enum ReferenceDataOption
   {
      Unknown = 0,
      Template = 1,
      Groups = 2,
      MapLinks = 3,
      TemplateAndGroups = 4,
      TemplateAndGroupAndMapLinks = 5,
      TestCodes = 100,
      AllGroups = 32767
   }

}
