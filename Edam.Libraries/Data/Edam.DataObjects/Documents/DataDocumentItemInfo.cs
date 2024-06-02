using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
//using Edam.Helpers;
using Edam.DataObjects.Medias;
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Documents
{
   public class DataDocumentItemInfo //: ObservableObject
   {

      #region -- 1.0 - DB Properties and definitions...

      protected string m_TableName;

      public string TableName
      {
         get { return m_TableName; }
      }

      #endregion

      protected int? m_DocumentNo;

      protected DateTime? m_CreatedDate { get; set; }
      protected DateTime? m_LastUpdateDate { get; set; }

      protected string m_DocumentId { get; set; }
      protected string m_Name { get; set; }
      protected string m_Description { get; set; }
      protected string m_Version { get; set; }
      protected long m_SizeInBytes { get; set; }
      protected string m_DataOwnerId { get; set; }
      protected string m_SessionId { get; set; }
      protected string m_SourceId { get; set; }
      protected string m_TargetId { get; set; }

      protected ObjectStatus m_Status { get; set; }
      protected MediaContentType m_ContentType { get; set; }

      protected byte[] m_BinaryData { get; set; }

      
   }
}
