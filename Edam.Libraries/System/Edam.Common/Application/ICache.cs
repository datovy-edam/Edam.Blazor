using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.Application
{

   public interface ICache
   {
      void Set<T>(string name, T item, string description = null);
      D Get<D>(string name);
   }

}
