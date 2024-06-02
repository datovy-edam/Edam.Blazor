using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.Assets.Asset;
using Edam.Diagnostics;

namespace Edam.Data.AssetSchema
{

   public interface IResourceData
   {
      IResultsLog Save(
         AssetDataElementList assets, NamespaceInfo ns, string domainName,
         AssetType type);
   }

   public interface IAssetElement
   {
      object AssetObject { get; }
      DateTime? LastUpdateDate { get; set; }
      int AssetNo { get; set; }
      Int16? AssetStatus { get; set; }
      int ElementNo { get; set; }

      List<String> Annotation { get; set; }

      string EntityQualifiedNameText { get; set; }
      QualifiedNameInfo EntityQualifiedName { get; set; }

      string ElementQualifiedNameText { get; set; }
      QualifiedNameInfo ElementQualifiedName { get; set; }

      string Domain { get; set; }
      string Namespace { get; set; }
      string DefaultValue { get; set; }
      string FixedValue { get; set; }
      string CommentText { get; set; }
      string SampleValue { get; set; }
      bool IsNillable { get; set; }
      int? MinOccurrence { get; set; }
      int? MaxOccurrence { get; set; }
      DataElementKind Kind { get; set; }
      string Tags { get; set; }

      string OriginalName { get; set; }
      string OriginalDataType { get; set; }

      string Occurs { get; set; }
      List<QualifiedNameInfo> QualifiedTypeNames { get; }
      string TypeName { get; }
      string DataType { get; set; }
      decimal? Length { get; }

      int? Precision { get; set; }
      int? Scale { get; set; }

      bool IsType { get; }

      ConstraintType KeyType { get; set; }
      ConstraintType AutoGenerateType { get; set; }
      ElementType ElementType { get; set; }
      AssetProcessInfo ProcessInstructionsBag { get; set; }
      string GetFullPath();
      List<AssetDataElement> GetAttributes();
   }

}
