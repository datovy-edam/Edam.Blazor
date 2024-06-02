using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetSchema;

namespace Edam.Data.AssetManagement
{

   public interface IResourceContext
   {
      String OrganizationDomainId { get; }
      Uri RootDomainUri { get; }
      List<DataDomain> DbDomains { get; set; }
      List<DataTerm> DbTerms { get; set; }
      AssetDataElementList DbElements { get; set; }
      AssetDataElementList GetTypeChildren(IDataElement element);
      AssetDataElementList GetUriTypes(string namespaceText);
      ElementTypeInfo FindType(String typeName);
      NamespaceInfo FindNamespace(string namespaceText);
   }

}
