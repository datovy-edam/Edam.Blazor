using System;
using System.Collections.Generic;

using Edam.Data.AssetManagement;

namespace Edam.Data.Asset
{
   
   public interface IResourceCallHelpers
   {
      // delegates
      BaseTypeResolverDelegate DataBaseTypeResolver { get; set; }
      TypeResolverDelegate DataTypeResolver { get; set; }
      NamespaceResolverDelegate DataNamespaceResolver { get; set; }
      DataTextMapResolverDelegate DataTextMapResolver { get; set; }
      DataTransformResolverDelegate DataTransformResolver { get; set; }
      DomainResolverDelegate DataDomainResolver { get; set; }

      // call-back funcs
      Func<IDataElement, List<IDataElement>> DataFindKeyCandidates { get; set; }
      Func<IDataElement, List<IDataElement>, String, List<IDataElement>>
         DataGetChildrenOfChildren { get; set; }
   }

}
