using System;

// ------------------------------------------------------------------------------
// Copied from Kifv4r0

namespace Edam
{

   /// <summary>
   /// Support for common DateTime operations...
   /// </summary>
   public class NullDateTime
   {
      /// <summary>
      /// Define the Null DateTime as 1800-01-01.
      /// </summary>
      static public readonly DateTime Value =
         new DateTime(1900, 1, 1);

      /// <summary>
      /// See if given DateTime is null...
      /// </summary>
      /// <param name="dateTime"></param>
      /// <returns></returns>
      static public Boolean IsNullDateTime(DateTime dateTime)
      {
         return dateTime == Value;
      }

      /// <summary>
      /// See if given Date is null...
      /// </summary>
      /// <param name="dateTime"></param>
      /// <returns></returns>
      static public Boolean IsNullDate(DateTime dateTime)
      {
         return dateTime == Value;
      }

      /// <summary>
      /// Parse given DateTime text string as a DateTime...
      /// </summary>
      /// <param name="dateTimeText">DateTime in text format to parse</param>
      /// <returns>the parse DateTime is returned NullDateTime if parsing
      /// was not possible</returns>
      static public DateTime Parse(String dateTimeText)
      {
         if (String.IsNullOrEmpty(dateTimeText))
            return Value;
         DateTime outDateTime;
         if (DateTime.TryParse(dateTimeText, out outDateTime))
            return outDateTime;
         return Value;
      }  // end of Parse

   }  // end of NullDateTime

}
