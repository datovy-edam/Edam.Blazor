using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.DataAnnotations;

// -----------------------------------------------------------------------------

//using DbField = Edam.Data.DataField;
using Edam.DataObjects.References;

namespace Edam.DataObjects.Locations
{

   /// <summary>
   /// Location Address Reference to other objects.
   /// </summary>
   public class LocationAddressReferenceInfo : LocationAddressInfo
   {

      private ReferenceObjectAssociationInfo<LocationType>
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

      public LocationType AssociationType
      {
         get { return m_Association.AssociationType; }
         set { m_Association.AssociationType = value; }
      }

      public ReferenceObjectAssociationInfo<LocationType> Association
      {
         get { return m_Association; }
      }

      public new String LocationTypeText
      {
         get { return GetLocationTypeText(); }
         set
         {
            LocationType t = LocationType.Unknown;
            var v = value.Replace(" ", "");
            if (Enum.TryParse<LocationType>(v, out t))
               Type = t;
            else
               Type = LocationType.Unknown;
         }
      }

      public LocationAddressReferenceInfo()
      {
         m_Association = new ReferenceObjectAssociationInfo<LocationType>();
         m_Association.AssociationType = LocationType.Unknown;
      }

      /// <summary>
      /// Validate, Set and return the Location Type Text.
      /// </summary>
      /// <returns>type text as a string is returned</returns>
      public String GetLocationTypeText()
      {
         LocationAddressReferenceInfo l = this;
         LocationHelpers.GetTypeText(Type, l);
         LocationTypeText = l.LocationTypeText;
         Category = l.Category;
         Type = l.Type;
         AssociationType = l.AssociationType;
         return l.LocationTypeText;
      }

   }

}
