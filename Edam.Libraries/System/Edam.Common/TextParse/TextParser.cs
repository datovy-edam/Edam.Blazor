using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edam.TextParse
{

public enum FileFormat
{
   Unknown = 0,
   JPEG = 1,
   MPEG = 2,
   TextFile = 3,
   RTFFile = 4,
   MSWordFile = 5,
   PDFFile = 6,
   XMLDocument = 7,
   JPEG2000 = 20,
   WSQ = 21,
   Bitmap = 22,
   VectorData = 23,
   FAXGroup4Standard = 24
}  // end of FileFormat

public class FilePath
{

   private String m_FilePath;
   private String m_DirectoryPath;
   private String m_FileName;
   private String m_FileNamePart;
   private String m_FileNamePartExt;

   private void Initialize()
   {
      m_FilePath = String.Empty;
      m_FileName = String.Empty;
      m_FileNamePart = String.Empty;
      m_FileNamePartExt = String.Empty;
   }  // end of Initialize

   public String DirectoryPath
   {
      get
      {
         return m_DirectoryPath;
      }
   }

   public String FileFullPath
   {
      get
      {
         return m_FilePath;
      }
   }

   public String FileName
   {
      get
      {
         return m_FileName;
      }
   }

   public String FileNamePart
   {
      get
      {
         return m_FileNamePart;
      }
   }

   public String FileNamePartExt
   {
      get
      {
         return m_FileNamePartExt;
      }
   }

   public FilePath(String filePath)
   {
      ParseFilePath(filePath);
   }
   public FilePath()
   {
      Initialize();
   }

   /// <summary>Given a file path decompose it into its commonly used parts
   /// </summary>
   /// <param name="filePath">file path</param>
   void ParseFilePath(String filePath)
   {
      if (String.IsNullOrEmpty(filePath))
      {
         m_DirectoryPath = String.Empty;
         m_FilePath = String.Empty;
         m_FileName = String.Empty;
         m_FileNamePart = String.Empty;
         m_FileNamePartExt = String.Empty;
         return;
      }

      Char [] m_FilePathSeparators = { '\\','/' };
      Char [] m_FileNameSeparators = { '.' };
      String [] m_FilePathTokens;
      String [] m_FileNameTokens;

      m_FilePath = filePath.Replace("\\","/");
      m_FilePathTokens = m_FilePath.Split(m_FilePathSeparators);

      if (m_FilePathTokens == null)
         m_FileName = String.Empty;
      else
      if (m_FilePathTokens.Length == 0)
         m_FileName = String.Empty;
      else
         m_FileName = m_FilePathTokens[m_FilePathTokens.Length - 1];

      m_FileNameTokens = m_FileName.Split(m_FileNameSeparators);

      if (m_FileNameTokens == null)
      {
         m_FileNamePart = String.Empty;
         m_FileNamePartExt = String.Empty;
      }
      else
      if (m_FileNameTokens.Length == 0)
      {
         m_FileNamePart = String.Empty;
         m_FileNamePartExt = String.Empty;
      }
      else
      {
         m_FileNamePart = m_FileNameTokens[0];
         if (m_FileNameTokens.Length > 1)
         {
            Int32 i;
            for(i=1; i < m_FileNameTokens.Length-1; i++)
               m_FileNamePart += "." + m_FileNameTokens[i];
            m_FileNamePartExt
               = m_FileNameTokens[m_FileNameTokens.Length-1];
         }
         else
            m_FileNamePartExt = String.Empty;
      }

      if ((filePath.Length > 0) && (m_FileName.Length > 0))
         m_DirectoryPath = filePath.Remove(
            filePath.Length - m_FileName.Length, m_FileName.Length);
      else
         m_DirectoryPath = String.Empty;

      if (m_FilePathTokens != null)
          m_FilePathTokens = null;
      if (m_FileNameSeparators != null)
          m_FileNameSeparators = null;
   }  // end of FilePath.FilePath (ctor)

   /// <summary>Get File Fromat using given file name extension</summary>
   /// <param name="fileExtension">file Extension</param>
   /// <returns>the "FileFormat" (enum) is returned</returns>
   public static FileFormat GetFileFormatWithFileExtension(String fileExtension)
   {
      String nm = fileExtension.ToLower();
      if (nm == "jpg")
         return FileFormat.JPEG;
      if (nm == "mpg")
         return FileFormat.MPEG;
      if (nm == "txt")
         return FileFormat.TextFile;
      if (nm == "rtf")
         return FileFormat.RTFFile;
      if (nm == "doc")
         return FileFormat.MSWordFile;
      if (nm == "pdf")
         return FileFormat.PDFFile;
      if (nm == "xdoc")
         return FileFormat.XMLDocument;
      if (nm == "jp2")
         return FileFormat.JPEG2000;
      if (nm == "wsq")
         return FileFormat.JPEG2000;
      if (nm == "bmp")
         return FileFormat.Bitmap;
      return FileFormat.Unknown;
   }  // end of GetFileFormatWithFileExtension

   /// <summary>Get File Fromat using given file name path</summary>
   /// <param name="fileExtension">file Extension</param>
   /// <returns>the "FileFormat" (enum) is returned</returns>
   static FileFormat GetFileFormatWithFilePath(String filePath)
   {
      FilePath path = new FilePath(filePath);
      return GetFileFormatWithFileExtension(path.FileNamePartExt);
   }  // end of GetFileFormatWithFilePath

   /// <summary>Get File Name from given path.</summary>
   /// <param name="filePath">file path to parse</param>
   /// <returns>the file name is returned.</returns>
   static String GetFileNameFromPath(String filePath)
   {
      Char [] sep = { '\\','/' };
      String [] tokens = filePath.Split(sep);
      if (tokens == null)
         return String.Empty;
      else
      if (tokens.Length == 0)
         return String.Empty;
      String name = tokens[tokens.Length - 1];
      tokens = null;
      return name;
   }  // end of GetFileNameFromPath

}  // end of FilePath

/// <summary>Detail the results after parsing a line...</summary>
public class ParseLineResults
{

   public Boolean Succeeded;
   public String OriginalText;
   public String [] CurrentLine;
   public Int32 ParsedLines;
   public Int32 ErrorLines;

   ParseLineResults()
   {
      Succeeded    = false;
      ParsedLines  = 0;
      ErrorLines   = 0;
      OriginalText = String.Empty;
   }
}  // end of ParseLineResults

/// <summary>Provided to Parse a delimited TextFile.</summary>
public class TextFile: IDisposable 
{

   private System.IO.StringReader m_TextDataReader;
   private String [] m_TextData;
   private Char [] m_Separators;
   private String [] m_SchemaLine;

   private ParseLineResults m_Result = null;
   private Int16 m_FieldsPerRow;

   private String m_NextLine;
   private Int32 m_CurrentLineNo;

   private String GetNextLine(Int32 lineIndex)
   {
      if (m_NextLine != null)
      {
         String currLine = m_NextLine;
         m_NextLine = null;
         return currLine;
      }
      if (m_TextData == null)
      {
         if (m_TextDataReader == null)
            return String.Empty;
         String currLine = m_TextDataReader.ReadLine();
         return String.IsNullOrEmpty(currLine) ? String.Empty : currLine;
      }
      return m_TextData[lineIndex];
   }  // end of GetNextLine

   private Boolean MoreLines(Int32 index)
   {
      if (m_TextData == null)
      {
         if (m_TextDataReader == null)
            return false;
         return m_TextDataReader.Peek() != -1;
      }
      return (index < m_TextData.Length);
   }  // end of MoreLines

   public String [] FirstLine
   {
      get
      {
         return m_SchemaLine;
      }
   }  // end of FirstLine

   public Boolean EndOfFile
   {
      get
      {
         return (!MoreLines(m_CurrentLineNo));
      }
   }  // end of EndOfFile

   public String [] ParsedLine
   {
      get
      {
         if (m_Result.Succeeded)
            return m_Result.CurrentLine;
         return null;
      }
   }  // end of ParsedLine

   public Int32 TotalParsedLines
   {
      get
      {
         return m_Result.ErrorLines + m_Result.ParsedLines;
      }
   }  // end of TotalParsedLines

   public Int32 TotalErrorsFound
   {
      get
      {
         return m_Result.ErrorLines;
      }
   }  // end of TotalParsedLines

   public String OriginalText
   {
      get
      {
         if (String.IsNullOrEmpty(m_Result.OriginalText))
            return String.Empty;
         return m_Result.OriginalText;
      }
   }  // end of OriginalText

   public TextFile()
   {
      m_TextData = null;
      m_Separators = new Char[] { ',' };
      m_CurrentLineNo = 0;
      m_FieldsPerRow  = 0;
      m_NextLine = null;
      m_TextDataReader = null;
   }  // end of TextFile (ctor)

   public void Dispose()
   {
      Close();
   }  // end of TextFile (dtor)

   public void Close()
   {
      if (m_TextDataReader != null)
         m_TextDataReader.Dispose();
      m_TextDataReader = null;
      m_TextData = null;
   }  // end of Close

   /// <summary>Set TextData using given string of lines</summary>
   /// <param name="textData">Text Data that contains a collection of
   /// String lines</param>
   public void SetTextData(String textData)
   {
      m_TextDataReader = new System.IO.StringReader(textData);
      m_TextData = null;
   }  // end of Set Text Data

   /// <summary>From File</summary>
   public Int32 FromFile(String filePath)
   {
      m_CurrentLineNo = 0;
      if (String.IsNullOrEmpty(filePath))
         return 0;
      if (!System.IO.File.Exists(filePath))
         return 0;
      m_TextData = System.IO.File.ReadAllLines(filePath);
      return m_TextData.Length;
   }  // end of From File

   /// <summary>Setup the schema by reading the first line that has something
   /// to be read by skiping all top lines that are empty.</summary>
   public void SetSchemaFromFirstLine()
   {
      String textData = String.Empty;
      Int32 l = 0;
      while (MoreLines(l))
      {
         textData = GetNextLine(l);
         if (String.IsNullOrEmpty(textData))
            l++;
         else
            break;
      }

      m_SchemaLine    = textData.Split(m_Separators);
      m_CurrentLineNo = l;
      m_FieldsPerRow  = (Int16)m_SchemaLine.Length;
   }  // end of SetSchemaFromFirstLine

   /// <summary>Parse current line and move to next...</summary>
   Boolean ReadLine()
   {
      if (EndOfFile)
         return false;

      String textDataLine = GetNextLine(m_CurrentLineNo);
      m_Result.CurrentLine = textDataLine.Split(m_Separators);
      if (m_Result.CurrentLine.Length == m_FieldsPerRow)
      {
         m_Result.Succeeded = true;
         m_CurrentLineNo++;
         m_Result.ParsedLines++;
         m_Result.OriginalText = textDataLine;
      }
      else
      if (m_Result.CurrentLine.Length < m_FieldsPerRow)
      {
         Int32 i;
         m_Result.Succeeded = false;
         String currLine = textDataLine;
         if (!MoreLines(m_CurrentLineNo + 1))
         {
            m_CurrentLineNo++;
            m_Result.OriginalText = currLine;
            m_Result.Succeeded = false;
            m_Result.ErrorLines++;
         }
         else
         {
            for(i=m_CurrentLineNo + 1; MoreLines(i); i++)
            {
               m_Result.OriginalText = currLine;
               m_NextLine = null;
               m_NextLine = GetNextLine(i);
               currLine += m_NextLine;
               m_Result.CurrentLine = currLine.Split(m_Separators);
               if (m_Result.CurrentLine.Length == m_FieldsPerRow)
               {
                  m_Result.OriginalText = currLine;
                  m_Result.Succeeded = true;
                  m_CurrentLineNo = i + 1;
                  m_Result.ParsedLines++;
                  m_NextLine = null;
                  break;
               }
               else
               if (m_Result.CurrentLine.Length > m_FieldsPerRow)
               {
                  m_CurrentLineNo = i;
                  m_Result.ErrorLines++;
                  break;
               }
            }
         }
      }
      else
      {
         m_Result.OriginalText = textDataLine;
         m_Result.Succeeded = false;
         m_Result.ErrorLines++;
         m_CurrentLineNo++;
      }

      return ParsedLine != null;
   }  // end of ReadLine

   /// <summary>Test TextFile Parsing</summary>
   /// <param name="filePath">File Path</param>
   void TestTextFileParsing(String filePath)
   {
      Edam.TextParse.TextFile tf = new TextFile();
      tf.FromFile(filePath);
      tf.SetSchemaFromFirstLine();
      while(tf.ReadLine())
         System.Console.WriteLine(tf.OriginalText);
      tf.Dispose();
   }  // end of TestTextFileParsing

   /// <summary>Test TextFile Parsing using a text data stream</summary>
   /// <param name="textData">Text Data</param>
   void TestTextStreamParsing(String textData)
   {
      Edam.TextParse.TextFile tf = new TextFile();
      tf.SetTextData(textData);
      tf.SetSchemaFromFirstLine();
      while(tf.ReadLine())
         System.Console.WriteLine(tf.OriginalText);
      tf.Dispose();
   }  // end of TestTextFileParsing

}  // end of TextFile

//generic <typename T>
public class Parse
{
   private Int32 currIndex, lastTokenCount;
   //array<T> buffer, lastToken;
   private Char [] buffer, lastToken;

   public Parse()
   {
      lastToken = new Char[(1)];
   }

   public const Char space = ' ';

   public Char [] Buffer
   {
      get { return buffer; }
      set
      {
         buffer = value;
         currIndex = 0;
      }
   }
      
   public Char [] LastToken
   {
      get { return lastToken; }
   }

   public bool EndOfBuffer
   {
      get
      {
         return(currIndex >= Buffer.Length);
      }
   }

   /// <summary>Skip Blacks and non-printable chars.</summary>
   /// <returns>the current buffer position info is returned</returns>
   public Int32 SkipBlanks()
   {
      while ((currIndex < buffer.Length) && (buffer[currIndex] <= (Char)space))
         currIndex++;
      return currIndex;
   }

   public Char [] GetToken()
   {
      // clear last token
      lastTokenCount = 0;
      System.Array.Clear(lastToken,0,lastToken.Length);

      // move to next token's first char
      SkipBlanks();
      if (EndOfBuffer)
         return lastToken;

      // get next token
      Int32 cindx = 0,sindx = currIndex;
      while ((currIndex < buffer.Length) && (buffer[currIndex] > (Char)space))
      {
         cindx++;
         currIndex++;
      }

      // prepare last token
      if (cindx == 0)
         return lastToken;
      if (lastToken.Length < cindx)
         Array.Resize(ref lastToken,cindx);
      Int32 i;
      for (i=0; i<cindx; i++, sindx++, lastTokenCount++)
         lastToken[i] = buffer[sindx];

      return lastToken;
   }  // end of GetToken

   public String GetTokenAsString()
   {
      GetToken();
      return new System.String(lastToken,0,lastTokenCount);
   }

}  // end of Parse

}
