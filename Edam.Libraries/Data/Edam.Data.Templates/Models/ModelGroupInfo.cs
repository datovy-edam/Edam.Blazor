using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
using System.Runtime.Serialization;
//using DbField = Edam.Data.DataField;
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Models;
using Edam.Serialization;

namespace Edam.DataObjects.Models
{

   [DataContract]
   public class ModelGroupInfo : ObjectInfo, IObjectInfo
   {

      private ElementNodeInfo m_Root;
      [DataMember]
      public PresentationContentInfo Content { get; set; }

      public ModelGroupInfo(ElementNodeInfo root) : base()
      {
         Content = new PresentationContentInfo();
         Type = ResourceType.PresentationGroup;
         m_Root = root;
      }

      public void SetParent(ElementNodeInfo element)
      {
         m_Root = element;
      }

      public new void ClearFields()
      {
         if (Content == null)
            Content = new PresentationContentInfo();
         Content.ClearFields();
      }

      public List<ElementItemInfo> GetItems()
      {
         return m_Root.Items;
      }

      /// <summary>
      /// Given a name of a model element return its definition.
      /// </summary>
      /// <param name="column">column information</param>
      /// <returns>instance of ElementItemInfo is returned if found</returns>
      public ElementItemInfo Find(PresentationColumnInfo column)
      {
         if (column == null)
            return null;
         var e = m_Root.Items.Find((x) => x.Name == column.Name);
         if (e == null)
            return null;
         return e;
      }

      public static String ToJsonText(List<ModelGroupInfo> groups)
      {
         return JsonSerializer.Serialize<List<ModelGroupInfo>>(groups);
      }
      public static List<ModelGroupInfo> FromJsonText(String jsonText)
      {
         return JsonSerializer.Deserialize<List<ModelGroupInfo>>(jsonText);
      }

   }

}
