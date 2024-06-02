using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Net;
using Edam.Net.Web;
using Edam.Diagnostics;
using Edam.Serialization;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Edam.Data.Assets.Dictionary;

namespace Edam.Data.Dictionary.Api
{

   public class FreeDictionaryApi : IDictionaryApi
   {
      public const string BASE_URI = 
         "https://api.dictionaryapi.dev/api/v2/entries/en/";

      private const string TO_MANY_REQUESTS = 
         "Too many requests, please try again later.";

      private WebApiClient? m_Client = null;
      private ResultsLog<ITermInfo?> m_Results = new ResultsLog<ITermInfo?>();
      private LookUpResultInfo m_LookupResults = new LookUpResultInfo();

      public FreeDictionaryApi()
      {

      }
      public FreeDictionaryApi(DictionaryContext context)
      {
         m_LookupResults.Context = context;
      }

      /// <summary>
      /// Set Context as needed.
      /// </summary>
      /// <returns>context is returned</returns>
      public DictionaryContext SetContext()
      {
         if (m_LookupResults.Context == null)
         {
            m_LookupResults.Context = new DictionaryContext();
            m_LookupResults.Context.Database.EnsureCreated();
         }
         return m_LookupResults.Context;
      }

      /// <summary>
      /// Return dictionaries interface that allow adding, removing, and 
      /// finding items
      /// </summary>
      /// <returns>instance of dictionaries instance is returned if any 
      /// is available</returns>
      public IDictionaries? GetDictionaries()
      {
         SetContext();
         return m_LookupResults == null ? null : m_LookupResults.Context;
      }

      /// <summary>
      /// Initialize HTTP Client for the FreeDictionaryApi.
      /// </summary>
      private void InitializeClient()
      {
         HttpRequestInfo request = new HttpRequestInfo();
         request.BaseUri = BASE_URI;
         request.ContentType = WebApiContentType.ApplicationJson;

         m_Client = new WebApiClient(request);
      }

      /// <summary>
      /// Get dictionary entry for given term.
      /// </summary>
      /// <param name="term">term to look-up</param>
      /// <returns>ITermInfo is returned</returns>
      private ITermInfo GetData(string term)
      {
         string uri = BASE_URI + term;
         string data = m_Client.GetDataAsText(uri);
         if (data == TO_MANY_REQUESTS)
         {
            ITermInfo info = new TermInfo();
            info.Term = term;
            info.Status = EntryStatus.Unavailable;
            return info;
         }
         return Parse(data);
      }

      /// <summary>
      /// Parse JSON document.
      /// </summary>
      /// <param name="jsonText">JSON text</param>
      /// <returns>ITermInfo return if something was found</returns>
      public ITermInfo? Parse(string jsonText)
      {
         //var result = jsonText.IndexOf("No Definitions");
         if (jsonText.IndexOf("No Definitions") > 0)
         {
            return null;
         }

         var tjson = "{ \"info\": " + jsonText + "}";
         JObject jobj = JsonSerializer.ToDynamic(tjson);
         JToken? tobj = jobj["info"][0];
         string word = tobj["word"].ToString();
         JToken? meaning = tobj["meanings"][0];

         string partOfSpeech = meaning["partOfSpeech"].ToString();

         JToken? def = meaning["definitions"][0];
         string definition = def["definition"].ToString();

         TermInfo t = new TermInfo();
         t.Term = word;
         t.OriginalTerm = word;
         t.Category = partOfSpeech;
         t.Definition = definition;
         t.Soundex = Edam.Strings.TextString.Soundex(word);
         t.Confidence = 1.0M;

         return t;
      }

      /// <summary>
      /// Lookup the given term.
      /// </summary>
      /// <param name="term">term to look-up</param>
      /// <returns>results log is returned</returns>
      private ResultsLog<ITermInfo?> LookUpOne(string term)
      {
         ResultsLog<ITermInfo?> results = new ResultsLog<ITermInfo?>();

         try
         {
            if (m_Client == null)
            {
               InitializeClient();
            }
            results.Data = GetData(term);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            m_Results.Failed(ex);
            results.Failed(ex);
         }

         return results;
      }

