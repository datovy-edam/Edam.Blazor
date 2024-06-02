using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------
// 2012-04-28 The following code was copied verbatum from the Open Knowledge -
// KIFv3r0 Framework Project.

namespace Edam.Diagnostics
{

   public class ExceptionEventArgs : EventArgs
   {

      public DateTime ReportedDate { get; set; }
      public String Location { get; set; }
      public ResultLog Results { get; set; }
      public Object Tag { get; set; }

      public ExceptionEventArgs()
      {
         ReportedDate = DateTime.Now;
         Location = String.Empty;
         Results = null;
         Tag = null;
      }
   }

   public delegate void ExceptionEvent (
      Object sender, ExceptionEventArgs e);
         
   /// <summary>
   /// Results Log
   /// </summary>
   public class ResultsLog<T> : ResultLog, IResultsLog
   {

      #region -- 1.10 Control/Form Members and Properties

      public T Data { get; set; }
      public new Object DataObject
      {
         get { return Data; }
      }

      #endregion
      #region    -- 1.30 Object Initialization - Configuration

      public ResultsLog()
      {
         base.InitializeLog();
         Data = default(T);
      }

      #endregion

   }  // end of ResultsLog

   /// <summary>
   /// Results List...
   /// </summary>
   /// <typeparam name="T">type to be supported by list</typeparam>
   public class ResultsList<T> : ResultLog, IResultsLog
   {
      private List<T> m_List;
      public List<T> List
      {
         get { return m_List; }
      }

      public ResultsList()
      {
         m_List = new List<T>();
      }
   }  // end of ResultsList

   /// <summary>
   /// Results List...
   /// </summary>
   /// <typeparam name="T">type to be supported by list</typeparam>
   public class ResultsList<T,K> : ResultLog
   {
      private List<T> m_List;
      public List<T> List
      {
         get { return m_List; }
      }

      public K ReturnCode { get; set; }

      public ResultsList()
      {
         m_List = new List<T>();
      }
   }  // end of ResultsList

}  // end of Edam.Diagnostics

