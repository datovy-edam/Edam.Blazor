using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Text
{

   public class IndentText
   {
      private readonly String m_IndentText = "   ";
      private String m_Indent;

      public IndentText()
      {
         m_Indent = String.Empty;
      }
      public String Identation
      {
         get
         { 
            return m_Indent ?? String.Empty; 
         }
      }
      public int IdentCount
      {
         get
         {
            if (m_Indent == null)
               return 0;
            return m_Indent.Length / m_IndentText.Length;
         }
      }
      public String Push()
      {
         if (m_Indent == null)
         {
            m_Indent = String.Empty;
         }
         else
         {
            m_Indent += m_IndentText;
         }
         return m_Indent;
      }
      public String Pop()
      {
         if (m_Indent == null)
            m_Indent = String.Empty;
         int l = m_Indent.Length;
         if (l - 3 <= 0)
            m_Indent = String.Empty;
         else
            m_Indent = m_Indent.Substring(0, l - 3);
         return m_Indent;
      }
      public void Clear()
      {
         m_Indent = String.Empty;
      }
   }

}
