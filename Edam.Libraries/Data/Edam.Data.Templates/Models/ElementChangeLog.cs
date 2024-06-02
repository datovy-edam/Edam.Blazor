using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Application;

namespace Edam.DataObjects.Models
{

   public class ElementChangeLog
   {
      public DateTime TimeStamp { get; set; }
      public string LogGuid { get; set; }
      public string OrganizationId { get; set; }
      public string AgentId { get; set; }
      public string ApplicationId { get; set; }

      private string m_ModuleId;
      public string ModuleId
      {
         get { return m_ModuleId; }
         set
         {
            m_ModuleId = string.IsNullOrWhiteSpace(value) ?
               string.Empty : value;
         }
      }

      public List<ElementChangeEntryInfo> Changes { get; set; }

      public bool HasChanges
      {
         get { return Changes.Count > 0; }
      }

      public ElementChangeLog(string? moduleId = null)
      {
         Changes = new List<ElementChangeEntryInfo>();
         OrganizationId = Session.OrganizationId;
         AgentId = Session.AgentId;
         ApplicationId = Session.ApplicationId;
         ModuleId = moduleId ?? null;
         LogGuid = Guid.NewGuid().ToString();
         TimeStamp = DateTime.UtcNow;
      }

      public void Add(ElementChangeEntryInfo entry)
      {
         Changes.Add(entry);
      }
   }

}
