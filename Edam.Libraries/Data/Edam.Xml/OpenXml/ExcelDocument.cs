using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

/*
 * https://docs.microsoft.com/en-us/office/open-xml/
 *    how-to-insert-text-into-a-cell-in-a-spreadsheet
 */

namespace Edam.Xml.OpenXml
{

   public class ExcelDocument : IDisposable
   {

      #region -- Declarations

      private UInt32Value m_SheetNo = new UInt32Value((UInt32)0);
      private SpreadsheetDocument m_Document;
      private WorkbookPart m_Workbook;
      private WorksheetPart m_CurrentWorksheet;
      private Columns m_Columns = null;

      #endregion
      #region -- Constructors - Destructors

      public ExcelDocument()
      {

      }

      public void Dispose()
      {
         if (m_Document != null)
            m_Document.Dispose();
         m_Document = null;
      }

      #endregion
      #region -- Document Support

      public void OpenDocument(string fileName, bool isEditable = false)
      {
         m_Document = SpreadsheetDocument.Open(fileName, isEditable);
      }

      public void CreateDocument(string fileName)
      {
         m_Document =
            SpreadsheetDocument.Create(
               fileName, SpreadsheetDocumentType.Workbook);

         // add workbook as needed
         m_Workbook = m_Document.AddWorkbookPart();
         m_Workbook.Workbook = new Workbook();
         m_Workbook.Workbook.AppendChild(new Sheets());

         // add stylesheets to the m_Workbook
         CreateStylesheets(m_Workbook);
      }

      public void Save()
      {
         m_Document.Save();
      }

      #endregion
      #region -- Workseet Support

      /// <summary>
      /// Get Current Worksheet...
      /// </summary>
      /// <returns>WorksheetPart is returned else null</returns>
      public WorksheetPart GetCurrentWorksheet()
      {
         if (m_CurrentWorksheet == null)
         {
            return null;
         }
         return m_CurrentWorksheet;
      }

      /// <summary>
      /// Get Worksheet by name.
      /// </summary>
      /// <param name="sheetName">worksheet name to search</param>
      /// <returns>the WorksheetPart is returned if found, else null</returns>
      public WorksheetPart GetWorksheetByName(string sheetName)
      {
         m_Workbook = m_Document.WorkbookPart;
         IEnumerable<Sheet> sheets =
            m_Document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

         if (sheets.Count() == 0)
         {
            // The specified worksheet does not exist.
            return null;
         }

         string relationshipId = sheets.First().Id.Value;
         WorksheetPart worksheetPart =
            (WorksheetPart)m_Document.WorkbookPart.GetPartById(relationshipId);
         
         m_CurrentWorksheet = worksheetPart;
         return worksheetPart;
      }

      /// <summary>
      /// Add Workbook Sheet with given name...
      /// </summary>
      /// <remarks>
      /// Columns must had been added before creating the worksheet. The
      /// internal columns list will be set to null to allow next worksheet to
      /// have its own columns...
      /// </remarks>
      /// <param name="name">Name of the worksheet</param>
      public void AddWorksheet(string name)
      {

         // add worksheet
         WorksheetPart worksheetPart = m_Workbook.AddNewPart<WorksheetPart>();
         worksheetPart.Worksheet = new Worksheet(new SheetData());

         if (m_Columns != null)
         {
            worksheetPart.Worksheet.InsertAfter(
               m_Columns, worksheetPart.Worksheet.SheetFormatProperties);
            m_Columns = null;
         }

         m_Document.Save();
         worksheetPart.Worksheet.Save();

         m_CurrentWorksheet = worksheetPart;

         Sheets sheets =
            m_Document.WorkbookPart.Workbook.GetFirstChild<Sheets>();

         // get a unique ID for the new worksheet.
         uint sheetId = 1;
         if (sheets.Elements<Sheet>().Count() > 0)
         {
            sheetId =
                sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
         }

         // prepare and add sheet
         Sheet sheet = new Sheet()
         {
            Id = m_Workbook.GetIdOfPart(worksheetPart),
            SheetId = sheetId,
            Name = name
         };

         sheets.Append(sheet);
         m_Workbook.Workbook.Save();
      }

      #endregion
      #region -- Shared items Support

