using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.DataObjects.ViewModels
{

   public interface IMenuProcess : IMenuItem
   {
      Action Exec { get; set; }
   }

}
