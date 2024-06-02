using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Application
{

   public interface IAppSettings
   {
      string GetDocumentPostfix();
      string GetTypePostfix();
   }

}
