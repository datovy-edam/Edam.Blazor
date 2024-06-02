using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects.DataCodes
{

   /// <summary>
   /// Helper to manage data-pages dispatch as requested 
   /// until the end of the set.
   /// </summary>
   public class DataPage
   {
      public static readonly Int32 DEFAULT_PAGE_SIZE = 256;

      public Int32 PageSize { get; set; }
      public Int32 CurrentPageNumber { get; set; }
      public Int32 TotalRecords { get; set; }
      public Boolean HasChanged { get; set; }

      public DataPage()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         PageSize = DEFAULT_PAGE_SIZE;
         CurrentPageNumber = 0;
         TotalRecords = 0;
         HasChanged = false;
      }

   }

}
