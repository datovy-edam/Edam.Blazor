using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerGraph.Connector
{

   public class TigerGraphException : Exception
   {
      string _code = "";

      public TigerGraphException() { }

      public TigerGraphException(string message) : base(message) { }

      public TigerGraphException(string message, string code) : base(message)
      {
         _code = code;
      }

      public TigerGraphException(string message, string code, Exception inner) :
         base(message, inner)
      {
         _code = code;
      }

      public string Code
      {
         //get => _code; set => _code = value; }
         get { return _code; }
         set { _code = value; }
      }
   }

}
