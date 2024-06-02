using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Text
{

   /// <summary>
   /// Helper to get (different kinds of ) Unique ID's...
   /// </summary>
   public class UniqueId
   {
      private static Int32 m_Counter = 0;

      /// <summary>
      /// Get a Base85 Encoded Guid...
      /// </summary>
      /// <returns>Encoded (20 chars long) Guid as text is returned</returns>
      public static String GetBase85EncodedGuid()
      {
         Edam.Text.Base85Encoder enc = new Text.Base85Encoder();
         String be = enc.Encode(Guid.NewGuid().ToByteArray());
         return be.Substring(2, be.Length - 4);
      }

      /// <summary>
      /// Generate a new sequential id.  This function will not guaranty
      /// uniqueness of it's ID, but it will be more or less if there are no
      /// re-entry and/or parallel processes generating those.
      /// </summary>
      /// <returns>sequential id is returned</returns>
      public static String GetUniqueSequentialId()
      {
         String cnt = m_Counter.ToString();
         m_Counter++;
         if (cnt.Length == 6)
         {
            m_Counter = 0;
            cnt = m_Counter.ToString();
         }
         String id = DateTime.Now.ToString("yyyyMMddHHmmss") +
            (new String('0',6-cnt.Length)) + cnt;
         return id;
      }

   }

}
