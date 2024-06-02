using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetReport;
using Edam.Data.AssetSchema;
using Edam.DataObjects.Locations;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.AssetUseCases
{

   public class AssetUseCaseReport
   {

      #region -- 1.00 - Fields and Properties 

      public const string USE_CASE_HEADER = "UseCase";
      public const string USE_CASES_LABEL = "Use Cases";

      private ReportInfo m_Report = new ReportInfo();
      public ReportInfo Report
      {
         get { return m_Report; }
      }

      #endregion
      #region -- 1.50 - Constructore

      public AssetUseCaseReport(AssetUseCaseList list)
      {
         m_Report.UseCases = list;
      }

      #endregion
      #region -- 4.00 - Helper methods

      public static string GetUseCaseHeader()
      {
         return "," + USE_CASE_HEADER;
      }

      #endregion
      #region -- 4.00 - Use Case Workbook Support - with Processing Instructions

      /// <summary>
      /// Append Use Case Asset Mappings and Processing instructions if any...
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="useCaseElement"></param>
      private static void AppendUseCaseProcessingInstructions(
         ITableBuilder builder, AssetDataElement element,
         TableRowHeaderInfo columns)
      {
         if (element.ProcessInstructionsBag == null)
         {
            for (var i = 0; i < columns.Items.Count; i++)
            {
               builder.AppendRowCell(null);
            }
            return;
         }

         // write columns in the order specified by the columns 
         foreach (var col in columns.Items)
         {
            foreach (var c in element.ProcessInstructionsBag.Items)
            {
               if (c.Column.Name == col.Name)
               {
                  builder.AppendRowCell(c.Value);
                  break;
               }
            }
         }
      }

      /// <summary>
      /// Append Use Cases into the related TAB
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="report">report details dependent on its implementation
      /// </param>
      public static void AppendUseCases(
         ITableBuilder builder, ReportInfo report)
      {
         // TODO: remove hardcoded (3) value...
         builder.AddColumns(hidden: true, count: 3);
         builder.AddWorksheet(USE_CASES_LABEL);

         string ucHeader = report.ReportHeader + GetUseCaseHeader();
         builder.AppendMainHeader(
            report.UseCaseColumns.ToList(), ucHeader);

         // write data
         foreach (var c in report.UseCases)
         {
            foreach (var i in c.Items)
            {
               AssetReportBuilder.AppendTableCells(builder, i);
               builder.AppendRowCell(i.UseCaseName);
               AppendUseCaseProcessingInstructions(
                  builder, i, report.UseCaseColumns);
               builder.AppendRowCellLast(null);
            }
         }
      }

      #endregion
      #region -- 4.00 - Use Case Map Item Support

      public static int SetupMapItemsHeader(
         ITableBuilder builder, ReportInfo report)
      {
         // prepare left side column header... (light blue)
         TableRowHeaderInfo columns = new TableRowHeaderInfo();
         var itemsHeader = columns.AddCommaDelimitedHeader(
            report.ReportHeader, false, (uint)TableRowStyle.Fill3Border1Font14);

         for(var i = 0; i < 3; i++)
         {
            itemsHeader[i].Hidden = true;
         }

         // add middle Use Case Header... (light gray)
         var useCaseHeader = columns.Add(USE_CASE_HEADER);
         useCaseHeader.StyleNo = (uint) TableRowStyle.Fill2Border1Font14;

         // append right side column headers... (light yellow)
         itemsHeader = columns.AddCommaDelimitedHeader(
            report.ReportHeader, false, (uint)TableRowStyle.Fill4Border1Font12);

         for (var i = 0; i < 3; i++)
         {
            itemsHeader[i].Hidden = true;
         }

         // renumber indexes...
         int indx = 1;
         foreach(var c in columns.Items)
         {
            c.Index = indx;
            indx++;
         }

         // add Description and Instructions columns
         columns.Add(indx, "Description", false, 
            (uint)TableRowStyle.Fill2Border1Font14);
         indx++;
         columns.Add(indx, "Instructions", false,
            (uint)TableRowStyle.Fill2Border1Font14);

         // add columns
         builder.AppendTabColumnsRow(USE_CASES_LABEL, columns);

         /*
         // add hidden columns...
         builder.AddColumns(hidden: true, count: 3);

         // add work-sheet named "USE CASES"...
         builder.AddWorksheet(USE_CASES_LABEL);

         // add report header...
         string ucHeader = report.ReportHeader + GetUseCaseHeader();
         builder.AppendMainHeader(
            report.UseCaseColumns.ToList(), ucHeader);
          */

         return itemsHeader.Count;
      }

      /// <summary>
      /// Append Use Cases into the related TAB
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="report">report details dependent on its implementation
      /// </param>
      public static void AppendUseCaseMapItems(
         ITableBuilder builder, ReportInfo report)
      {
         int targetCount = SetupMapItemsHeader(builder, report);

         // write data
         string desc, inst, mapId = null;
         foreach (var c in report.UseCases)
         {
            foreach (var i in c.MappedItems)
            {
               AssetReportBuilder.AppendTableCells(builder, i.Source);
               builder.AppendRowCell(c.Name);

               if (i.Target != null)
               {
                  AssetReportBuilder.AppendTableCells(builder, i.Target);
                  //AppendUseCaseProcessingInstructions(
                  //   builder, i.Target, report.UseCaseColumns);
               }
               else
               {
                  builder.AppendCellFillers(targetCount);
               }

               desc = mapId == i.MapItem.MapItemId ||
                  String.IsNullOrWhiteSpace(i.MapItem.Description) ?
                  String.Empty : i.MapItem.Description;
               inst = mapId == i.MapItem.MapItemId ||
                  String.IsNullOrWhiteSpace(i.MapItem.Instructions) ?
                  String.Empty : i.MapItem.Instructions;

               builder.AppendRowCell(desc);
               builder.AppendRowCell(inst);

               builder.AppendRowCellLast(null);
               mapId = i.MapItem.MapItemId;
            }
         }
      }

      #endregion
      #region -- 4.00 - (Report) To Output support

      /// <summary>
      /// To Output.
      /// </summary>
      /// <param name="file">output file information</param>
      /// <returns>report string is returned</returns>
      private string ToOutput(InOut.FileInfo file)
      {
         AssetReportBuilder b = new AssetReportBuilder();
         return b.ToWorkbookFile(file, m_Report);
      }

      /// <summary>
      /// Prepare Report given an output file, asset-data item and a list of 
      /// Use Cases.
      /// </summary>
      /// <param name="file">output file information</param>
      /// <param name="item">asset data details</param>
      /// <param name="useCases">list of Use Cases</param>
      public static void ToOutput(
         InOut.FileInfo file, AssetData item, AssetUseCaseList useCases,
         ReportOptions options = null)
      {
         AssetUseCaseReport helper = new AssetUseCaseReport(useCases);

         helper.Report.Namespaces = item.Namespaces;
         helper.Report.Items = item.Items;
         helper.Report.AssetCustomColumns = new TableRowHeaderInfo();
         helper.Report.UseCaseColumns = helper.Report.AssetCustomColumns;
         helper.Report.UseCasesMergedItems = null;
         helper.Report.UseCases = useCases;

         helper.Report.Options =
            options != null && options.UseForUseCaseReport ?
               options : null;

         helper.ToOutput(file);
      }

      #endregion

   }

}
