using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

using Edam.DataObjects.References;

namespace Edam.DataObjects.Activities
{

   public class ActivityContentReferenceInfo : ActivityContentInfo
   {

      private ReferenceObjectAssociationInfo<ReferenceBaseType>
         m_Association;

      public String ReferenceId
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

      public ReferenceObjectAssociationInfo<ReferenceBaseType> Association
      {
         get { return m_Association; }
      }

      public ActivityContentReferenceInfo()
      {
         m_Association = new ReferenceObjectAssociationInfo<ReferenceBaseType>();
         m_Association.AssociationType = ReferenceBaseType.ActivityThread;
      }

      public static void FixNullValues(ActivityContentReferenceInfo record)
      {
         ActivityContentInfo.FixNullValues((ActivityContentInfo)record);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
      }

   }

}
