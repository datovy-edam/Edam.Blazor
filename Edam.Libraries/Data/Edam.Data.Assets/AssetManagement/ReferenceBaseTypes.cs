using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
    public partial class ReferenceBaseTypes
    {
        public ReferenceBaseTypes()
        {
            ReferenceObjects = new HashSet<ReferenceObjects>();
        }

        public short IdNo { get; set; }
        public string Description { get; set; }
        public string SpanishDescription { get; set; }
        public bool ForceComposedName { get; set; }
        public bool ApplyToIndividual { get; set; }
        //public string UpdateSessionId { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime LastUpdateDate { get; set; }
        //public string RecordStatusCode { get; set; }

        public virtual ICollection<ReferenceObjects> ReferenceObjects { get; set; }
    }
}
