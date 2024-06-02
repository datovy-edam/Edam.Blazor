using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Models;
using Edam.Diagnostics;

namespace Edam.DataObjects.ReferenceData
{

   public class ReferenceDataEditTemplateCodesInfo
   {

      public List<IDataCode> Groups { get; set; }
      public List<ReferenceDataEditTemplateInfo> Templates { get; set; }
      public List<MapInfo> Maps { get; set; }

      public ReferenceDataEditTemplateCodesInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         Groups = null;
         Templates = null;
         Maps = null;
      }

   }

}
