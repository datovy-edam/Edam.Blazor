using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.DataObjects.DataCodes;
using Edam.Serialization;

namespace Edam.DataObjects.Models
{

   public class ElementNodeBaseInfo : IElementInfo
   {
      public String Title { get; set; }
      public String Description { get; set; }

      /// <summary>
      /// Data-store resource name (i.e. Table or SP name, other)
      /// </summary>
      public String ResourceName { get; set; }

      public String Name { get; set; }
      public Int64? GroupNo { get; set; }
      public ResourceType Type { get; set; }

      public Int64? TemplateNo { get; set; }

      /// <summary>
      /// Indicate what is the type of resource in the data-store see
      /// ResourceName above
      /// </summary>
      public Int64? TemplateTypeNo { get; set; }
      public ResourceType? TemplateType
      {
         get
         {
            return TemplateTypeNo.HasValue ? 
               (ResourceType?) TemplateTypeNo.Value : null; 
         }
         set
         {
            TemplateTypeNo = (Int64?)value;
         }
      }

      public Boolean Validate()
      {
         Boolean isvalid = true;
         return isvalid;
      }

      public void ClearFields()
      {
         Title = String.Empty;
         Description = String.Empty;
         Name = String.Empty;
         ResourceName = null;
         GroupNo = null;
         TemplateNo = null;
         TemplateTypeNo = null;
         Type = ResourceType.Unknown;
      }
   }

   /// <summary>
   /// This is a Child of ElementInfo (the root) that may contain none one or
   /// many Items (see ElementItemInfo) leafs.
   /// </summary>
   public class ElementNodeInfo : ElementNodeBaseInfo, IElementInfo
   {

      public DataPage? Paging { get; set; }
      
      public List<ElementItemInfo> Items { get; set; }
      public List<MapInfo>? Maps { get; set; }
      public List<ModelGroupInfo> Groups { get; set; }

      public ElementNodeInfo()
      {
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         Maps = null;
         Items = new List<ElementItemInfo>();
         Groups = new List<ModelGroupInfo>();
      }

      /// <summary>
      /// Find an element by name.
      /// </summary>
      /// <param name="name">name to search</param>
      /// <returns>if found instance of ElementItemInfo is returned, else null
      /// </returns>
      public ElementItemInfo? GetItem(string name)
      {
         return Items.Find((x) => x.Name == name);
      }

      /// <summary>
      /// Set Item Value.
      /// </summary>
      /// <param name="name">item name whose value will be set</param>
      /// <param name="value">value in text format</param>
      public void SetItemValue(string name, string value)
      {
         var itm = GetItem(name);
         if (itm == null)
         {
            return;
         }
         itm.ValueText = value;
      }

      /// <summary>
      /// Get/Find the Leaf (ModelColumInfo) using element name.
      /// </summary>
      /// <param name="element">element to get related ModelColumInfo</param>
      /// <returns>if found returns an instance of MoodelColumnInfo, else null
      /// </returns>
      public ModelColumnInfo? GetLeaf(ElementItemInfo element)
      {
         var i = GetItem(element.Name);
         if (i != null)
         {
            ModelColumnInfo m = new ModelColumnInfo
            {
               ColumnName = element.Name,
               Title = i.Title,
               Width = 0,
               Visible = i.Visibility == 
                  Objects.ObjectVisibility.Visible,
               Element = element
            };
            return m;
         }
         return null;
      }

      public MapInfo? GetMap(string columnName)
      {
         foreach (var i in Maps)
         {
            if (i.Link.Count > 0)
            {
               if (i.Link[0].ChildElementName == columnName)
               {
                  return i;
               }
            }
         }
         return null;
      }

      /// <summary>
      /// Get the ModelData that will contain all the ModelColumnInfoes.
      /// </summary>
      /// <returns>instance of ModelData is returned</returns>
      public ModelData GetModelData()
      {
         return ModelData.GetModelData(this);
      }

      /// <summary>
      /// Get Node Items (ElementItemInfo's) from a json file.
      /// </summary>
      /// <param name="filePath">file full path</param>
      /// <param name="name">node name</param>
      /// <param name="description">node description</param>
      /// <returns>instance of ElementNodeInfo is returned</returns>
      public static ElementNodeInfo FromJsonFile(string filePath, string name, 
         string description)
      {
         ElementNodeInfo n = new ElementNodeInfo
         {
            Name = name,
            Description = description
         };

         string jsonText = System.IO.File.ReadAllText(filePath);
         List<ElementItemInfo> items =
            JsonSerializer.Deserialize<List<ElementItemInfo>>(jsonText);
         n.Items.AddRange(items);
         return n;
      }

   }

}
