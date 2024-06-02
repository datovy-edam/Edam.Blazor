using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------
// Open Knowledge (c) 2010 - 2021.  Kifv3r0.

namespace Edam.Data
{

}

namespace Edam
{

   /// <summary>
   /// Manage System and/or Application message/s log...
   /// </summary>
   public class MessageLog
   {

      #region    -- 1.10 Control/Form Members and Properties

      private Edam.Diagnostics.ResultLog m_Results;
      public Edam.Diagnostics.ResultLog Results
      {
         get { return m_Results; }
      }

      #endregion
      #region    -- 1.20 Delegates and Callback Support

      public Edam.MessageEvent MessageEvent;

      #endregion
      #region    -- 1.30 -- Control/Form Initialization - Configuration

      public MessageLog()
      {
         m_Results = new Diagnostics.ResultLog();
      }

      #endregion
      #region    -- 1.40 -- Security and Policies Methods

      #endregion
      #region    -- 1.50 -- Exception / Errors Handling Code

      public static Edam.MessageEventArgs GetEventArgs(
         Edam.Diagnostics.ResultLog results,
         Object tag = null, String location = null)
      {
         Edam.MessageEventArgs e =
            new Edam.MessageEventArgs();
         e.Add(results);
         return e;
      }

      public static Edam.MessageEventArgs GetEventArgs(
         Exception exception,
         Object tag = null, String location = null)
      {
         Edam.MessageEventArgs e =
            new Edam.MessageEventArgs();
         e.Add(exception);
         return e;
      }

      public static Edam.MessageEventArgs GetEventArgs(
         String message,
         Object tag = null, String location = null)
      {
         Edam.MessageEventArgs e =
            new Edam.MessageEventArgs();
         e.Add(message);
         return e;
      }

      #endregion
      #region    -- 2.10 -- Data Validation or Validation Methods

      #endregion
      #region    -- 2.20 -- Data Binding Support Methods

      #endregion
      #region    -- 2.30 Messages and Feedback Methods

      /// <summary>
      /// Send messages accumulated in the Results Log.  Calling this
      /// </summary>
      public void SendMessages()
      {
         if (MessageEvent == null)
            return;
         Edam.MessageEventArgs e = new MessageEventArgs();
         e.Add(m_Results);
         MessageEvent(this, e);
         m_Results.Clear();
      }

      /// <summary>
      /// Send Messages to delegates...
      /// </summary>
      public void SendMessages(Object sender, Edam.MessageEventArgs args)
      {
         if (MessageEvent == null)
            return;
         MessageEvent(sender, args);
      }

      /// <summary>
      /// Send Messages to delegates...
      /// </summary>
      public void SendMessages(Exception exception, String location = null)
      {
         if (MessageEvent == null)
            return;
         Edam.MessageEventArgs e = new MessageEventArgs();
         e.Add(exception, location);
         MessageEvent(this, e);
      }

      #endregion
      #region    -- 3.10 -- Find or Search and Selection Support Methods

      #endregion
      #region    -- 3.20 -- Add, Update, Delete Methods

      #endregion
      #region    -- 3.30 -- Print, Preview, Export

      #endregion
      #region    -- 4.10 -- Manage Support Module/Control/Form Methods


      #endregion
      #region    -- 4.20 -- Manage Static/Shared Methods
      #endregion
      #region    -- 7.10 -- Window / Form Support Code

      #endregion
      #region    -- 9.10 -- Windows Form Designer generated code

      #endregion
      #region    -- 9.20 -- Control/Form Event Handlers

      #endregion

   }

}
