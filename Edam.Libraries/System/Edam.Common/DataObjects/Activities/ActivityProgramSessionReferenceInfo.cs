using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

using Edam.DataObjects.References;

namespace Edam.DataObjects.Activities
{

   public class ActivitySessionReferenceInfo :
      ActivitySessionInfo
   {

      private ReferenceObjectAssociationInfo<ReferenceBaseType> m_Association =
         new ReferenceObjectAssociationInfo<ReferenceBaseType>();

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

      private void InitializeObject()
      {
         m_Association = new ReferenceObjectAssociationInfo<ReferenceBaseType>();
         m_Association.AssociationType =
            ReferenceBaseType.ActivityProgramThreadSession;
      }

      public ActivitySessionReferenceInfo()
      {
         InitializeObject();
      }

      public ActivitySessionReferenceInfo(ActivitySessionInfo session)
      {
         Copy(session);
         ReferenceId = session.GroupId;
         InitializeObject();
      }

      public static void FixNullValues(
         ActivitySessionReferenceInfo record)
      {
         ActivitySessionInfo.FixNullValues(
            (ActivitySessionInfo)record);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
      }

   }

}
