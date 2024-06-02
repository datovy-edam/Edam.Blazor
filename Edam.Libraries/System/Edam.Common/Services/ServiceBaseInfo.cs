using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;

// -----------------------------------------------------------------------------
using Edam.Data;
using Edam.DataObjects.Objects;
using Edam.Application;

namespace Edam.Services
{

   public enum ServiceBaseType
   {
      Unknown = 0,
      Publisher = 1,           // client / publisher
      Consumer = 2,            // server / consumer
      Feed = 3,                // feed (pull)
      Tcp = 4
   }
   
   /// <summary>
   /// Base Service Configuration Details.
   /// </summary>
   [XmlType("Service")]
   public class ServiceBaseInfo : IServiceInfo
   {

      public String Key { get; set; }
      public String Alias { get; set; }
      public String ServicePathUri { get; set; }
      public String OrganizationId { get; set; }
      public String AccountId { get; set; }
      public String AccountUserId { get; set; }
      public Edam.Security.Password AccountPassword { get; set; }
      public String AccountDomain { get; set; }
      public bool IsActive { get; set; }
      public String Version { get; set; }
      public Boolean Enable { get; set; }
      public String ServiceLogFileFolderPath { get; set; }
      public String ServiceLogFileName { get; set; }

      public ObjectDomain ServiceDomain { get; set; }
      public String ServiceKeyId { get; set; }
      public String ServiceGuid { get; set; }
      public ServiceBaseType ServiceType { get; set; }
      public String ServiceGroup { get; set; }

      public Int32 Retries { get; set; }
      public Int32 FetchRateInMilliseconds { get; set; }

      public String AssemblyName { get; set; }
      public String TypeName { get; set; }

      /// <summary>
      /// Returns true if we do have a Service-URI, a User Id and a Password.
      /// </summary>
      public Boolean IsValid
      {
         get
         {
            return (!String.IsNullOrEmpty(ServicePathUri) &&
               !String.IsNullOrEmpty(AccountUserId) &&
               !String.IsNullOrEmpty(AccountPassword.ClearText));
         }
      }

      public ServiceBaseInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         ServiceType = ServiceBaseType.Unknown;
         OrganizationId = Session.GetDefaultAgentOrganization();
         Key = Session.GetDefaultSessionKey();
         Alias = Session.DEFAULT_TEXT;
         ServicePathUri = String.Empty;
         AccountId = Session.DEFAULT_TEXT;
         AccountDomain = Session.GetDefaultDomain();
         AccountUserId = String.Empty;
         if (AccountPassword == null)
            AccountPassword = new Security.Password();
         AccountPassword.PasswordText = String.Empty;
         Enable = true;
         ServiceLogFileFolderPath = String.Empty;
         ServiceLogFileName = String.Empty;
         Retries = 3;
         FetchRateInMilliseconds = 300000; // 5 mins...
         AssemblyName = null;
         TypeName = null;
      }

#if DATA_SUPPORT_
      
      public static ServiceBaseInfo ReadData(ServiceBaseInfo item,
         System.Data.Common.DbDataReader reader)
      {
         if (item == null)
            item = new ServiceBaseInfo();

         if (reader == null)
         {
            item.ClearFields();
            return item;
         }

         item.Key = DataField.GetString(reader[1]);
         item.ServiceKeyId = DataField.GetString(reader[2]);
         item.ServiceGuid = DataField.GetString(reader[3]);
         item.Alias = DataField.GetString(reader[4]);
         item.ServiceGroup = DataField.GetString(reader[5]);
         item.ServiceType = (ServiceBaseType) DataField.GetInt16(reader[6]);
         item.AccountId = DataField.GetString(reader[7]);
         item.OrganizationId = DataField.GetString(reader[8]);
         item.ServicePathUri = DataField.GetString(reader[9]);
         item.AccountUserId = DataField.GetString(reader[10]);
         
         item.AccountPassword = new Security.Password(
            DataField.GetString(reader[11]),
            Security.SecretOption.NotHashed);
         item.AccountDomain = DataField.GetString(reader[12]);
         item.IsActive = DataField.GetBool(reader[13]);
         item.Version = DataField.GetString(reader[14]);
         item.Enable = DataField.GetBool(reader[15]);
         item.ServiceLogFileFolderPath = DataField.GetString(reader[16]);
         item.ServiceLogFileName = DataField.GetString(reader[17]);
         item.Retries = DataField.GetInt16(reader[18]);
         item.FetchRateInMilliseconds = DataField.GetInt32(reader[19]);
         item.AssemblyName = DataField.GetString(reader[20]);
         item.TypeName = DataField.GetString(reader[21]);

         return item;
      }

      /// <summary>
      /// Prepare a list with data supplied in given reader.
      /// </summary>
      /// <param name="list">list to add items too</param>
      /// <param name="reader">reader (source of data)</param>
      /// <returns>instance List of MediaBlobInfo'es</returns>
      public static List<ServiceBaseInfo> GetList(
         List<ServiceBaseInfo> list,
         System.Data.Common.DbDataReader reader)
      {
         ServiceBaseInfo record;
         if (list == null)
            list = new List<ServiceBaseInfo>();
         while (reader.Read())
         {
            record = ReadData(null, reader);
            list.Add(record);
         }
         return list;
      }

#endif

   }

}
