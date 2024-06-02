using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Lexicon
{

   public class LexiconContextHelper
   {

      public const string LEXICON_CONNECTION_STRING = "LexiconConnectionString";

      /// <summary>
      /// Get Connection String associated with default application settings key
      /// associated with the Connection String.
      /// </summary>
      /// <returns>returns the connection string</returns>
      public static string GetConnectionString()
      {
         return Application.AppSettings.
            GetConnectionString(LEXICON_CONNECTION_STRING);
      }


   }

}
