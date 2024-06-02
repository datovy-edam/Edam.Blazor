using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

using Edam.DataObjects.References;

namespace Edam.DataObjects.Documents
{

   public class DocumentReferenceInfo : DocumentInfo
   {

      private ReferenceObjectAssociationInfo<DocumentAssociationType>
         m_Association;

      public new String ReferenceId
      {
         get
         {
            return m_Association.Reference.ReferenceId;
         }
         set
         {
            m_Association.Reference.ReferenceId = value;
         }
      }

      public String ReferenceDescription
      {
         get
         {
            return m_Association.Reference.ReferenceDescription;
         }
         set
         {
            m_Association.Reference.ReferenceDescription = value;
         }
      }

      public new References.ReferenceType ReferenceType
      {
         get { return base.ReferenceType; }
         set
         { 
            m_Association.ReferenceType = value;
            base.ReferenceType = value;
         }
      }

      public new DocumentAssociationType AssociationType
      {
         get { return base.AssociationType; }
         set
         { 
            m_Association.AssociationType = value;
            base.AssociationType = value;
         }
      }

      public ReferenceObjectAssociationInfo<DocumentAssociationType> Association
      {
         get
         {
            m_Association.ReferenceType = base.ReferenceType;
            m_Association.AssociationType = base.AssociationType;
            return m_Association; 
         }
      }

      public DocumentReferenceInfo()
      {
         m_Association =
            new ReferenceObjectAssociationInfo<DocumentAssociationType>();
         m_Association.AssociationType = DocumentAssociationType.Owned;
      }

      public static void FixNullValues(DocumentReferenceInfo record)
      {
         DocumentInfo.FixNullValues((DocumentInfo)record);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
      }

   }

}
