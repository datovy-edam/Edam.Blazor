using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Xml.XPath;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.TextParse;
using Edam.Data.AssetManagement;
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Diagnostics;
using Edam.Data.AssetConsole;
using Edam.Data.AssetUseCases;

namespace Edam.Xml.XmlHelper
{

    public class XmlAssetUseCase: IDisposable
   {
      private const string CLASS_NAME = "XmlExplore";
      private const string AM_USE_CASE = "amusecase";
      private const string NAME = "name";

      private readonly AssetUseCase m_UseCase;

      private XmlDocument m_Document;
      private readonly XmlAsset.XmlAssetItemInfo m_Asset;
      private readonly ResultLog m_Results = new ResultLog();
      private NamespaceInfo m_DefaultNamespace;
      public List<NamespaceInfo> Namespaces = new List<NamespaceInfo>();

      public bool Success
      {
         get { return m_Results.Success; }
      }

      public ResultLog Results
      {
         get { return m_Results; }
      }

      public AssetUseCase UseCase
      {
         get { return m_UseCase; }
      }

      public XmlAssetUseCase(
         string xmlText, AssetConsoleArgumentsInfo arguments)
      {
         m_UseCase = new AssetUseCase(
            arguments.Namespace, arguments.Project.VersionId);
         if (String.IsNullOrWhiteSpace(xmlText))
            throw new Exception(CLASS_NAME + ": expected a document " +
               "a null or empty string was given.");

         m_DefaultNamespace = arguments.Namespace;
         m_Asset = new XmlAsset.XmlAssetItemInfo(
            m_UseCase.Items, arguments.Namespace);
         m_Document = new XmlDocument();
         m_Document.LoadXml(xmlText);
         ToDataElements();
      }

      public void Dispose()
      {
         if (m_Document != null)
            m_Document = null;
      }

      private AssetDataElement ToDataElement(
         XmlNode item, AssetDataElement parent)
      {
         var e = m_Asset.ToDataElement(item, parent);
         if (e == null)
         {
            return null;
         }

         var ns = Namespaces.Find(
            (x) => x.Uri.AbsoluteUri == item.NamespaceURI);
         if (ns == null)
         {
            ns =
               new NamespaceInfo(
                  item.Prefix, item.NamespaceURI, m_DefaultNamespace);
            Namespaces.Add(ns);
         }

         e.UseCaseName = UseCase.Name;

         // read atttributes
         if (item.Attributes.Count > 0)
            ReadAttributes(item.Attributes, parent);
         return e;
      }

      private void ReadAttributes(
         XmlAttributeCollection reader, AssetDataElement parent)
      {
         foreach(XmlAttribute a in reader)
         {
            var e = m_Asset.ToDataElement(a, parent);
            if (e != null)
            {
               e.UseCaseName = UseCase.Name;
            }
         }
      }

      /// <summary>
      /// Recurse over all nodes...
      /// </summary>
      /// <param name="node">node to process...</param>
      /// <param name="parent">parent node</param>
      private void ReadAllNodes(XmlNode node, AssetDataElement parent)
      {
         if (node.NodeType == XmlNodeType.Element)
         {
            m_Asset.Push(node);
         }
         else if (node.NodeType == XmlNodeType.ProcessingInstruction)
         {
            ParseProcessingInstruction(node);
         }
         else
         {
            m_UseCase.Instructions.CurrentInstruction = null;
         }

         var element = ToDataElement(node, parent);

         // a previous nameless instruction gets assigned to next Element
         if (m_UseCase.Instructions.CurrentInstruction != null &&
            element != null)
         {
            if (String.IsNullOrWhiteSpace(
               m_UseCase.Instructions.CurrentInstruction.Tag.Value))
            {
               m_UseCase.Instructions.CurrentInstruction.Tag.Value =
                  node.Name;
            }

            element.ProcessInstructionsBag =
               m_UseCase.Instructions.CurrentInstruction;
            m_UseCase.Instructions.CurrentInstruction.Parent = element;

            m_UseCase.Instructions.CurrentInstruction = null;
         }

         if (node.ChildNodes.Count > 0)
         {
            foreach (XmlNode subNode in node)
            {
               ReadAllNodes(subNode, element);
            }
         }
         if (node.NodeType == XmlNodeType.Element)
         {
            m_Asset.Pop();
         }
      }

      /// <summary>
      /// Parse Processing Instructions in top of element being examine.
      /// </summary>
      /// <param name="node"></param>
      /// <returns></returns>
      private AssetProcessInfo ParseProcessingInstruction(XmlNode node)
      {
         if (node == null)
         {
            m_UseCase.Instructions.CurrentInstruction = null;
            return null;
         }
         var i = m_UseCase.Instructions.ParseProcessingInstruction(
            node.Name, node.Value);
         i.UseCaseName = m_UseCase.Name;
         i.Tag = new AssetProcessItem
         {
            Type = AssetProcessType.Tag,
            Value = node.Name
         };
         return i;
      }

      /// <summary>
      /// Examine Document Processing Instructions at the top...
      /// </summary>
      /// <param name="item">given XmlNode item</param>
      /// <returns>true if all is good and as expected, else false</returns>
      private bool ExamineProcessingInstructions(XmlNode item)
      {
         string FUNC = CLASS_NAME + "::ExamineProcessingInstructions";

         // processing instructions are expected to be next
         if (item.NodeType != XmlNodeType.ProcessingInstruction ||
            item.Name.ToLower() != AM_USE_CASE)
         {
            m_Results.Failed(FUNC
               + ": Missing (Use Case) Processing Instructions.");
            return false;
         }

         var i = item.Value.Split("=");
         if (i.Length == 2 && i[0].ToLower() == NAME)
         {
            m_UseCase.Name = i[1].Replace("\"","").Trim();
         }
         else
         {
            m_Results.Failed(FUNC + ": Missing Use Case Name.");
            return false;
         }

         return true;
      }

      /// <summary>
      /// Traverse through the XML Document visiting all nodes, managing any 
      /// provided instructions, mappings, or description while visiting each
      /// node from the root to bottom...
      /// </summary>
      /// <returns>return a list of data elements</returns>
      public List<AssetDataElement> ToDataElements()
      {
         string FUNC = CLASS_NAME + "::" + "ToDataElements";

         var item = m_Document.FirstChild;
         if (item.NodeType == XmlNodeType.XmlDeclaration)
            item = item.NextSibling;

         if (!ExamineProcessingInstructions(item))
            return null;
         item = item.NextSibling;

         // go through all other nodes...
         try
         {
            ReadAllNodes(item, null);
            m_Results.Succeeded();
         }
         catch(Exception ex)
         {
            m_Results.Failed(FUNC, ex);
         }

         if (!m_Results.Success)
            return null;

         return m_Asset.Items;
      }

   }

}
