using System;
using System.Net;

// -----------------------------------------------------------------------------

namespace Edam // copied from Kif
{

   /// <summary>
   /// Supported Locale Languages
   /// </summary>
   public enum LocaleLanguage
   {
      Unknown = 99,
      Spanish = 0,
      English = 1
   }  // end of LocaleLanguage

   /// <summary>
   /// Define details about a language and related culture...
   /// </summary>
   public partial struct LanguageInfo
   {
      private LocaleLanguage m_Language;
      private String m_CultureId;
      private String m_LanguageName;

      public LocaleLanguage Language
      {
         get { return m_Language; }
      }
      public String CultureId
      {
         get { return m_CultureId; }
      }
      public String LanguageName
      {
         get { return m_LanguageName; }
      }

      public LanguageInfo(LocaleLanguage locale, String cultureId, String name)
      {
         m_Language = locale;
         m_CultureId = cultureId;
         m_LanguageName = name;
      }

      /// <summary>
      /// Get Language Number related to text...
      /// </summary>
      /// <param name="language">language Text (spa, spanish, español,
      /// esp, es-PR, es = for Spanish (0))</param>
      /// <returns>corresponding Language No. is returned</returns>
      static public Int16 GetLanguageNo(String language)
      {
         if (String.IsNullOrEmpty(language))
            language = "spa";

         String lang = language.ToLower();
         if ((lang == "es-pr") || (lang == "esp") ||
             (lang == "español") || (lang == "espanol") ||
             (lang == "es") || (lang == "0"))
            language = "spa";
         else
            language = "eng";

         language = language.ToLower();
         Int16 langNo = (Int16)((language.Substring(0, 1) == "s") ? 0 : 1);
         return langNo;
      }  // end of GetLanguageNo

      /// <summary>
      /// Given a Language No. return corresponding culture-ID. String.
      /// </summary>
      /// <param name="languageNo">language No. to get Id of.</param>
      /// <returns>the mapped no. to id. text representation is returned
      /// </returns>
      static public String GetLanguage(Int16 languageNo)
      {
         String l = LanguageCollection.Items[0].CultureId;
         if (languageNo == (Int16)LocaleLanguage.Spanish)
            l = LanguageCollection.Items[0].CultureId;
         else
            l = LanguageCollection.Items[1].CultureId;
         return l;
      }  // end of GetLanguage

   }  // end of LanguageInfo

   /// <summary>
   /// Helper to manage supported Language selection...
   /// </summary>
   public class LanguageCollection
   {
      static public readonly LanguageInfo [] Items =
      {
         new LanguageInfo(LocaleLanguage.Spanish, "es-PR", "Español"),
         new LanguageInfo(LocaleLanguage.English, "en-US", "English")
      }  ;
      static public Int32 Count
      {
         get { return Items.Length; }
      }

   }  // end of LanguageCollection

}  // end of Kif // copied from Kif
