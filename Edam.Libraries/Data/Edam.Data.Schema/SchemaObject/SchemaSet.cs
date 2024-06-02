using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Xml.Xsd;
using Edam.Data.AssetSchema;

namespace Edam.Data.Schema.SchemaObject
{

   /// <summary>
   /// The Schema Set holds information about:
   ///    - Catalogs
   ///       - Schemas
   ///          - Resources
   ///             - Elements
   /// </summary>
   public class SchemaSet
   {

      public List<CatalogInfo> Catalogs { get; set; }

      public string TargetNamespace { get; set; }
      public string NamespaceAlias { get; set; }

      public void Add(List<SchemaResource> items, 
         List<SchemaResourceConstraint> constraints, string versionId)
      {
         if (Catalogs == null)
            Catalogs = new List<CatalogInfo>();

         string catName = "";
         CatalogInfo cat = null;
         SchemaInfo sch = null;
         ResourceInfo tbl = null;
         ElementInfo col = null;

         foreach (var i in items)
         {
            if (catName != i.CatalogName)
            {
               cat = Catalogs.Find((x) => { return x.Name == i.CatalogName; });
               if (cat == null)
               {
                  cat = new CatalogInfo(versionId);

                  cat.Name = i.CatalogName;
                  cat.Constraints = constraints;
                  cat.VersionId = versionId; 
                  Catalogs.Add(cat);
               }
            }

            if (sch == null || sch.Name != i.SchemaName)
            {
               sch = cat.Add(i.SchemaName);
            }

            if (tbl == null || tbl.Name != i.ResourceName)
            {
               tbl = sch.Add(i.ResourceName);
            }

            // update metadata
            SchemaResource.SynchronizeIndexes(i);

            // preapre columns
            if (col == null || col.Name != i.ElementName)
            {
               col = tbl.Add(
                  i.ElementName, i.DataType, i.DataSize, i.IsNullable,
                  i.Metadata.IsKey || i.Metadata.IsUniqueTimestamp, 
                  i.Metadata.IsIdentity || i.Metadata.IsAutoIncrement);
            }
         }
      }

      public string ToByReferenceXsd()
      {
         return XsdWriter.ToReferenceXsd(
            Catalogs, TargetNamespace, NamespaceAlias);
      }

      public string ToByElementXsd()
      {
         return XsdWriter.ToElementXsd(
            Catalogs, TargetNamespace, NamespaceAlias);
      }

   }

}