      /// <summary>
      /// Lookup the given term.
      /// </summary>
      /// <param name="term">term to look-up</param>
      /// <returns>results log is returned</returns>
      public ResultsLog<ITermInfo?> LookUp(string term)
      {
         var d = LookUpOne(term);
         m_Results.Succeeded();
         return d;
      }

      /// <summary>
      /// Given a Term information look-it-up in dictionary.
      /// </summary>
      /// <param name="term">term to look-up</param>
      /// <returns>results log is returned</returns>
      public ResultsLog<ITermInfo?> LookUp(ITermInfo term)
      {
         var results = LookUpOne(term.Term);
         if (results.Success && results.Data != null)
         {
            results.Data.KeyID = term.KeyID;
            results.Data.Soundex = term.Soundex;
         }
         return results;
      }

      /// <summary>
      /// Go through the list of terms and get all definitions.
      /// </summary>
      /// <param name="terms">list of ITermInfoes</param>
      /// <param name="callBack">(optional) callBack function</param>
      /// <returns>returns results</returns>
      public ILookUpResult LookUp(
         List<ITermInfo> terms, Action<ILookUpResult> callBack = null,
         int topCount = 0)
      {
         m_LookupResults.ResultsLog = m_Results;
         var client = m_Client;
         int sleepMilliSeconds = 300;

         try
         {
            int count = 0;
            foreach (var term in terms)
            {
               if (!String.IsNullOrWhiteSpace(term.Definition) ||
                  term.Status != EntryStatus.Unknown)
               {
                  continue;
               }

               if (count > topCount && topCount != 0)
               {
                  break;
               }
               count++;

               var results = LookUp(term);
               if (results.Success)
               {
                  m_LookupResults.CurrentTerm = term;
                  m_LookupResults.Acknowleged++;

                  if (results.Data != null)
                  {
                     if (results.Data.Status == EntryStatus.Unavailable)
                     {
                        continue;
                     }

                     results.Data.Status = EntryStatus.Found;
                     m_LookupResults.Found++;
                     m_LookupResults.ProcessedTerm = results.Data;
                  }
                  else
                  {
                     term.Status = EntryStatus.NotFound;
                     m_LookupResults.NotFound++;
                     m_LookupResults.ProcessedTerm = term;
                  }

                  if (callBack != null)
                  {
                     callBack(m_LookupResults);
                  }
               }

               System.Threading.Thread.Sleep(sleepMilliSeconds);
            }

            m_LookupResults.Context.SaveChanges();
         }
         catch(Exception ex)
         {
            m_LookupResults.ResultsLog.Failed(ex);
         }
         finally
         {
            if (client == null)
            {
               Dispose();
            }
         }
         return m_LookupResults;
      }

      /// <summary>
      /// Process looked-up entry.
      /// </summary>
      /// <param name="results">found look-up entry</param>
      private void ProcessLookUpEntry(ILookUpResult results)
      {
         DictionaryContextHelper.UpsertQueueEntry(
            (DictionaryContext)results.ContextInstance, results.ProcessedTerm);
      }

      /// <summary>
      /// Look Up all dictionary entries in the database.
      /// </summary>
      /// <param name="callBack">(optional) call back function</param>
      public void LookUp(
         Action<ILookUpResult> callBack = null, int topCount = 0)
      {
         var cback = callBack ?? ProcessLookUpEntry;
         var context = SetContext();
         var entries = context.WordQueue.ToList<ITermInfo>();
         FreeDictionaryApi api = new FreeDictionaryApi(context);
         api.LookUp(entries, cback, topCount);
         Dispose();
         context.Dispose();
      }

      /// <summary>
      /// Dispose of unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         if (m_Client != null)
         {
            m_Client.Dispose();
            m_Client = null;
         }
      }

   }

}
