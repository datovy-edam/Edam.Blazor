using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.DataAnnotations;

// -----------------------------------------------------------------------------

//using DbField = Edam.Data.DataField;
using Edam.DataObjects.References;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Location Address Reference to other objects.
   /// </summary>
   public class EntityGroupMemberReferenceInfo : EntityGroupMemberInfo
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

      public EntityGroupMemberReferenceInfo()
      {
         m_Association = new ReferenceObjectAssociationInfo<ReferenceBaseType>();
         m_Association.AssociationType = ReferenceBaseType.GroupMember;
      }

      public static void FixNullValues(EntityGroupMemberReferenceInfo record)
      {
         EntityGroupMemberInfo.FixNullValues((EntityGroupMemberInfo)record);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
      }

   }

}
