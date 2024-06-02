using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using voca = Edam.Data.Lexicon.Vocabulary;
using Edam.Data.Lexicon.Vocabulary;
using Edam.Text;
using Edam.Diagnostics;
using Edam.Xml.OpenXml;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using reader = Edam.Text.StringReader;
using DocumentFormat.OpenXml.Wordprocessing;
using Edam.Data.AssetSchema;
using Edam.Data.AssetReport;
using Edam.Application;
using Edam.Data.Lexicon.ImportExport;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Edam.Data.Lexicon.ImportExport
{

   public class ExportWriter
   {

      public const string DECIMAL_STRING_FORMAT = "0.00";
      private DataSet m_DataSet { get; set; }

      public ExportWriter(DataSet dataSet)
      {
         m_DataSet = dataSet;
      }

      #region -- 4.00 - Add Data to Set

      /// <summary>
      /// Create a new Area.
      /// </summary>
      /// <param name="domain"></param>
      /// <returns></returns>
      public AreaItemInfo AddArea(string domain, string area)
      {
         string fullPath = domain + "/" + area;

         if (!String.IsNullOrWhiteSpace(fullPath) && 
            m_DataSet.Areas.TryGetValue(
            fullPath, out AreaItemInfo? value))
         {
            return value;
         }

         // create new area
         AreaItemInfo item = new AreaItemInfo();

         item.KeyID = String.Empty;
         item.BusinessDomainID = domain;
         item.AreaInclude = true;
         item.AreaName = area;
         item.OriginalAreaName = area;
         item.Synonyms = String.Empty;
         item.Aliases = String.Empty;
         item.Base = String.Empty;
         item.Description = String.Empty;
         item.Notes = String.Empty;

         item.KeyID = VerifyKeyId(item);
         VerifyReferenceLexicon(item);

         m_DataSet.Areas.Add(item);

         return item;
      }

      /// <summary>
      /// Create a new Entity.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public EntityItemInfo AddEntity(AssetDataElement item, string entity,
         string domain = "", string area = "")
      {
         string fullPath = item.FullPath;

         if (!String.IsNullOrWhiteSpace(fullPath) && 
            m_DataSet.Entities.TryGetValue(
            fullPath, out EntityItemInfo? value))
         {
            return value;
         }

         // create new entity
         EntityItemInfo eitem = new EntityItemInfo();

         eitem.KeyID = item.ElementId;
         eitem.BusinessDomainID = domain;
         eitem.BusinessAreaID = area;
         eitem.EntityInclude = true;
         eitem.EntityName = entity;
         eitem.OriginalEntityName = String.IsNullOrWhiteSpace(item.EntityName) ?
            entity : item.EntityName;
         eitem.Synonyms = String.Empty;
         eitem.Aliases = String.Empty;
         eitem.Base = item.EntityQualifiedName == null ?
            String.Empty : item.EntityQualifiedName.OriginalName;

         // TODO: provide option to use Element Name or Description
         //     : see AddElement also
         eitem.Description = Text.Convert.ToProperCase(item.ElementName);
         eitem.Notes = String.IsNullOrWhiteSpace(item.Description) ?
            item.AnnotationText : item.Description; ;

         eitem.KeyID = VerifyKeyId(eitem);
         VerifyReferenceLexicon(eitem);

         m_DataSet.Entities.Add(eitem);

         return eitem;
      }

      /// <summary>
      /// Add URI.
      /// </summary>
      /// <param name="items">namespaces to add</param>
      public void AddUri(NamespaceList items)
      {
         foreach(var item in items)
         {
            voca.UriItemInfo uri = new voca.UriItemInfo();
            uri.Prefix = item.Prefix;
            uri.KeyID = VerifyKeyId(uri);
            uri.ScopeID = "Namespace";
            uri.URI = item.UriText;
            uri.Description = String.Empty;
            m_DataSet.Uris.Add(uri);

            VerifyReferenceLexicon(uri);
         }
      }

      /// <summary>
      /// Create a new Element.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public ElementItemInfo AddElement(AssetDataElement item,
         string entity, string domain = "", string area = "")
      {
         string fullPath = item.FullPath;

         if (!String.IsNullOrWhiteSpace(fullPath) && 
            m_DataSet.Elements.TryGetValue(
            fullPath, out ElementItemInfo? value))
         {
            return value;
         }

         // create new element
         ElementItemInfo element = new ElementItemInfo();

         element.KeyID = item.ElementId;
         element.BusinessDomainID = domain;
         element.BusinessAreaID = area;
         element.EntityName = entity;
         element.ElementInclude = true;
         element.ElementName = item.ElementQualifiedName.OriginalName;
         element.OriginalElementName = item.ElementName;
         element.Synonyms = String.Empty;
         element.Aliases = String.Empty;
         element.Tags = String.Empty;
         element.Confidence = 1;

         // TODO: provide option to use Element Name or Description
         //     : see AddEntity also
         element.Description = Text.Convert.ToProperCase(element.ElementName);
         element.Notes = String.IsNullOrWhiteSpace(item.Description) ?
            item.AnnotationText : item.Description;

         element.MetadataBag = String.Empty;

         element.KeyID = VerifyKeyId(element);
         VerifyReferenceLexicon(element);

         m_DataSet.Elements.Add(element);

         return element;
      }

      /// <summary>
      /// Add Relation/Constraint.
      /// </summary>
      /// <param name="constraint"></param>
      /// <param name="entity"></param>
      /// <param name="domain"></param>
      /// <param name="area"></param>
      /// <returns></returns>
      public RelationshipItemInfo AddRelationship(
         AssetElementConstraintInfo constraint, string element,
         string entity, string domain = "", string area = "")
      {
         string relationshipId = "fk_" + entity + "_" + element;
         string fullPath = domain + "/" + area + "/" + relationshipId;

         if (!String.IsNullOrWhiteSpace(fullPath) && 
            m_DataSet.Relationships.TryGetValue(
            fullPath, out RelationshipItemInfo? value))
         {
            return value;
         }

         // create new constraint
         RelationshipItemInfo relationship = new RelationshipItemInfo();

         string type;
         switch(constraint.ContraintType)
         {
            case AssetElementContraintType.ForeignKey:
               type = AssetElementConstraintInfo.FOREIGN_KEY;
               break;
            case AssetElementContraintType.Key:
               type = AssetElementConstraintInfo.KEY;
               break;
            default:
               type = AssetElementConstraintInfo.UNKNOWN;
               break;
         }

         relationship.KeyID = String.Empty;
         relationship.RelationshipID = relationshipId;
         relationship.BusinessDomainID = domain;
         relationship.BusinessAreaID = area;
         relationship.EntityName = entity;
         relationship.ElementName = element;
         relationship.ReferenceDomainID = domain;
         relationship.ReferenceAreaID = constraint.ReferenceSchemaName;
         relationship.ReferenceType = type;
         relationship.ReferenceEntityName = constraint.ReferenceEntityName;
         relationship.ReferenceElementName = constraint.ReferenceElementName;
         relationship.Description = constraint.ContraintDescription;
         relationship.Notes = string.Empty;

         relationship.KeyID = VerifyKeyId(relationship);
         VerifyReferenceLexicon(relationship);

         m_DataSet.Relationships.Add(relationship);

         return relationship;
      }

      #endregion
      #region -- 4.00 - Export Data Set

      /// <summary>
      /// Export Lexicon.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportLexicon(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.LEXICON);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(LexiconItemInfo.KeyID));
         items.Add(nameof(LexiconItemInfo.Title));
         items.Add(nameof(LexiconItemInfo.Uri));
         items.Add(nameof(LexiconItemInfo.Synonyms));
         items.Add(nameof(LexiconItemInfo.Aliases));
         items.Add(nameof(LexiconItemInfo.Labels));
         items.Add(nameof(LexiconItemInfo.Description));
         items.Add(nameof(LexiconItemInfo.Notes));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // set Lexicon values
         builder.AppendRowCell(m_DataSet.Lexicon.KeyID);
         builder.AppendRowCell(m_DataSet.Lexicon.Title);
         builder.AppendRowCell(m_DataSet.Lexicon.Uri);
         builder.AppendRowCell(m_DataSet.Lexicon.Synonyms);
         builder.AppendRowCell(m_DataSet.Lexicon.Aliases);
         builder.AppendRowCell(m_DataSet.Lexicon.Labels);
         builder.AppendRowCell(m_DataSet.Lexicon.Description);
         builder.AppendRowCellLast(m_DataSet.Lexicon.Notes);
      }

      /// <summary>
      /// Export Areas.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportAreas(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.AREAS);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(AreaItemInfo.KeyID));
         items.Add(nameof(AreaItemInfo.BusinessDomainID));
         items.Add(nameof(AreaItemInfo.AreaInclude));
         items.Add(nameof(AreaItemInfo.AreaName));
         items.Add(nameof(AreaItemInfo.OriginalAreaName));
         items.Add(nameof(AreaItemInfo.Synonyms));
         items.Add(nameof(AreaItemInfo.Aliases));
         items.Add(nameof(AreaItemInfo.Base));
         items.Add(nameof(AreaItemInfo.Description));
         items.Add(nameof(AreaItemInfo.Notes));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add areas
         foreach (var item in m_DataSet.Areas)
         {
            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.BusinessDomainID);
            builder.AppendRowCell(item.Value.AreaInclude.ToString());
            builder.AppendRowCell(item.Value.AreaName);
            builder.AppendRowCell(item.Value.OriginalAreaName);
            builder.AppendRowCell(item.Value.Synonyms);
            builder.AppendRowCell(item.Value.Aliases);
            builder.AppendRowCell(item.Value.Base);
            builder.AppendRowCell(item.Value.Description);
            builder.AppendRowCellLast(item.Value.Notes);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Entities.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportEntities(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.ENTITIES);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(EntityItemInfo.KeyID));
         items.Add(nameof(EntityItemInfo.BusinessDomainID));
         items.Add(nameof(EntityItemInfo.BusinessAreaID));
         items.Add(nameof(EntityItemInfo.EntityInclude));
         items.Add(nameof(EntityItemInfo.EntityName));
         items.Add(nameof(EntityItemInfo.OriginalEntityName));
         items.Add(nameof(EntityItemInfo.Synonyms));
         items.Add(nameof(EntityItemInfo.Aliases));
         items.Add(nameof(EntityItemInfo.Base));
         items.Add(nameof(EntityItemInfo.Description));
         items.Add(nameof(EntityItemInfo.Notes));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add entities
         foreach (var item in m_DataSet.Entities)
         {
            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.BusinessDomainID);
            builder.AppendRowCell(item.Value.BusinessAreaID);
            builder.AppendRowCell(item.Value.EntityInclude.ToString());
            builder.AppendRowCell(item.Value.EntityName);
            builder.AppendRowCell(item.Value.OriginalEntityName);
            builder.AppendRowCell(item.Value.Synonyms);
            builder.AppendRowCell(item.Value.Aliases);
            builder.AppendRowCell(item.Value.Base);
            builder.AppendRowCell(item.Value.Description);
            builder.AppendRowCellLast(item.Value.Notes);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Elements.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportElements(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.ELEMENTS);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(ElementItemInfo.KeyID));
         items.Add(nameof(ElementItemInfo.BusinessDomainID));
         items.Add(nameof(ElementItemInfo.BusinessAreaID));
         items.Add(nameof(ElementItemInfo.EntityName));
         items.Add(nameof(ElementItemInfo.ElementInclude));
         items.Add(nameof(ElementItemInfo.ElementName));
         items.Add(nameof(ElementItemInfo.OriginalElementName));
         items.Add(nameof(ElementItemInfo.Synonyms));
         items.Add(nameof(ElementItemInfo.Aliases));
         items.Add(nameof(ElementItemInfo.Tags));
         items.Add(nameof(ElementItemInfo.Confidence));
         items.Add(nameof(ElementItemInfo.Description));
         items.Add(nameof(ElementItemInfo.Notes));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add elements
         foreach (var item in m_DataSet.Elements)
         {
            decimal confidence = item.Value.Confidence.HasValue ?
               item.Value.Confidence.Value : 0.0M;

            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.BusinessDomainID);
            builder.AppendRowCell(item.Value.BusinessAreaID);
            builder.AppendRowCell(item.Value.EntityName);
            builder.AppendRowCell(item.Value.ElementInclude.ToString());
            builder.AppendRowCell(item.Value.ElementName);
            builder.AppendRowCell(item.Value.OriginalElementName);
            builder.AppendRowCell(item.Value.Synonyms);
            builder.AppendRowCell(item.Value.Aliases);
            builder.AppendRowCell(item.Value.Tags);
            builder.AppendRowCell(confidence.ToString(DECIMAL_STRING_FORMAT));
            builder.AppendRowCell(item.Value.Description);
            builder.AppendRowCellLast(item.Value.Notes);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Relationships.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportRelationships(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.RELATIONSHIPS);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(RelationshipItemInfo.KeyID));
         items.Add(nameof(RelationshipItemInfo.RelationshipID));
         items.Add(nameof(RelationshipItemInfo.BusinessDomainID));
         items.Add(nameof(RelationshipItemInfo.BusinessAreaID));
         items.Add(nameof(RelationshipItemInfo.EntityName));
         items.Add(nameof(RelationshipItemInfo.ElementName));
         items.Add(nameof(RelationshipItemInfo.ReferenceDomainID));
         items.Add(nameof(RelationshipItemInfo.ReferenceAreaID));
         items.Add(nameof(RelationshipItemInfo.ReferenceType));
         items.Add(nameof(RelationshipItemInfo.ReferenceEntityName));
         items.Add(nameof(RelationshipItemInfo.ReferenceElementName));
         items.Add(nameof(RelationshipItemInfo.Description));
         items.Add(nameof(RelationshipItemInfo.Notes));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add relationships
         foreach (var item in m_DataSet.Relationships)
         {
            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.RelationshipID);
            builder.AppendRowCell(item.Value.BusinessDomainID);
            builder.AppendRowCell(item.Value.BusinessAreaID);
            builder.AppendRowCell(item.Value.EntityName);
            builder.AppendRowCell(item.Value.ElementName);
            builder.AppendRowCell(item.Value.ReferenceDomainID);
            builder.AppendRowCell(item.Value.ReferenceAreaID);
            builder.AppendRowCell(item.Value.ReferenceType);
            builder.AppendRowCell(item.Value.ReferenceEntityName);
            builder.AppendRowCell(item.Value.ReferenceElementName);
            builder.AppendRowCell(item.Value.Description);
            builder.AppendRowCellLast(item.Value.Notes);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Terms.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportTerms(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.TERMS);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(TermItemInfo.KeyID));
         items.Add(nameof(TermItemInfo.BusinessDomainID));
         items.Add(nameof(TermItemInfo.Category));
         items.Add(nameof(TermItemInfo.Tag));
         items.Add(nameof(TermItemInfo.TermInclude));
         items.Add(nameof(TermItemInfo.Term));
         items.Add(nameof(TermItemInfo.OriginalTerm));
         items.Add(nameof(TermItemInfo.Synonyms));
         items.Add(nameof(TermItemInfo.Confidence));
         items.Add(nameof(TermItemInfo.Description));
         items.Add(nameof(TermItemInfo.Count));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add terms
         foreach (var item in m_DataSet.Terms)
         {
            string oterm = String.IsNullOrWhiteSpace(item.Value.OriginalTerm) ?
               item.Value.Term : item.Value.OriginalTerm;
            decimal confidence = item.Value.Confidence.HasValue ?
               item.Value.Confidence.Value : 0.0M;

            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.BusinessDomainID);
            builder.AppendRowCell(item.Value.Category);
            builder.AppendRowCell(item.Value.Tag);
            builder.AppendRowCell(item.Value.TermInclude.ToString());
            builder.AppendRowCell(item.Value.Term);
            builder.AppendRowCell(oterm);
            builder.AppendRowCell(item.Value.Synonyms);
            builder.AppendRowCell(confidence.ToString(DECIMAL_STRING_FORMAT));
            builder.AppendRowCell(item.Value.Description);
            builder.AppendRowCellLast(item.Value.Count.HasValue ?
               item.Value.Count.Value.ToString() : String.Empty);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Tags.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportTags(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.TAGS);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(TagItemInfo.KeyID));
         items.Add(nameof(TagItemInfo.ScopeID));
         items.Add(nameof(TagItemInfo.TagID));
         items.Add(nameof(TagItemInfo.Name));
         items.Add(nameof(TagItemInfo.Notes));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add tags
         foreach (var item in m_DataSet.Tags)
         {
            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.ScopeID);
            builder.AppendRowCell(item.Value.TagID);
            builder.AppendRowCell(item.Value.Name);
            builder.AppendRowCellLast(item.Value.Notes);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Metadata.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportMetadata(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.METADATA);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(MetadataItemInfo.KeyID));
         items.Add(nameof(MetadataItemInfo.ScopeID));
         items.Add(nameof(MetadataItemInfo.ElementID));
         items.Add(nameof(MetadataItemInfo.Value));
         items.Add(nameof(MetadataItemInfo.Synonyms));
         items.Add(nameof(MetadataItemInfo.Description));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add metadata
         foreach (var item in m_DataSet.Metadata)
         {
            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.ScopeID);
            builder.AppendRowCell(item.Value.ElementID);
            builder.AppendRowCell(item.Value.Value);
            builder.AppendRowCell(item.Value.Synonyms);
            builder.AppendRowCellLast(item.Value.Description);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Export Uris.
      /// </summary>
      /// <param name="builder"></param>
      private void ExportUris(ITableBuilder builder)
      {
         builder.AddWorksheet(ImportItemInfo.URIS);

         // prepare header
         List<string> items = new List<string>();
         items.Add(nameof(voca.UriItemInfo.KeyID));
         items.Add(nameof(voca.UriItemInfo.ScopeID));
         items.Add(nameof(voca.UriItemInfo.Prefix));
         items.Add(nameof(voca.UriItemInfo.URI));
         items.Add(nameof(voca.UriItemInfo.Description));

         builder.AppendMainHeader(items, string.Empty);
         builder.SetStyleNo((uint)TableRowStyle.Fill1Font12);

         // add uris
         foreach (var item in m_DataSet.Uris)
         {
            builder.AppendRowCell(VerifyKeyId(item.Value));
            builder.AppendRowCell(item.Value.ScopeID);
            builder.AppendRowCell(item.Value.Prefix);
            builder.AppendRowCell(item.Value.URI);
            builder.AppendRowCellLast(item.Value.Description);

            VerifyReferenceLexicon(item.Value);
         }
      }

      /// <summary>
      /// Verify Reference Lexicon.
      /// </summary>
      /// <param name="item">item to verify</param>
      private void VerifyReferenceLexicon(IItemInfo item)
      {
         if (item.Lexicon == null)
         {
            item.Lexicon = m_DataSet.Lexicon;
         }
         if (String.IsNullOrWhiteSpace(item.LexiconID))
         {
            item.LexiconID = m_DataSet.Lexicon.KeyID;
         }
      }

      /// <summary>
      /// Ensure that there is a valid KeyID.
      /// </summary>
      /// <param name="keyId">key ID to verify/set</param>
      /// <returns></returns>
      private string VerifyKeyId(IItemInfo item)
      {
         if (String.IsNullOrWhiteSpace(item.KeyID))
         {
            item.KeyID = Guid.NewGuid().ToString();
         }
         return item.KeyID;
      }

      /// <summary>
      /// Export Data Set...
      /// </summary>
      /// <param name="file"></param>
      public void ExportDataSet(AssetConsoleArgumentsInfo arguments,
         NamespaceList namespaces)
      {
         Edam.InOut.FileInfo file;

         // prepare file info to export into...
         InOut.FileInfo exportFile = LexiconFileInfo.GetFileInfo(arguments);

         // export data set
         ITableBuilder builder = AssetReportBuilder.GetBuilder(exportFile);

         ExportLexicon(builder);
         ExportAreas(builder);
         ExportEntities(builder);
         ExportElements(builder);
         ExportRelationships(builder);

         ExportTerms(builder);
         ExportTags(builder);
         ExportMetadata(builder);

         // setup URI's
         foreach (var uri in namespaces)
         {
            voca.UriItemInfo item = new voca.UriItemInfo();
            item.ScopeID = "Namespace";
            item.Prefix = uri.Prefix;
            item.URI = uri.UriText;
            item.Description = uri.ToReferenceUriText();
            m_DataSet.Uris.Add(item);
         }

         ExportUris(builder);

         builder.Close();
      }

      /// <summary>
      /// Add Constraints based on info as given in the element.
      /// </summary>
      /// <param name="element"></param>
      /// <param name="entity"></param>
      /// <param name="domain"></param>
      /// <param name="area"></param>
      public void AddConstraints(AssetDataElement element,
         string entity, string domain = "", string area = "")
      {
         foreach (var constraint in element.Constraints)
         {
            AddRelationship(constraint,
               element.ElementQualifiedName.OriginalName,
               entity, domain, area);
         }
      }

      /// <summary>
      /// Given a Data Asset (collection of Data Elements), get its vocabulary.
      /// </summary>
      /// <remarks>
      /// An asset data model may have 2, 3 or more levels, for example:
      ///    1st level = the root element representing the business domain such
      ///                as HealthCare, CriminalJustice, Finaice, and others.
      ///    2nd level = business area
      ///                root element and children with leaf elements like 
      ///                you will have with tables. Hopefully this represents
      ///                the business-areas (like finance, sales, others).
      ///    3rd level = business entity (or data-component)
      ///                root element and children such as a schema that contain
      ///                children that look like tables that in turn have
      ///                children (or columns) as leaf elements. Hopefully this
      ///                represents the business-entities.
      /// Database based data assets will have 3 levels.
      /// </remarks>
      /// <param name="arguments">arguments</param>
      /// <param name="toWorksheet">true to export to a workbook</param>
      /// <returns>results log</returns>
      public static IResultsLog ExportDataSet(
         AssetConsoleArgumentsInfo arguments, bool toWorksheet = true)
      {
         ResultsLog<DataSet> results = new ResultsLog<DataSet>();
         var lexiconData = LexiconData.GetLexiconData(arguments, toWorksheet);

         // all done
         results.Data = lexiconData.DataSet;
         results.Succeeded();

         return results;
      }

      #endregion

   }

}
