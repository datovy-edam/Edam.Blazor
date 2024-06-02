using System;
using System.Collections.Generic;
using System.Text;

using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;
using Edam.Data.Asset;

namespace Edam.Data.Asset
{

   public enum DataElementInclusionType
   {
      Unknown = 0,
      Include = 1,
      Exclude = 2,
      Selected = 3,
      Unselected = 4
   }

   public interface IDataElement
   {
      string AnnotationText { get; }
      string CommentText { get; }

      DateTime LastUpdateDate { get; set; }
      int AssetNo { get; set; }
      int OrdinalNo { get; set; }
      int ElementNo { get; set; }
      string Root { get; set; }
      string Domain { get; set; }
      string ResourceId { get; }
      string NamespaceText { get; }
      string ElementId { get; set; }
      string DefaultValue { get; set; }
      string Type { get; set; }
      QualifiedNameInfo TypeQualifiedName { get; set; }
      int TypeNo { get; set; }
      string ElementUri { get; set; }
      string ElementName { get; set; }
      string RealName { get; set; }
      string AlternateName { get; set; }
      QualifiedNameInfo ElementQualifiedName { get; set; }
      string ElementDataType { get; set; }
      string ElementPath { get; set; }
      string EntityName { get; set; }
      ElementSequence ElementSequence { get; set; }
      string Description { get; set; }
      string PropertiesBagText { get; set; }
      int StatusNo { get; set; }
      short ValueTypeNo { get; set; }
      int? MinLength { get; set; }
      int? MaxLength { get; set; }
      int? MinOccurrence { get; set; }
      int? MaxOccurrence { get; set; }
      bool? Nillable { get; set; }
      DateTime ReferenceDate { get; set; }
      string Tags { get; set; }
      string Guid { get; set; }

      bool IsType { get; }

      DataElementInclusionType InclusionType { get; set; }

      String EntityElementNameText { get; }

      string FixedValue { get; set; }
      string SampleValue { get; set; }
      string TypeName { get; }

      ConstraintType KeyType { get; set; }
      ConstraintType AutoGenerateType { get; set; }
      AssetElementConstraintList Constraints { get; }

      int ElementTypeNo { get; set; }
      int ElementGroupNo { get; set; }
      int AutoGenerateTypeNo { get; set; }

      string OriginalName { get; set; }
      string OriginalDataType { get; set; }

      ElementType ElementType { get; set; }
      ElementGroup ElementGroup { get; set; }

      // runtime only data-elements
      DataTypeInfo TypeElement { get; set; }
      string TypeEntityName { get; set; }
      DataTextElementInfo MapElement { get; set; }

      DataElementKind Kind { get; set; }

      int? Precision { get; set; }
      int? Scale { get; set; }

      AssetDataElement DeepCopy();
   }
}
