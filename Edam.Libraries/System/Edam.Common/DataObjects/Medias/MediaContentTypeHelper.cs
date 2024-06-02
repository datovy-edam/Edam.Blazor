using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// http://www.iana.org/assignments/media-types/media-types.xhtml
using Edam.Data;

namespace Edam.DataObjects.Medias
{

   /// <summary>
   /// Helper to Support Media Content-Type's.
   /// </summary>
   public struct MediaContentTypeHelper
   {

      public const String DataUri = "data:image/png;base64,";

      public const String ExtPDF = "pdf";
      public const String ExtPNG = "png";
      public const String ExtJPEG = "jpeg";
      public const String ExtDOCX = "docx";
      public const String ExtDOC = "doc";
      public const String ExtRTF = "rtf";
      public const String ExtTXT = "txt";
      public const String ExtXML = "xml";
      public const String ExtXSD = "xsd";
      public const String ExtJSON = "json";

      public const String TextFile = "text/plain";
      public const String XmlDocument = "text/xml";
      public const String PNG = "image/png";
      public const String JPEG = "image/jpeg";
      public const String Bitmap = "image/bmp";
      public const String MPEG = "video/mpeg";
      public const String RtfFile = "text/richtext";
      public const String JsonDocument = "application/json";
      public const String MsWordFile = "application/msword";
      public const String OfficeWordXmlFile = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
      public const String PdfFile = "application/pdf";
      public const String FaxGroup4Standard = "image/g3fax";
      public const String UnknownType = "application/octet-stream";

      public const String TextFileDescription = "Plain Text File";
      public const String XmlDocumentDescription = "XML Text File";
      public const String PNGDescription = "PNG Image File";
      public const String JPEGDescription = "JPEG Image File";
      public const String BitmapDescription = "BMP Image File";
      public const String MPEGDescription = "MPEG Video File";
      public const String RtfFileDescription = "Rich-Text File";
      public const String MsWordFileDescription = "MS-Word Document";
      public const String OfficeWordFileDescription = "Office-Word Document";
      public const String PdfFileDescription = "PDF Document";
      public const String FaxGroup4StandardDescription =
         "FAX Group 4 Standard Image";
      public const String UnknownTypeDescription =
         "Octet-Stream File";

      /// <summary>
      /// Given a media format enum convert it to a known Content-Type.
      /// </summary>
      /// <param name="format">given MediaFormat</param>
      /// <returns>the mapped content-type is returned, if it was not mapped
      /// to a recognize type the "application/octet-stream" will be returned.
      /// </returns>
      public static String ToContentTypeText(MediaFormat format)
      {
         String ctype = String.Empty;
         switch (format)
         {
            case MediaFormat.TextFile:
               ctype = TextFile;
               break;
            case MediaFormat.XmlDocument:
               ctype = XmlDocument;
               break;
            case MediaFormat.PNG:
               ctype = PNG;
               break;
            case MediaFormat.JPEG:
               ctype = JPEG;
               break;
            case MediaFormat.Bitmap:
               ctype = Bitmap;
               break;
            case MediaFormat.MPEG:
               ctype = MPEG;
               break;
            case MediaFormat.RtfFile:
               ctype = RtfFile;
               break;
            case MediaFormat.MsWordFile:
               ctype = MsWordFile;
               break;
            case MediaFormat.OfficeWordXml:
               ctype = OfficeWordXmlFile;
               break;
            case MediaFormat.PdfFile:
               ctype = PdfFile;
               break;
            case MediaFormat.FaxGroup4Standard:
               ctype = FaxGroup4Standard;
               break;
            default:
               // undefined content type (binary)
               ctype = UnknownType;
               break;
         }
         return ctype;
      }

      /// <summary>
      /// Get the Content Type Description.
      /// </summary>
      /// <param name="format">Corresponding Media Format</param>
      /// <returns>description is returned</returns>
      public static String ToContentTypeDescription(MediaFormat format)
      {
         String ctype = String.Empty;
         switch (format)
         {
            case MediaFormat.TextFile:
               ctype = TextFileDescription;
               break;
            case MediaFormat.XmlDocument:
               ctype = XmlDocumentDescription;
               break;
            case MediaFormat.PNG:
               ctype = PNGDescription;
               break;
            case MediaFormat.JPEG:
               ctype = JPEGDescription;
               break;
            case MediaFormat.Bitmap:
               ctype = BitmapDescription;
               break;
            case MediaFormat.MPEG:
               ctype = MPEGDescription;
               break;
            case MediaFormat.RtfFile:
               ctype = RtfFileDescription;
               break;
            case MediaFormat.MsWordFile:
               ctype = MsWordFileDescription;
               break;
            case MediaFormat.OfficeWordXml:
               ctype = OfficeWordFileDescription;
               break;
            case MediaFormat.PdfFile:
               ctype = PdfFileDescription;
               break;
            case MediaFormat.FaxGroup4Standard:
               ctype = FaxGroup4StandardDescription;
               break;
            default:
               // undefined content type (binary)
               ctype = UnknownTypeDescription;
               break;
         }
         return ctype;
      }

