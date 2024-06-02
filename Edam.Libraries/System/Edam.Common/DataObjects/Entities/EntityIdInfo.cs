using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Objects;

namespace Edam.DataObjects.Entities
{

   public class EntityIdInfo
   {

      public String ReferenceId { get; set; }
      public EntityIdType IdType { get; set; }
      public String Id { get; set; }
      public String IdIssuer { get; set; }

      private DateTime? m_IdIssuedDate = null;
      public DateTime? IdIssuedDate
      {
         get { return m_IdIssuedDate; }
         set
         {
            if (value.HasValue)
            {
               if (value.Value.Year == 1800)
                  value = null;
            }
            m_IdIssuedDate = value;
         }
      }

      private DateTime? m_IdExpirationDate = null;
      public DateTime? IdExpirationDate
      {
         get { return m_IdExpirationDate; }
         set
         {
            if (value.HasValue)
            {
               if (value.Value.Year == 1800)
                  value = null;
            }
            m_IdExpirationDate = value;
         }
      }

      public ObjectStatus IdStatus { get; set; }
      public String IdCategoryCode { get; set; }

      public EntityIdInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         ReferenceId = String.Empty;
         IdType = EntityIdType.Unknown;
         Id = String.Empty;
         IdIssuer = String.Empty;
         IdIssuedDate = null;
         IdExpirationDate = null;
         IdStatus = ObjectStatus.Unknown;
         IdCategoryCode = String.Empty;
      }

      public void Copy(EntityIdInfo id, EntityIdType type)
      {
         ReferenceId = id.ReferenceId;
         IdType = type;
         Id = id.Id;
         IdIssuer = id.IdIssuer;
         IdIssuedDate = id.IdIssuedDate;
         IdExpirationDate = id.IdExpirationDate;
         IdStatus = id.IdStatus;
         IdCategoryCode = id.IdCategoryCode;
      }

   }

}
