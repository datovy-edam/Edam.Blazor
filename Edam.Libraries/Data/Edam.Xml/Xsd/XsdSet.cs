using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.IO;

// -----------------------------------------------------------------------------

using Edam.Data.Asset;
using Edam.InOut;
using Edam.Diagnostics;

namespace Edam.Xml.Xsd
{

   public class XsdSet
   {

      private readonly IWriter m_Writer;
      private readonly XmlSchemaSet m_SchemaSet = new XmlSchemaSet();
      private readonly ResultLog m_ValidationLog;

      public XsdSet(IWriter writer, ResultLog validationLog = null)
      {
         m_Writer = writer;
         m_ValidationLog = validationLog ?? new ResultLog();
      }

      public void ValidationCallback(object sender, ValidationEventArgs e)
      {
         if (e.Exception != null)
            m_ValidationLog.Add(e.Exception);
         if (!String.IsNullOrWhiteSpace(e.Message))
            m_ValidationLog.Add(e.Message);
      }

      #region -- Manage Namespaces

      /// <summary>
      /// Get Namespace Manager for given list of namespaces.
      /// </summary>
      /// <param name="items">optional namespaces to be added</param>
      /// <returns>instance of XmlNamespaceManager is returned</returns>
      public static XmlNamespaceManager GetNamespaceManager(
         List<NamespaceInfo> items = null)
      {
         XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
         nsmgr.AddNamespace("xs", XsdHelper.XSD_NAMESPACE);
         foreach (var i in items)
         {
            nsmgr.AddNamespace(i.Prefix, i.Uri.OriginalString);
         }
         return nsmgr;
      }

      #endregion

      public void AddSchema(XsdSchema schema)
      {
         m_SchemaSet.Add(schema.Instance);
      }

      public void CompileSchemas(
         Func<ValidationEventArgs, string> validationCallback)
      {
         m_SchemaSet.ValidationEventHandler +=
            new ValidationEventHandler(ValidationCallback);
         m_SchemaSet.Compile();
      }

      public void CompileSchema(XmlSchemaSet schemaSet)
      {
         schemaSet.ValidationEventHandler +=
            new ValidationEventHandler(ValidationCallback);
         schemaSet.Compile();
      }

      public void GenerateSchema(XsdSchema xsd)
      {
         // prepare xml schema set...
         XmlSchemaSet sset = new XmlSchemaSet();
         sset.Add(xsd.Instance);

         // compile schema
         CompileSchema(sset);

         // output schema
         XmlNamespaceManager nsmgr = GetNamespaceManager(xsd.Namespaces);
         foreach (XmlSchema compiledSchema in sset.Schemas())
         {
            compiledSchema.Write(Console.Out, nsmgr);
         }
      }

      public IResultsLog GenerateSchemas(List<NamespaceInfo> namespaces = null)
      {
         int schemaIndex = 0;
         ResultLog results = new ResultLog();
         XmlNamespaceManager nsmgr = GetNamespaceManager(namespaces);
         foreach (XmlSchema compiledSchema in m_SchemaSet.Schemas())
         {
            StringWriter w = new StringWriter();
            try
            {
               compiledSchema.Write(w, nsmgr);
               if (m_Writer != null)
               {
                  String schemaName =
                     NamespaceInfo.UriToFileName(
                        compiledSchema.TargetNamespace);
                  m_Writer.Write(schemaName, w.ToString());
               }
               results.Succeeded();
            }
            catch(Exception e)
            {
               results.Failed(e);
            }
            finally
            {
               w.Dispose();
            }
            schemaIndex++;
         }
         return results;
      }

   }

}
