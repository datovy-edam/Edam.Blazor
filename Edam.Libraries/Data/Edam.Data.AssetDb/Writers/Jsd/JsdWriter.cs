using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.Json.Jsd;
using Edam.Data.Asset;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetConsole;
using Edam.InOut;

namespace Edam.Data.AssetManagement.Writers.Jsd
{

   public class JsdWriter : SchemaWriterAbstract<ResourceContext>
   {

      private readonly List<JsdSchema> m_Schemas = new List<JsdSchema>();

      public JsdWriter(ResourceContext context)
         : base(context)
      {
         m_Context = context;
      }

      public void PrepareSchemas(bool jsonLdIndicator = false)
      {
         foreach (var ns in m_Context.Namespaces)
         {
            var sw = new JsdSchemaWriter(m_Context, ns, jsonLdIndicator);
            sw.WriteStart();
            sw.WriteComplexTypes();
            sw.WriteElements();
            sw.WriteEnd();

            sw.WriteDocument();

            m_Schemas.Add(sw.Schema);
         }
      }

      public void WriteSet(IWriter writer)
      {
         JsdSet jset = new JsdSet(writer);
         foreach (var i in m_Schemas)
         {
            foreach (var n in i.Namespaces)
            {
               i.Instance.Namespaces.Add(n.Prefix, n.Uri.OriginalString);
               if (i.Namespace.Prefix == n.Prefix)
                  continue;
               //i.Instance.Includes.Add(GetImport(n));
            }
            jset.AddSchema(i);
         }

         jset.CompileSchemas((e) =>
         {
            return e.Message;
         });
         jset.GenerateSchemas(m_Context.Namespaces);
      }

      /// <summary>
      /// Reading Data Assets / Dictionaries Write JSON shema using given
      /// IWriter.
      /// </summary>
      /// <param name="context">Assets DB context</param>
      /// <param name="writer">instance of a Writer</param>
      public static void WriteJsd(ResourceContext context, IWriter writer)
      {
         JsdWriter wr = new JsdWriter(context);
         wr.PrepareSchemas();

         writer.Open();
         wr.WriteSet(writer);
         writer.Close();
      }

      /// <summary>
      /// Using arguments determine how to target some storage, reading the
      /// Assets info from the the DB context write the corresponding 
      /// </summary>
      /// <param name="arguments"></param>
      public static void WriteSchema(AssetConsoleArgumentsInfo arguments,
         FileInfo outFile = null)
      {
         if (outFile == null)
         {
            outFile = arguments.OutputFile;
         }

         IWriter writer = new FolderWriter(
            outFile.Path, outFile.Name, outFile.Extension);

         // TODO: dependency to EF Context was removed, remove need for context
         ResourceContext context = ResourceContext.GetContext(arguments);
         JsdWriter wr = new JsdWriter(context);
         wr.PrepareSchemas(arguments.Procedure == AssetConsoleProcedure.AssetsToJld);

         writer.Open();
         wr.WriteSet(writer);
         writer.Close();
      }

   }

}

