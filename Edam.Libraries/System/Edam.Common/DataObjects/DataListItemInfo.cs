using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------

namespace Edam.DataObjects
{

/// <summary>
/// Data List Item Info...
/// </summary>
public class DataListItemInfo
{

   private String m_ItemId;

   public Object Item { get; set; }
   public String Description { get; set; }
   // public System.Data.DataRowView DataRowView { get; set; }

   public String ItemId
   {
      get
      {
         if (m_ItemId == null)
            m_ItemId = Item.ToString();
         return(m_ItemId);
      }
      set { m_ItemId = value; }
   }

   public DataListItemInfo()
   {
      Item = null;
      m_ItemId = null;
      Description = String.Empty;
      //DataRowView = null;
   }  // end of DataListItemRecord

   public Int32 IdNo
   {
      get
      {
         return(Edam.Convert.ToInt32(Item));
      }
      set { Item = value; }
   }  // end of IdNo

   public Int16 SmallIdNo
   {
      get
      {
         return(Edam.Convert.ToInt16(Item));
      }
      set { Item = value; }
   }  // end of SmallIdNo

   public String Id
   {
      get
      {
         if (Item == null)
            return String.Empty;
         return(Item.ToString());
      }
      set { Item = value; }
   }  // end of Id

}  // end of DataListItemInfo

}  // end of Edam.DataObjects
