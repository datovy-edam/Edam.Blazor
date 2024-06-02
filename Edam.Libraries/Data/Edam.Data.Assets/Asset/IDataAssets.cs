using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Data.AssetConsole;

namespace Edam.Data.Asset
{
   public interface IDataAssets
   {
      IResultsLog ToDatabase(AssetConsoleArgumentsInfo arguments);
      IResultsLog ToAsset(AssetConsoleArgumentsInfo arguments);
   }
}
