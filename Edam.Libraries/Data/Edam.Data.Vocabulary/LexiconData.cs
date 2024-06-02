using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Diagnostics;
using Edam.Data.AssetConsole;
using Edam.Data.Lexicon.ImportExport;
using Edam.Data.Lexicon.Vocabulary;
using Edam.Data.Assets.Asset;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Data.Lexicon
{

   public class LexiconData : ILexiconData
   {

      private static bool m_DatabaseIsReady = false;

      private DataSet m_DataSet { get; set; } = null;
      public LexiconInfo LexiconInfo { get; set; } = null;

      public DataSet DataSet
      {
         get { return m_DataSet; }
      }

      public LexiconData() { }
      public LexiconData(DataSet dataSet)
      {
         m_DataSet = dataSet;
      }

      /// <summary>
      /// Set Lexicon Information using arguments info...
      /// </summary>
      /// <param name="arguments"></param>
      public void SetLexiconInfo(AssetConsoleArgumentsInfo arguments)
      {
         // add Lexicon ID if none is proovided
         if (arguments.Lexicon == null)
         {
            arguments.Lexicon = new LexiconInfo();
         }

         if (String.IsNullOrWhiteSpace(arguments.Lexicon.LexiconId))
         {
            arguments.Lexicon.LexiconId =
               arguments.Namespace.OrganizationDomainId + "." +
               LexiconFileInfo.LEXICON_FOLDER.ToLower();
         }
         LexiconInfo = arguments.Lexicon;
      }

      /// <summary>
      /// Read Lexicon workbook into a data-set.
      /// </summary>
      /// <param name="uriList">list of uri's/file paths</param>
      /// <returns>instance of data set is returned</returns>
      public Object Import(List<string> uriList)
      {
         var reader = new Data.Lexicon.ImportExport.ImportReader();
         var results = reader.ImportDataSet(uriList);

         DataSet? data = results.DataObject as DataSet;
         Introspector intro = new Introspector(data);
         intro.IntrospectElements();

         m_DataSet = data;
         return data;
      }

      /// <summary>
      /// Export Data Asset(s) to a workbook.
      /// </summary>
      /// <param name="arguments">export process arguments</param>
      /// <returns>returns the prepared Data Set that was exported</returns>
      public Object Export(AssetConsoleArgumentsInfo arguments)
      {
         DataSet? data = null;
         if (arguments != null)
         {
            var results = ExportWriter.ExportDataSet(arguments, true);
            data = results.DataObject as DataSet;

            EnsureDatabaseReady();
            data.ToLexiconAsync();
         }

         m_DataSet = data;
         return data;
      }

      /// <summary>
      /// true is returned if Lexicon ID exists in repository.
      /// </summary>
      /// <param name="lexiconId">lexicon ID to search</param>
      /// <returns>true is returns if it exist</returns>
      public bool LexiconExists(string lexiconId)
      {
         bool found = false;
         var l = GetLexicons();
         foreach (var lexicon in l )
         {
            if (lexicon.KeyID == lexiconId)
            {
               found = true;
               break;
            }
         }
         return found;
      }

      /// <summary>
      /// Data Asset(s) to a database.
      /// </summary>
      /// <param name="arguments">export process arguments</param>
      /// <param name="deleteDatabase">true to delete database</param>
      /// <returns>returns the prepared Data Set that was exported</returns>
      public Object ToDatabase(
         AssetConsoleArgumentsInfo arguments, bool deleteDatabase = false)
      {
         SetLexiconInfo(arguments);

         // see if the lexicon is already in the database
         if (LexiconExists(arguments.Lexicon.LexiconId))
         {
            DataSet set = new DataSet();
            set.FromLexiconDatabase(arguments.Lexicon.LexiconId);
            return set;
         }

         // lexicon is new... save it to the database
         DataSet? data = null;
         if (arguments != null)
         {
            var results = ExportWriter.ExportDataSet(arguments, false);
            data = results.DataObject as DataSet;

            if (deleteDatabase)
            {
               Delete(LexiconInfo.LexiconId);
            }

            EnsureDatabaseReady();
            data.ToLexiconAsync();
         }

         m_DataSet = data;
         return data;
      }

      /// <summary>
      /// Set Data Set using Data Asset Elements and given arguments list.
      /// </summary>
      /// <param name="arguments">base arguments list</param>
      /// <param name="toWorksheet">true to export to a workbook</param>
      /// <returns>instance of Lexicon Data is returned</returns>
      public static LexiconData GetLexiconData(
         AssetConsoleArgumentsInfo arguments, bool toWorksheet = true)
      {
         DataSet dataSet = new DataSet();
         ExportWriter writer = new ExportWriter(dataSet);
         ResultsLog<DataSet> results = new ResultsLog<DataSet>();

         // see if arguments have the busines-domain and default-business-area
         string domain = String.IsNullOrWhiteSpace(arguments.Domain.Domain) ?
            arguments.Domain.DomainId : arguments.Domain.Domain;
         string area = arguments.Domain.Business;

         // prepare namespaces
         NamespaceList namespaceList = new NamespaceList();

         // go through each data-set (most of the time just 1)
         foreach (var asset in arguments.AssetDataItems)
         {
            // add asset namespaces...
            foreach (var ns in asset.Namespaces)
            {
               namespaceList.Add(ns);
            }

            // do we have a root element? (the DOMAIN container)
            var rootElement =
               asset.Items.GetRootElement(arguments.RootElementName);

            // if root was found, get its children elements
            var rootChildren =
               AssetDataElementList.GetChildren(asset.Items, rootElement);
            if (rootChildren == null)
            {
               continue;
            }

            // go through all root children (level 2) [business areas]
            foreach (var item in rootChildren.Children)
            {

               // treat the business-area as a namespace of related resources
               var type = AssetDataElementList.GetDataType(asset.Items, item);

               var aitem = writer.AddArea(
                  domain, item.ElementQualifiedName.OriginalName);

               // get element type children (level 2) [business area entities]
               var areaChildren =
                  AssetDataElementList.GetChildren(asset.Items, type);

               // go through the business area entities
               foreach (var entity in areaChildren.Children)
               {

                  // get entity children (level 3) [entity data element]
                  var entityChildren =
                     AssetDataElementList.GetChildren(asset.Items, entity);

                  EntityItemInfo eitem;
                  if (entityChildren.Children.Count == 0)
                  {
                     var entityType =
                        AssetDataElementList.GetDataType(asset.Items, entity);
                     var adomain = String.IsNullOrWhiteSpace(
                        aitem.BusinessDomainID) ?
                           domain : aitem.BusinessDomainID;

                     eitem = writer.AddEntity(entityType,
                        entity.ElementQualifiedName.OriginalName,
                        adomain, aitem.AreaName);
                     entityChildren =
                     AssetDataElementList.GetChildren(
                           asset.Items, entityType);
                  }
                  else
                  {
                     eitem = writer.AddEntity(entity, domain, area);
                  }

                  // add all children (entity - elements)
                  foreach (var child in entityChildren.Children)
                  {
                     if (child.DataType != entity.DataType)
                     {
                        var edomain = String.IsNullOrWhiteSpace(
                           eitem.BusinessDomainID) ?
                              domain : eitem.BusinessDomainID;
                        writer.AddElement(
                           child, entity.ElementQualifiedName.OriginalName,
                           edomain, eitem.BusinessAreaID);
                        writer.AddConstraints(
                           child, entity.ElementQualifiedName.OriginalName,
                           edomain, eitem.BusinessAreaID);
                     }
                  }
               }
            }
         }

         // write URIs
         writer.AddUri(namespaceList);

         // setup Lexicon
         LexiconData lexiconData = new LexiconData(dataSet);
         lexiconData.SetLexiconInfo(arguments);
         dataSet.SetupLexicon(arguments);
         lexiconData.UpdateTermsCount();

         // output to file
         if (toWorksheet)
         {
            writer.ExportDataSet(arguments, namespaceList);
         }

         return lexiconData;
      }

      /// <summary>
      /// Prepare a Lexicon Database... if exists nothing is done, else it will
      /// try to create it.
      /// </summary>
      /// <returns>results log is returned</returns>
      public IResultsLog EnsureDatabaseReady()
      {
         ResultLog resultLog = new ResultLog();

         if (m_DatabaseIsReady)
         {
            resultLog.Succeeded();
            return resultLog;
         }

         Edam.Data.Lexicon.LexiconContext? context = null;

         try
         {
            context = new Edam.Data.Lexicon.LexiconContext();
            context.Database.EnsureCreated();
            context.Dispose();
            resultLog.Succeeded();
            m_DatabaseIsReady = true;
         }
         catch (Exception ex)
         {
            resultLog.Failed(ex);
         }
         finally
         {
            if (context != null)
            {
               context.Dispose();
            }
         }

         return resultLog;
      }

      /// <summary>
      /// Using loaded lexicon (data set) update terms count.
      /// </summary>
      public void UpdateTermsCount()
      {
         TermCounter.UpdateTermsCount(this);
      }

      /// <summary>
      /// Load Lexicon using given Id.
      /// </summary>
      /// <param name="lexiconId">lexicon Id to load</param>
      /// <returns>DataSet that was loaded</returns>
      public Object Load(string lexiconId)
      {
         DataSet data = new DataSet();
         data.FromLexicon(lexiconId);

         m_DataSet = data;
         return data;
      }

      /// <summary>
      /// Persist Lexicon.
      /// </summary>
      /// <param name="arguments">arguments whose AssetData should be persisted
      /// </param>
      /// <returns>result log is returned</returns>
      public IResultsLog? PersistLexicon(AssetConsoleArgumentsInfo arguments)
      {
         var results = ExportWriter.ExportDataSet(arguments, false);
         if (!results.Success)
         {
            return results;
         }
         var data = results.DataObject as DataSet;
         data.ToLexiconAsync();
         return results;
      }

      /// <summary>
      /// Ensure to load the data by persisting if lexicon can't be found.
      /// </summary>
      /// <param name="arguments">instance of arguments</param>
      /// <returns>results log is returned</returns>
      public IResultsLog? EnsureLoad(AssetConsoleArgumentsInfo arguments)
      {
         EnsureDatabaseReady();
         var data = Load(arguments.Lexicon.LexiconId) as DataSet;
         if (data != null && data.Lexicon.KeyID ==
            arguments.Lexicon.LexiconId)
         {
            var results = new ResultLog();
            results.ResultValueObject = data;
            results.Succeeded();
            return results;
         }

         return PersistLexicon(arguments);
      }

      /// <summary>
      /// Delete Lexicon using given Id.
      /// </summary>
      /// <param name="lexiconId">lexicon Id to delete</param>
      public void Delete(string lexiconId)
      {
         DataSet ds = new DataSet();
         ds.DeleteLexicon(lexiconId);
         m_DataSet = null;
      }

      /// <summary>
      /// Return the list of lexicons in current repository.
      /// </summary>
      /// <returns>returns the list of lexicons</returns>
      public List<LexiconItemInfo> GetLexicons()
      {
         ResultLog results = null;
         LexiconContext context = null;
         List<LexiconItemInfo> list = null;
         try
         {
            context = new LexiconContext();
            list = context.Lexicon.ToList();
         }
         catch (Exception ex)
         {
            results = new ResultLog();
            results.Failed(ex);
            list = new List<LexiconItemInfo>();
         }
         return list;
      }

      public List<AreaItemInfo> GetAreas()
      {
         if (m_DataSet == null)
         {
            return new List<AreaItemInfo>(); ;
         }
         return m_DataSet.Areas.Values.ToList();
      }

      public List<ElementItemInfo> GetElements()
      {
         if (m_DataSet == null)
         {
            return new List<ElementItemInfo>(); ;
         }
         return m_DataSet.Elements.Values.ToList();
      }

      public List<EntityItemInfo> GetEntities()
      {
         if (m_DataSet == null)
         {
            return new List<EntityItemInfo>(); ;
         }
         return m_DataSet.Entities.Values.ToList();
      }

      public List<TermItemInfo> GetTerms()
      {
         if (m_DataSet == null)
         {
            return new List<TermItemInfo>(); ;
         }
         return m_DataSet.Terms.Values.ToList();
      }

      public List<MetadataItemInfo> GetMetadata()
      {
         if (m_DataSet == null)
         {
            return new List<MetadataItemInfo>(); ;
         }
         return m_DataSet.Metadata.Values.ToList();
      }

      public List<RelationshipItemInfo> GetRelationships()
      {
         if (m_DataSet == null)
         {
            return new List<RelationshipItemInfo>(); ;
         }
         return m_DataSet.Relationships.Values.ToList();
      }

      public List<TagItemInfo> GetTags()
      {
         if (m_DataSet == null)
         {
            return new List<TagItemInfo>(); ;
         }
         return m_DataSet.Tags.Values.ToList();
      }

      public List<Vocabulary.UriItemInfo> GetUris()
      {
         if (m_DataSet == null)
         {
            return new List<Vocabulary.UriItemInfo>(); ;
         }
         return m_DataSet.Uris.Values.ToList();
      }

      public TermItemInfo? FindTerm(string token)
      {
         m_DataSet.Terms.TryGetValue(token, out var value);
         return value;
      }

      public void AddTerm(TermItemInfo term)
      {
         m_DataSet.Terms.Add(term);
      }

      //public List<LexiconItemInfo> GetLexicons()
      //{
      //   if (m_DataSet == null)
      //   {
      //      return new List<LexiconItemInfo>(); ;
      //   }
      //   return m_DataSet.Lexicon
      //}

   }

}
