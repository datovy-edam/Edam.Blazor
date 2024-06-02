using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Diagnostics;

namespace Edam.Data.AssetMapping
{

   public interface IMapLanguageInfo
   {
      string Name { get; }
      string GetDefaultLanguageName();
      string GetDefaultEmptyCodeText();
      string GetPath(string originalPath);
      string Join(string header, string tail);
      IResultsLog Execute(string jsonText, string query);
   }

}
