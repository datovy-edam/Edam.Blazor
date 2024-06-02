using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

// -----------------------------------------------------------------------------
using Edam.Xml.Xsd;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetConsole;
using Edam.InOut;

namespace Edam.Data.AssetManagement.Writers.Xsd
{

   public class XsdWriter : SchemaWriterAbstract<ResourceContext>
   {

      private DataTextMap m_DataTextMap;
      private readonly List<XsdSchema> m_Schemas = new List<XsdSchema>();

      public XsdWriter(ResourceContext context, DataTextMap textMap = null)
         : base(context)
      {
         m_Context = context;
         m_DataTextMap = textMap;
         m_Context.DataTextMapResolver = 
            new DataTextMapResolverDelegate(MapText);
      }

      public void PrepareSchemas(AssetDataElementList assets)
      {

         // if assets is null write types and elements especified in the context
         // (m_Context) that is the main dictionary of types and resources
         if (assets == null)
         {
            foreach (var ns in m_Context.Namespaces)
            {
               if (ns.IsW3CSchema)
               {
                  continue;
               }
               var sw = new XsdSchemaWriter(m_Context, ns);
               sw.WriteComplexTypes(m_Context);

               // TODO: context had already provided the type and children,
               // further investigate why we need to try adding aditional elements
               sw.WriteElements(m_Context);

               m_Schemas.Add(sw.Schema);
            }
            return;
         }

         // assets are provided, so write xsd according to the assets list and
         // using the main context (m_Context) as the types and resources 
         // dictionary
         var schms = XsdSchemaWriter.WriteAssetsSchema(assets, m_Context);
         m_Schemas.AddRange(schms);
      }

      public XmlSchemaImport GetImport(NamespaceInfo ns)
      {
         XmlSchemaImport i = new XmlSchemaImport();
         i.Namespace = ns.Uri.OriginalString;
         i.SchemaLocation = "./" + NamespaceInfo.UriToFileName(
            ns.Uri.OriginalString);
         return i;
      }

      //public void WriteXsd()
      //{
      //   _ = new XsdSet(null);
      //   foreach (var i in m_Schemas)
      //   {
      //      foreach (var n in i.Namespaces)
      //      {
      //         i.Instance.Namespaces.Add(n.Prefix, n.Uri.OriginalString);
      //         if (i.Namespace.Prefix == n.Prefix)
      //            continue;
      //         i.Instance.Includes.Add(GetImport(n));
      //      }
      //      xset.GenerateSchema(i);
      //   }
      //}

      public void WriteSet(IWriter writer)
      {
         XsdSet xset = new XsdSet(writer);
         foreach (var i in m_Schemas)
         {
            foreach (var n in i.Namespaces)
            {
               i.Instance.Namespaces.Add(n.Prefix, n.Uri.OriginalString);
               if (i.Namespace.Prefix == n.Prefix ||
                   (NamespaceInfo.IsUrn(n.Uri.OriginalString) ||
                    NamespaceInfo.IsW3CUri(n.Uri.OriginalString)))
                  continue;
               i.Instance.Includes.Add(GetImport(n));
            }
            xset.AddSchema(i);
         }

         xset.CompileSchemas((e) =>
         {
            return e.Message;
         });
         xset.GenerateSchemas(m_Context.Namespaces);
      }

      public object MapText(string qualifiedName)
      {
         if (m_DataTextMap == null)
         {
            return null;
         }
         return m_DataTextMap.FindElement(qualifiedName);
      }

      /// <summary>
      /// Using arguments determine how to target some storage, reading the
      /// Assets info from the the DB context write the corresponding 
      /// </summary>
      /// <param name="context">provide the dictionary of types and elements to
      /// use for the schema preparation</param>
      /// <param name="arguments">parameters and directives on how to write the 
      /// schema or drive the flow</param>
      public static void WriteSchema(
         ResourceContext context, AssetConsoleArgumentsInfo arguments,
         AssetDataElementList assets)
      {
         IWriter writer = new FolderWriter(
            arguments.OutFilePath, arguments.OutFileName,
            arguments.OutFileExtension);

         var dataTextMap = DataTextMap.FromFile(arguments);

         // TODO: verify that Root Element Name can be used...
         //var t = context.RegisterTypes(arguments.RootElementName);
         XsdWriter wr = new XsdWriter(context, dataTextMap);
         wr.PrepareSchemas(assets);

         writer.Open();
         wr.WriteSet(writer);
         writer.Close();
      }

   }

}
