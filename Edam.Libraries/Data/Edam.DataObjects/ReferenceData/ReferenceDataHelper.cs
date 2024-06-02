using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;

namespace Edam.DataObjects.ReferenceData
{
   public class ReferenceDataHelper
   {
      private const string CONNECTION_STRING_KEY = 
         "ConnectionStringKey";

      public static string GetConnectionStringKey()
      {
         string connectionString = AppSettings.GetSectionString(
            CONNECTION_STRING_KEY, "ReferenceData");
         return connectionString;
      }
   }
}
