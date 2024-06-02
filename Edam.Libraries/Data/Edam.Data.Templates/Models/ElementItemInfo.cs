using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using Edam.DataObjects.ReferenceData;
using Edam.DataObjects.References;
using Edam.DataObjects.Objects;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.Models
{

   public enum KeyType 
   {
      Unknown = 0,
      Key = 1,
      Exists = 2,
      NonKey = 9
   }

   /// <summary>
   /// This is a child of ElementInfo(root)/ElementNodeInfo and contains a 
   /// reference object that is represented by a (universal unique) Name and a
   /// Value.  Every reference object may carry (bag of) many properties that 
   /// may be of interest to others (or not).  A value may be a simple text
   /// describing the object or as complex as needed that expose its bag of
   /// properties.  There is no need to expose all since it will make consumers
   /// of the object too knowledgable of intimate details, let those be known 
   /// to all of those that really don't care about them and may just like to 
   /// carry them everywhere... it's just a bag of things.
   /// </summary>
   public class ElementItemInfo : ReferenceItemValueInfo, IElementInfo
   {

      private String m_Title;

      public String Title
      {
         get { return (String.IsNullOrWhiteSpace(m_Title)) ? Name : m_Title; }
         set
         {
            m_Title = (String.IsNullOrWhiteSpace(value)) ? Name : value;
         }
      }

      public ResourceType Type { get; set; }
      public KeyType KeyType { get; set; }

      public Int16 MinLength { get; set; }
      public Int16 MaxLength { get; set; }

      public ObjectEditable Editable { get; set; }
      public ObjectSelectable Selectable { get; set; }
      public ObjectVisibility Visibility { get; set; }
      public ObjectGridable Gridable { get; set; }
      public ObjectDiscernible Discernible { get; set; }
      public ObjectGroupable Groupable { get; set; }

      public string Locale { get; set; }

      public DateTime? LastUpdateDate { get; set; }

      /// <summary>
      /// Define lookup resource...
      /// </summary>
      public ReferenceDataCodeInfo ValueCode { get; set; }

      /// <summary>
      /// optional set of domain code values...
      /// </summary>
      public List<DataCodeInfo> LinkCodes { get; set; }

      public ElementItemInfo()
      {
         ClearAll();
      }

      public ElementItemInfo(string name, ObjectValueType type, 
         ResourceType resourceType, KeyType keyType, 
         short maxLength, short minLength = 0)
      {
         ClearAll();
         Name = name;
         Title = Text.Convert.ToProperCase(name);
         ValueType = type;
         Type = resourceType;
         KeyType = keyType;
         MaxLength = maxLength;
         MinLength = minLength;
         MinLength = 0;
      }

      public ElementItemInfo(string name, ObjectValueType type,
         ResourceType resourceType, KeyType keyType,
         int? maxLength, short minLength = 0)
      {
         ClearAll();
         Name = name;
         Title = Text.Convert.ToProperCase(name);
         ValueType = type;
         Type = resourceType;
         KeyType = keyType;

         var mxlen = maxLength.HasValue ? maxLength.Value : 0;
         MaxLength =
            maxLength > short.MaxValue ? short.MaxValue : (short)mxlen;

         MinLength = minLength;
         MinLength = 0;
      }

      public void ClearAll()
      {
         ClearFields();
         LinkCodes = null;
      }

      public new void ClearFields()
      {
         base.ClearFields();
         MinLength = 0;
         MaxLength = 1024;
         Type = ResourceType.Column;
         KeyType = KeyType.NonKey;

         Editable = ObjectEditable.CanEdit;
         Selectable = ObjectSelectable.CanSelect;
         Visibility = ObjectVisibility.Visible;
         Gridable = ObjectGridable.Show;

         if (ValueCode == null)
            ValueCode = new ReferenceDataCodeInfo();
         else
            ValueCode.ClearFields();

         LastUpdateDate = null;
      }

      public Boolean Validate()
      {
         Boolean isvalid = true;
         return isvalid;
      }

   }

}
