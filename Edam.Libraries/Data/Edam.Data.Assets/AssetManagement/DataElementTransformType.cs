using System;
namespace Edam.Data.AssetManagement
{
   public enum DataElementTransformType
   {
      Unknown = 0,

      // bring in all reference type fields
      Merge = 1,

      // treat as a Foreign Key
      Reference = 2
   }
}
