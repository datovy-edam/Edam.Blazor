using System;
using System.Runtime.Serialization.Formatters.Binary;

// -----------------------------------------------------------------------------

namespace Edam.Serialization
{

   public class Serialize
   {

      #region -- Binary Serialization

      ///// <summary>Store object members in a binary formatted file.</summary>
      ///// <param name="Path">File path name.</param>
      ///// <param name="Obj">Object to be serialized.</param>
      ///// <returns>true is returned if file was created.</returns>
      ///// <date>Nov/2k3  ESob</date>
      //public static Byte[] ToBinary(System.Object obj)
      //{
      //   System.IO.MemoryStream ms = new System.IO.MemoryStream();
      //   BinaryFormatter bf = new BinaryFormatter();
      //   bf.Serialize(ms, obj);
      //   ms.Flush();
      //   Byte[] buffer = ms.ToArray();
      //   ms.Dispose();
      //   return (buffer);
      //}

      ///// <summary>Read object members from a binary formatted file.</summary>
      ///// <param name="Path">File path name to read from.</param>
      ///// <returns>Read object is returned.</returns>
      ///// <date>Nov/2k3  ESob</date>
      //public static System.Object FromBinary(Byte[] buffer)
      //{
      //   System.Object obj = null;
      //   System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
      //   BinaryFormatter bf = new BinaryFormatter();

      //   if ((ms != null) && (bf != null))
      //   {
      //      obj = bf.Deserialize(ms);
      //      ms.Close();
      //   }

      //   return (obj);
      //}  // end of de-serialize object from binary

      ///// <summary>Store object members in a binary formatted file.</summary>
      ///// <param name="Path">File path name.</param>
      ///// <param name="Obj">Object to be serialized.</param>
      ///// <returns>true is returned if file was created.</returns>
      ///// <date>Nov/2k3  ESob</date>
      //public static bool ToBinary(System.String Path, System.Object Obj)
      //{
      //   bool done = false;
      //   System.IO.Stream sw;
      //   BinaryFormatter bf = new BinaryFormatter();

      //   if (System.IO.File.Exists(Path))
      //      System.IO.File.Delete(Path);

      //   sw = System.IO.File.Open(Path, System.IO.FileMode.Create);

      //   if ((sw != null) && (bf != null))
      //   {
      //      bf.Serialize(sw, Obj);
      //      sw.Close();
      //      done = true;
      //   }

      //   return (done);
      //}  // end of serialize object to binary

      ///// <summary>Read object members from a binary formatted file.</summary>
      ///// <param name="Path">File path name to read from.</param>
      ///// <returns>Read object is returned.</returns>
      ///// <date>Nov/2k3  ESob</date>
      //public static System.Object FromBinary(System.String Path)
      //{
      //   System.Object obj = null;
      //   System.IO.Stream sw;
      //   BinaryFormatter bf = new BinaryFormatter();

      //   if (!System.IO.File.Exists(Path))
      //   {
      //      return (obj);
      //   }

      //   if (System.IO.File.Exists(Path))
      //      sw = System.IO.File.Open(Path, System.IO.FileMode.Open);
      //   else
      //      sw = null;

      //   if ((sw != null) && (bf != null))
      //   {
      //      obj = bf.Deserialize(sw);
      //      sw.Close();
      //   }

      //   return (obj);
      //}  // end of deserialize object from binary

      #endregion
      #region -- Base64 Serialization

      /// <summary>
      /// Serialize an object into a Base64 encoded Text string.
      /// </summary>
      /// <param name="bytesArray">binary data</param>
      /// <returns>Base-64 encoded text is returned</returns>
      public static String ToBase64(Byte[] bytesArray)
      {
         return System.Convert.ToBase64String(bytesArray);
      }

      ///// <summary>
      ///// Serialize an object into a Base64 encoded Text string.
      ///// </summary>
      ///// <param name="obj">object to encode</param>
      ///// <returns>Base-64 encoded text is returned</returns>
      //public static String ToBase64(System.Object obj)
      //{
      //   Byte[] bytesArray = ToBinary(obj);
      //   return System.Convert.ToBase64String(bytesArray);
      //}

