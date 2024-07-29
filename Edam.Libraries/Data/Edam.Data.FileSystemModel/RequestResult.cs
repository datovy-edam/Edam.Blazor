using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.FileSystemModel
{

   public class RequestResult : ResultLogBase
   {

   }

   public class RequestResult<T> : ResultLogBase
   {
      private T? _value;
      public T? ResultValue
      {
         get { return _value; }
         set
         {
            ResultValueObject = value;
            _value = value;
         }
      }
   }

}
