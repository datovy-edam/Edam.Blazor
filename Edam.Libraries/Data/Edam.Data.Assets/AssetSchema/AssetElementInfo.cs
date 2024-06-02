using System;
using System.Collections.Generic;
using System.Text;

using Edam.Data.Asset;

namespace Edam.Data.AssetSchema
{

   public class AssetElementInfo<T> : AssetDataElement, IAssetElement
   {

      protected T m_ElementInstance;
      protected String m_FullPath = null;

      public T Instance
      {
         get { return m_ElementInstance; }
         set { m_ElementInstance = value; }
      }

      public override object AssetObject
      {
         get => this;
      }

      public object Item { get; set; }

   }

}
