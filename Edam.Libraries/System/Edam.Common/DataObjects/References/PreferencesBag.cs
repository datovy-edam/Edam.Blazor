using System;
using System.Collections.Generic;
//using System.Data.Common;

// -----------------------------------------------------------------------------
//using DbField = Edam.Data.DataField;

namespace Edam.DataObjects.References
{

   public class PreferencesBag
   {

      public String Value { get; set; }

      public PreferencesBag()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         Value = String.Empty;
      }

      /// <summary>
      /// Read Data...
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>instance of UserLoggedInfo is returned</returns>
      //public static PreferencesBag ReadData(DbDataReader reader)
      //{
      //   PreferencesBag b = new PreferencesBag();
      //   b.Value = DbField.GetString(reader[0]);
      //   return b;
      //}

   }

}
