using System;

// -----------------------------------------------------------------------------
// Copied from the Open Knowledged Kif Library...

namespace Edam  // Kif
{

/// <summary>
/// Helper class to manage the creation of unique names.
/// </summary>
public class Names
{

   private static Int32 uniqueCounter;

   static Names()
   {
      uniqueCounter = -1;
   }

   /// <summary>Create a unique id...</summary>
   /// <returns>Created unique text is returned (length=26chars)</returns>
   public static String CreateUniqueId()
   {
      uniqueCounter++;
      if (uniqueCounter >= 1000000)
         uniqueCounter = 0;

      // prepare a unique name...
      String uName =
         DateTime.Now.Year.ToString("d4")
      +  DateTime.Now.Month.ToString("d2")
      +  DateTime.Now.Day.ToString("d2")
      +  DateTime.Now.Hour.ToString("d4")
      +  DateTime.Now.Minute.ToString("d2")
      +  DateTime.Now.Second.ToString("d2")
      +  DateTime.Now.Millisecond.ToString("d4")
      +  uniqueCounter.ToString("d6");

      return(uName);
   }  // end of CreateUniqueId

}  // end of Names class

}
