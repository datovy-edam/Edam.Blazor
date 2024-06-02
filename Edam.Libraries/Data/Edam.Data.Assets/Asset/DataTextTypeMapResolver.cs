using System;
using Edam.Data.AssetManagement;
namespace Edam.Data.Asset
{
   public delegate DataTextTypeInfo DataTextTypeMapResolverDelegate(
      IDataElement element, DataTextTypeInfo defaultType);
}
