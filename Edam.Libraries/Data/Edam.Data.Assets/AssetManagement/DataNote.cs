using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
    public partial class DataNote
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string OrganizationId { get; set; }
        public string AgentId { get; set; }
        public string ReferenceId { get; set; }
        public int NoteNo { get; set; }
        public string NoteText { get; set; }
        public int NoteTypeNo { get; set; }
        public string UpdateSessionId { get; set; }
        public string RecordStatusCode { get; set; }

        public virtual DataNoteType NoteTypeNoNavigation { get; set; }
        public virtual DataReferenceObject Reference { get; set; }
    }
}
