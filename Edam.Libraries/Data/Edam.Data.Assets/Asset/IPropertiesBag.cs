using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.DataCodes;
using Edam.Data.AssetSchema;

namespace Edam.Data.Asset
{

   public interface IPropertiesBag
   {
      List<IDataCode> DataCodes { get; set; }
      object AssetTemplateInstance { get; }
      void SetTemplate(object template);
      string ToJsonText(object propertiesBag);
      IPropertiesBag FromJsonText(string jsonText);
   }

   public interface IPropertiesBag<T> : IPropertiesBag
   {
      T AssetTemplate { get; set; }
   }

}
