using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Wordprocessing;
using Edam.Data.AssetSchema;
using Edam.Diagnostics;

// -----------------------------------------------------------------------------
using Edam.Text;
namespace Edam.Xml.OpenXml
{

   public class ExcelRowBuilder : ITableBuilder
   {
      public static readonly string DEFAULT_NAME = "Table";
      public const UInt32 DEFAULT_STYLE = 0U;

      private ExcelDocument m_Document;
      private UInt32 m_CurrentRowIndex = 1U;
      private UInt32 m_CurrentColumnIndex = 1U;
      private UInt32 m_CurrentStyleNo = 0U;

      public String Name { get; set; }
      public TableBuilderType Type { get; set; }

      public IResultsLog Results { get; set; }

      public ITableReport ReportDetails { get; set; }

      public ExcelRowBuilder(ExcelDocument document)
      {
         m_Document = document ?? new ExcelDocument();
      }

      public ExcelRowBuilder()
      {
         m_Document = new ExcelDocument();
      }

      public void PrepareResultsLog()
      {
         Results = new ResultLog();
      }

      /// <summary>
      /// Add continous columns
      /// </summary>
      /// <param name="hidden">true if columns should be hidden</param>
      /// <param name="count">(optional) number of columns to add, default = 3
      /// </param>
      /// <param name="startIndex">start index (default: 0)</param>
      public void AddColumns(
         bool hidden = true, int count = 3, int startIndex = 0)
      {
         // create hidden columns: IndexNo; Status; LastUpdateDate;
         for(var i = 1; i <= count; i++)
         {
            AppendColumn((uint)(i + startIndex), hidden, true);
         }
      }

      public void SetStyleNo(UInt32 styleNo = 0U)
      {
         m_CurrentStyleNo = styleNo;
      }

      /// <summary>
      /// Append Header
      /// </summary>
      /// <param name="items">comma separated header titles</param>
      public void AppendHeader(string items,
         uint rowStyle = (uint)TableRowStyle.Fill3Border1Font14)
      {
         //System.Text.StringBuilder sb = new StringBuilder();
         // write header - light blue
         AppendRow(items, styleNo: rowStyle);
      }

      /// <summary>
      /// Append Header
      /// </summary>
      /// <param name="columns"></param>
      public void AppendMainHeader(
         List<string> columns, string headerText = null,
         uint rowStyle = (uint)TableRowStyle.Fill3Border1Font14)
      {
         string header = headerText +
            (String.IsNullOrWhiteSpace(headerText) ||  columns.Count == 0 ? 
               String.Empty : ",");

         // add additional columns to append in each row
         int cnt = 0;
         StringBuilder sb = new StringBuilder();
         foreach (var c in columns)
         {
            if (cnt > 0)
            {
               sb.Append(",");
            }
            sb.Append(c);
            cnt++;
         }
         header += sb.ToString();

         AppendHeader(header, rowStyle);
      }

      public ITableBuilder AppendColumn(
         UInt32 columnIndex, bool hidden = false, bool bestFit = true)
      {
         m_Document.AddColumn(columnIndex, hidden, bestFit);
         return this;
      }

      /// <summary>
      /// Add columns and related row using given columns info.
      /// </summary>
      /// <param name="columns">columns details</param>
      public void AppendTabColumnsRow(string tabName, TableRowHeaderInfo columns)
      {
         // add columns
         foreach(var c in columns.Items)
         {
            if (c.Hidden)
            {
               m_Document.AddColumn(
                  (uint)c.Index, c.Hidden);
            }
         }

         // add work-sheet TAB (if provided)...
         if (!String.IsNullOrWhiteSpace(tabName))
         {
            AddWorksheet(tabName);
         }

         // add row
         uint columnIndex = 1;
         foreach (var i in columns.Items)
         {
            var header = i.Name.Trim();
            m_Document.InsertCellText(
               columnIndex, m_CurrentRowIndex, header, i.StyleNo);
            columnIndex++;
         }

         m_CurrentRowIndex++;
         m_CurrentColumnIndex = 1;
         m_CurrentStyleNo = DEFAULT_STYLE;
      }

      public ITableBuilder AppendRowCell(string text)
      {
         var txt = text ?? String.Empty;
         m_Document.InsertCellText(
            m_CurrentColumnIndex, m_CurrentRowIndex, txt, m_CurrentStyleNo);
         m_CurrentColumnIndex++;
         return this;
      }

      /// <summary>
      /// Append Cell Fillers (empty cells)
      /// </summary>
      /// <param name="cellCount">how many to append</param>
      /// <param name="styleNo">cell stype no (default: 0U)</param>
      public void AppendCellFillers(int cellCount, uint styleNo = 0U)
      {
         for(var i = 0; i < cellCount; i++)
         {
            AppendRowCell(String.Empty);
         }
      }

      public ITableBuilder AppendRowCellLast(string text = null)
      {
         var txt = text ?? String.Empty;
         m_Document.InsertCellText(
            m_CurrentColumnIndex, m_CurrentRowIndex, txt, m_CurrentStyleNo);
         m_CurrentRowIndex++;
         m_CurrentColumnIndex = 1;
         return this;
      }

      public ITableBuilder AppendRowCellLast()
      {
         m_CurrentRowIndex++;
         m_CurrentColumnIndex = 1;
         return this;
      }

      public void AddWorksheet(string name)
      {
         // make sure to start at the top...
         m_CurrentRowIndex = 1U;
         var nm = String.IsNullOrWhiteSpace(name) ? Name : name;
         m_Document.AddWorksheet(nm);
      }

      /// <summary>
      /// Set file path for output document as needed.
      /// </summary>
      /// <param name="resourceUri">resource URI</param>
      public void Open(string resourceUri)
      {
         m_Document ??= new ExcelDocument();
         if (String.IsNullOrWhiteSpace(Name))
            Name = DEFAULT_NAME;
         if (String.IsNullOrWhiteSpace(resourceUri))
         {
            // TODO: replace hardcoded label 
            throw new Exception(
               "ExcelRowBuilder::SetFilePath: resoource URI expected!");
         }
         else
         {
            m_Document.CreateDocument(resourceUri);
         }
      }

      public void Close()
      {
         if (m_Document == null)
            return;
         m_Document.Save();
         m_Document.Dispose();
         m_Document = null;
      }

      /// <summary>
      /// Given a row of string items delimited by given string write an
      /// OpenXml / Excel row;
      /// </summary>
      /// <param name="text">text</param>
      /// <param name="delimeter">delimeter</param>
      /// <param name="styleNo">style No</param>
      /// <returns></returns>
      public ITableBuilder AppendRow(
         string text, string delimeter = ",", UInt32 styleNo = DEFAULT_STYLE)
      {
         String[] l = text.Split(delimeter.ToCharArray());
         uint columnIndex = 1;
         foreach(var i in l)
         {
            var item = i.Trim();
            m_Document.InsertCellText(
               columnIndex, m_CurrentRowIndex, item, styleNo);
            columnIndex++;
         }
         m_CurrentRowIndex++;
         m_CurrentColumnIndex = 1;
         m_CurrentStyleNo = styleNo;
         return this;
      }
   }

}
