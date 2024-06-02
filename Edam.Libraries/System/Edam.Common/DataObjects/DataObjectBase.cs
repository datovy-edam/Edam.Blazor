using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;
//using Edam.DataObjects.Medias;
//using Edam.DataObjects.Notes;

namespace Edam.DataObjects
{

   /// <summary>
   /// Base class for most the Data Object classes
   /// </summary>
   public class DataObjectBase : IDataObjectBase
   {

      public String RecordGuid { get; set; }
      public String ObjectId { get; set; }
      public Labels.LabelInfo Label { get; set; }
      //public List<MediaReferenceInfo> ReferenceMedias { get; set; }

      public Int32 MediaCount 
      {
         get 
         {
            //return ReferenceMedias != null ? 
            //   ReferenceMedias.Count + GetMediasCount() : 0; 
            return 0;
         }
      }

      public DataObjectBase()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         RecordGuid = Guid.NewGuid().ToString();
         ObjectId = String.Empty;
         Label = new Labels.LabelInfo();
         //ReferenceMedias = new List<MediaReferenceInfo>();
      }

      protected virtual Int32 GetMediasCount()
      {
         return 0;
      }

   }

}
