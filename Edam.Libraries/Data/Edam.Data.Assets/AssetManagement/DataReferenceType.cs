using System;
using System.Collections.Generic;

namespace Edam.Data.AssetManagement
{
    public partial class DataReferenceType
    {
        public DataReferenceType()
        {
            DataReferenceObject = new HashSet<DataReferenceObject>();
        }

        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int IdNo { get; set; }
        public string Description { get; set; }
        public string SpanishDescription { get; set; }
        public string UpdateSessionId { get; set; }
        public string RecordStatusCode { get; set; }

        public virtual ICollection<DataReferenceObject> DataReferenceObject { get; set; }
    }
}
