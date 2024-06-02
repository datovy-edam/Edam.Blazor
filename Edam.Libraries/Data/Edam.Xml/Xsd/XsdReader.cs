using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Edam.Xml.XmlExplore;

// -------------------------------------------------------------------------------------------------

namespace XmlHelper.Xsd
{

   public class XsdReader
   {

      private readonly XmlSchema m_Schema = null;
      public XmlSchema Schema
      {
         get { return m_Schema; }
      }

      public XsdReader(XmlSchema schema)
      {
         m_Schema = schema;
      }

      public XsdReader(String path)
      {
         XmlTextReader reader = new XmlTextReader(path);
         m_Schema = XmlSchema.Read(reader, ValidationCallback);
      }

      /// <summary>
      /// Read Schema.
      /// </summary>
      /// <param name="path"></param>
      /// <returns>XsdReader instance is returned with inner schema</returns>
      public static XsdReader ReadSchema(String path)
      {
         XsdReader r = new XsdReader(path);
         return r;
      }

      public void GetSampleXml(String rootElementName, String ns,
         String outFilePath)
      {
         XmlQualifiedName n = new XmlQualifiedName(rootElementName, ns);
         XmlCrawler g = new XmlCrawler(m_Schema, n);
         XmlTextWriter textWriter = new XmlTextWriter(outFilePath, null)
         {
            Formatting = Formatting.Indented
         };
         XmlCrawler generator = new XmlCrawler(m_Schema, n);
         generator.WriteXml(textWriter);
      }

      /// <summary>
      /// Validate Schema Call-Back...
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="args"></param>
      static void ValidationCallback(object sender, ValidationEventArgs args)
      {
         if (args.Severity == XmlSeverityType.Warning)
            Console.Write("WARNING: ");
         else if (args.Severity == XmlSeverityType.Error)
            Console.Write("ERROR: ");

         Console.WriteLine(args.Message);
      }

   }

}
