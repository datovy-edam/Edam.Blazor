using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.Data.AssetUseCases;
using Edam.DataObjects.DataCodes;
using Edam.Text;

namespace Edam.Data.AssetReport
{

   /// <summary>
   /// Asset Report Information
   /// </summary>
   public class ReportInfo : ITableReport
   {

      public List<NamespaceInfo> Namespaces { get; set; }

      // data element to report about
      public List<AssetDataElement> Items { get; set; }

      // to report on the code sets...
      public List<AssetDataElement> CodeSetItems { get; set; }

      /// <summary>
      /// Row Header
      /// </summary>
      public ITableRowHeader RowHeader { get; set; }

      // custom columns to support
      public TableRowHeaderInfo AssetCustomColumns { get; set; }

      public bool PrepareNamespacesTab { get; set; }
      public bool PrepareEnumSummaryTab { get; set; }
      public bool PrepareEnumTabs { get; set; }

      /// <summary>
      /// Output Fully Qualified Names...
      /// </summary>
      public bool OutputFullyQualifiedNames { get; set; } = true;

      /// <summary>
      /// header (title) items separated by commas
      /// </summary>
      public string ReportHeader { get; set; }

      /// <summary>
      /// Report Options...
      /// </summary>
      public ITableReportOptions Options { get; set; }

      /// <summary>
      /// List of all Use Cases
      /// </summary>
      public AssetUseCaseList UseCases { get; set; }

      /// <summary>
      /// List of Reporting Elements / Column - Headers
      /// </summary>
      public TableRowHeaderInfo UseCaseColumns { get; set; }

      /// <summary>
      /// This are the Use Case merged items...
      /// </summary>
      public List<AssetUseCaseElement> UseCasesMergedItems { get; set; }

      /// <summary>
      /// (Optional) Row Header JSON file path definition.
      /// </summary>
      public string RowHeaderJsonFilePath { get; set; } = null;

      public bool IsCustomReport
      {
         get
         {
            return Options != null && 
               !String.IsNullOrWhiteSpace(Options.RowHeaderFilePath);
         }
      }

      public ReportInfo()
      {
         PrepareNamespacesTab = false;
         PrepareEnumSummaryTab = false;
         PrepareEnumTabs = false;
         CodeSetItems = new List<AssetDataElement>();
      }

   }

}
