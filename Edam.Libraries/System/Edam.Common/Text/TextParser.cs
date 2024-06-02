using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Text
{

   public enum TextParseStatus
   {
      FileNotFound = -1,
      Uknown = 0,
      Working = 1,
      Done = 99
   }

   public class TextParser
   {
      private Int32 m_LineNo = -1;
      private String m_Line = String.Empty;
      private String [] m_Text = null;

      public TextParseStatus Status { get; set; }

      public Boolean NotDoneYet
      {
         get { return Status == TextParseStatus.Working; }
      }

      public String Line
      {
         get { return m_Line; }
         set
         {
            if (value == null)
               value = String.Empty;
            m_Line = value;
         }
      }

      public Int32 Length
      {
         get { return m_Text == null ? -1 : m_Text.Length; }
      }

      public void InitializeParser(String filePath)
      {
         if (!System.IO.File.Exists(filePath))
         {
            m_Text = null;
            Status = TextParseStatus.Uknown;
            return;
         }
         m_Text = System.IO.File.ReadAllLines(filePath);
         m_Line = String.Empty;
         Status = TextParseStatus.Working;
         m_LineNo = 0;
         //throw new Exception("InitializeParser::file: NOT SUPPORTED");
      }

      public TextParser(String filePath)
      {
         InitializeParser(filePath);
      }

      public String NextLine(String pattern = null)
      {
         if (m_Text == null)
            return null;

         if (m_LineNo + 1 >= m_Text.Length)
         {
            Status = TextParseStatus.Done;
            return null;
         }

         if (pattern == null)
         {
            m_LineNo++;
            m_Line = m_Text[m_LineNo];
            return m_Line;
         }

         do
         {
            m_LineNo++;
            if (m_LineNo + 1 >= m_Text.Length)
               break;
            m_Line = m_Text[m_LineNo];
            if (m_Line.IndexOf(pattern) >= 0)
               break;
         }  while (m_LineNo < m_Text.Length);

         return m_Line;
      }

      public Boolean PeekFind(String pattern)
      {
         if (m_LineNo + 1 >= m_Text.Length)
            return false;
         return m_Text[m_LineNo+1].IndexOf(pattern) < 0 ? false : true;
      }

   }

}
