using System;
using System.Xml.Serialization;

namespace Edam.TextParse
{

// SubmissionBaseQueue.h
// -----------------------------------------------------------------------------

[ Serializable ]
public enum ColumnType
{
   Unknown  = 0,
   Char     = 1,
   Varchar  = 2,
   Date     = 3,
   DateTime = 4,
   Time     = 5,
   Int16    = 6,
   Int32    = 7
}  // end of ColumnType

[ Serializable ]
public class Column
{

   private String m_Name { get; set; }
   private String m_Header { get; set; }
   private String m_Description { get; set; }

   [XmlAttribute]
   public String Name
   {
      get { return m_Name; }
      set
      {
         m_Name = (value == null) ? String.Empty : value;
      }
   }

   [XmlAttribute]
   public String Header
   {
      get { return m_Header; }
      set
      {
         m_Header = (value == null) ? String.Empty : value;
      }
   }

   [XmlAttribute]
   public ColumnType Type { get; set; }
   [XmlAttribute]
   public Int32 Length { get; set; }
   [XmlAttribute]
   public String Description
   {
      get { return m_Description; }
      set
      {
         m_Description = (value == null) ? String.Empty : value;
      }
   }

   public Column()
   {
      ClearFields();
   }

   public void ClearFields()
   {
      Name   = String.Empty;
      Header = String.Empty;
      Type   = ColumnType.Unknown;
      Length = 0;
      Description = String.Empty;
   }  // end of ClearFields

}  // end of Column

[ Serializable ]
public enum FileUploadConfigurationType
{
   Unknown = 0,
   FlatFileDelimited = 1
}  // end of FileUploadConfigurationType

[ Serializable ]
public class TextFileUploadConfiguration
{

   private String m_KeyId { get; set; }
   private String m_StoredProcedureName { get; set; }
   private String m_Description { get; set; }
   private String m_Delimiter { get; set; }
   private String m_StringDelimiter { get; set; }

   public String KeyId
   {
      get { return m_KeyId; }
      set
      {
         m_KeyId = (value == null) ? String.Empty : value;
      }
   }
   public String StoredProcedureName
   {
      get { return m_StoredProcedureName; }
      set
      {
         m_StoredProcedureName = (value == null) ? String.Empty : value;
      }
   }
   public String Description
   {
      get { return m_Description; }
      set
      {
         m_Description = (value == null) ? String.Empty : value;
      }
   }
   public Column [] Columns;
   public FileUploadConfigurationType FileUploadType;
   public String Delimiter
   {
      get { return m_Delimiter; }
      set
      {
         m_Delimiter = (value == null) ? String.Empty : value;
      }
   }
   public String StringDelimiter
   {
      get { return m_StringDelimiter; }
      set
      {
         m_StringDelimiter = (value == null) ? String.Empty : value;
      }
   }

   public void ClearFields()
   {
      KeyId = String.Empty;
      StoredProcedureName = String.Empty;
      Description = String.Empty;
      Columns = null;
      FileUploadType = FileUploadConfigurationType.Unknown;
      Delimiter = String.Empty;
      StringDelimiter = String.Empty;
   }

}  // end of TextFileUploadConfiguration

[ Serializable ]
public class TextFileUploadConfigurationCollection
{

   [System.Xml.Serialization.XmlElement(
      ElementName = "TextFileUploadConfiguration")]
   public TextFileUploadConfiguration [] Configurations;

   TextFileUploadConfigurationCollection()
   {
      Configurations = null;
   }

   public static TextFileUploadConfigurationCollection FromFile(String filePath)
   {
      if (String.IsNullOrEmpty(filePath))
         return null;
      return (TextFileUploadConfigurationCollection)
         Edam.Serialization.Serialize.FromXml(
            filePath, typeof(TextFileUploadConfigurationCollection));
   }

   public Boolean ToFile(String filePath)
   {
      if (String.IsNullOrEmpty(filePath))
         return false;
      return Edam.Serialization.Serialize.ToXml(filePath, (Object)this);
   }

   public TextFileUploadConfiguration Find(String keyId)
   {
      TextFileUploadConfiguration foundQueue = null;
      foreach(TextFileUploadConfiguration q in Configurations)
      {
         if (q.KeyId == keyId)
         {
            foundQueue = q;
            break;
         }
      }
      return foundQueue;
   }

}  // end of TextFileUploadConfigurationCollection

}  // end of Edam.TextParse

