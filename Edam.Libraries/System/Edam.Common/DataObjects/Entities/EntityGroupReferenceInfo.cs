using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

//using DbField = Edam.Data.DataField;
using Edam.DataObjects.References;

namespace Edam.DataObjects.Entities
{

   public class EntityGroupReferenceInfo : EntityGroupInfo
   {

      private ReferenceObjectAssociationInfo<ReferenceBaseType>
         m_Association = new ReferenceObjectAssociationInfo<ReferenceBaseType>();

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

      public EntityGroupReferenceInfo()
      {
         m_Association = new ReferenceObjectAssociationInfo<ReferenceBaseType>();
         m_Association.AssociationType = ReferenceBaseType.Group;
      }

      public EntityGroupReferenceInfo(EntityGroupInfo group)
      {
         if (group == null)
            return;
         ReferenceId = group.EntityId;
         ReferenceDescription = group.GroupName;
         m_Association.ReferenceType = ReferenceType.Group;
         Copy(group);
      }

      public static void FixNullValues(EntityGroupReferenceInfo record)
      {
         EntityGroupInfo.FixNullValues((EntityGroupInfo)record);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
      }

   }

}
