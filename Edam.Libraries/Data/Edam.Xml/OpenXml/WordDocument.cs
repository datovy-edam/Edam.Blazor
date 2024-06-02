using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using wp = DocumentFormat.OpenXml.Wordprocessing;

/*
 * https://learn.microsoft.com/en-us/office/open-xml/word-processing
 */

namespace Edam.Xml.OpenXml
{

   public class WordDocument : IDisposable
   {

      #region -- Declarations

      private const string SCHEMA_URI = 
         "https://schemas.openxmlformats.org/wordprocessingml/2006/main";

      private WordprocessingDocument m_WordDocument;
      private Document m_Document;
      private Body m_Body;

      #endregion
      #region -- Constructors - Destructors

      public WordDocument()
      {

      }

      /// <summary>
      /// Release all resources
      /// </summary>
      public void Dispose()
      {
         if (m_WordDocument != null)
            m_WordDocument.Dispose();
         m_WordDocument = null;
         m_Document = null;
      }

      #endregion
      #region -- Document Support

      /// <summary>
      /// Open existing document.
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="isEditable"></param>
      public void OpenDocument(string fileName, bool isEditable = false)
      {
         m_WordDocument = WordprocessingDocument.Open(fileName, isEditable);
         m_Document = m_WordDocument.MainDocumentPart.Document;
      }

      /// <summary>
      /// Create Document.
      /// </summary>
      /// <param name="fileName"></param>
      public void CreateDocument(string fileName)
      {
         m_WordDocument =
            WordprocessingDocument.Create(
               fileName, WordprocessingDocumentType.Document);
         var mainPart = m_WordDocument.AddMainDocumentPart();
         mainPart.Document = new Document();
         m_Document = mainPart.Document;
         m_Body = new Body();
         m_Document.AppendChild(m_Body);
      }

      public void Save()
      {
         m_WordDocument.Save();
      }

      #endregion
      #region -- 4.00 - Manage Table Preparation

      /// <summary>
      /// Prepare Table by setting table properties.
      /// </summary>
      /// <param name="fontSize">font size (default: 12)</param>
      /// <param name="borderValues">border style value</param>
      /// <returns>instance of prepared table is returned</returns>
      public Table PrepareTable(UInt32? fontSize = 12, 
         BorderValues? borderValues = null)
      {
         if (borderValues == null)
         {
            borderValues = BorderValues.Single;
         }

         Table table = new Table();

         TableProperties props = new TableProperties(
             new TableBorders(
             new TopBorder
             {
                Val = new EnumValue<BorderValues>(borderValues),
                Size = fontSize
             },
             new BottomBorder
             {
                Val = new EnumValue<BorderValues>(borderValues),
                Size = fontSize
             },
             new LeftBorder
             {
                Val = new EnumValue<BorderValues>(borderValues),
                Size = fontSize
             },
             new RightBorder
             {
                Val = new EnumValue<BorderValues>(borderValues),
                Size = fontSize
             },
             new InsideHorizontalBorder
             {
                Val = new EnumValue<BorderValues>(borderValues),
                Size = fontSize
             },
             new InsideVerticalBorder
             {
                Val = new EnumValue<BorderValues>(borderValues),
                Size = fontSize
             }));

         table.AppendChild<TableProperties>(props);
         return table;
      }

      /// <summary>
      /// Add Table using given data.
      /// </summary>
      /// <param name="table">table to insert table into...</param>
      /// <param name="data">list of rows and for each list of columns</param>
      public void AddTableData(Table table, List<List<string>> data)
      {
         foreach(var row in data)
         {
            var tr = new TableRow();
            foreach(var cell in row)
            {
               var tc = new TableCell();
               tc.Append(new Paragraph(new Run(new wp.Text(cell))));
               tc.Append(new TableCellProperties(
                  new TableCellWidth { Type = TableWidthUnitValues.Auto }));
               tr.Append(tc);
            }
            table.Append(tr);
         }
      }

      /// <summary>
      /// Add Table into the document.
      /// </summary>
      /// <param name="table">table to be added</param>
      public void AddTable(Table table)
      {
         m_Body.AppendChild(table);
      }

      #endregion
      #region -- 4.00 - Manage Paragraph

      public void AddParagraph(string text)
      {
         Paragraph para = m_Body.AppendChild(new Paragraph());
         var run = para.AppendChild(new Run());
         run.AppendChild(new wp.Text(text));
      }

      #endregion
      #region -- 4.00 - Manage Revisions

      /// <summary>
      /// Accept Revisions.
      /// </summary>
      /// <param name="fileName">file name</param>
      /// <param name="authorName">author name</param>
      public static void AcceptRevisions(string fileName, string authorName)
      {
         // Given a document name and an author name, accept revisions. 
         using (WordprocessingDocument wdDoc =
             WordprocessingDocument.Open(fileName, true))
         {
            Body body = wdDoc.MainDocumentPart.Document.Body;

            // Handle the formatting changes.
            List<OpenXmlElement> changes =
                body.Descendants<ParagraphPropertiesChange>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList();

            foreach (OpenXmlElement change in changes)
            {
               change.Remove();
            }

            // Handle the deletions.
            List<OpenXmlElement> deletions =
                body.Descendants<Deleted>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList();

            deletions.AddRange(body.Descendants<DeletedRun>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList());

            deletions.AddRange(body.Descendants<DeletedMathControl>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList());

            foreach (OpenXmlElement deletion in deletions)
            {
               deletion.Remove();
            }

            // Handle the insertions.
            List<OpenXmlElement> insertions =
                body.Descendants<Inserted>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList();

            insertions.AddRange(body.Descendants<InsertedRun>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList());

            insertions.AddRange(body.Descendants<InsertedMathControl>()
                .Where(c => c.Author.Value == authorName).
                   Cast<OpenXmlElement>().ToList());

            foreach (OpenXmlElement insertion in insertions)
            {
               // Found new content.
               // Promote them to the same level as node,
               // and then delete the node.
               foreach (var run in insertion.Elements<Run>())
               {
                  if (run == insertion.FirstChild)
                  {
                     insertion.InsertAfterSelf(new Run(run.OuterXml));
                  }
                  else
                  {
                     insertion.NextSibling().
                        InsertAfterSelf(new Run(run.OuterXml));
                  }
               }
               insertion.RemoveAttribute("rsidR", SCHEMA_URI);
               insertion.RemoveAttribute("rsidRPr", SCHEMA_URI);
               insertion.Remove();
            }
         }
      }

      #endregion

   }

}
