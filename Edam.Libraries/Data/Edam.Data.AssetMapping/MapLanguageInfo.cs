using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Data.Asset;
using Edam.Json.JsonQuery;
using Edam.Diagnostics;

namespace Edam.Data.AssetMapping
{

   /// <summary>
   /// Mapping Language support/helpers...
   /// </summary>
   public class MapLanguageInfo : IMapLanguageInfo
   {

      public string Name { get; } = MapLanguageHelper.DEFAULT_LANGUAGE;

      /// <summary>
      /// Given a name path get the path in the JSONata language.
      /// </summary>
      /// <param name="originalPath">orignal path</param>
      /// <returns>JSONata path</returns>
      public string GetPath(string originalPath)
      {
         NamePath path = NamePath.Parse(originalPath);
         return path.Join(NamePath.DELIMITER_DOT);
      }

      /// <summary>
      /// Join header and tail for current language.
      /// </summary>
      /// <param name="header">header string</param>
      /// <param name="tail">tail string</param>
      /// <returns>joined string is returned</returns>
      public string Join(string header, string tail)
      {
         return header + NamePath.DELIMITER_DOT + tail;
      }

      /// <summary>
      /// Get Default Language Name.
      /// </summary>
      /// <returns>default Language name is returned for this instance</returns>
      public string GetDefaultLanguageName()
      {
         return MapLanguageHelper.DEFAULT_LANGUAGE;
      }

      /// <summary>
      /// Get Default Empty Code Text...
      /// </summary>
      /// <returns>returns a text string with the default code text</returns>
      public string GetDefaultEmptyCodeText()
      {
         return "{\r\n\r\n}";
      }

      /// <summary>
      /// Execute given JSONata code on current selected Language
      /// as found in query.
      /// </summary>
      /// <param name="jsonDocumentText">JSON document as text</param>
      /// <param name="query">query/code to execute</param>
      /// <returns>results of execution are returned, see DataObject for those
      /// </returns>
      public IResultsLog Execute(string jsonDocumentText, string query)
      {
         return JsonQuery.Execute(jsonDocumentText, query);
      }

   }

}
