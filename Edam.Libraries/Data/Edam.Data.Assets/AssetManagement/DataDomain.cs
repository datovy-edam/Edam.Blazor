using Edam.Data.Asset;
using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.Application;

namespace Edam.Data.AssetManagement
{
   public partial class DataDomain
   {
      public DateTime CreatedDate { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public string OrganizationId { get; set; }
      public string DataOwnerId { get; set; }
      public string Prefix { get; set; }
      public string Root { get; set; }
      public string Domain { get; set; }
      public int DomainNo { get; set; }
      public short TypeNo { get; set; }
      public string DomainId { get; set; }
      public string DomainUri { get; set; }
      public string DomainName { get; set; }
      public string Business { get; set; }
      public string BusinessId { get; set; }
      public string Description { get; set; }
      public string UpdateSessionId { get; set; }
      public string RecordStatusCode { get; set; }

      public DomainType Type
      {
         get { return (DomainType)TypeNo; }
         set { TypeNo = (short)value; }
      }

      public string TypeText
      {
         get { return TypeNo.ToString(); }
         set
         {
            if (short.TryParse(value, out var type))
            {
               TypeNo = (short)type;
            }
         }
      }

      public DataDomain()
      {

      }

      public DataDomain(NamespaceInfo ns)
      {
         Prefix = ns.Prefix;
         Root = ns.NamePath.Root;
         Domain = ns.NamePath.Domain;
         DomainUri = ns.Uri.OriginalString;
         DomainName = String.Empty;
         TypeNo = (short) DomainType.Asset;
      }

      public void ClearFields()
      {
         OrganizationId = Session.OrganizationId;
         DataOwnerId = String.Empty;
         Prefix = String.Empty;
         Root = String.Empty;
         Domain = String.Empty;
         DomainNo = 0;
         Type = DomainType.Asset;
         DomainId = String.Empty;
         DomainUri = String.Empty;
         DomainName = String.Empty;
         Description = String.Empty;
         UpdateSessionId = Session.SessionId;
         RecordStatusCode = String.Empty;
      }
   }
}
