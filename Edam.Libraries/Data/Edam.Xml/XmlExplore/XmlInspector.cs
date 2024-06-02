// based on original from Microsoft:
//    https://github.com/mganss/XmlSchemaClassGenerator

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using helper = Edam.Data.AssetConsole.ConsoleHelper.AssetConsoleHelper;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Data.AssetUseCases;

namespace Edam.Xml.XmlExplore
{

    /// <summary>
    /// 
    /// </summary>
    public class XmlInspector
   {

      private XmlSchemaSet m_SchemaSet = new XmlSchemaSet();

      private AssetConsoleArgumentsInfo m_Arguments;
      public AssetConsoleArgumentsInfo Arguments
      {
         get { return m_Arguments; }
      }

      public bool ToAssets
      {
         get { return m_Arguments.Procedure != AssetConsoleProcedure.Unknown; }
      }

      public bool ToAssetsReport
      {
         get
         {
            return m_Arguments.Procedure ==
            AssetConsoleProcedure.XsdToAssetsReport;
         }
      }

      public XmlInspector(AssetConsoleArgumentsInfo arguments)
      {
         m_Arguments = arguments;
         PrepareSchemaSet();
      }

      /// <summary>
      /// Given arguments prepare schema set.
      /// </summary>
      /// <returns>instance of schema set is returned</returns>
      private XmlSchemaSet PrepareSchemaSet()
      {
         if (String.IsNullOrWhiteSpace(m_Arguments.OutFileExtension))
            m_Arguments.OutFileExtension = "csv";

         m_SchemaSet = new XmlSchemaSet();

         // no URI list, try using arguments input-file if any
         if (m_Arguments.UriList.Count == 0 && 
            !String.IsNullOrEmpty(m_Arguments.InputFile.Full))
         {
            m_SchemaSet.Add(null, m_Arguments.InputFile.Full);
         }

         // URI list was given, scan all XSD's
         var uriList = UriResourceInfo.GetUriList(
            m_Arguments.UriList, UriResourceType.xsd);
         foreach (var i in uriList)
         {
            m_SchemaSet.Add(null, i);
         }

         // prepare use cases
         m_Arguments.UseCases =
            PrepareUseCases(UriResourceInfo.GetUriList(
               m_Arguments.UriList, UriResourceType.xml),
               m_Arguments);

         return m_SchemaSet;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="asset"></param>
      private AssetData ToOutput(AssetData asset, bool toOutput)
      {
         asset.MapDataTypes(m_Arguments.TextMapFilePath);
         asset.SetUseCases(m_Arguments);

         if (toOutput)
            asset.ToOutput(null, m_Arguments);

         return asset;
      }

      /// <summary>
      /// 
      /// </summary>
      private AssetData InspectSchema(bool toOutput)
      {
         if (m_SchemaSet.Count == 0)
            return null;
         XmlSchemaInspector r = 
            new XmlSchemaInspector(m_Arguments, m_SchemaSet);
         r.DefaultNamespace = m_Arguments.Namespace;
         r.Inspect();
         return ToOutput(r.Asset, toOutput);
      }

      #region -- Prepare, Manage Crawler...

      /// <summary>
      /// Prepare Crawler.
      /// </summary>
      /// <returns>instance of crawler is returned</returns>
      private XmlCrawler PrepareCrawler()
      {
         XmlQualifiedName qname = new XmlQualifiedName(
            m_Arguments.RootElementName, m_Arguments.Namespace.Uri.AbsoluteUri);

         XmlCrawler crawler = new XmlCrawler(m_SchemaSet, qname);

         int max = Int16.Parse(m_Arguments.MaxThreshold);
         if (max > 0)
            crawler.MaxThreshold = max;

         int listLength = Int16.Parse(m_Arguments.ListLength);
         if (listLength > 0)
            crawler.ListLength = listLength;

         return crawler;
      }

      /// <summary>
      /// Generate sample XML, or output base on given arguments.
      /// </summary>
      /// <param name="arguments">arguments</param>
      public void Crawl()
      {
         var crawler = PrepareCrawler();

         // if toAssets is true...  Write XSD definitions to target extention
         if (ToAssets)
         {
            Xml.XmlAsset.XmlDataAsset asset = new XmlAsset.XmlDataAsset(
               Arguments.Namespace, AssetType.Schema, Arguments. 
               ProjectVersionId);
            crawler.WriteXmlAsset(asset);
            ToOutput(asset, true);
         }

         // write an XML file with sample values in each data-element
         else
         {
            XmlTextWriter textWriter = new XmlTextWriter("Sample.xml", null)
            {
               Formatting = Formatting.Indented
            };
            crawler.WriteXml(textWriter);
         }
      }

      #endregion

      /// <summary>
      /// 
      /// </summary>
      public void ToOutput()
      {
         if (ToAssetsReport)
            InspectSchema(true);
         else
            Crawl();
      }

      /// <summary>
      /// Prepare Use Cases.
      /// </summary>
      /// <param name="items">list of XML instance files</param>
      /// <returns></returns>
      public static AssetUseCaseList PrepareUseCases(List<string> items,
         AssetConsoleArgumentsInfo arguments)
      {
         AssetUseCaseList cases = new AssetUseCaseList();
         foreach (var i in items)
         {
            var xmlText = System.IO.File.ReadAllText(i);
            var uc = new XmlHelper.XmlAssetUseCase(xmlText, arguments);
            if (uc.Success)
               cases.Add(uc.UseCase);
         }
         return cases;
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="arguments">list of string arguments</param>
      public static void ToFile(AssetConsoleArgumentsInfo arguments)
      {
         var i = new XmlInspector(arguments);
         i.ToOutput();
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="args">list of string arguments</param>
      public static void ToFile(String[] args)
      {
         var arguments = AssetConsoleArgumentsInfo.FromArguments(args);
         ToFile(arguments);
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="arguments">list of string arguments</param>
      public static AssetData ToAssetList(AssetConsoleArgumentsInfo arguments)
      {
         var i = new XmlInspector(arguments);
         var assetData = i.InspectSchema(false);
         arguments.AssetDataItems = new AssetDataList();
         arguments.AssetDataItems.Add(assetData);
         return assetData;
      }

      /// <summary>
      /// Given an XSD file write corresponding Asset definitions to requested
      /// format.
      /// </summary>
      /// <param name="args">list of string arguments</param>
      public static AssetData ToAssetList(String[] args)
      {
         var arguments = AssetConsoleArgumentsInfo.FromArguments(args);
         return ToAssetList(arguments);
      }

   }

}