      ///// <summary>
      ///// Given a Base-64 encoded text string, get the original object.
      ///// </summary>
      ///// <param name="base64EncodedText"></param>
      ///// <returns></returns>
      //public static System.Object FromBase64(String base64EncodedText)
      //{
      //   Byte[] bytesArray = System.Convert.FromBase64String(base64EncodedText);
      //   return FromBinary(bytesArray);
      //}

      #endregion
      #region -- XML Serialization

      /// <summary>Store object members in an Xml string buffer.</summary>
      /// <param name="Obj">Object to be serialized.</param>
      /// <returns>returns a StringBuilder instance.</returns>
      /// <date>Nov/2k3  ESob</date>
      public static System.String ToXmlString(System.Object Obj)
      {
         System.Text.StringBuilder outString ;
         System.IO.StringWriter sw ;
         System.Xml.Serialization.XmlSerializer bf =
            new System.Xml.Serialization.XmlSerializer(Obj.GetType()) ;

         outString = new System.Text.StringBuilder() ;
         sw = new System.IO.StringWriter(outString) ;

         if ((sw != null) && (bf != null)) {
            bf.Serialize(sw,Obj) ;
            //sw.Close() ;
         }

         return(outString.ToString()) ;
      }  // end of ToXmlString(...)

      /// <summary>Store object members in an Xml string buffer.</summary>
      /// <param name="Obj">Object to be serialized.</param>
      /// <returns>returns a StringBuilder instance.</returns>
      /// <date>Nov/2k3  ESob</date>
      public static System.String ToXmlString(System.Object Obj, Boolean omitXmlDeclaration)
      {
         //System.Text.StringBuilder outString;
         //System.IO.StringWriter sw;

         System.Xml.Serialization.XmlSerializer bf =
            new System.Xml.Serialization.XmlSerializer(Obj.GetType());

         System.Xml.XmlWriterSettings settings =
               new System.Xml.XmlWriterSettings();
         settings.OmitXmlDeclaration = omitXmlDeclaration;
         settings.Encoding = System.Text.Encoding.UTF8;
         settings.Indent = true;
         settings.IndentChars = "\t";
         settings.NewLineChars = Environment.NewLine;
         settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;

         using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
         {
            using (System.Xml.XmlWriter writer =
               System.Xml.XmlWriter.Create(ms, settings))
            {               
               bf.Serialize(writer, Obj);
            }

            System.Text.Encoding encoding = new System.Text.UTF8Encoding();

            string xml = encoding.GetString(ms.ToArray(), 0, (int)ms.Length);
            return xml;
         }

         //if ((sw != null) && (bf != null))
         //{
         //   bf.Serialize(sw, Obj);
         //   sw.Close();
         //}

         //return (outString.ToString());
      }  // end of ToXmlString(...)

      /// <summary>Read object members from an Xml string buffer.</summary>
      /// <param name="StringBuffer">String containing the XML to extract object
      ///    instance from.</param>
      /// <param name="ObjType">Object type to be deserialized.</param>
      /// <returns>Read object is returned.</returns>
      /// <date>Nov/2k3  ESob</date>
      public static System.Object FromXmlString(
         System.String StringBuffer, Type ObjType)
      {
         System.Object obj = null ;
         System.IO.StringReader sw ;
         System.Xml.Serialization.XmlSerializer bf =
            new System.Xml.Serialization.XmlSerializer(ObjType) ;

         sw = new System.IO.StringReader(StringBuffer);

         if ((sw != null) && (bf != null))
         {
            try {
               obj = bf.Deserialize(sw) ;
            }
            finally {
               sw.Dispose(); // Close() ;
            }
         }

         return(obj) ;
      }  // end of deserialize object from a string buffer

