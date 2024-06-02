using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.B2b.Edi
{

   public class EdiInstance
   {

      public List<EdiSegmentList> Instances { get; set; } = 
         new List<EdiSegmentList>();
      public EdiSegmentList Items { get; set; } = new EdiSegmentList();

      /// <summary>
      /// Instances to JSON.
      /// </summary>
      /// <returns>Instances JSON text is returned</returns>
      public string ToJson()
      {
         return EdiInstanceJson.ToJson(Instances);
      }

   }

}
