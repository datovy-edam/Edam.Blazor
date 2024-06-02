using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
// Copied from Kif Library v5r0

namespace Edam.DataObjects.References
{

   public class ReferenceObjectInfo: IDataObjectBase
   {
      public String RecordGuid { get; set; }
      public Labels.LabelInfo Label { get; set; }

      public ReferenceInfo Reference { get; set; }

      public References.ReferenceType ReferenceType 
      {
         get { return Reference.ReferenceType; }
         set { Reference.ReferenceType = value; }
      }
      
      public String ObjectId
      {
         get
         {
            return Reference.ReferenceId;
         }
         set
         {
            Reference.ReferenceId = value;
         }
      }
      public String ReferenceId 
      {
         get 
         {
            return Reference.ReferenceId;
         }
         set 
         {
            Reference.ReferenceId = value; 
         }
      }

      public String ReferenceDescription
      {
         get
         {
            return Reference.ReferenceDescription;
         }
         set
         {
            Reference.ReferenceDescription = value;
         }
      }

      //pointer to an instance of IDataObjectBase
      public IDataObjectBase ReferencedDataObject { get; set; }

      public ReferenceObjectInfo()
      {
         Reference = new ReferenceInfo();
         Label = new Labels.LabelInfo();
         ClearFields();
      }

      public void ClearFields()
      {
         RecordGuid = String.Empty;
         Label.ClearFields();
         Reference.ClearFields();
      }

   }

}