      /// <summary>
      /// Given text and a SharedStringTablePart, creates a SharedStringItem
      /// with the specified text and inserts it into the SharedStringTablePart.
      /// If the item already exists, returns its index.
      /// </summary>
      /// <param name="text"></param>
      /// <param name="shareStringPart"></param>
      /// <returns></returns>
      private static int InsertSharedStringItem(
         string text, SharedStringTablePart shareStringPart)
      {
         // If the part does not contain a SharedStringTable, create one.
         if (shareStringPart.SharedStringTable == null)
         {
            shareStringPart.SharedStringTable = new SharedStringTable();
         }

         int i = 0;

         // Iterate through all the items in the SharedStringTable. If the text
         // already exists, return its index.
         foreach (SharedStringItem item in shareStringPart.
            SharedStringTable.Elements<SharedStringItem>())
         {
            if (item.InnerText == text)
            {
               return i;
            }

            i++;
         }

         // The text does not exist in the part. Create the SharedStringItem
         // and return its index.
         shareStringPart.SharedStringTable.AppendChild(
            new SharedStringItem(
               new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
         shareStringPart.SharedStringTable.Save();

         return i;
      }

      #endregion
      #region -- Columns Support

      /// <summary>
      /// Insert a column.
      /// </summary>
      /// <param name="columnNumber"></param>
      /// <param name="hidden"></param>
      /// <param name="bestFit"></param>
      public void AddColumn(
         UInt32 columnNumber, bool hidden = false, bool bestFit = true)
      {
         if (m_Columns == null)
         {
            m_Columns = new Columns();
         }
         Column column = new Column
         {
            Hidden = hidden,
            BestFit = bestFit,
            Min = columnNumber,
            Max = columnNumber
         };
         m_Columns.Append(column);
      }

      /// <summary>
      /// Add Columns...
      /// </summary>
      public void AddColumns()
      {
         if (m_Columns == null)
         {
            return;
         }
         m_CurrentWorksheet.Worksheet.InsertAfter(
            m_Columns, m_CurrentWorksheet.Worksheet.SheetFormatProperties);
         //m_Document.Save();
      }

      #endregion
      #region -- Cell Support

      /// <summary>
      /// Given an index return the Column Name (i.e. "A", "AB",...)
      /// </summary>
      /// <param name="index">collumn index</param>
      /// <returns>the column name is returned</returns>
      public static string ColumnIndexToName(UInt32 index)
      {
         int div = (int)index;
         string colLetter = String.Empty;
         int mod;

         while (div > 0)
         {
            mod = (div - 1) % 26;
            colLetter = (char)(65 + mod) + colLetter;
            div = (int)((div - mod) / 26);
         }
         return colLetter;
      }

      /// <summary>
      /// Given a column name return its index.
      /// </summary>
      /// <param name="name">column name</param>
      /// <returns>the column index is returned</returns>
      public static UInt32 ColumnNameToIndex(string name)
      {
         name = name.ToUpper();
         int sum = 0;

         for (int i = 0; i < name.Length; i++)
         {
            sum *= 26;
            sum += (name[i] - 'A' + 1);
         }
         return (UInt32)sum;
      }


      /// <summary>
      /// Given a column name, a row index, and a WorksheetPart, inserts a cell
      /// into the worksheet.   If the cell already exists, returns it.
      /// </summary>
      /// <param name="columnName">column name (i.e. "A")</param>
      /// <param name="rowIndex">row index</param>
      /// <param name="worksheetPart">worksheet part</param>
      /// <returns></returns>
      private static Cell InsertWorksheetCell(
         string columnName, uint rowIndex, WorksheetPart worksheetPart)
      {
         Worksheet worksheet = worksheetPart.Worksheet;
         SheetData sheetData = worksheet.GetFirstChild<SheetData>();
         string cellReference = columnName + rowIndex;

         // if the worksheet does not contain a row with the specified
         // row index, insert one.
         Row row;
         if (sheetData.Elements<Row>().
            Where(r => r.RowIndex == rowIndex).Count() != 0)
         {
            row = sheetData.Elements<Row>().
               Where(r => r.RowIndex == rowIndex).First();
         }
         else
         {
            row = new Row() { RowIndex = rowIndex };
            sheetData.Append(row);
         }

         // If there is not a cell with the specified column name, insert one.  
         if (row.Elements<Cell>().
            Where(c => c.CellReference.Value == columnName
               + rowIndex).Count() > 0)
         {
            return row.Elements<Cell>().
               Where(c => c.CellReference.Value == cellReference).First();
         }
         else
         {
            // Cells must be in sequential order according to CellReference.
            // Determine where to insert the new cell.
            Cell refCell = null;
            foreach (Cell cell in row.Elements<Cell>())
            {
               if (cell.CellReference.Value.Length == cellReference.Length)
               {
                  if (string.
                     Compare(cell.CellReference.Value, cellReference, true) > 0)
                  {
                     refCell = cell;
                     break;
                  }
               }
            }

            Cell newCell = new Cell() { CellReference = cellReference };
            row.InsertBefore(newCell, refCell);

            //worksheet.Save();
            return newCell;
         }
      }

      /// <summary>
      /// In current worksheet insert a text in a cell at a given column name
      /// and row index.
      /// </summary>
      /// <param name="columnName">column name</param>
      /// <param name="rowIndex">row index</param>
      /// <param name="text">text to insert</param>
      public void InsertCellText(
         string columnName, uint rowIndex, string text,
         UInt32Value styleNo = null)
      {
         // Get the SharedStringTablePart. If it does not exist, create a new one.
         SharedStringTablePart shareStringPart;
         if (m_Document.WorkbookPart.
            GetPartsOfType<SharedStringTablePart>().Count() > 0)
         {
            shareStringPart = m_Document.WorkbookPart.
               GetPartsOfType<SharedStringTablePart>().First();
         }
         else
         {
            shareStringPart = m_Document.WorkbookPart.
               AddNewPart<SharedStringTablePart>();
         }

         // Insert the text into the SharedStringTablePart.
         int index = InsertSharedStringItem(text, shareStringPart);

         // Insert cell A1 into the current worksheet.
         Cell cell = InsertWorksheetCell(
            columnName, rowIndex, m_CurrentWorksheet);

         // Set the value of cell A1.
         cell.CellValue = new CellValue(index.ToString());
         cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

         if (styleNo != null)
            cell.StyleIndex = (UInt32Value)styleNo;

         // Save the new worksheet.
         //m_CurrentWorksheet.Worksheet.Save();
      }

      /// <summary>
      /// In current worksheet insert a text in a cell at a given column index
      /// and row index.
      /// </summary>
      /// <param name="columnIndex">column index</param>
      /// <param name="rowIndex">row index</param>
      /// <param name="text">text to insert</param>
      public void InsertCellText(
         uint columnIndex, uint rowIndex, string text,
         UInt32Value styleNo = null)
      {
         InsertCellText(
            ColumnIndexToName(columnIndex), rowIndex, text, styleNo);
      }

      #endregion
      #region -- Stylesheets Support

      private void CreateStylesheets(WorkbookPart workbookPart)
      {
         WorkbookStylesPart workbookStylesPart =
            workbookPart.AddNewPart<WorkbookStylesPart>();
         // add styles to sheet
         workbookStylesPart.Stylesheet = PrepareStylesheet();
         workbookStylesPart.Stylesheet.Save();
      }

      private static Stylesheet PrepareStylesheet()
      {
         Fonts fonts = new Fonts(
             new Font( // Index 0 - default
                 new FontSize() { Val = 10 }

             ),
             new Font( // Index 1 - header
                 new FontSize() { Val = 12 }
                 //new Bold()
                 //new Color() { Rgb = "000000" }

             ),
             new Font( // Index 1 - header
                 new FontSize() { Val = 12 },
                 new Bold()
                 //new Color() { Rgb = "000000" }

             ),
             new Font( // Index 1 - header
                 new FontSize() { Val = 14 },
                 new Bold()
                 //new Color() { Rgb = "000000" }

             ));

         Fills fills = new Fills(
                 new Fill(new PatternFill() {
                    PatternType = PatternValues.None }),

                 // light gray 1
                 new Fill(new PatternFill(
                    new ForegroundColor
                    {
                       Rgb = new HexBinaryValue() { Value = "FFE3E3E3" }
                    })
                 { PatternType = PatternValues.Solid }),
                 // medium gray 2
                 new Fill(new PatternFill(
                    new ForegroundColor
                    {
                       Rgb = new HexBinaryValue() { Value = "FFDEDEDE" }
                    })
                 { PatternType = PatternValues.Solid }),

                 // light blue 3
                 new Fill(new PatternFill(
                    new ForegroundColor
                    {
                       Rgb = new HexBinaryValue() { Value = "FFDBEDFF" }
                    })
                 { PatternType = PatternValues.Solid }),
                 // light yellow 4
                 new Fill(new PatternFill(
                    new ForegroundColor
                    {
                       Rgb = new HexBinaryValue() { Value = "FFFFEFD7" }
                    })
                 { PatternType = PatternValues.Solid }),
                 // light green 5
                 new Fill(new PatternFill(
                    new ForegroundColor
                    {
                       Rgb = new HexBinaryValue() { Value = "FFE3FFD7" }
                    })
                 { PatternType = PatternValues.Solid }),
                 // light orange 6
                 new Fill(new PatternFill(
                    new ForegroundColor
                    {
                       Rgb = new HexBinaryValue() { Value = "FFFFEFEC" }
                    })
                 { PatternType = PatternValues.Solid })
             );

         Borders borders = new Borders(
                 new Border(), // index 0 default
                 new Border( // index 1 black border
                     new LeftBorder(new Color() {
                        Auto = true }) { Style = BorderStyleValues.Thin },
                     new RightBorder(new Color() {
                        Auto = true }) { Style = BorderStyleValues.Thin },
                     new TopBorder(new Color() {
                        Auto = true }) { Style = BorderStyleValues.Thin },
                     new BottomBorder(new Color() {
                        Auto = true }) { Style = BorderStyleValues.Thin },
                     new DiagonalBorder())
             );

         CellFormats cellFormats = new CellFormats(
                 new CellFormat(), // default

                 new CellFormat
                 {
                    FontId = 1,
                    FillId = 0,
                    BorderId = 1,
                    ApplyFill = true
                 },

                 // light gray
                 new CellFormat
                 {
                    FontId = 2,
                    FillId = 1,
                    BorderId = 1,
                    ApplyFill = true
                 },
                 new CellFormat {
                    FontId = 3,
                    FillId = 2,
                    BorderId = 1,
                    ApplyFill = true },

                 // light blue
                 new CellFormat
                 {
                    FontId = 2,
                    FillId = 3,
                    BorderId = 1,
                    ApplyFill = true
                 },
                 new CellFormat
                 {
                    FontId = 3,
                    FillId = 3,
                    BorderId = 1,
                    ApplyFill = true
                 },

                 // light yellow
                 new CellFormat
                 {
                    FontId = 2,
                    FillId = 4,
                    BorderId = 1,
                    ApplyFill = true
                 },
                 new CellFormat
                 {
                    FontId = 3,
                    FillId = 4,
                    BorderId = 1,
                    ApplyFill = true
                 }
             );

         Stylesheet styleSheet = new Stylesheet(
            fonts, fills, borders, cellFormats);

         return styleSheet;
      }

      #endregion
      #region -- Worksheet Reader Support

      /// <summary>
      /// Try to find a given worksheet by name and if found create a reader.
      /// </summary>
      /// <param name="worksheetName">worksheet name to find</param>
      /// <returns>if worksheet is found the instance of a workbook reader is
      /// returned, else null is returned instead</returns>
      public OpenXmlReader GetWorksheetReader(string worksheetName)
      {
         var worksheet = GetWorksheetByName(worksheetName);
         if (worksheet == null)
         {
            return null;
         }
         return OpenXmlReader.Create(worksheet);
      }

      public List<List<string>> ReadWorksheet(
         OpenXmlReader reader, WorksheetPart worksheet)
      {
         SheetData d = worksheet.Worksheet.Elements<SheetData>().First();
         List<List<string>> rowList = new List<List<string>>();
         List<string> colList;
         List<string> header = new List<string>();

         var stringTable = 
            m_Workbook.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
         string txt;
         int rowCount = 0;
         int colCount;

         foreach(Row r in d.Elements<Row>())
         {
            colList = new List<string>();
            colCount = 0;
            foreach(Cell c in r.Elements<Cell>())
            {
               string colRef = Regex.Replace(
                  c.CellReference, @"[\d-]", string.Empty);
               
               if (rowCount == 0)
               {
                  header.Add(colRef);
               }
               else if (header[colCount] != colRef)
               {
                  for(int i = colCount; i < header.Count; i++)
                  {
                     if (header[colCount] != colRef)
                     {
                        colList.Add(null);
                        colCount++;
                     }
                  }
               }

               txt = String.Empty;
               if (c.DataType != null)
               {
                  switch (c.DataType.Value)
                  {
                     case CellValues.SharedString:
                        txt = stringTable.SharedStringTable.ElementAt(
                           int.Parse(c.InnerText)).InnerText;
                        break;
                     case CellValues.String:
                        txt = c.InnerText;
                        break;
                     default:
                        txt = c.InnerText;
                        break;
                  }
               }
               else
               {
                  txt = c.InnerText;
               }
               colList.Add(txt);
               colCount++;
            }
            for(var i=colCount; i< header.Count; i++)
            {
               colList.Add(null);
            }
            rowList.Add(colList);
            rowCount++;
         }
         return rowList;
      }

      #endregion

   }

}
