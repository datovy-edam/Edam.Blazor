using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Models;
using Edam.Serialization;
using System.Text.Json.Serialization;

namespace Edam.DataObjects.ReferenceData
{

   public class ReferenceDataTemplateInfo : ReferenceDataTemplateBaseInfo
   {

      public ReferenceDataTemplateInfo() : base()
      {

      }

      /// <summary>
      /// Element Group helps to keep a dictionary of elements in the group...
      /// </summary>
      public ElementNodeGroup ElementNodeGroup { get; set; }

      public static ReferenceDataTemplateInfo FromJson(string jsonText)
      {
         return JsonSerializer.Deserialize<ReferenceDataTemplateInfo>(jsonText);
      }
   }

}
