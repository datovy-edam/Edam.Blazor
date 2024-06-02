using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Diagnostics
{

   /// <summary>
   /// Trace Builder Logger...
   /// </summary>
   public class TraceBuilderLogger
   {

      private StringBuilder m_TextLogger { get; set; }
      public Int32 Counter { get; set; }
      public StringBuilder TextLogger
      {
         get { return m_TextLogger; }
      }
      public TraceBuilderLogger()
      {
         m_TextLogger = new StringBuilder();
      }
      public void WriteLine(String message)
      {
         Counter++;
         m_TextLogger.AppendLine(message);
      }
      public override String ToString()
      {
         return m_TextLogger.ToString();
      }

   }

}
