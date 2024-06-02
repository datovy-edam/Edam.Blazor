using DocumentFormat.OpenXml.Wordprocessing;
using Edam.Application;
using Edam.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using languages = Edam.Language;

namespace Edam.Data.Lexicon.Semantics
{

   public class TextSimilarityInstance : IDisposable, ITextSimilarityInstance
   {

      public const string FAILED_DEPENDENCIES_LOADING = 
         "Failed Loading Interpreter Dependencies";
      public const string FAILED_FETCH_SCORES = "Failed Fetching Scores";

      public const string SCRIPT_NAME = "semanticTextSimilarity";
      public const string TEXT2_METHOD_NAME = "get_TextSimilarityScore";

      private static languages.IInterpreter? _Interpreter;

      public TextSimilarityInstance()
      {
         LoadDependencies();
      }

      /// <summary>
      /// Load Dependencies.
      /// </summary>
      public static void LoadDependencies()
      {
         if (_Interpreter == null)
         {
            _Interpreter = languages.LanguageHelper.GetPythonLanguageInstance();
         }
      }

      /// <summary>
      /// Get default Semantic Similarities Module.
      /// </summary>
      /// <returns>module information is returned</returns>
      public static ModuleInfo GetSemanticSimilaritiesModule()
      {
         ModuleInfo mod = new ModuleInfo();
         mod.ScriptName = SCRIPT_NAME;
         mod.MethodName = TEXT2_METHOD_NAME;
         return mod;
      }

      /// <summary>
      /// Run Script and return results.
      /// </summary>
      /// <param name="scriptName">python script name</param>
      /// <param name="functionName">function name</param>
      /// <param name="parameters">parameters for function</param>
      public IResultsLog? ExecuteScript(
         string scriptName, string functionName,
         languages.Parameters? parameters = null)
      {
         IResultsLog? results = null;
         if (_Interpreter == null)
         {
            LoadDependencies();
            if (_Interpreter == null)
            {
               IResultsLog resultsLog = new ResultLog();
               resultsLog.Failed(FAILED_DEPENDENCIES_LOADING);
               return resultsLog;
            }
         }

         results = _Interpreter.ExecuteScript(
            scriptName, functionName, parameters);

         ResultLog rlog = new ResultLog();
         rlog.Copy(results);
         rlog.ResultValueObject = results.ResultValueObject != null ?
            new TextSimilarityScoreInfo(results.ResultValueObject) : null;

         return rlog;
      }

      /// <summary>
      /// Run semantic similarity beetween to given sentences.
      /// </summary>
      /// <remarks>module information is optional and if not provided the
      /// default script and module (to compare 2 sentences) is instanced
      /// </remarks>
      /// <param name="text1">text 1</param>
      /// <param name="text2">text 2</param>
      /// <param name="module">module info (optional)</param>
      /// <returns>comparison results are returned</returns>
      public static ITextSimilarityScore ExecuteScript( 
         string text1, string text2, ModuleInfo? module = null)
      {
         TextSimilarityScoreInfo scores;
         ModuleInfo? mod = module == null ? 
            GetSemanticSimilaritiesModule() : module;
         mod.MethodName = TEXT2_METHOD_NAME;
         mod.PrepareParameters(text1, text2);

         // run script...
         var script = new TextSimilarityInstance();
         var results = script.ExecuteScript(
            mod.ScriptName, mod.MethodName, mod.Parameters);

         // any results?
         if (results == null || !results.Success)
         {
            IResultsLog resultsLog = 
               results == null ? new ResultLog() : results;
            resultsLog.Failed(FAILED_FETCH_SCORES);
            scores = new TextSimilarityScoreInfo(null);
            scores.Results = resultsLog;
            return scores;
         }

         // prepare scores to return
         scores = new TextSimilarityScoreInfo((dynamic?)results.DataObject);
         scores.Results = results;

         return scores;
      }

      /// <summary>
      /// Release allocated resources.
      /// </summary>
      public void Dispose()
      {
         if (_Interpreter != null)
         {
            _Interpreter.Dispose();
            _Interpreter = null;
         }
      }

   }

}
