using System;

// -----------------------------------------------------------------------------
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;

namespace Edam.Data.AssetUseCases
{

    public class AssetUseCaseElement
    {
        public string NamespaceUri { get; set; }
        public int UseCaseNo { get; set; }
        public int ElementNo { get; set; }
        public string Name { get; set; }
        public string ElementPath { get; set; }
        public string SampleValue { get; set; }
        public string VersionId { get; set; }

        public AssetUseCase UseCase { get; set; }
        public AssetDataElement Element { get; set; }
    }

}
