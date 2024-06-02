
using Edam.Diagnostics;
using Edam.Xml.OpenXml;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.Lexicon.Vocabulary;
using reader = Edam.Text.StringReader;

namespace Edam.Data.Lexicon.ImportExport
{

   public enum ImportItemType
   {
      Unknown = 0,
      Lexicon = 1,
      Area = 2,
      Entity = 3,
      Element = 4,
      Relationship = 5,
      Metadata = 6,
      Tag = 7,
      Uri = 8,
      Term = 9
   }

   public class ImportItemInfo
   {

      public const string LEXICON = "LEXICON";
      public const string AREAS = "AREAS";
      public const string ENTITIES = "ENTITIES";
      public const string ELEMENTS = "ELEMENTS";
      public const string RELATIONSHIPS = "RELATIONSHIPS";
      public const string TAGS = "TAGS";
      public const string METADATA = "METADATA";
      public const string TERMS = "TERMS";
      public const string URIS = "URIS";

      public string? ItemId { get; set; }
      public ImportItemType ImportItemType { get; set; }
      public int PropertiesCount { get; set; }
      public Action Action { get; set; }
      
      public ImportItemInfo(
         Type type, string? itemId, ImportItemType importItemType)
      {
         ItemId = itemId;
         ImportItemType = importItemType;
         PropertiesCount = type.GetProperties().Length;
      }

      public void Validate(int propertiesCount)
      {
         String func = "Validate";
         if (propertiesCount > PropertiesCount)
         {
            throw new ArgumentException(nameof(ImportItemInfo) + "::" + func +
               ": in " + ImportItemType.ToString() + " Expected no more than " +
               PropertiesCount.ToString() + " columns got (" +
               propertiesCount.ToString() + ")");
         }
      }

   }

   public class ImportReader
   {
      private const string CLASS_NAME = "ImportReader";

      private AssetConsoleArgumentsInfo? m_Arguments;
      private readonly List<ImportItemInfo> m_Items;

      private DataSet m_Vocabulary = new DataSet();

      public ImportReader()
      {
         m_Items = new List<ImportItemInfo>();
         m_Items.Add(new ImportItemInfo(
            typeof(LexiconItemInfo), ImportItemInfo.LEXICON,
            ImportItemType.Lexicon));
         m_Items.Add(new ImportItemInfo(
            typeof(EntityItemInfo), ImportItemInfo.AREAS,
            ImportItemType.Area));
         m_Items.Add(new ImportItemInfo(
            typeof(EntityItemInfo), ImportItemInfo.ENTITIES,
            ImportItemType.Entity));
         m_Items.Add(new ImportItemInfo(
            typeof(ElementItemInfo), ImportItemInfo.ELEMENTS,
            ImportItemType.Element));
         m_Items.Add(new ImportItemInfo(
            typeof(RelationshipItemInfo), ImportItemInfo.RELATIONSHIPS,
            ImportItemType.Relationship));
         m_Items.Add(new ImportItemInfo(
            typeof(TagItemInfo), ImportItemInfo.TAGS,
            ImportItemType.Tag));
         m_Items.Add(new ImportItemInfo(
            typeof(MetadataItemInfo), ImportItemInfo.METADATA,
            ImportItemType.Metadata));
         m_Items.Add(new ImportItemInfo(
            typeof(TermItemInfo), ImportItemInfo.TERMS,
            ImportItemType.Term));
         m_Items.Add(new ImportItemInfo(
            typeof(UriItemInfo), ImportItemInfo.URIS,
            ImportItemType.Uri));
      }

      public LexiconItemInfo ImportLexiconItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(8);

         LexiconItemInfo item = new LexiconItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.Title = reader.GetString(values[1]);
         item.Uri = reader.GetString(values[2]);
         item.Synonyms = reader.GetString(values[3]);
         item.Aliases = reader.GetString(values[4]);
         item.Labels = reader.GetString(values[5]);
         item.Description = reader.GetString(values[6]);
         item.Notes = reader.GetString(values[7]);

         return item;
      }

      public AreaItemInfo ImportAreaItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(10);

         AreaItemInfo item = new AreaItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.BusinessDomainID = reader.GetString(values[1]);
         item.AreaInclude = reader.GetBool(values[2]);
         item.AreaName = reader.GetString(values[3]);
         item.OriginalAreaName = reader.GetString(values[4]);
         item.Synonyms = reader.GetString(values[5]);
         item.Aliases = reader.GetString(values[6]);
         item.Base = reader.GetString(values[7]);
         item.Description = reader.GetString(values[8]);
         item.Notes = reader.GetString(values[9]);

