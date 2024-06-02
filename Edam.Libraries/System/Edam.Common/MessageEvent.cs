using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------

namespace Edam
{

   /// <summary>
   /// Provided to allow any application to send text messages throug a
   /// delegate.
   /// </summary>
   public class MessageEventArgs : EventArgs
   {
      private String [] m_Messages = new String[0];
      public String [] Messages
      {
         get { return m_Messages; }
      }

      /// <summary>
      /// Get the messages as a single message.
      /// </summary>
      /// <returns>A string with the concatenated messages
      /// is returned</returns>
      public String GetSingleMessage()
      {
         StringBuilder sb = new StringBuilder();
         for(Int32 i=0; i < m_Messages.Length; i++)
            sb.AppendLine(m_Messages[i]);
         return sb.ToString();
      }

      public void Add(String message)
      {
         Int32 i = m_Messages.Length;
         Array.Resize<String>(ref m_Messages, m_Messages.Length+1);
         m_Messages[i] = message;
      }
      public void Add(String [] messages)
      {
         if (messages == null)
            return;
         if (messages.Length == 0)
            return;
         for(Int32 i=0; i < messages.Length; i++)
            Add(messages[i]);
      }
      public void Add(Diagnostics.ResultLog results)
      {
         if (results == null)
            return;
         for(Int32 i=0; i < results.Count; i++)
            Add(results[i].Message);
      }
      public void Add(Exception exception, String location = null)
      {
         Boolean hasLocation = String.IsNullOrEmpty(location);
         while(exception != null)
         {
            Add(hasLocation ? location + ": " + exception.Message :
               exception.Message);
            exception = exception.InnerException;
         }
      }
   }

   public delegate void MessageEvent(Object sender, MessageEventArgs e);

}  // end of Kif
