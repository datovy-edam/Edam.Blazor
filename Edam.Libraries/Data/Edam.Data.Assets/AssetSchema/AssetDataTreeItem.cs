using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.InOut;
using Edam.Data.Asset;
using Edam.Text;
using Edam.DataObjects.Objects;
using Edam.Data.AssetManagement;

namespace Edam.Data.AssetSchema
{

   public class AssetDataTreeItem
   {

      public NamespaceInfo Namespace { get; set; }
      public AssetDataTreeItem Parent { get; set; }
      public AssetDataElement OriginalElement { get; set; }
      public bool IsVisited { get; set; } = false;
      public bool IsValueBaseType { get; set; }

      private AssetDataElement m_Element = null;
      public AssetDataElement Element
      {
         get { return m_Element; }
         set
         {
            m_Element = value;
            m_ItemType = value == null ? ItemType.Unknow :
                  value.IsType ? ItemType.Folder : ItemType.File;
            m_Title = value == null ? String.Empty : value.ElementName;
            SetIsValueBaseType();
         }
      }

      public List<AssetDataTreeItem> Children = new List<AssetDataTreeItem>();

      private ItemType m_ItemType = ItemType.Unknow;
      public ItemType Type
      {
         get { return m_ItemType; }
         set { m_ItemType = value; }
      }

      public bool IsCollection
      {
         get { return m_Element.MaxOccurrence > 1; }
      }
      public bool IsSimpleType
      {
         get { return Type == ItemType.File; }
      }

      private string m_Title = String.Empty;
      public string Title
      {
         get { return m_Title; }
      }

      public string LinkText { get; set; }

      public string ElementFullPath
      {
         get { return OriginalElement.FullPath; }
      }

      /// <summary>
      /// Get Title by reviewing settings / options.
      /// </summary>
      /// <returns></returns>
      public void SetTitle(string title)
      {
         m_Title = title;
      }

      public bool SetIsValueBaseType()
      {
         IsValueBaseType = ElementBaseTypeInfo.IsBase(m_Element.DataType);
         return IsValueBaseType;
      }

      /// <summary>
      /// Get / Generate sample value.
      /// </summary>
      /// <returns></returns>
      public string GetSampleValue()
      {
         if (IsSimpleType)
         {
            if (String.IsNullOrWhiteSpace(m_Element.SampleValue))
            {
               if (m_Element.ValueTypeNo == 0)
               {
                  m_Element.ValueTypeNo = 
                     (short)ObjectValueTypes.GetValueType(Element.DataType);
               }
               m_Element.SampleValue = TextValue.GenerateValue(
                  (ObjectValueType)m_Element.ValueTypeNo);
            }
            return m_Element.SampleValue;
         }
         return String.Empty;
      }

   }

}
