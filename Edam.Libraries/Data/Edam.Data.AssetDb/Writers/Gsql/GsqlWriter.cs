using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Gsql;
using Edam.Data.Asset;
using Edam.Data.AssetManagement.Resources;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.InOut;

namespace Edam.Data.AssetManagement.Writers.Gsql
{

   public class GsqlWriter : SchemaWriterAbstract<ResourceContext>
   {

      private List<GsqlSchema> m_Schemas = new List<GsqlSchema>();
      private DataTextMap m_DataTextMap;

      public GsqlWriter(ResourceContext context)
         : base(context)
      {
         m_Context = context;
      }

      public void PrepareSchemas()
      {
         NamespaceList nsl = new NamespaceList();
         nsl.AddRange(m_Context.Namespaces);
         foreach (var ns in nsl)
         {
            var sw = new GsqlSchemaWriter(m_Context, ns);
            sw.SetDataTextMap(m_DataTextMap);
            sw.WriteStart();
            sw.WriteElements();
            sw.WriteEnd();

            sw.WriteDocument();

            m_Schemas.Add(sw.Schema);
         }
      }

      public void SetDataTextMap(string mapFilePath)
      {
         m_DataTextMap = DataTextMap.FromFile(mapFilePath);
      }

      public void WriteSet(IWriter writer)
      {
         foreach (var s in m_Schemas)
         {
            String fname =
               NamespaceInfo.UriToFileName(
                  s.Namespace.Uri.OriginalString);
            writer.Write(fname, s.ToString());
         }
      }

      /// <summary>
      /// Using arguments determine how to target some storage, reading the
      /// Assets info from the the DB context write the corresponding 
      /// </summary>
      /// <remarks>
      /// Make sure that arguments AssetDataItems had been merged into one and
      /// therefore it is expected that those will be available in the first
      /// element of this collection.
      /// </remarks>
      /// <param name="arguments">arguments</param>
      public static void WriteSchema(AssetConsoleArgumentsInfo arguments)
      {
         IWriter writer = new FolderWriter(
            arguments.OutFilePath, arguments.OutFileName,
            arguments.OutFileExtension);

         // it is assumed that assets had been merged
         var dataAsset = arguments.AssetDataItems[0];
         dataAsset.SetDefaultNamespace(arguments.Namespace);
         ResourceContext context = new ResourceContext(dataAsset);

         GsqlWriter wr = new GsqlWriter(context);

         wr.SetDataTextMap(arguments.TextMapFilePath);
         wr.PrepareSchemas();

         writer.Open();
         wr.WriteSet(writer);
         writer.Close();
      }

   }

}
