using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.DataAnnotations;

// -----------------------------------------------------------------------------
// Copied from Kif Library v5r0

//using DbField = Edam.Data.DataField;
using Edam.DataObjects.References;

namespace Edam.DataObjects.Entities
{

   /// <summary>
   /// Location Address Reference to other objects.
   /// </summary>
   public class EntityReferenceInfo : EntityInfo
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

      public References.ReferenceType ReferenceType
      {
         get { return m_Association.ReferenceType; }
         set { m_Association.ReferenceType = value; }
      }

      public ReferenceBaseType AssociationType
      {
         get { return m_Association.AssociationType; }
         set { m_Association.AssociationType = value; }
      }

      public ReferenceObjectAssociationInfo<ReferenceBaseType> Association
      {
         get { return m_Association; }
      }

      public EntityReferenceInfo()
      {
         m_Association = new ReferenceObjectAssociationInfo<ReferenceBaseType>();
         m_Association.AssociationType = ReferenceBaseType.Entity;
      }

      public static void FixNullValues(EntityReferenceInfo record)
      {
         EntityInfo.FixNullValues((EntityInfo)record);
         record.ReferenceId = Edam.Convert.ToNotNullString(record.ReferenceId);
      }

   }

}
