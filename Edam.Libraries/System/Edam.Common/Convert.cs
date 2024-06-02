using System;
using System.Collections.Generic;
using System.Text;
using cnv = System.Convert;
using System.Globalization;

// -----------------------------------------------------------------------------
// Copied from Kif v3r0

#if !SILVERLIGHT
// namespace Kif
namespace Edam
#else
namespace Edam.Silverlight
#endif
{

/// <summary>Provide support for general conversions from a string to a
/// requested type. The user of this class must understand that the conversion
/// exceptions/errors are ignored by the class trying to always return a value.
/// When an exception or error is found a defualt value is returned instead.
/// </summary>
public class Convert
{

   #region -- Labels and Literals

   public static Int32 BadInt32Value = Int32.MinValue;
   public static Int16 BadInt16Value = Int16.MinValue;
   public static Double BadDoubleValue = Double.MinValue;
   public static DateTime BadDateValue = new DateTime(1800,1,1);
   public static String Base64TaggedHeader = "data:image/jpeg;base64,";

   #endregion
   #region -- Base85 Conversions

   /// <summary>
   /// To Base85 Text.
   /// </summary>
   /// <param name="bytes">Bytes to convert.</param>
   /// <returns>Base85 string is returned</returns>
   public static String ToBase85Text(Byte[] bytes)
   {
      if (bytes == null)
         return String.Empty;
      Edam.Text.Base85Encoder e = new Text.Base85Encoder();
      return e.Encode(bytes);
   }

   /// <summary>
   /// To Base85 Text.
   /// </summary>
   /// <param name="text">Text to convert.</param>
   /// <returns>Base85 string is returned</returns>
   public static String ToBase85Text(String text)
   {
      if (text == null)
         text = String.Empty;
      UTF8Encoding enc = new UTF8Encoding();
      Byte[] bytes = enc.GetBytes(text.ToCharArray());
      Edam.Text.Base85Encoder e = new Text.Base85Encoder();
      return e.Encode(bytes);
   }

   /// <summary>
   /// Given a Base85 text, decode it to it's original text.
   /// </summary>
   /// <param name="text"></param>
   /// <returns>decoded Text is returned</returns>
   public static String FromBase85Text(String base86Text)
   {
      if (base86Text == null)
         base86Text = String.Empty;
      Edam.Text.Base85Encoder e = new Text.Base85Encoder();
      UTF8Encoding enc = new UTF8Encoding();
      Char[] chars = enc.GetChars(e.Decode(base86Text));
      return new String(chars);
   }

   #endregion
   #region -- String Conversions

   public static String ToNotNullString(String value, String defaultValue = "")
   {
      if ((value == null) ||
          (value == Edam.Application.Resources.Strings.Undefined))
         return defaultValue;
      return value;
   }

   public static String ToString(String text,String defaultValue)
   {
      if (String.IsNullOrEmpty(text))
         return(defaultValue);
      return text;
   }  // end of ToString

   public static String ToString(Object text,String defaultValue)
   {
      if (text == null)
         return(defaultValue);
      return text.ToString();
   }  // end of ToString

   public static String ToNumbers(String text)
   {
      String t = string.Empty;
      char n;
      char[] l = text.ToCharArray();
      for (var c=0; c < l.Length; c++)
      {
         n = l[c];
         if (n >= '0' && n <= '9')
         {
            t += n.ToString();
         }
      }
      return t;
   }

   #endregion
   #region -- Capitalization Conversions

   /// <summary>
   /// Capitalize firt word only.
   /// </summary>
   /// <param name="text"></param>
   /// <returns></returns>
   public static String ToFirstCharCapital(String text)
   {
      var t = text.Trim().ToLower();
      char[] letters = t.ToCharArray();
      letters[0] = char.ToUpper(letters[0]);
      t = new string(letters);
      return t;
   }

   /// <summary>
   /// Capitalize all words in given string.
   /// </summary>
   /// <param name="text"></param>
   /// <returns></returns>
   public static String ToTitleCase(String text)
   {
      if (text == null)
      {
         return String.Empty;
      }
      var t = text.TrimStart().ToLower();
      if (string.IsNullOrEmpty(t))
         return string.Empty;

      System.Text.StringBuilder sb = new StringBuilder();
      string[] w = t.Split(' ');
      foreach(var i in w)
      {
         if (i.Length == 0)
            continue;
         char[] letters = i.ToCharArray();
         letters[0] = char.ToUpper(letters[0]);
         sb.Append((new string(letters)) + " ");
      }
      t = sb.ToString().Trim();
      return t;
   }

   #endregion
   #region -- Phone Numbers Conversions

   public static String ToFormattedPhone(String phone)
   {
      if (String.IsNullOrWhiteSpace(phone))
         return String.Empty;

      Int16 cnt = 0;
      string p = String.Empty;
      char[] l = phone.ToCharArray();
      char n;
      for(var c=l.Length-1; c >= 0; c--)
      {
         n = l[c];
         if (n >= '0' && n <= '9')
         {
            p = n.ToString() + p;
            cnt++;
            if ((cnt == 4) || (cnt == 7) || (cnt == 10))
            {
               p = ((cnt >= 10) ? " " : "-") + p;
            }
         }
      }
      return p.Trim();
   }

   #endregion
   #region -- Time Conversions

   /// <summary>
   /// Given a DateTime prepare its relative repesentation as a Text String
   /// such as:
   ///    Today, September 1, one hour ago
   /// </summary>
   /// <param name="date">date-time to convert</param>
   /// <param name="language">requested language</param>
   /// <returns>relative date-time string is returned</returns>
   public static String ToRelativeTimeString(
   DateTime date, LocaleLanguage language = LocaleLanguage.English)
{
   String tAgo = ToTimeAgoString(date, language);
   DateTime dt = DateTime.Now;
   String header = String.Empty;
   if ((dt.Year == date.Year) && (dt.Month == date.Month) &&
         (dt.Day == date.Day))
      header = (language == LocaleLanguage.English) ?
         "Today, " : "Hoy, ";
   return header + date.ToString("m") + " " + tAgo.Trim();
}  // end of ToRelativeTimeString

/// <summary>
/// Given a DateTime prepare a verbose datetime string such as:
///    2 years and 16 hours ago
/// </summary>
/// <param name="date">reference date time</param>
/// <param name="language">requested language</param>
/// <returns>formatted string is returned</returns>
public static String ToTimeAgoString(
   DateTime date, LocaleLanguage language = LocaleLanguage.English)
{
   DateTime now = DateTime.Now;
   TimeSpan diff = now - date;
   String outDateText, one;

   one = (language == LocaleLanguage.English) ? "one" : "un";

   // take in account the years...

   String year = String.Empty;
   Int32 yearDiff = now.Year - date.Year;
   Int32 years = 0;

   if (yearDiff > 0)  // the past
   {
      String hasBeen = (language == LocaleLanguage.English) ?
         "been" : "hace";
      years = yearDiff;
      year = (language == LocaleLanguage.English) ? " year" : " año";
      year = hasBeen + " " + ((yearDiff == 1) ? one : yearDiff.ToString()) +
         year + ((years > 1) ? "s " : " ");
   }
   else
   if (yearDiff < 0)  // the future
   {
      years = date.Year - now.Year;
      String hasBeen = (language == LocaleLanguage.English) ?
         "in " : "en ";
      year = (language == LocaleLanguage.English) ? " year" : " año";
      if (years > 1)
         year = hasBeen + " " + years.ToString() + year +
            ((years > 1) ? "s " : " ");
      else
         year += " ";
   }

   // within a year, where are we...

   if (diff.Seconds <= 60)
      outDateText = String.Empty;
   if (diff.Days > 30)
   {
      Int32 months = diff.Days / 30;
      if (months >= 12)
         months -= (years * 12);
      if (months > 0)
      {
         String month = (language == LocaleLanguage.English) ?
            " month" : " mes";
         outDateText = ((months == 1) ? one : months.ToString()) +
            month + (months >= 2 ? "s " : " ");
      }
      else
         outDateText = String.Empty;
   }
   else if (diff.Days > 7)
   {
      Int32 days = diff.Days / 7;
      String week = (language == LocaleLanguage.English) ?
         " week" : " semana";
      one = (language == LocaleLanguage.English) ?
         " one" : " una";
      outDateText = ((days == 1) ? one : days.ToString()) + week +
         (days >= 2 ? "s " : " ");
   }
   else if (diff.Days >= 1)
   {
      String day = (language == LocaleLanguage.English) ?
         " day" : " día";
      outDateText = ((diff.Days == 1) ? one : diff.Days.ToString()) +
         day + (diff.Days >= 2 ? "s " : " ");
   }
   else if (diff.Hours >= 1)
   {
      String hour = (language == LocaleLanguage.English) ?
         " hour" : " hora";
      one = (language == LocaleLanguage.English) ?
         " one" : " una";
      outDateText = ((diff.Hours == 1) ? one : diff.Hours.ToString()) +
         hour + (diff.Hours >= 2 ? "s " : " ");
   }
   else if (diff.Minutes >= 1)
   {
      String minute = (language == LocaleLanguage.English) ?
         " minute" : " minuto";
      outDateText = ((diff.Minutes == 1) ? one :
         diff.Minutes.ToString()) + minute +
         (diff.Minutes >= 2 ? "s " : " ");
   }
   else
   {
      String second = (language == LocaleLanguage.English) ?
         " second" : " segundo";
      outDateText = ((diff.Seconds == 1) ? one :
         diff.Seconds.ToString()) + second +
         (diff.Seconds >= 2 ? "s " : " ");
   }

   // plug how long or when it will be + the year into response as needed
   if (outDateText.Length > 0)
   {
      if (yearDiff >= 0)
      {
         String ago = (language == LocaleLanguage.English) ?
            "ago" : "atrás";
         outDateText += ago;
      }
   }
   else
   if (year.Length == 0)
      outDateText = (language == LocaleLanguage.English) ?
         "now" : "en estos momentos";

   if ((!String.IsNullOrEmpty(year)) && (!String.IsNullOrEmpty(outDateText)))
      year += (language == LocaleLanguage.English) ? "and " : "y ";

   outDateText = year + outDateText;

   return outDateText;
}

#endregion
   #region -- Boolean Conversions

   /// <summary>Try to convert from an object to a Int32</summary>
   /// <date>Mar/2k6 (ESob)<date>
   public static Boolean ToBool(Object value,Boolean defaultValue)
   {
      if (value == null)
         return(defaultValue);

      Boolean rval = false;
      Type t = value.GetType();
      String errMess = String.Empty;

      try
      {
         if (t == typeof(Boolean))
            rval = (Boolean)(value);
         else
         if (t == typeof(Int32))
         {
            Int32 v = (Int32)(value);
            rval = v != 0;
         }
         else
         if (t == typeof(Int16))
         {
            Int16 v = (Int16)(value);
            rval = v != 0;
         }
         else
         if (t == typeof(Double))
         {
            Double v = (Double)(value);
            rval = v != 0.0;
         }
         else
         if (t == typeof(String))
         {
            String v = value.ToString();
            if (!String.IsNullOrEmpty(v))
            {
               v = v.ToLower();
               rval = v == "true";
            }
         }
      }
      catch(Exception ex)
      {
         errMess = ex.Message;  // just ignore error
         rval = defaultValue;
      }

      return(rval);
   }  // end of ToBool

   public static Boolean ToBool(Object value)
   {
      return(ToBool(value,false));
   }  // end of ToBool

   #endregion
   #region -- Numeric Conversions

   /// <summary>Try to convert from an object to a Double</summary>
   /// <date>Mar/2k6 (ESob)<date>
   public static Double ToDouble(Object value, Double defaultValue)
   {
      if (value == null)
         return (defaultValue);

      Double rval = 0.0;
      Type t = value.GetType();
      String errMess = String.Empty;

      try
      {
         if (t == typeof(Double))
         {
            Double v = (Double)(value);
            rval = (Double)(v);
         }
         else
            if (t == typeof(Int32))
            {
               Int32 v = (Int32)(value);
               rval = (Int32)(v);
            }
            else
               if (t == typeof(Int16))
               {
                  rval = (Int16)(value);
               }
               else
                  if (t == typeof(String))
                  {
                     rval = Double.Parse(value.ToString());
                  }
      }
      catch (Exception ex)
      {
         errMess = ex.Message;  // just ignore error
         rval = defaultValue;
      }

      return (rval);
   }

   public static Double ToDouble(Object value)
   {
      return (ToDouble(value, BadDoubleValue));
   }

   /// <summary>Try to convert from an object to a Decimal</summary>
   /// <date>Mar/2k6 (ESob)<date>
   public static Decimal ToDecimal(
      Object value, Decimal defaultValue = (Decimal)0.0)
   {
      if (value == null)
         return (defaultValue);

      Decimal rval = (Decimal)0.0;
      Type t = value.GetType();
      String errMess = String.Empty;

      try
      {
         if (t == typeof(Double))
         {
            Double v = (Double)(value);
            rval = (Decimal)(v);
         }
         else
            if (t == typeof(Int32))
            {
               Int32 v = (Int32)(value);
               rval = (Decimal)(v);
            }
            else
               if (t == typeof(Int16))
               {
                  rval = (Decimal)(value);
               }
               else
                  if (t == typeof(String))
                  {
                     rval = Decimal.Parse(value.ToString());
                  }
      }
      catch (Exception ex)
      {
         errMess = ex.Message;  // just ignore error
         rval = defaultValue;
      }

      return (rval);
   }

   /// <summary>Try to convert from an object to a Int32</summary>
   /// <date>Mar/2k6 (ESob)<date>
   public static Int32 ToInt32(Object value, Int32 defaultValue)
   {
      if (value == null)
         return (defaultValue);

      Int32 rval = 0;
      Type t = value.GetType();
      String errMess = String.Empty;

      try
      {
         if (t == typeof(Int32))
         {
            Int32 v = (Int32)(value);
            rval = (Int32)(v);
         }
         else
            if (t == typeof(Int16))
            {
               rval = (Int16)(value);
            }
            else
               if (t == typeof(Double))
               {
                  Double v = (Double)(value);
                  rval = (Int32)(v);
               }
               else
                  if (t == typeof(String))
                  {
                     rval = Int32.Parse(value.ToString());
                  }
      }
      catch (Exception ex)
      {
         errMess = ex.Message;  // just ignore error
         rval = defaultValue;
      }

      return (rval);
   }

   public static Int32 ToInt32(Object value)
   {
      return (ToInt32(value, BadInt32Value));
   }

   /// <summary>Try to convert from an object to a Int16</summary>
   /// <date>Mar/2k6 (ESob)<date>
   public static Int16 ToInt16(Object value,Int16 defaultValue)
   {
      if (value == null)
         return(defaultValue);

      Int16 rval = 0;
      Type t = value.GetType();
      String errMess = String.Empty;

      try
      {
         if (t == typeof(Int16))
         {
            rval = (Int16)(value);
         }
         else
         if (t == typeof(Int32))
         {
            Int32 v = (Int32)(value);
            rval = (Int16)(v);
         }
         else
         if (t == typeof(Double))
         {
            Double v = (Double)(value);
            rval = (Int16)(v);
         }
         else
         if (t == typeof(String))
         {
            rval = Int16.Parse(value.ToString());
         }
      }
      catch(Exception ex)
      {
         errMess = ex.Message;  // just ignore error
         rval = defaultValue;
      }

      return(rval);
   }  // end of ToInt16

   public static Int16 ToInt16(Object value)
   {
      return(ToInt16(value,BadInt16Value));
   }  // end of ToInt16

   /// <summary>Try to convert from a string to a Int32</summary>
   /// <param name="value">string value to convert</param>
   /// <param name="defaultValue">default value</param>
   /// <date>Oct/2k5 (ESob)<date>
   public static Int32 ToInt32(String value, Int32 defaultValue)
   {
      if (value == null)
         value = String.Empty;
      if (value.Trim() == String.Empty)
         return(defaultValue);

      Int32 i;
      String errMess = String.Empty;

      try
      {
         i = Int32.Parse(value);
      }
      catch(Exception ex)
      {
         // TODO: manage the exception here
         errMess = ex.Message;  // just ignore error
         i = defaultValue;
      }

      return (i);
   }  // end of ToInt32

   public static Int32 ToInt32(String value)
   {
      return(ToInt32(value,BadInt32Value));
   }  // end of ToInt32

   /// <summary>Try to convert from a string to a Int16</summary>
   /// <param name="value">string value to convert</param>
   /// <param name="defaultValue">default value</param>
   /// <date>Oct/2k5 (ESob)<date>
   public static Int16 ToInt16(String value, Int16 defaultValue)
   {
      if (value == null)
         value = String.Empty;
      if (value.Trim() == String.Empty)
         return(defaultValue);

      Int16 i;
      String errMess = String.Empty;

      try
      {
         i = Int16.Parse(value);
      }
      catch(Exception ex)
      {
         // TODO: manage the exception here
         errMess = ex.Message;  // just ignore error
         i = defaultValue;
      }

      return (i);
   }  // end of ToInt16

   public static Int16 ToInt16(String value)
   {
      return(ToInt16(value,BadInt16Value));
   }  // end of ToInt16

   /// <summary>Try to convert from a string to a Double</summary>
   /// <date>Sep/2k5 (ESob)<date>
   public static Double ToDouble(String value, Double defaultValue)
   {
      if (String.IsNullOrEmpty(value))
         return(0.0);

      Double d = 0.0;
      String errMess = String.Empty;

      try
      {
         d = Double.Parse(value);
      }
      catch(Exception ex)
      {
         // TODO: manage the exception here
         errMess = ex.Message;  // just ignore error
         d = defaultValue;
      }

      return(d);
   }  // end of ToDouble

   public static Double ToDouble(String value)
   {
      return(ToDouble(value,BadDoubleValue));
   }  // end of ToDouble

   #endregion
   #region -- Date and DateTime Conversions

   /// <summary>Try to convert from an object to a DateTime</summary>
   /// <date>Mar/2k6 (ESob)<date>
   public static DateTime ToDateTime(Object value, DateTime defaultValue)
   {
      if (value == null)
         return (defaultValue);
      //if (value == System.DBNull.Value)
      //   return (defaultValue);

      DateTime rval = BadDateValue;
      Type t = value.GetType();
      String errMess = String.Empty;

      try
      {
         if (t == typeof(DateTime))
         {
            rval = (DateTime)(value);
         }
         else
            if (t == typeof(String))
            {
               rval = DateTime.Parse(value.ToString());
            }
      }
      catch (Exception ex)
      {
         errMess = ex.Message;  // just ignore error
         rval = defaultValue;
      }

      return (rval);
   }  // end of ToDateTime

   public static DateTime ToDateTime(Object value)
   {
      return (ToDateTime(value, BadDateValue));
   }  // end of ToDateTime

   /// <summary>Given a date and a time compose a new datetime with
   /// both pieces of information</summary>
   /// <param name="date">date part</param>
   /// <param name="time">time part</param>
   /// <returns>a composed datetime is returned</returns>
   public static DateTime ToDateTime(DateTime date, DateTime time)
   {
      return (new DateTime(
         date.Year, date.Month, date.Day,
         time.Hour, time.Minute, time.Second));
   }  // end of ToDateTime

   /// <summary>
   /// Try to convert a given text into a valid DateTime without time.
   /// </summary>
   /// <param name="DateText">Date text</param>
   /// <returns>returns a DateTime string</returns>
   public static DateTime ToDate(String dateText, DateTime defaultValue)
   {
      DateTime dt;
      if (dateText == null)
         return (defaultValue);
      if (dateText.Length <= 0)
         return (defaultValue);

      String errMess = String.Empty;

      try
      {
         dt = DateTime.Parse(dateText);
      }
      catch (Exception ex)
      {
         // TODO: manage the exception here
         errMess = ex.Message;  // just ignore error
         dt = defaultValue;
      }
      return (dt);
   }  // end of Convert.ToDate

   public static DateTime ToDate(String value)
   {
      return (ToDate(value, BadDateValue));
   }  // end of ToDate

   /// <summary>Try to convert from a given date to an age as of now/today</summary>
   /// <date>Sep/2k5 (ESob)<date>
   public static Int32 ToAge(String DOBText)
   {
      DateTime DateOfBirth;
      Int32 years = 0;
      String errMess = String.Empty;

      try
      {
         DateOfBirth = DateTime.Parse(DOBText);
      }
      catch(Exception ex)
      {
         // TODO: manage the exception here
         errMess = ex.Message;  // just ignore error
         DateOfBirth = DateTime.Now;
         years = -1;
      }

      if (years == -1)
         return(years);

      DateTime currDate = DateTime.Now;
      years = currDate.Year - DateOfBirth.Year;

      // adjust age based on month of birth

      if (currDate.Month < DateOfBirth.Month)
         years -= 1;
      else
      if (currDate.Month == DateOfBirth.Month)
      {
         if (currDate.Day > DateOfBirth.Day) 
            years -= 1;
      }

      return(years);
   }  // end of ToAge

   /// <summary>
   /// Calcualte the difference between a start - end datetimes in minutes.
   /// </summary>
   /// <param name="startDate">start date</param>
   /// <param name="endDate">end date</param>
   public static Double ToMinutes(DateTime startDate, DateTime endDate)
   {
      // make sure that startDate > endDate
      if (startDate < endDate)
      {
         DateTime t = startDate;
         startDate = endDate;
         endDate = t;
      }

      TimeSpan ts = startDate - endDate;
      return (ts.TotalMinutes);
   }  // end of DateTimeMinutesDifference

   #endregion
   #region -- HEX String Conversions

   public static Byte HexTextToByte(string hexText)
   {
      if (Byte.TryParse("0x" + hexText, out Byte value))
         return value;
      return (Byte)0;
   }

   /*
    * Function : String ToHexString(array<Byte> ar)
    * Purpose  : Convert an array of bytes into a string hex representation.
    * In       : ar = array of bytes to convert (Byte[])
    * Date     : Apr/2k2  (ESob)
    */

   /// <summary>
   /// From a 4-bit encoded bytes array to a Hex String.
   /// </summary>
   /// <param name="buffer">buffer of bytes to convert</param>
   /// <returns>A Hex string is returned representing one Hex value
   /// per each byte 4-bit fields</returns>
   public static String From4BitEncodedToHexString(Byte [] buffer)
   {  Byte [] hx = new Byte[16]
      { (Byte)'0',(Byte)'1',(Byte)'2',(Byte)'3',(Byte)'4',
        (Byte)'5',(Byte)'6',(Byte)'7',(Byte)'8',(Byte)'9',
        (Byte)'A',(Byte)'B',(Byte)'C',(Byte)'D',(Byte)'E',(Byte)'F' } ;

      Char [] x = new Char[(buffer.Length*2)] ;
      int x1, x2 ;

      for (int i = 0,o = 0 ; i < buffer.Length ; i++,o+=2)
      {
         x1 =  buffer[i] & 0x0F ;
         x2 = (buffer[i] & 0xF0) >> 4 ;
         x[o]   = (Char)hx[x2] ;
         x[o+1] = (Char)hx[x1] ;
      }

      return (new String(x)) ;
   }  // end of convert an array of bytes into a string hex representation

   /*
    * Function : String *Convert.HexStringToByteArray(String *str)
    * Purpose  : Convert hex string into a byte array.
    * In       : str = hex string (HexString)
    * Date     : Apr/2k2  (ESob)
    */
   /// <summary>
   /// Convert a Hex String into a 4-bit encoded bytes array.
   /// </summary>
   /// <param name="hexString">HEX string to convert</param>
   /// <returns>Each char is encoded into a 4-bit field within each
   /// Byte and the result is returned</returns>
   public static Byte [] FromHexStringTo4BitEncodedBytes(
      String hexString)
   {  Char [] ca = hexString.ToCharArray();
      Byte [] hx = new Byte[(hexString.Length/2)];
      Byte b1,b2;

      for (int i = 0,o = 0 ; i < ca.Length ; i+=2,o++)
      {  b1 = (Byte)ca[i];
         b2 = (Byte)ca[i+1];
         b1 = (Byte)((b1 < 'A') ? (b1 - '0') : (b1 - 'A') + 10);
         b2 = (Byte)((b2 < 'A') ? (b2 - '0') : (b2 - 'A') + 10);
         hx[o] = (Byte)((b1 << 4) | b2) ;
      }

      return (hx) ;
   }  // end of convert a hex string into a byte array

   #endregion
   #region -- Byte Array Conversions

   /*
      * Function : array<Byte> Convert.ToByteArray(String *pString)
      * Purpose  : Convert string into a byte array.
      * In       : pString = pointer to array to convert (String*)
      * Return   : The resulted byte array is returned (Byte[])
      * Date     : Apr/2k2  (ESob)
      */

   public static Byte [] ToByteArray(String pString)
   {
      UTF8Encoding enc = new UTF8Encoding();
      return enc.GetBytes(pString);

      //array<Char> ca = pString.ToCharArray();
      //array<Byte> ba = new array<Byte>(ca.Length) ;

      //for (int i = 0 ; i < ba.Length ; i++)
      //{
      //   ba[i] = (unsigned char)ca[i] ;
      //}

      //return ba ;
   }  // end of convert a string ot a byte array

   /*
      * Function : String ToString(array<Byte> ar)
      * Purpose  : Convert a byte array into a string.
      * In       : ar = byte array to convert (Byte [])
      * Return   : The resulted string is returned (String*)
      * Date     : Apr/2k2  (ESob)
      */

   public static String ToString(Byte [] ar)
   {
      Char [] ca = new Char[(ar.Length)] ;
      for (int i = 0 ; i < ar.Length ; i++)
         ca[i] = (Char)ar[i] ;
      String pOutStr = new String(ca) ;

      return(pOutStr) ;
   }  // end of convert a byte array into a string

   #endregion
   #region -- Tagged Base64 Conversions

   /// <summary>
   /// Convert a tagged base64 string to an Byte Array...
   /// </summary>
   /// <param name="base64TaggedText">Base64 tagged text string</param>
   /// <returns>bytes array is returned (null if not converted)</returns>
   public static Byte[] TaggedBase64ToBinary(String base64TaggedText)
   {
      Byte[] result = null;
      if (base64TaggedText.Length <= Base64TaggedHeader.Length)
      {
         return result;
      }
      String b64Text = base64TaggedText.Remove(0, Base64TaggedHeader.Length);
      result = cnv.FromBase64String(b64Text);
      return result;
   }

   /// <summary>
   /// Convert a byte array to a Tagged Base64 string.
   /// </summary>
   /// <param name="bytes">bytes array</param>
   /// <returns>Tagged base 64 text string is returned</returns>
   public static String BinaryToTaggedBase64(Byte [] bytes)
   {
      String base64Text = cnv.ToBase64String(bytes);
      return Base64TaggedHeader + base64Text;
   }

   #endregion

}

}
