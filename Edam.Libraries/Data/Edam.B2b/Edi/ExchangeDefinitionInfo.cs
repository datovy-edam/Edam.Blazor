using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.B2b.Edi
{

   public class BaseDefinitionInfo
   {
      public string SegmentName { get; set; }

      // may represent a schema in a database
      public string EntityID { get; set; }
      // may represent a table in a database
      public string EntityName { get; set; }
      // may represent a column in a database
      public string EntityElementName { get; set; }
      // may represent a foreign key in a database
      public string EntityLink { get; set; }

      public string Position { get; set; }
      public string SegmentCode { get; set; }
      public string SegmentRepeat { get; set; }
      public string SegmentRequiredType { get; set; }
      public string SegmentReference { get; set; }
      public string ElementID { get; set; }
      public string ElementType { get; set; }
      public string Element { get; set; }
      public string ElementDescription { get; set; }
      public string ElementRequiredType { get; set; }
      public string DataType { get; set; }
      public short? MinimumLength { get; set; }
      public short? MaximumLength { get; set; }
      public string Loop { get; set; }
      public string Parent { get; set; }
      public string Codes { get; set; }
   }

   public class ExchangeDefinitionInfo : BaseDefinitionInfo
   {
      public string DataOwnerId { get; set; }
      public int? ItemNo { get; set; }

      public string ExchangeCode { get; set; }

      public AssetDataElement ToAsset(NamespaceInfo ns)
      {
         AssetDataElement asset = new AssetDataElement();

         asset.TypeQualifiedName = new QualifiedNameInfo(ns.Prefix, DataType);
         asset.ElementQualifiedName =
            new QualifiedNameInfo(ns.Prefix, SegmentReference);
         asset.EntityQualifiedName =
            new QualifiedNameInfo(ns.Prefix, SegmentCode);

         //asset.EntityName = SegmentCode;
         //asset.ElementName = SegmentReference;
         //asset.DataType = DataType;

         asset.ElementType = Data.Asset.ElementType.element;
         asset.Namespace = ns.Uri.OriginalString;
         asset.Description = Element;
         asset.Annotation.Add(ElementDescription);
         asset.MinLength = MinimumLength;
         asset.MaxLength = MaximumLength;
         return asset;
      }
   }

}
