using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.DataObjects.Medias;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;
using Edam.Application;

namespace Edam.DataObjects.Documents
{

   public class DataDocumentItemHelper
   {
      public const string CLASS_NAME = "DataDocumentItemHelper";
      public const string REFERENCE_DATA_TEMPLATE_LISTS = 
         "Reference.ReferenceDataTemplate.List";

      /// <summary>
      /// Get/Find a document by name.
      /// </summary>
      /// <param name="name">name of document</param>
      /// <returns>Results Log is returned.  See .Data member to get the
      /// result set (if any), or see the Success and/or Messages</returns>
      public static async Task<List<T>>
         GetDocumentByName<T>(string name) where T : IDataDocumentItem, new()
      {
         T m = (T)AppAssembly.CreateInstance(typeof(T));
         return await (m as IDataDocumentItem).FindRecordByName<T>(name);
      }

      /// <summary>
      /// Insert a document.
      /// </summary>
      /// <param name="documentName"></param>
      /// <param name="binaryData"></param>
      /// <param name="contentType"></param>
      /// <param name="description"></param>
      /// <param name="version"></param>
      /// <param name="dataOwnerId"></param>
      /// <returns>Results Log is returned.  See the Success and/or Messages
      /// </returns>
      public static async Task<int> Insert<T>(
         string documentName, byte[] binaryData, 
         MediaContentType contentType, string description = null,
         string version = null, string dataOwnerId = null) 
            where T : IDataDocumentItem, new()
      {
         T document = 
            DataDocumentItemRegistry.InitializeInstance<T>(
               name: documentName,
               contentType: contentType, binaryData: binaryData,
               dataOwnerId: dataOwnerId,
               version: (String.IsNullOrWhiteSpace(version) ? "v*r*" : version),
               description: (String.IsNullOrWhiteSpace(description) ?
                  Convert.ToTitleCase(documentName) : description));
         document.IdNo = -1;
         return await document.SaveRecord<T>(document);
      }

      /// <summary>
      /// Insert (if found) or Update the record if new...
      /// </summary>
      /// <param name="documentName"></param>
      /// <param name="binaryData"></param>
      /// <param name="contentType"></param>
      /// <param name="description"></param>
      /// <param name="version"></param>
      /// <param name="dataOwnerId"></param>
      /// <returns>Results Log is returned.  See the Success and/or Messages
      /// </returns>
      public static async Task<int> InsertUpdate<T>(
         string documentName, byte[] binaryData,
         MediaContentType contentType, string description = null,
         string version = null, string dataOwnerId = null) 
            where T : IDataDocumentItem, new()
      {
         // first try to find record...
         var results = await GetDocumentByName<T>(documentName);
         if (results.Count == 0)
         {
            return await Insert<T>(documentName, binaryData, contentType, 
               description, version, dataOwnerId);
         }

         // record exists... update it...
         var m = results[0];
         m.BinaryData = binaryData;
         m.ContentType = contentType == MediaContentType.Unknown ? 
            m.ContentType : contentType;
         m.Description = String.IsNullOrWhiteSpace(description) ?
            m.Description : description;
         m.Version = String.IsNullOrWhiteSpace(version) ?
            m.Version : version;

         return await m.SaveRecord<T>(m);
      }

      public static async Task<int> DeleteDocument(int documentNo)
      {
         DataDocumentItem m = new DataDocumentItem();
         return await m.DeleteRecord(documentNo);
      }
   }

}