      /// <summary>Store object members in an Xml file.</summary>
      /// <param name="Path">File path name.</param>
      /// <param name="Obj">Object to be serialized.</param>
      /// <returns>true is returned if file was created.</returns>
      /// <date>Nov/2k3  ESob</date>
      public static bool ToXml(System.String Path, System.Object Obj)
      {
         bool done = false;
         System.IO.Stream sw;
         System.Xml.Serialization.XmlSerializer bf =
            new System.Xml.Serialization.XmlSerializer(Obj.GetType());

         if (System.IO.File.Exists(Path))
            System.IO.File.Delete(Path);

         sw = System.IO.File.Open(Path, System.IO.FileMode.Create);

         if ((sw != null) && (bf != null))
         {
            bf.Serialize(sw, Obj);
            sw.Close();
            done = true;
         }

         return (done);
      }  // end of ToXml(...)

      /// <summary>Read object members from an Xml file.</summary>
      /// <param name="Path">File path name to read from.</param>
      /// <param name="ObjType">Object type to be deserialized.</param>
      /// <returns>Read object is returned.</returns>
      /// <date>Nov/2k3  ESob</date>
      public static System.Object FromXml(System.String Path, Type ObjType)
      {
         System.Object obj = null;
         System.IO.Stream sw;
         System.Xml.Serialization.XmlSerializer bf =
            new System.Xml.Serialization.XmlSerializer(ObjType);

         if (!System.IO.File.Exists(Path != null ? Path : String.Empty))
            throw new System.IO.FileNotFoundException(
               "FromXml. " + (
               Path != null ? "File (" + Path + ") not found." :
               "No file name Provided!"));

         sw = null;
         if (System.IO.File.Exists(Path))
            sw = System.IO.File.OpenRead(Path);

         if ((sw != null) && (bf != null))
         {
            try
            {
               obj = bf.Deserialize(sw);
            }
            finally
            {
               //sw.Close();
            }
         }

         return (obj);
      }  // end of deserialize object from binary

      #endregion
      #region -- SOAP Serialization

      // TODO: Find the SoapFormatter and uncomment this section

      ///// <summary>Store object members in a soap formatted file.</summary>
      ///// <param name="Path">File path name.</param>
      ///// <param name="Obj">Object to be serialized.</param>
      ///// <returns>true is returned if file was created.</returns>
      ///// <date>Nov/2k3  ESob</date>
      //public static bool ToSoap(System.String Path, System.Object Obj)
      //{
      //   bool done = false;
      //   System.IO.Stream sw;
      //   SoapFormatter bf = new SoapFormatter();

      //   if (System.IO.File.Exists(Path))
      //      System.IO.File.Delete(Path);

      //   sw = System.IO.File.Open(Path, System.IO.FileMode.Create);

      //   if ((sw != null) && (bf != null))
      //   {
      //      bf.Serialize(sw, Obj);
      //      sw.Close();
      //      done = true;
      //   }

      //   return (done);
      //}  // end of serialize object to soap

      ///// <summary>Read object members from a soap formatted file.</summary>
      ///// <param name="Path">File path name to read from.</param>
      ///// <returns>Read object is returned.</returns>
      ///// <date>Nov/2k3  ESob</date>
      //public static System.Object FromSoap(System.String Path)
      //{
      //   System.Object obj = null;
      //   System.IO.Stream sw;
      //   SoapFormatter bf = new SoapFormatter();

      //   if (!System.IO.File.Exists(Path))
      //   {
      //      return (obj);
      //   }

      //   if (System.IO.File.Exists(Path))
      //      sw = System.IO.File.Open(Path, System.IO.FileMode.Open);
      //   else
      //      sw = null;

      //   if ((sw != null) && (bf != null))
      //   {
      //      try
      //      {
      //         obj = bf.Deserialize(sw);
      //      }
      //      finally
      //      {
      //         sw.Close();
      //      }
      //   }

      //   return (obj);
      //}  // end of deserialize object from soap

      #endregion
      #region -- JSON Serialization

