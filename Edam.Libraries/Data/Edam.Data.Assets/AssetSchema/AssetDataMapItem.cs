
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// -----------------------------------------------------------------------------
using util = Edam.Serialization;
using Edam.Data.Asset;
using Edam.Data.Books;
using Edam.Data.AssetUseCases;
using Edam.Data.Lexicon;
using Edam.Data.AssetConsole;

namespace Edam.Data.AssetSchema
{

   public enum MapItemType
   {
      Unknown = 0,
      Source = 1,
      Target = 2
   }

   public class MapAnnotationInfo
   {
      public string Description { get; set; }
      public string Instructions { get; set; }
   }

   public class MapExpressionInfo
   {
      public string Language { get; set; }
      public string ExpressionText { get; set; }
   }

   public class MapNamespaceInfo
   {
      public string Prefix { get; set; }
      public string UriText { get; set; }
   }

   /// <summary>
   /// Map Item Information.
   /// </summary>
   public class MapItemInfo
   {
      public string MapItemId { get; set; }
      public string ItemId { get; set; } = Guid.NewGuid().ToString();
      public MapNamespaceInfo Namespace { get; set; } = new MapNamespaceInfo();

      [JsonIgnore]
      public object TreeItem { get; set; }

      [JsonIgnore]
      public AssetDataElement DataElement { get; set; }

      public string Name { get; set; }
      public string Path { get; set; }
      public string DisplayPath { get; set; }
      public string DisplayFullPath { get; set; }
      public string QualifiedName { get; set; }

      public MapItemType Side { get; set; } = MapItemType.Unknown;

      private MapAnnotationInfo m_Annotation;
      public MapAnnotationInfo Annotation
      {
         get { return m_Annotation ?? GetAnnotation(); }
         set { m_Annotation = value; }
      }

      /// <summary>
      /// Get Annotation.
      /// </summary>
      /// <returns>return annotation instance</returns>
      public MapAnnotationInfo GetAnnotation()
      {
         if (m_Annotation != null)
         {
            return m_Annotation;
         }

         string token = string.Empty;
         string previousToken = string.Empty;
         string outText = string.Empty;

         m_Annotation = new MapAnnotationInfo();
         var lt = Path.Split("/");

         foreach (var l in lt)
         {
            var it = l.Split(':');
            token = it.Length == 1 ? it[0] : it[1];
            token = (outText == string.Empty && token == "dbo") ?
               string.Empty : token;
            token = Edam.Text.Convert.ToProperCase(token).Trim();

            // try not to duplicate words...
            var dt = token.Split(' ');
            if (previousToken != string.Empty)
            {
               if (previousToken == dt[0])
               {
                  int cnt = 0;
                  token = string.Empty;
                  foreach (var t in dt)
                  {
                     if (cnt > 0)
                     {
                        token += (token == string.Empty) ? t : " " + t;
                     }
                     cnt++;
                  }
               }
            }
            previousToken = dt[0];

            // prepare out text... appending found words/tokens
            outText += (outText == string.Empty) ? token : " " + token;
         }

         m_Annotation.Description = outText;

         return m_Annotation;
      }

   }

   /// <summary>
   /// Define a collection of source and target items that are used to define
   /// a particular Use Case mapping requirement.
   /// </summary>
   public class AssetDataMapItem
   {
      public string MapItemId { get; set; } = Guid.NewGuid().ToString();

      public string ItemPath { get; set; }

      public List<MapItemInfo> SourceElement { get; set; }
      public List<MapItemInfo> TargetElement { get; set; }

      public string Description { get; set; }
      public string Instructions { get; set; }

      public List<MapExpressionInfo> Expresions { get; set; }

      public AssetDataMapItem()
      {
         SourceElement = new List<MapItemInfo>();
         TargetElement = new List<MapItemInfo>();
         Expresions = new List<MapExpressionInfo>();
         Description = String.Empty;
         Instructions = String.Empty;
      }
   }

   public interface IDataMapContext
   {
      object Instance { get; }
      string ContextId { get; }
      AssetUseCaseMap UseCase { get; set; }
   }

   public class AssetDataLexiconContext : IDataMapContext, ILexiconContext
   {

      private AssetConsoleArgumentsInfo m_Arguments = null;

      public object Instance { get; set; }
      public string ContextId { get; set; }
      public AssetUseCaseMap UseCase { get; set; }

      public LexiconSettingsInfo LexiconSettings { get; set; }
      public ILexiconData LexiconData { get; set; }

      /// <summary>
      /// Set Lexicon based on provided arguments set.
      /// </summary>
      /// <param name="arguments">(optional) console arguments, if none is 
      /// given the current project arguments will be used</param>
      public void SetLexicon(AssetConsoleArgumentsInfo arguments)
      {
         if (arguments != null && arguments.Lexicon != null)
         {
            if (m_Arguments == null || m_Arguments.Lexicon == null  ||
               m_Arguments.Lexicon.LexiconId != arguments.Lexicon.LexiconId)
            {
               m_Arguments = arguments;
               LexiconData = LexiconHelper.GetLexiconDataInstance();
               var data = LexiconData.EnsureLoad(arguments);
            }
         }
      }

   }

}
