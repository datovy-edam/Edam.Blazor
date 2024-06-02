using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Text;

namespace Edam.B2b.Edi
{

   public class EdiInstanceJson
   {

      public static string ToJson(EdiSegmentList items)
      {
         JsonBuilder builder = new JsonBuilder();
         string jtext = null;
         foreach (var item in items)
         {

         }
         return jtext;
      }

      public static string ToJson(List<EdiSegmentList> segments)
      {
         string jtext = null;
         foreach (var item in segments)
         {
            ToJson(item);
         }
         return jtext;
      }

   }

}
