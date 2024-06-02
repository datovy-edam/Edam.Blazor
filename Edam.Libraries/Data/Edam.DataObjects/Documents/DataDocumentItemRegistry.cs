using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

// -----------------------------------------------------------------------------
//using Edam.Helpers;
using Edam.Data;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Medias;
using Edam.Application;
using Edam.Diagnostics;
using Edam.Serialization;

namespace Edam.DataObjects.Documents
{

   public class DataDocumentItemRegistry : DataDocumentItemInfo,
      IDataDocumentItem
   {

      #region -- 1.0 - Properties and definitions...

      [PrimaryKey, AutoIncrement]
      public int IdNo
      {
         get { return m_DocumentNo ?? 0; }
         set { m_DocumentNo = value; }
      }

      public DateTime? CreatedDate
      {
         get { return m_CreatedDate; }
         set { m_CreatedDate = value; }
      }
      public DateTime? LastUpdateDate
      {
         get { return m_LastUpdateDate; }
         set { m_LastUpdateDate = value; }
      }

      public string DocumentId
      {
         get { return m_DocumentId; }
         set
         {
            if (m_DocumentId != value)
            {
               m_DocumentId = value;
               //OnPropertyChanged("DocumentId");
            }
         }
      }

      [Indexed]
      public string Name
      {
         get { return m_Name; }
         set
         {
            if (m_Name != value)
            {
               m_Name = value;
               //OnPropertyChanged("Name");
            }
         }
      }

      public string Description
      {
         get { return m_Description; }
         set
         {
            if (m_Description != value)
            {
               m_Description = value;
               //OnPropertyChanged("Description");
            }
         }
      }

      public string Version
      {
         get { return m_Version; }
         set
         {
            if (m_Version != value)
            {
               m_Version = value;
               //OnPropertyChanged("Version");
            }
         }
      }

      public string DataOwnerId
      {
         get { return m_DataOwnerId; }
         set
         {
            if (m_DataOwnerId != value)
            {
               m_DataOwnerId = value;
               //OnPropertyChanged("DataOwnerId");
            }
         }
      }

      public long SizeInBytes
      {
         get { return m_SizeInBytes; }
         set
         {
            if (m_SizeInBytes != value)
            {
               m_SizeInBytes = value;
               //OnPropertyChanged("SizeInBytes");
            }
         }
      }

      public MediaContentType ContentType
      {
         get { return m_ContentType; }
         set
         {
            if (m_ContentType != value)
            {
               m_ContentType = value;
               //OnPropertyChanged("ContentType");
            }
         }
      }

      public byte[] BinaryData
      {
         get { return m_BinaryData; }
         set { m_BinaryData = value; }
      }

      public string SessionId
      {
         get { return m_SessionId; }
         set { m_SessionId = value; }
      }

      public string SourceId
      {
         get { return m_SourceId; }
         set { m_SourceId = value; }
      }

      public string TargetId
      {
         get { return m_TargetId; }
         set { m_TargetId = value; }
      }

      public ObjectStatus Status
      {
         get { return m_Status; }
         set { m_Status = value; }
      }

      [Ignore]
      public ResultLog LastResultLog { get; set; }
      [Ignore]
      public List<IDataDocumentItem> Items { get; set; }

      #endregion
      #region -- 1.0 - Commands

      //public ICommand LoginCommand { protected set; get; }
      //public ICommand LogoutCommand { protected set; get; }
      //public ICommand SaveRecordCommand { protected set; get; }

      #endregion
      #region -- 1.5 - Initialize Resources

      public static T InitializeInstance<T>(
         string name, string description,
         string dataOwnerId, string version, long? sizeInBytes = null,
         MediaContentType? contentType = MediaContentType.Unknown,
         string sourceId = null, string targetId = null,
         ObjectStatus status = ObjectStatus.Unknown,
         byte[] binaryData = null, string sessionId = null) 
            where T : IDataDocumentItem
      {
         T document = (T)AppAssembly.CreateInstance(typeof(T));
         document.CreatedDate = DateTime.UtcNow;
         document.LastUpdateDate = DateTime.UtcNow;
         document.IdNo = -1;
         document.DocumentId = Guid.NewGuid().ToString();
         document.Name = name;
         document.Description = description;
         document.Version = version;
         document.DataOwnerId = String.IsNullOrWhiteSpace(dataOwnerId) ?
            Session.OrganizationId : dataOwnerId;
         document.BinaryData = binaryData;
         document.SizeInBytes = binaryData == null ? 0 : binaryData.Length;
         document.ContentType = contentType.HasValue ?
            contentType.Value : MediaContentType.Unknown;
         document.SessionId = String.IsNullOrWhiteSpace(sessionId) ?
            Session.SessionId : sessionId;
         document.SourceId = sourceId;
         document.TargetId = targetId;
         document.Status = status;
         return document;
      }

      public DataDocumentItemRegistry()
      {
         IdNo = -1;
         CreatedDate = DateTime.Now;
      }

      protected void SetTableName(string tableName)
      {
         m_TableName = tableName;
      }

      #endregion
      #region -- 4.0 - Define Services...

      /// <summary>
      /// Save Record locally.
      /// </summary>
      public async Task<int> SaveRecord<T>(T item)
         where T : IDataDocumentItem, new()
      {
         LDbConnection c = new LDbConnection();

         item.LastUpdateDate = DateTime.UtcNow;
         Items = null;

         ValidateRecord();

         if (IdNo < 0)
         {
            if (IdNo < 0)
            {
               IdNo = 0;
               item.IdNo = IdNo;
            }
            return await c.InsertAsync<T>(item);
         }
         else
         {
            return await c.UpdateAsync<T>(item);
         }
      }

      /// <summary>
      /// Read Record.
      /// </summary>
      public Task ReadRecord<T>(int idNo) 
         where T : IDataDocumentItem, new()
      {
         Task task = null;
         Boolean isOk = false;
         String errMess;
         LastResultLog = new ResultLog();
         Items = new List<IDataDocumentItem>();
         LDbConnection c = new LDbConnection();
         try
         {
            task = c.GetIntIdObjectAsync<T>(idNo).
               ContinueWith((t) =>
               {
                  var l = t.Result;
                  if (l != null)
                  {
                     IdNo = l.IdNo;
                     DocumentId = l.DocumentId;
                     Name = l.Name;
                     Description = l.Description;
                     ContentType = l.ContentType;
                     BinaryData = l.BinaryData;
                     Items.Add(l);
                  }
                  LastResultLog.Succeeded();
               });
            isOk = true;
         }
         catch (Exception ex)
         {
            LastResultLog.Failed(ex);
            errMess = ex.Message;
            isOk = false;
         }
         finally
         {
            if (c != null)
               c.Dispose();
         }
         c = null;

         if (!isOk)
         {
            IdNo = -1;
         }
         return task;
      }

      /// <summary>
      /// Read Record.
      /// </summary>
      public async Task<List<T>> FindRecordByName<T>(string name) 
         where T : IDataDocumentItem, new()
      {
         LastResultLog = new ResultLog();
         Items = new List<IDataDocumentItem>();
         LDbConnection c = new LDbConnection();
         return await c.GetAsync<T>(m_TableName, "Name", name);
      }

      /*
      /// <summary>
      /// Read Record.
      /// </summary>
      public Task FindRecordByNameOrderByVersion<T>(string name)
         where T : IDataDocumentItem, new()
      {
         Task task = null;
         Boolean isOk = false;
         String errMess;
         LastResultLog = new ResultLog();
         Items = new List<IDataDocumentItem>();
         LDbConnection c = new LDbConnection();
         try
         {
            task = c.GetAsync<T>(
               m_TableName, "WHERE Name = ? ORDER BY Version",
                  new string[1] { name }).ContinueWith((t) =>
                  {
                     if (t.Status == TaskStatus.Faulted)
                     {
                        LastResultLog.Failed(t.Exception);
                     }
                     else
                     {
                        var result = (List<T>)t.Result;
                        SetRecord(t.Result);
                        Items.AddRange(t.Result);
                        LastResultLog.Succeeded();
                     }
                  });
            isOk = true;
         }
         catch (Exception ex)
         {
            LastResultLog.Failed(ex);
            errMess = ex.Message;
            isOk = ex.HResult != Devices.DeviceUserViewModel.
               HRESULT_TABLE_NOT_FOUND;
         }
         finally
         {
            if (c != null)
               c.Dispose();
         }
         c = null;

         if (!isOk)
         {
            InitializeInstance();
            IdNo = -1;
         }
         return task;
      }
       */

      /// <summary>
      /// Read Record.
      /// </summary>
      public async Task<int> DeleteRecord(int idNo)
      {
         Items = null;
         LDbConnection c = new LDbConnection();
         return await c.DeleteAsync<DataDocumentItem>(idNo);
      }

      #endregion
      #region -- 4.0 - Support Methods

      public void ValidateRecord()
      {
         SizeInBytes = BinaryData == null ? 0 : BinaryData.Length;
      }

      public void SetRecord(List<IDataDocumentItem> list)
      {
         if (list != null && list.Count > 0)
         {
            SetRecord(list[0]);
         }
      }

      public void SetRecord(IDataDocumentItem item)
      {
         if (item != null)
         {
            IdNo = item.IdNo;
            DocumentId = item.DocumentId;
            Name = item.Name;
            Description = item.Description;
            ContentType = item.ContentType;
            BinaryData = item.BinaryData;
            Version = item.Version;
            DataOwnerId = item.DataOwnerId;
            SourceId = item.SourceId;
            TargetId = item.TargetId;
            SizeInBytes = (BinaryData == null ? 0 : BinaryData.Length);
            CreatedDate = item.CreatedDate;
            LastUpdateDate = item.LastUpdateDate;
            SessionId = item.SessionId;
            Status = item.Status;
         }
      }

      public void InsertUpdateDone(bool done)
      {

      }

      #endregion
      #region -- 4.0 - Binary, Text and Json Serialization Support

      public static string ToText(byte[] binaryData)
      {
         if (binaryData == null || binaryData.Length == 0)
         {
            return String.Empty;
         }
         return Encoding.ASCII.GetString(binaryData);
      }

      public static byte[] ToBinary(string textData)
      {
         return UTF8Encoding.ASCII.GetBytes(textData);
      }

      public static T FromJson<T>(string jsonText)
      {
         return JsonSerializer.Deserialize<T>(jsonText);
      }

      public static T FromJson<T>(byte[] binaryText)
      {
         var jsonText = ToText(binaryText);
         return JsonSerializer.Deserialize<T>(jsonText);
      }

      public static string ToJson<T>(T item)
      {
         return JsonSerializer.Serialize<T>(item);
      }

      #endregion

   }

}

