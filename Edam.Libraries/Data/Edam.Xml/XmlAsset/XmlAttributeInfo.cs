using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

// -----------------------------------------------------------------------------
using Edam.Xml.XmlExplore;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Xml.XmlAsset
{

   public class XmlAttributeInfo : AssetDataElement, IAssetElement
   {
      protected String m_FullPath = null;
      public InstanceAttribute Instance { get; set; }
      //public String FullPath
      //{
      //   get { return m_FullPath; }
      //}

      public new String GetFullPath()
      {
         return String.IsNullOrWhiteSpace(m_FullPath) ? String.Empty :
            m_FullPath;
      }

      //public void SetFullPath(String path)
      //{
      //   m_FullPath = path;
      //}
   }

}
