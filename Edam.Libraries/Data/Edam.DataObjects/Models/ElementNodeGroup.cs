using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Data;
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Documents;

namespace Edam.DataObjects.Models
{

   public class ElementNodeGroup : ElementGroup
   {

      #region -- 1.50 - Initialize Resources

      public ElementNodeGroup(ElementGroup group = null)
      {
         if (group != null)
         {
            GlobalDictionary = group.GlobalDictionary;
            ElementGroupItem = group.ElementGroupItem;
         }
      }

      #endregion
      #region -- 4.00 - Manage Element Node, Maps & other

      public async Task<MapInfo> GetMap(ElementNodeInfo node, string columnName)
      {
         MapInfo map = node.GetMap(columnName);
         if (map == null)
         {
            return map;
         }
         if (!map.ParentNodeNo.HasValue)
            return null;
         var codeSetNode = GetNode(map.ParentNodeNo.Value);
         if (codeSetNode == null)
            return null;

         map.LinkCodes = await GetCodeSet(codeSetNode.Name, codeSetNode);
         if (map.LinkCodes == null || map.LinkCodes.Count == 0)
         {
            return null;
         }

         return map;
      }

      #endregion
      #region -- 4.00 - Manage Local / Remote Code Sets

      public async Task<List<DataCodeInfo>> GetCodeSet(
         string name, ElementNodeInfo node = null)
      {
         List<DataCodeInfo> items = null;

         // try finding the code-set locally...
         var c = await DataDocumentItem.GetCodeSet(name);
         if (c != null && c.Count > 0)
         {
            return c;
         }

         // the code-set was not found locally... so fetch info from remote...
         // get the items node
         node = node ?? GetNode(name);
         if (node == null)
         {
            items = new List<DataCodeInfo>();
            return items;
         }

         // synchronically... fetch the data from remote service
         var r = Services.ReferenceDataEditTemplateService.GetReferenceDataSync(
            node.TemplateNo, optionNo:
               ReferenceDataOption.TemplateAndGroupAndMapLinks);

         if (r.Success)
         {
            var m = ModelData.FromJson(r.ResponseData[0], node);
            return await SaveToLocalItems(node, m);
         }

         // return the data if all is OK... else empty set
         else
         {
            items = new List<DataCodeInfo>();
            return items;
         }
      }

      /// <summary>
      /// Given an ElementNodeInfo instance, try to catch in permanent local storage.
      /// </summary>
      /// <param name="node">node</param>
      /// <param name="data">data</param>
      public static async Task<List<DataCodeInfo>> SaveToLocalItems(
         ElementNodeInfo node, ModelData data)
      {
         var codeSet = ModelData.ToCodeGroupValue(node, data);
         if (codeSet != null && codeSet.Count > 0)
         {
            await DataDocumentItem.SaveItem<List<DataCodeInfo>>(node.Name,
               codeSet, Convert.ToTitleCase(node.Name));
         }
         return codeSet;
      }

      #endregion

   }

}
