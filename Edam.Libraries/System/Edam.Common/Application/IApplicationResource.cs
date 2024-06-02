using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Application
{

   public interface IApplicationResource
   {
      string GetString(string key);
      string GetLocalString(string key);
   }

}
