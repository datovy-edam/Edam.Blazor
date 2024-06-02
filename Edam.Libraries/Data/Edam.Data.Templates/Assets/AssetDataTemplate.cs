using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Models;
using Edam.Data.AssetSchema;
using Edam.Data.Asset;
using Edam.Serialization;

namespace Edam.DataObjects.Assets
{

   public class AssetDataTemplate
   {

      #region -- 1.0 - Properties and Fields

      #endregion
      #region -- 4.0 - Manage Asset to Template

      public static ElementItemInfo AssetToItem(
         AssetDataElement item, int sequenceNo)
      {
         ElementItemInfo element = new ElementItemInfo();
         element.Title = AssetDataElement.GetAnnotattion(item);
         element.Name = item.ElementQualifiedName.OriginalName;

         ValueType? vtype = ObjectValueBase.GetType(
            item.TypeQualifiedName.OriginalName);
         element.ValueType = (Objects.ObjectValueType)((vtype == null) ?
            Objects.ObjectValueType.String : vtype);

         element.SerialNo = sequenceNo;
         element.MinLength = (short)(item.MinLength.HasValue ?
            item.MinLength.Value : 0);
         element.MaxLength = (short)(item.MaxLength.HasValue ?
            item.MaxLength.Value : 0);

         return element;
      }

      /// <summary>
      /// Prepare model data template element nodes and while doing this update
      /// the AssetDataElement instances properties bag...
      /// </summary>
      /// <param name="items">list of AssetDataElement</param>
      /// <returns>list of element nodes is returned</returns>
      public static List<ElementNodeInfo> AssetToTemplate(
         List<AssetDataElement> items)
      {
         int count = 0;

         // get all type declarations
         var types = from c in items
                     where c.IsType
                     select c;

         List<ElementNodeInfo> nodes = new List<ElementNodeInfo>();
         foreach(AssetDataElement item in types)
         {
            // prepare node
            ElementNodeInfo node = new ElementNodeInfo();
            node.Title = item.ElementQualifiedName.OriginalName;
            node.Description = item.Description;
            node.ResourceName = ResourceType.TemplateGroup.ToString();
            node.Name = item.ElementQualifiedName.OriginalName;
            node.Type = ResourceType.TemplateGroup;
            node.TemplateType = ResourceType.TemplateGroup;

            // get the properties bag
            var bag = item.PropertiesBag;
            ElementNodeInfo n = (ElementNodeInfo)bag.AssetTemplateInstance;
            node.Maps = n.Maps;

            // prepare children
            var children = from c in items
                           where c.EntityQualifiedNameText == 
                              item.ElementQualifiedNameText
                           select c;

            foreach (AssetDataElement child in children)
            {
               count++;
               var element = AssetToItem(item, count);
               node.Items.Add(element);
            }

            bag.SetTemplate(node);
            item.PropertiesBag = bag;

            nodes.Add(node);
         }

         return nodes;
      }

      #endregion

   }

}
