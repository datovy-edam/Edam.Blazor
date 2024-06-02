using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

// -----------------------------------------------------------------------------
using Edam.Serialization;
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Dynamic;

namespace Edam.DataObjects.Models
{

   public class ModelData
   {

      public const string ROW_NUMBER = "row_number";
      public const string ROW_NUMBER_TITLE = "Row Number";

      public ElementNodeInfo ParentNode { get; set; }
      public List<ModelColumnInfo> Columns { get; set; }
      public List<MapInfo> Maps { get; set; }
      public dynamic ModelObject { get; set; }

      [IgnoreDataMember]
      public ObservableCollection<dynamic> Data { get; set; }

      public ModelData()
      {
         Data = null;
         ModelObject = null;
      }

      public void Dispose()
      {
         foreach (ModelColumnInfo c in Columns)
         {
            c.Dispose();
         }
      }

      /// <summary>
      /// Given an ElementNodeInfo get corresponding ModelData...
      /// </summary>
      /// <param name="node">element node</param>
      /// <returns>instane of ModelData is returned</returns>
      public static ModelData GetModelData(ElementNodeInfo node)
      {
         ModelData data = new ModelData
         {
            Columns = new List<ModelColumnInfo>()
         };

         // the first column should always be "row_number"
         ModelColumnInfo c = new ModelColumnInfo
         {
            ColumnName = ROW_NUMBER,
            Element = null,
            Title = ROW_NUMBER_TITLE,
            Visible = false
         };

         data.Columns.Add(c);

         // add all other columns...
         foreach (var i in node.Items)
         {
            c = new ModelColumnInfo
            {
               ColumnName = i.Name,
               Element = i,
               Title = i.Title,
               Width = i.MaxLength,
               Visible = i.Visibility == ObjectVisibility.Visible,
               ValueType = i.ValueType
            };
            c.ElementValue.ValueText = i.ValueText;
            data.Columns.Add(c);
         }

         data.Data = null;
         data.Maps = node.Maps;
         data.ParentNode = node;
         return data;
      }

      /// <summary>
      /// Given an ElementGroup get corresponding ModelData...
      /// </summary>
      /// <param name="group">element group that contains the item of interest
      /// </param>
      /// <returns>instane of ModelData is returned</returns>
      public static ModelData GetModelData(ElementGroup group)
      {
         ElementNodeInfo node = group.GetNode(group.ElementGroupItem.Name);
         return GetModelData(node);
      }

      /// <summary>
      /// Get the model data out of the JSON and match columns with node 
      /// elements, finally prepare dynamic data/value list.
      /// </summary>
      /// <remarks>
      /// Note that the row_number that head the columns list will not be 
      /// found within the node items since is only used to address a row by
      /// number (if needed)...
      /// </remarks>
      /// <param name="jsonText">JSON text</param>
      /// <param name="node">if provided columns will be matched with 
      /// corresponding elements</param>
      /// <returns>Built ModelData instance is returned</returns>
      public static ModelData FromJson(
         string jsonText, ElementNodeInfo node = null)
      {
         // deserialize json message 
         ModelData model = 
            JsonSerializer.Deserialize<ModelData>(jsonText);
         model.Data = new ObservableCollection<dynamic>();
         model.ParentNode = node;

         // get node data element for each column...
         if (node != null)
         {
            foreach (var c in model.Columns)
            {
               var i = node.Items.Find((x) => x.Name == c.ColumnName);
               if (i == null)
               {
                  c.ElementValue = new ObjectValueBase();
                  continue;
               }

               // TODO: put label in const lists...
               if (c.ColumnName == "SpanishDescription")
               {
                  c.Visible = false;
               }

               c.Element = i;
               c.ElementValue = c.Element;
            }
         }

         // prepare list of objects based on model...
         dynamic d = JsonSerializer.ToDynamic(jsonText);
         int indx;
         foreach (var i in d.Data)
         {
            indx = 0;
            var eobject = new ModelExpandoObject();
            foreach (var c in i)
            {
               object value = model.Columns[indx].ColumnName == "row_number" ?
                  Convert.ToInt32(c.Value.ToString()) : c.Value.ToString();
               eobject.AddProperty(
                  model.Columns[indx].ColumnName, value);
               if (model.Columns[indx].Element != null)
               {
                  model.Columns[indx].ElementValue.OriginalValue = value;
                  model.Columns[indx].ElementValue.ValueText = value.ToString();
               }
               indx++;
            }
            model.Data.Add(eobject.Instance);
         }
         return model;
      }

      /// <summary>
      /// Given a node and its data get the code-value list if any 
      /// is available.
      /// </summary>
      /// <remarks>
      /// The element node must have the data Code - Group - Description 
      /// properties that identify candidate code (key), group and description
      /// (dicernible [that allow us to understand the entity row meaning by
      /// somehow describing it]).
      /// </remarks>
      /// <param name="node">contains the reference object details including 
      /// the list of source properties (see Items)</param>
      /// <param name="data">list of properties as a dynamic class prepared 
      /// with the "Model Expando Object".</param>
      /// <returns>the list of Code-Value-(Group) items</returns>
      public static List<DataCodeInfo> ToCodeGroupValue(
         ElementNodeInfo node, ModelData data)
      {
         List<DataCodeInfo> l = new List<DataCodeInfo>();
         int kIndex = -1;  // key (code)
         int dIndex = -1;  // discernible
         int gIndex = -1;  // grouper

         // get the column index for the CodeId and its Value (or descriptor)
         bool ok;
         for (var i = 0; i < node.Items.Count; i++)
         {
            if (node.Items[i].KeyType == KeyType.Key)
            {
               //if (kIndex != -1)
               //{
               //   kIndex = -1;
               //   break;
               //}
               kIndex = i;
            }
            if (node.Items[i].Discernible == ObjectDiscernible.DescribeEntity)
            {
               if (node.Items[i].Locale == null)
               {
                  dIndex = i;
               }
            }
            if (node.Items[i].Groupable == ObjectGroupable.GroupEntity)
            {
               gIndex = i;
            }

            if (kIndex != -1 && dIndex != -1 && gIndex != -1)
            {
               break;
            }
         }

         // grouper is optional! so if not found is all good...
         ok = (kIndex != -1 && dIndex != -1);

         if (!ok)
         {
            return l;
         }

         // assign property names for code, value and/or group as needed
         string cname = node.Items[kIndex].Name;
         string vname = node.Items[dIndex].Name;
         string gname = gIndex == -1 ? null : node.Items[gIndex].Name;

         // populate the code - value list with given data
         foreach (dynamic i in data.Data)
         {
            DataCodeInfo c = new DataCodeInfo();
            ModelExpandoObject e = new ModelExpandoObject(i);
            c.CodeId = e.GetValue(cname).ToString();
            c.Value = e.GetValue(vname).ToString();
            if (gname != null)
            {
               c.GroupId = e.GetValue(gname).ToString();
            }
            l.Add(c);
         }
         return l;
      }

   }
}
