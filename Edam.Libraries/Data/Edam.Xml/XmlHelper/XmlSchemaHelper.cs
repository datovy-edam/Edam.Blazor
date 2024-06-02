using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Edam.Xml.XmlHelper
{

   public class XmlSchemaHelper
   {

      public static List<String> XmlReaderToXmlSchema(XmlReader reader)
      {
         List<String> items = new List<string>();
         XmlSchemaInference schema = new XmlSchemaInference();
         XmlSchemaSet schemaSet = schema.InferSchema(reader);

         foreach (XmlSchema s in schemaSet.Schemas())
         {
            using var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
               s.Write(writer);
            }

            items.Add(stringWriter.ToString());
         }
         return items;
      }

      public static List<String> XmlFileToXmlSchema(String inputUri)
      {
         XmlReader reader = XmlReader.Create(inputUri);
         return XmlReaderToXmlSchema(reader);
      }

   }

}
