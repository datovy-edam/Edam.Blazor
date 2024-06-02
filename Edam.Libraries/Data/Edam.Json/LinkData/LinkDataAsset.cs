using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetConsole;

namespace Edam.Json.LinkData
{

   public class LinkDataAsset
   {

      public static void ToFile(LinkData item)
      {

      }

      public static IResultsLog WriteText(string fileName, string payload,
         InOut.FileInfo outFile, IResultsLog results)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         sb.AppendLine("{");
         sb.AppendLine("  \"@context\": {");

         sb.Append(payload);

         sb.AppendLine("   }");
         sb.AppendLine("}");
         string outText = sb.ToString();

         InOut.FolderWriter fwriter = new InOut.FolderWriter(
            outFile.Path, outFile.Name, outFile.Extension);
         fwriter.Open();
         fwriter.Write(fileName + "Context", outText);
         fwriter.Close();

         return results;
      }

      public static IResultsLog ToLinkData(List<AssetData> assets,
         InOut.FileInfo outFile)
      {
         IResultsLog results = new ResultLog();
         LinkData ld = new LinkData();

         // prepare link-data list for vocabularies
         foreach (var dataAssets in assets)
         {
            foreach (var asset in dataAssets.Items)
            {
               ld.Add(asset);
            }
         }

         List<string> nsItems = new List<string>();
         List<string> sbAll = new List<string>();
         System.Text.StringBuilder nsAll = new System.Text.StringBuilder();

         List<List<LinkDataItemInfo>> items = ld.GetDictionaryItems();
         foreach (var item in items)
         {
            List<LinkDataItemInfo> olist = LinkData.SortByNamespace(item);
            NamespaceInfo ns = new NamespaceInfo();

            int count = 0, total = olist.Count;
            if (total == 0)
            {
               continue;
            }

            var sb = new StringBuilder();
            foreach (var i in olist)
            {
               if (ns.Uri.AbsolutePath != i.Namespace.Uri.AbsolutePath)
               {
                  ns = i.Namespace;
                  string nsText = "      \"" +
                     ns.Prefix + "\": \"" + ns.Uri.AbsoluteUri + "/#\"" +
                     (count < total ? "," : String.Empty);

                  var nsItem = nsItems.Find((x) => x == nsText);
                  if (nsItem == null)
                  {
                     nsAll.AppendLine(nsText);
                     nsItems.Add(nsText);
                  }

                  sb.AppendLine(nsText);
               }
               count++;

               string iname = LinkData.GetCamelCaseName(i.ItemName);

               var comma = (count < total ? "," : String.Empty);
               string ldText = "      \"" + iname + "\": \"" +
                  i.LinkName + "\"" + comma;
               sb.AppendLine(ldText);
               sbAll.Add(ldText + (comma == String.Empty ? "," : String.Empty));
            }

            // write individual type context / vocabulary entries
            WriteText(olist[0].EntityName.Replace(":", String.Empty),
               sb.ToString(), outFile, results);
         }

         // write all context URIs and vocabulary entries...
         nsAll.AppendLine();
         int itmCnt = 0;
         foreach (var i in sbAll)
         {
            itmCnt++;
            string lineText = itmCnt < sbAll.Count ? i : i.Replace(",", "");
            nsAll.AppendLine(lineText);
         }

         WriteText("All", nsAll.ToString(), outFile, results);

         results.Succeeded();
         return results;
      }

      public static IResultsLog ToLinkData(AssetConsoleArgumentsInfo arguments)
      {
         string outFileName = arguments.OutputFile.Name;
         IResultsLog results = 
            ToLinkData(arguments.AssetDataItems, arguments.OutputFile);

         foreach(var a in arguments.AssetDataItems)
         {
            if (a.UseCases == null)
            {
               continue;
            }
            foreach(var uc in a.UseCases)
            {
               List<AssetData> ucAsset = new List<AssetData>();
               AssetData ad = new AssetData(arguments.Namespace, 
                  AssetType.Instance, arguments.ProjectVersionId);
               ad.Items = uc.Items;
               ad.Namespaces = uc.Namespaces;
               ucAsset.Add(ad);

               arguments.OutputFile.Name = uc.Name + "_" + outFileName;
               results = ToLinkData(ucAsset, arguments.OutputFile);
            }
         }
         return results;
      }

   }

}