      /// <summary>
      /// Given an instance of a type, serialize it as a JSON string.
      /// </summary>
      /// <typeparam name="T">type of object to serialize</typeparam>
      /// <param name="data">object instance to serialize</param>
      /// <returns>a JSON sting is returned. If not possible an
      /// exception is trown</returns>
      public static String ToJsonString<T>(Object data)
      {
         System.IO.MemoryStream stream = null;
         String jsonData = String.Empty;
         try
         {
            System.Runtime.Serialization.
               Json.DataContractJsonSerializer serializer =
                  new System.Runtime.Serialization.
                     Json.DataContractJsonSerializer(typeof(T));
            stream = new System.IO.MemoryStream();
            serializer.WriteObject(stream, data);
            System.Text.UTF8Encoding e = new System.Text.UTF8Encoding();
            jsonData = e.GetString(
               stream.ToArray(), 0, (Int32)stream.Length);
            //stream.Dispose(); // Close();
         }
         catch (Exception ex)
         {
            throw new Exception(
               "Serialize.ToJsonString: fail serializing object", ex);
         }
         finally
         {
            if (stream != null)
               stream.Dispose();
         }
         return jsonData;
      }  // end of ToJsonString

      /// <summary>
      /// Deserialize a given JSON string into it's original object.
      /// </summary>
      /// <typeparam name="T">type to deserialize</typeparam>
      /// <param name="jsonData">JSON string data</param>
      /// <returns>instance of type. If not possible an
      /// exception is trown</returns>
      public static T FromJsonString<T>(String jsonData)
      {
         T data;
         try
         {
            System.Runtime.Serialization.
               Json.DataContractJsonSerializer serializer =
                  new System.Runtime.Serialization.
                     Json.DataContractJsonSerializer(typeof(T));
            System.Text.UTF8Encoding e = new System.Text.UTF8Encoding();
            System.IO.MemoryStream stream = new System.IO.MemoryStream(
               e.GetBytes(jsonData));
            data = (T)serializer.ReadObject(stream);
            //stream.Close();
         }
         catch (Exception ex)
         {
            throw new Exception("Serialize.FromJsonString: " +
               "fail deserializing object", ex);
         }
         return data;
      }  // end of FromJsonString

      #endregion
      #region -- HEX Serialization

      private static Char[] m_HexChars =
         ((String)"0123456789ABCDEF").ToCharArray();

      public static Int32 ParseNybble(Char c)
      {
         if (c >= '0' && c <= '9')
            return c - '0';
         if (c >= 'A' && c <= 'F')
            return c - 'A' + 10;
         if (c >= 'a' && c <= 'f')
            return c - 'A' + 10;
         throw new ArgumentOutOfRangeException("Invalid hex digit: " + c);
      }  // end of ParseNybble

      public static String BytesToHex(Byte[] data)
      {
         System.Text.StringBuilder builder =
            new System.Text.StringBuilder(data.Length * 2);
         foreach (Byte b in data)
         {
            builder.Append(m_HexChars[b >> 4]);
            builder.Append(m_HexChars[b & 0xf]);
         }
         return builder.ToString();
      }  // end of BytesToHex

      public static Byte[] HexToBytes(String text)
      {
         if ((text.Length & 1) != 0)
            throw new ArgumentException("Invalid hex: odd length");
         Byte[] ret = new Byte[(text.Length / 2)];
         for (int i = 0; i < text.Length; i += 2)
         {
            ret[i / 2] = (Byte)(ParseNybble(text[i]) << 4 | ParseNybble(text[i + 1]));
         }
         return ret;
      }  // end of HexToBytes

      public static String TextToHex(String text)
      {
         System.Text.UTF8Encoding e = new System.Text.UTF8Encoding();
         Byte[] data = e.GetBytes(text);
         return BytesToHex(data);
      }  // end of TextToHex

      public static String HexToText(String text)
      {
         System.Text.UTF8Encoding e = new System.Text.UTF8Encoding();
         Byte[] data = HexToBytes(text);
         return e.GetString(data, 0, data.Length);
      }  // end of HexToText

      #endregion

   }  // end of Serialize

}  // end of Edam.Serialization