         return item;
      }

      public EntityItemInfo ImportEntityItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(10);

         EntityItemInfo item = new EntityItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.BusinessDomainID = reader.GetString(values[1]);
         item.BusinessAreaID = reader.GetString(values[2]);
         item.EntityInclude = reader.GetBool(values[3]);
         item.EntityName = reader.GetString(values[4]);
         item.OriginalEntityName = reader.GetString(values[5]);
         item.Synonyms = reader.GetString(values[6]);
         item.Aliases = reader.GetString(values[7]);
         item.Base = reader.GetString(values[8]);
         item.Description = reader.GetString(values[9]);
         item.Notes = reader.GetString(values[10]);

         return item;
      }

      public ElementItemInfo ImportElementItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(14);

         ElementItemInfo item = new ElementItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.BusinessDomainID = reader.GetString(values[1]);
         item.BusinessAreaID = reader.GetString(values[2]);
         item.EntityName = reader.GetString(values[3]);
         item.ElementInclude = reader.GetBool(values[4]);
         item.ElementName = reader.GetString(values[5]);
         item.OriginalElementName = reader.GetString(values[6]);
         item.Synonyms = reader.GetString(values[7]);
         item.Aliases = reader.GetString(values[8]);
         item.Tags = reader.GetString(values[9]);
         item.Confidence = reader.GetDecimal(values[10]);
         item.Description = reader.GetString(values[11]);
         item.Notes = reader.GetString(values[12]);
         item.MetadataBag = reader.GetString(values[13]);

         return item;
      }

      public RelationshipItemInfo ImportRelationshipItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(12);

         RelationshipItemInfo item = new RelationshipItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.RelationshipID = reader.GetString(values[1]);
         item.BusinessDomainID = reader.GetString(values[2]);
         item.BusinessAreaID = reader.GetString(values[3]);
         item.EntityName = reader.GetString(values[4]);
         item.ElementName = reader.GetString(values[5]);
         item.ReferenceDomainID = reader.GetString(values[6]);
         item.ReferenceAreaID = reader.GetString(values[7]);
         item.ReferenceEntityName = reader.GetString(values[8]);
         item.ReferenceElementName = reader.GetString(values[9]);
         item.Description = reader.GetString(values[10]);
         item.Notes = reader.GetString(values[11]);

         return item;
      }

      public MetadataItemInfo ImportMetadataItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(5);

         MetadataItemInfo item = new MetadataItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.ScopeID = reader.GetString(values[2]);
         item.ElementID = reader.GetString(values[2]);
         item.Value = reader.GetString(values[3]);
         item.Synonyms = reader.GetString(values[4]);
         item.Description = reader.GetString(values[5]);

         return item;
      }

      public Vocabulary.UriItemInfo ImportUriItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(4);

         Vocabulary.UriItemInfo item = new Vocabulary.UriItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.ScopeID = reader.GetString(values[1]);
         item.Prefix = reader.GetString(values[2]);
         item.URI = reader.GetString(values[3]);
         item.Description = reader.GetString(values[4]);

         return item;
      }

      public TagItemInfo ImportTagItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(4);

         TagItemInfo item = new TagItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.ScopeID = reader.GetString(values[1]);
         item.TagID = reader.GetString(values[2]);
         item.Name = reader.GetString(values[3]);
         item.Notes = reader.GetString(values[4]);

         return item;
      }

      public TermItemInfo ImportTermItem(
         List<string> values, ImportItemInfo importItem)
      {

         importItem.Validate(7);

         TermItemInfo item = new TermItemInfo();

         item.KeyID = reader.GetString(values[0]);
         item.BusinessDomainID = reader.GetString(values[1]);
         item.Category = reader.GetString(values[2]);
         item.Tag = reader.GetString(values[3]);
         item.TermInclude = reader.GetBool(values[4]);
         item.Term = reader.GetString(values[5]);
         item.OriginalTerm = reader.GetString(values[6]);
         item.Synonyms = reader.GetString(values[7]);
         item.Confidence = reader.GetDecimal(values[8]);
         item.Description = reader.GetString(values[9]);

         return item;
      }

      /// <summary>
      /// Read input file as specified in arguments and convert it to a Data 
      /// Asset (collection of Data Elements).
      /// </summary>
      /// <param name="uriList">URI List</param>
      /// <returns>results log</returns>
      public IResultsLog ImportDataSet(List<string> uriList)
      {

         ResultsLog<DataSet> resultsLog = new ResultsLog<DataSet>();

         foreach (var fname in uriList)
         {

            foreach (var item in m_Items)
            {
               var results = ExcelDocumentReader.
                  ReadDocument(fname, item.ItemId);
               if (results.Success)
               {
                  int cnt = -1;
                  foreach (var list in results.Data)
                  {
                     cnt++;

                     // skip cnt == 0 to remove header...
                     if (cnt == 0)
                     {
                        continue;
                     }

                     // skip empty rows
                     if (ExcelDocumentReader.IsEmptyList(list))
                     {
                        continue;
                     }

                     switch (item.ImportItemType)
                     {
                        case ImportItemType.Lexicon:
                           m_Vocabulary.SetLexicon(
                              ImportLexiconItem(list, item));
                           break;
                        case ImportItemType.Area:
                           m_Vocabulary.Areas.Add(ImportAreaItem(list, item));
                           break;
                        case ImportItemType.Entity:
                           m_Vocabulary.Entities.Add(
                              ImportEntityItem(list, item));
                           break;
                        case ImportItemType.Element:
                           m_Vocabulary.Elements.Add(
                              ImportElementItem(list, item));
                           break;
                        case ImportItemType.Relationship:
                           m_Vocabulary.Relationships.Add(
                              ImportRelationshipItem(list, item));
                           break;
                        case ImportItemType.Metadata:
                           m_Vocabulary.Metadata.Add(
                              ImportMetadataItem(list, item));
                           break;
                        case ImportItemType.Uri:
                           m_Vocabulary.Uris.Add(ImportUriItem(list, item));
                           break;
                        case ImportItemType.Tag:
                           m_Vocabulary.Tags.Add(ImportTagItem(list, item));
                           break;
                        case ImportItemType.Term:
                           m_Vocabulary.Terms.Add(ImportTermItem(list, item));
                           break;
                     }
                  }

                  resultsLog.Data = m_Vocabulary;
               }
            }

         }

         resultsLog.Succeeded();
         return resultsLog;
      }

      /// <summary>
      /// Read input file as specified in arguments and convert it to a Data 
      /// Asset (collection of Data Elements).
      /// </summary>
      /// <param name="arguments">arguments</param>
      /// <returns>results log</returns>
      public IResultsLog ImportDataSet(
         AssetConsoleArgumentsInfo arguments)
      {
         m_Arguments = arguments;
         var uriList = UriResourceInfo.GetUriList(
           arguments.UriList, UriResourceType.xlsx);
         return ImportDataSet(uriList);
      }

   }

}