      /// <summary>
      /// Convert a Content-Type text into a MediaFormat enum.
      /// </summary>
      /// <param name="contentType">Content-Type text</param>
      /// <returns>corresponding MediaFormat enum is returned</returns>
      public static MediaFormat ToFormat(String contentType)
      {
         MediaFormat f = MediaFormat.Unknown;

         if (contentType == TextFile)
            return MediaFormat.TextFile;
         if (contentType == XmlDocument)
            return MediaFormat.XmlDocument;
         if (contentType == PNG)
            return MediaFormat.PNG;
         if (contentType == JPEG)
            return MediaFormat.JPEG;
         if (contentType == Bitmap)
            return MediaFormat.Bitmap;
         if (contentType == MPEG)
            return MediaFormat.MPEG;
         if (contentType == RtfFile)
            return MediaFormat.RtfFile;
         if (contentType == MsWordFile)
            return MediaFormat.MsWordFile;
         if (contentType == OfficeWordXmlFile)
            return MediaFormat.OfficeWordXml;
         if (contentType == PdfFile)
            return MediaFormat.PdfFile;
         if (contentType == FaxGroup4Standard)
            return MediaFormat.FaxGroup4Standard;
         if (contentType == UnknownType)
            return MediaFormat.Unknown;

         return f;
      }

      /// <summary>
      /// Convert a media format to a Content-Type enum.
      /// </summary>
      /// <param name="format">format</param>
      /// <returns>corresponding MediaContentType enum is returned</returns>
      public static MediaContentType ToContentType(MediaFormat format)
      {
         MediaContentType c = MediaContentType.application_octetStream;

         switch (format)
         {
            case MediaFormat.JPEG:
               c = MediaContentType.image_jpeg;
               break;
            case MediaFormat.MPEG:
               c = MediaContentType.video_mpeg;
               break;
            case MediaFormat.TextFile:
               c = MediaContentType.text_plain;
               break;
            case MediaFormat.RtfFile:
               c = MediaContentType.text_richtext;
               break;
            case MediaFormat.MsWordFile:
               c = MediaContentType.application_msword;
               break;
            case MediaFormat.OfficeWordXml:
               c = MediaContentType.application_officeword;
               break;
            case MediaFormat.PdfFile:
               c = MediaContentType.application_pdf;
               break;
            case MediaFormat.XmlDocument:
               c = MediaContentType.text_xml;
               break;
            case MediaFormat.PNG:
               c = MediaContentType.image_png;
               break;
            default:
               c = MediaContentType.application_octetStream;
               break;
         }
         
         return c;
      }

      /// <summary>
      /// Get Media Format base on an extension...
      /// </summary>
      /// <param name="extension">given extension</param>
      /// <returns>MediaFormat enum is returned</returns>
      public static MediaFormat GetMediaFormat(String extension)
      {
         MediaFormat f = MediaFormat.TextFile;
         if (extension == ExtDOC)
            f = MediaFormat.MsWordFile;
         else if (extension == ExtDOCX)
            f = MediaFormat.OfficeWordXml;
         else if (extension == ExtJPEG)
            f = MediaFormat.JPEG;
         else if (extension == ExtPDF)
            f = MediaFormat.PdfFile;
         else if (extension == ExtPNG)
            f = MediaFormat.PNG;
         else if (extension == ExtRTF)
            f = MediaFormat.RtfFile;
         return f;
      }

      /// <summary>
      /// Get file name with an extention corresponding to its content type.
      /// </summary>
      /// <param name="name">(optional) file name</param>
      /// <param name="contentType">(optional) file content type to derive the
      /// file extension from</param>
      /// <returns>a full file name with extention is returned</returns>
      public static String GetFileName(String name = null, 
         MediaFormat contentType = MediaFormat.TextFile)
      {
         String nm = (String.IsNullOrWhiteSpace(name)) ? 
            Edam.Text.UniqueId.GetUniqueSequentialId() : name;
         String ext;
         switch(contentType)
         {
            case MediaFormat.PdfFile:
               ext = ExtPDF;
               break;
            case MediaFormat.PNG:
               ext = ExtPNG;
               break;
            case MediaFormat.JPEG:
               ext = ExtJPEG;
               break;
            case MediaFormat.OfficeWordXml:
               ext = ExtDOCX;
               break;
            case MediaFormat.MsWordFile:
               ext = ExtDOC;
               break;
            case MediaFormat.RtfFile:
               ext = ExtRTF;
               break;
            case MediaFormat.TextFile:
               ext = ExtTXT;
               break;
            default:
               ext = ExtTXT;
               break;
         }
         return nm + "." + ext;
      }

   }
   
}
