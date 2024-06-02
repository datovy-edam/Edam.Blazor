using System;
using System.Collections.Generic;
using System.Text;

using Edam.Data.AssetManagement.Resources;

namespace Edam.Data.Asset
{
   public delegate DataTypeInfo TypeResolverDelegate(IDataElement element);
   public delegate ElementTypeInfo ElementTypeResolverDelegate(string name);
}
