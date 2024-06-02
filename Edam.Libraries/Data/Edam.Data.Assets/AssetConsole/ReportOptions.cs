using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Text;

namespace Edam.Data.AssetConsole
{

   public class ReportOptions : ITableReportOptions
   {
      public string RowHeaderFilePath { get; set; }
      public string TypeMapFilePath { get; set; }

      /// <summary>
      /// true to use show fully qualified names.
      /// </summary>
      public bool ShowFullyQualifiedNames { get; set; } = true;

      /// <summary>
      /// true to use the RowHeader custom headers instead of defaults.
      /// </summary>
      public bool UseForAssetReport { get; set; } = true;

      /// <summary>
      /// true to use the RowHeader custom headers instead of defaults
      /// for the UseCase report.
      /// </summary>
      public bool UseForUseCaseReport { get; set; } = false;

      /// <summary>
      /// 
      /// </summary>
      public bool OnlyUseCaseEntries { get; set; } = false;

      /// <summary>
      /// true to set elements that are type as an entity... will show type
      /// name in the Entity Column not the Element Column.
      /// </summary>
      public bool SetElementTypeAsEntity { get; set; } = false;

   }

}
