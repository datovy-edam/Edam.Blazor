using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
// -----------------------------------------------------------------------------
namespace Edam.Text
{

   public class TableBuilder: ITableBuilder
   {
      private readonly StringBuilder m_Builder = new StringBuilder();
      public static readonly string DEFAULT_DELIMITER = ",";
      private string m_Delimiter = DEFAULT_DELIMITER;
      private int m_ColumnIndex = 0;

      public ITableReport ReportDetails { get; set; }

      public String Name { get; set; }
      public TableBuilderType Type { get; set; }

      public IResultsLog Results { get; set; } = new ResultLog();

      public TableBuilder(string delimiter = null)
      {
         m_Delimiter = delimiter ?? DEFAULT_DELIMITER;
      }

      public void SetStyleNo(UInt32 styleNo)
      {

      }

      public void AddWorksheet(string name)
      {

      }

      public ITableBuilder AppendColumn(
         UInt32 columnIndex, bool hidden = false, bool bestFit = true)
      {
         return this;
      }

      public ITableBuilder AppendRow(
         String text, String delimeter = null, UInt32 styleNo = 0U)
      {
         m_ColumnIndex = 0;
         m_Builder.AppendLine(text);
         return this;
      }

      public void AppendCellFillers(int cellCount, uint styleNo = 0U)
      {
         throw new NotImplementedException();
      }

      public ITableBuilder AppendRowCellLast(String text = null)
      {
         if (String.IsNullOrWhiteSpace(text))
         {
            if (m_ColumnIndex > 0)
               m_Builder.Append(m_Delimiter);
            m_Builder.AppendLine(text);
         }
         m_ColumnIndex = 0;
         return this;
      }

      public ITableBuilder AppendRowCellLast()
      {
         m_ColumnIndex = 0;
         return this;
      }
      public ITableBuilder AppendRowCell(String text)
      {
         if (m_ColumnIndex > 0)
            m_Builder.Append(m_Delimiter);
         m_Builder.Append(text);
         m_ColumnIndex++;
         return this;
      }
      public new String ToString()
      {
         return m_Builder.ToString();
      }
      public void Open(string resourdeUri)
      {

      }
      public void Close()
      {

      }

      public void AppendHeader(string columns, uint rowStyle = 5)
      {
         throw new NotImplementedException();
      }

      public void AppendMainHeader(
         List<string> header, string headerText, uint rowStyle = 5)
      {
         throw new NotImplementedException();
      }

      public void AddColumns(
         bool hidden = true, int count = 3, int startIndex = 0)
      {
         throw new NotImplementedException();
      }

      public void AppendTabColumnsRow(string tabName, TableRowHeaderInfo columns)
      {
         throw new NotImplementedException();
      }

   }

}
