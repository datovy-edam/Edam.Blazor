using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.DataItem
{

   public class ItemInstanceItemInfo
   {
      public string BusinessAreaName { get; set; }
      public string Name { get; set; }
      public string Prefix { get; set; }
      public string OriginalName { get; set; }
      public string Description { get; set; }
      public string Path { get; set; }
      public string Value { get; set; }

      public bool MaxOccursUnbounded = false;

      public ItemInstanceType Type { get; set; }
      public DataElementKind Kind { get; set; } = DataElementKind.Property;

      private string m_DataType;
      public string DataType
      {
         get { return m_DataType ?? Type.ToString(); }
         set { m_DataType = value; }
      }

      public decimal? MinimumLength { get; set; } = 0.0M;
      public decimal? MaximumLength { get; set; } = 256.0M;

      public bool IsPrimaryKey { get; set; } = false;
      public bool IsIdentity { get; set; } = false;
      public bool IsRequired { get; set; } = false;
      public bool IsValue { get; set; } = false;

      public string Tags { get; set; }

      private string m_ParentName = null;
      public ItemInstanceItemInfo Parent { get; set; }
      public string ParentName
      {
         get
         {
            if (Parent == null)
               return String.Empty;
            if (m_ParentName == null)
            {
               m_ParentName = Parent.Name.Trim();
            }
            return m_ParentName;
         }
      }

      public NamespaceInfo Namespace { get; set; }
      public AssetDataElement Element { get; set; }

      /// <summary>
      /// Set Business Area and item Names.
      /// </summary>
      public void SetName()
      {
         string[] names = Path.Split('.');
         var first = names[0];
         if (!String.IsNullOrWhiteSpace(first))
         {
            BusinessAreaName = first;
         }

         var last = names[names.Length - 1];
         if (!String.IsNullOrWhiteSpace(last))
         {
            Name = last;
            OriginalName = last;
         }

         int pindx = Name.IndexOf('[');
         if (pindx != -1)
         {
            Name = Name.Substring(0, pindx);
            OriginalName = Name;
         }

         pindx = Name.IndexOf(':');
         if (pindx != -1)
         {
            Prefix = Name.Substring(0, pindx);
            Name = Name.Substring(pindx + 1);
            OriginalName = Name;
         }
      }
   }

}
