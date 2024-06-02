using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.DataObjects.Models;
using Edam.DataObjects.DataCodes;
using Edam.Data.AssetSchema;
using Edam.Serialization;

namespace Edam.DataObjects.Assets
{

   public class PropertiesBag : IPropertiesBag<ElementNodeInfo>, IPropertiesBag
   {
      [IgnoreDataMember]
      public object AssetTemplateInstance
      {
         get { return AssetTemplate; }
      }

      public List<IDataCode> DataCodes { get; set; }
      public ElementNodeInfo AssetTemplate { get; set; }

      public void SetTemplate(object node)
      {
         AssetTemplate = node as ElementNodeInfo;
      }

      public string? ToJsonText(object propertiesBag)
      {
         PropertiesBag? bag = propertiesBag as PropertiesBag;
         if (bag == null)
         {
            return null;
         }
         return JsonSerializer.Serialize<PropertiesBag>(bag);
      }

      public IPropertiesBag FromJsonText(string jsonText)
      {
         return JsonSerializer.Deserialize<PropertiesBag>(jsonText);
      }

   }

}
