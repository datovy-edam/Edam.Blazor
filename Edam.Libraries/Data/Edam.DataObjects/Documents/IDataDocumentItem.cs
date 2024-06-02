using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Medias;
using Edam.Application;
using Edam.Diagnostics;
using Edam.Serialization;

namespace Edam.DataObjects.Documents
{

   public interface IDataDocumentItem : ILDbIntIdObject, ILDbObject
   {
      DateTime? CreatedDate { get; set; }
      DateTime? LastUpdateDate { get; set; }
      string DocumentId { get; set; }
      string Name { get; set; }
      string Description { get; set; }
      string Version { get; set; }
      string DataOwnerId { get; set; }
      long SizeInBytes { get; set; }
      MediaContentType ContentType { get; set; }
      byte[] BinaryData { get; set; }
      string SessionId { get; set; }
      string SourceId { get; set; }
      string TargetId { get; set; }

      ObjectStatus Status { get; set; }

      ResultLog LastResultLog { get; set; }
      List<IDataDocumentItem> Items { get; set; }
      Task<int> SaveRecord<T>(T item) where T: IDataDocumentItem, new();
      void SetRecord(List<IDataDocumentItem> items);
      Task<List<T>> FindRecordByName<T>(string name) where T : IDataDocumentItem, new();
   }

}
