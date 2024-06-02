using System;

// -----------------------------------------------------------------------------

namespace Edam.Application
{

   public interface IApplicationLog
   {
      void WriteMessage(Exception exception);
      void WriteMessage(string message);
      void WriteEmptyLine();
   }

   public class ApplicationLog : IApplicationLog
   {
      private Edam.Text.IndentText m_Indent = new Edam.Text.IndentText();
      public Edam.Text.IndentText Indent
      {
         get { return m_Indent; }
      }

      public ApplicationLog()
      {
      }

      public void WriteMessage(Exception exception)
      {

      }

      public void WriteMessage(string message)
      {
         System.Console.WriteLine(m_Indent.Identation + message);
      }

      public void WriteEmptyLine()
      {
         System.Console.WriteLine();
      }

   }

}
