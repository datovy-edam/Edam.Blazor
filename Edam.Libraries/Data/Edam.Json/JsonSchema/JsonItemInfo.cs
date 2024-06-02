using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Data.AssetManagement;
using Edam.DataObjects.DataCodes;

namespace Edam.Json.JsonSchema
{

   public class JsonItemInfo : AssetElementInfo<JToken>, IAssetElement
   {

      protected List<JsonItemInfo> m_Children = new List<JsonItemInfo>();

      public List<JsonItemInfo> Children
      {
         get { return m_Children; }
      }

      public JsonItemInfo(JToken token, NamespaceList namepaces)
      {
         Namespaces = namepaces;
         Instance = token;
         SetItem(token);
      }

      #region -- 4.00 - Manage Item definition

      /// <summary>
      /// Set Item properties based on JSON definition.
      /// </summary>
      /// <param name="token">JSON token with definitions</param>
      private void SetItem(JToken token)
      {
         switch (token.Type)
         {
            case JTokenType.Property:
               SetProperty(token as JProperty, Namespaces);
               break;
            default:
               break;
         }

         if (Children.Count > 0)
         {
            ElementType = ElementType.type;
         }
      }

      /// <summary>
      /// Set the JSON properties and details...
      /// </summary>
      /// <param name="property">JSON property to inspect</param>
      /// <param name="ns">namespace list</param>
      private void SetProperty(JProperty property, NamespaceList ns)
      {
         ElementQualifiedName = new QualifiedNameInfo(property.Name);
         JsonQualifiedNameInfo.CheckQualifiedName(
            ElementQualifiedName, ns);
         EntityName = property.Name;
         ElementType = ElementType.type;

         GetChildren(property, this);

         if (ElementType == ElementType.enumerator)
         {
            EntityName = null;
            EntityQualifiedName = null;
         }
      }

      #endregion
      #region -- 4.00 - Manage Asset Element definition

      public static void SetFullPath(AssetElementInfo<IAssetElement> asset,
         JsonAssetItemInfo assets)
      {

         // prepare full path
         StringBuilder sb = new StringBuilder();
         bool needSlash = false;

         //if (!String.IsNullOrWhiteSpace(ParentEntityName))
         //{
         //   sb.Append(ParentEntityName);
         //   needSlash = true;
         //}
         //if (needSlash)
         //{
         //   sb.Append(JsonLabel.SLASH);
         //   needSlash = false;
         //}
         if (!string.IsNullOrWhiteSpace(asset.EntityName))
         {
            sb.Append(asset.EntityName);
            needSlash = true;
         }
         if (needSlash)
         {
            sb.Append(JsonLabel.SLASH);
            needSlash = false;
         }
         if (!string.IsNullOrWhiteSpace(
            asset.ElementQualifiedName.Name))
            sb.Append(asset.ElementQualifiedName.Name);

         // check property type and see if we need to look for children elements
         // TODO: add assets entries by going in depth with the current asset
         //     : QualifiedName - Original Name Item (see previous Append)

         /*
         String propType = null;
         if (properties != null)
         {
            var cprop = properties.Find((x) =>
            {
               var qn = new JsonQualifiedNameInfo(x.EntityName);
               return qn.OriginalName == asset.QualifiedName.OriginalName;
            });
            if (cprop != null)
            {
               propType = cprop.TypeName;
            }
         }
         JsonComplexType cxType = null;
         if (definitions != null && propType != null)
         {
            var cdef = definitions.Find((x) =>
            {
               var qn = new JsonQualifiedNameInfo(x.EntityName);
               var qp = new JsonQualifiedNameInfo(propType);
               return qn.OriginalName == qp.OriginalName;
            });
            if (cdef != null)
               cxType = cdef;
         }

         if (cxType != null && cxType.Children.Count > 0)
         {
            foreach(var c in cxType.Children)
            {
               var nAsset = c.ToAsset(null, properties, definitions);
            }
         }
          */

         asset.SetFullPath(sb.ToString());
      }

      /// <summary>
      /// Conver JSON Item into an Asset Element ...
      /// </summary>
      /// <param name="ParentEntityName"></param>
      /// <param name="properties"></param>
      /// <returns></returns>
      public AssetElementInfo<IAssetElement> ToAsset(
         string parentEntityName = null,
         JsonAssetItemInfo assetItem = null)
      {
         var me = this;
         JsonQualifiedNameInfo.CheckQualifiedName(
            ElementQualifiedName, Namespaces);

         JsonQualifiedNameInfo jQName = new JsonQualifiedNameInfo(TypeName);
         JsonQualifiedNameInfo.CheckQualifiedName(jQName, Namespaces);
         string typeName = TypeName;

         if (me.TypeQualifiedName == null)
         {
            me.TypeQualifiedName = new QualifiedNameInfo(TypeName);
         }

         AssetElementInfo<IAssetElement> a =
            new AssetElementInfo<IAssetElement>
            {
               Namespaces = me.Namespaces,
               Namespace = string.IsNullOrWhiteSpace(me.Namespace) ?
                  me.Namespaces.DefaultNamespace.Uri.OriginalString :
                  me.Namespace,
               TypeQualifiedName = me.TypeQualifiedName,
               ElementQualifiedName = me.ElementQualifiedName,
               CommentText = me.CommentText,
               DefaultValue = me.DefaultValue,
               EntityName = me.EntityName,
               FixedValue = me.FixedValue,
               IsMixed = me.IsMixed,
               IsNillable = me.IsNillable,
               Occurs = me.Occurs,
               SampleValue = me.SampleValue,
               ElementType = me.ElementType,
               DataType = me.DataType == null ? me.TypeQualifiedName.Name :
                   me.DataType
            };

         if (jQName.NameType == JsonQualifiedNameType.Property &&
             assetItem.Assets != null)
         {
            var pname = jQName.Name.Replace("#/", "").Replace("/", ".");
            var tn = assetItem.FindProperty(pname);
            if (tn != null)
            {
               typeName = tn.TypeName;
               //foreach(var n in tn.Annotation)
               //{
               //   a.Annotation.Add(n);
               //}
            }
            var aqName = new QualifiedNameInfo(typeName);
            a.QualifiedTypeNames.Add(aqName);
            aqName.Prefix = jQName.Prefix;
         }
         else
         {
            foreach (var i in QualifiedTypeNames)
            {
               JsonQualifiedNameInfo.CheckQualifiedName(i, Namespaces);
               a.AddQualifiedTypeName(i.Name, jQName.Prefix);
            }
         }

         var grpQName = new JsonQualifiedNameInfo(EntityName);
         if (IsType)
         {
            a.ElementQualifiedName = new QualifiedNameInfo(EntityName);
            a.ElementQualifiedName.Prefix = grpQName.Prefix;
            a.EntityName = string.Empty;
         }
         else if (ElementType == ElementType.element)
         {
            a.EntityName = parentEntityName;
         }

         foreach (var n in Annotation)
         {
            if (!string.IsNullOrWhiteSpace(n))
               a.Annotation.Add(n);
         }
         if (a.Namespaces == null)
            a.Namespaces = new NamespaceList();
         if (Namespaces != null)
         {
            foreach (var s in Namespaces)
            {
               a.Namespaces.Add(s);
            }
         }

         JsonQualifiedNameInfo qnEntityName =
            new JsonQualifiedNameInfo(a.EntityName);
         JsonQualifiedNameInfo qnTypeName =
            new JsonQualifiedNameInfo(typeName);
         JsonQualifiedNameInfo.CheckQualifiedName(qnTypeName, Namespaces);

         a.EntityName = qnEntityName.OriginalName;
         a.QualifiedTypeNames.Clear();

         var qName = new QualifiedNameInfo(qnTypeName.OriginalName);
         a.QualifiedTypeNames.Add(qName);
         qName.Prefix = qnTypeName.Prefix;
         a.Type = qName.Name;

         if (a.Annotation.Count == 0)
         {
            a.Annotation.Add(Text.Convert.ToTitleCase(
               ElementQualifiedName.OriginalName));
         }

         SetFullPath(a, assetItem);

         return a;
      }

      #endregion
      #region -- 4.00 - Manage Groups (oneOf, allOf or other)

      /// <summary>
      /// Get Item Children.
      /// </summary>
      /// <remarks>
      /// such as:
      ///    "nc:Vehicle": {
      ///       <-- nc:Vehicle is the item name
      ///       <-- the rest are the Children as follows
      ///       "description": "...",
      ///       "oneOf": [ ... ]
      /// </remarks>
      /// <param name="token">JSON parent item definition</param>
      /// <param name="item">parent item definition</param>
      /// <param name="group">(optional) group if any (oneOf, allOf, other)
      /// </param>
      private static void GetChildren(JToken token, JsonItemInfo item,
         ElementGroup? group = null)
      {
         foreach (JToken c in token.Children<JToken>())
         {
            switch (c.Type)
            {
               case JTokenType.Property:
                  item.GetProperty(c as JProperty, item);
                  break;
               case JTokenType.Object:
                  item.GetObject(c, item);
                  break;
               case JTokenType.Array:
                  if (group == null)
                  {
                     item.Occurs = JsonLabel.ZERO_TO_MANY;
                  }
                  else
                  {
                     item.ElementGroup = group.Value;
                     item.TypeQualifiedName = new QualifiedNameInfo(
                        item.ElementQualifiedName.Prefix, item.DataTypeText);
                  }

                  GetChildren(c, item);
                  break;
            }
         }
      }

      /// <summary>
      /// Get Item Of ... a group such as oneOf, allOf or other.
      /// </summary>
      /// <param name="token">token with item representation</param>
      /// <param name="group">group being targeted (oneOf or other)</param>
      /// <param name="item">instance of self or child item</param>
      private void GetItemOf(
         JToken token, ElementGroup group, JsonItemInfo item)
      {
         GetChildren(token, item, group);
      }

      #endregion
      #region -- 4.00 - Manage Enumerators

      public static string GetEnumDescription(JProperty property)
      {
         string d = null;
         if (property == null)
            return d;
         if (property.Name == JsonLabel.DESCRIPTION)
         {
            switch (property.Value.Type)
            {
               case JTokenType.String:
                  d = (string)property.Value;
                  break;
               default:
                  d = null;
                  break;
            }
         }
         return d;
      }

      public static List<ElementKeyValue> GetEnumKeyValue(JToken token)
      {
         List<ElementKeyValue> e = new List<ElementKeyValue>();
         if (token != null && token.HasValues)
         {
            switch (token.Type)
            {
               case JTokenType.Array:
                  foreach (var t in token)
                  {
                     switch (t.Type)
                     {
                        case JTokenType.String:
                           var v = new ElementKeyValue
                           {
                              Key = t.Value<string>()
                           };
                           e.Add(v);
                           break;
                        default:
                           var d = new ElementKeyValue
                           {
                              Key = t.Value<string>()
                           };
                           e.Add(d);
                           break;
                     }
                  }
                  break;
               default:
                  break;
            }
         }
         return e;
      }

      public static ElementKeyValue GetEnum(JProperty property)
      {
         ElementKeyValue keyValue = new ElementKeyValue();
         switch (property.Value.Type)
         {
            case JTokenType.String:
               keyValue.Value = (string)property.Value;
               break;
            case JTokenType.Array:
               if (property.Name == JsonLabel.ENUM)
               {
                  var kv = GetEnumKeyValue(property.Value);
                  var desc = GetEnumDescription(property.Next as JProperty);
                  keyValue.Key = kv.Count > 0 ? kv[0].Key : desc;
                  keyValue.Description = desc;
               }
               else
                  keyValue.Value = JsonLabel.NOT_SUPPORTED;
               break;
            default:
               keyValue.Key = JsonLabel.NOT_SUPPORTED;
               keyValue.Description = JsonLabel.NOT_SUPPORTED;
               break;
         }
         return keyValue;
      }

      /// <summary>
      /// Get the list of Enums (Key - Values)
      /// </summary>
      /// <param name="property"></param>
      /// <returns></returns>
      public static List<ElementKeyValue> GetEnums(JProperty property)
      {
         List<ElementKeyValue> items = new List<ElementKeyValue>();
         switch (property.Value.Type)
         {
            case JTokenType.String:
               var kyv = new ElementKeyValue();
               kyv.Value = (string)property.Value;
               items.Add(kyv);
               break;
            case JTokenType.Array:
               if (property.Name == JsonLabel.ENUM)
               {
                  var kv = GetEnumKeyValue(property.Value);
                  foreach (var k in kv)
                  {
                     var desc = GetEnumDescription(property.Next as JProperty);
                     var ckeyValue = new ElementKeyValue();
                     ckeyValue.Key = kv.Count > 0 ? k.Key : desc;
                     ckeyValue.Description = desc;
                     items.Add(ckeyValue);
                  }
               }
               else
               {
                  var nval = new ElementKeyValue();
                  nval.Value = JsonLabel.NOT_SUPPORTED;

               }
               break;
            default:
               var dval = new ElementKeyValue();
               dval.Key = JsonLabel.NOT_SUPPORTED;
               dval.Description = JsonLabel.NOT_SUPPORTED;
               items.Add(dval);
               break;
         }
         return items;
      }

      /// <summary>
      /// Set enumerator key - description codes...
      /// </summary>
      /// <param name="property"></param>
      /// <param name="item"></param>
      private void SetEnumInfo(
         JProperty property, JsonItemInfo item)
      {
         if (item.ElementType != ElementType.enumerator)
         {
            item.ElementType = ElementType.enumerator;
            item.Occurs = JsonLabel.ZERO_TO_ONE;
            item.EnumItems = new List<DataCodeInfo>();
         }

         var e = GetEnums(property);

         foreach (var k in e)
         {
            DataCodeInfo code = new DataCodeInfo();
            code.Description = String.IsNullOrWhiteSpace(k.Description) ?
               k.Key : k.Description;
            code.Value = String.IsNullOrWhiteSpace(k.Value) ? k.Key : k.Value;

            item.EnumItems.Add(code);
         }

         /*
         JsonItemInfo i = new JsonItemInfo(property, item.Namespaces)
         {
            ElementType = ElementType.enumerator,
            ElementQualifiedName = new QualifiedNameInfo(e.Key),
            DefaultValue = e.Description,
            EntityName = item.EntityName,
            Occurs = JsonLabel.ZERO_TO_ONE
         };
         i.QualifiedTypeNames.Add(new QualifiedNameInfo(JsonLabel.STRING));
         i.SetFullPath(property.Name);
         if (property.Value.Type == JTokenType.Object)
         {
            GetTypeElementProperties(property.Value, i);
         }
         item.Children.Add(i);
          */
      }

      #endregion

      public static string GetValue(JProperty property)
      {
         string value = null;
         switch (property.Value.Type)
         {
            case JTokenType.String:
               value = (string)property.Value;
               break;
            case JTokenType.Array:
               if (property.Name == JsonLabel.ENUM)
                  value = JsonLabel.NOT_SUPPORTED;
               else
                  value = JsonLabel.NOT_SUPPORTED;
               break;
            default:
               value = null;
               break;
         }
         return value;
      }

      #region -- 4.00 - Manage Property definitions

      /// <summary>
      /// Add Child Property...
      /// </summary>
      /// <param name="property">JSON property details</param>
      /// <param name="item">parent item</param>
      public void AddProperty(JProperty property, JsonItemInfo item)
      {
         bool isReference = false;

         JsonItemInfo i = new JsonItemInfo(property, item.Namespaces)
         {
            ElementType = ElementType.element,
            ElementQualifiedName = new QualifiedNameInfo(property.Name)
         };

         // magne References...
         if (property.Name == JsonLabel.REF)
         {
            isReference = true;
            SetReferenceElement(property, i);
         }

         // manage ARRAY's
         if (item.TypeQualifiedName != null &&
            item.TypeQualifiedName.OriginalName == JsonLabel.ARRAY)
         {
            string[] tks = item.ElementPath.Split('.');
            if (tks.Length >= 3)
            {
               string prefix = item.ElementQualifiedName.Prefix;
               string name = tks[tks.Length - 3];
               item.EntityQualifiedName = 
                  new QualifiedNameInfo(prefix, name);
            }

            item.Occurs = JsonLabel.ZERO_TO_MANY;
            item.TypeQualifiedName = i.TypeQualifiedName;
            item.DataType = i.TypeQualifiedName.Name;
            item.ElementType = ElementType.element;
            return;
         }

         switch (property.Value.Type)
         {
            case JTokenType.Object:
               GetTypeElementProperties(property.Value, i);
               break;
            default:
               if (isReference)
               {
                  i.ElementType = ElementType.reference;
                  i.ElementQualifiedName = new QualifiedNameInfo(
                     i.TypeQualifiedName.Prefix,
                     i.TypeQualifiedName.OriginalName + "_Reference");
                  JsonQualifiedNameInfo.CheckQualifiedName(
                     i.ElementQualifiedName, i.Namespaces);
               }
               else
               {
                  i.TypeQualifiedName = new QualifiedNameInfo(
                     item.Namespaces.GetDefaultPrefix(),
                     property.Value.Type.ToString().ToLower());
                  JsonQualifiedNameInfo.CheckQualifiedName(
                     i.TypeQualifiedName, i.Namespaces);
                  i.ElementType = ElementType.element;
               }
               i.EntityQualifiedName = ElementQualifiedName;
               break;
         }

         if (i.ElementQualifiedName.OriginalName.StartsWith("$"))
            return;

         item.Children.Add(i);
      }

      /// <summary>
      /// Get Property...
      /// </summary>
      /// <param name="property">property information</param>
      /// <param name="item">target item</param>
      public void GetProperty(JProperty property, JsonItemInfo item)
      {
         switch (property.Name)
         {
            case JsonLabel.FORMAT:
               SetFormat(GetValue(property), item);
               break;
            case JsonLabel.DESCRIPTION:
               item.AddAnnotation(GetValue(property));
               break;
            case JsonLabel.ONE_OF:
               GetItemOf(property, ElementGroup.OptionOne, item);
               break;
            case JsonLabel.ANY_OF:
               GetItemOf(property, ElementGroup.OptionAny, item);
               break;
            case JsonLabel.ALL_OF:
               GetItemOf(property, ElementGroup.OptionAll, item);
               break;
            case JsonLabel.TYPE:
               var value = GetValue(property);
               if (value == null)
               {
                  AddProperty(property, item);
               }
               else
               {
                  SetTypeElementInfo(item, value);
               }
               break;
            case JsonLabel.ENUM:
               SetEnumInfo(property, item);
               break;
            case JsonLabel.REF:
               SetReferenceElement(property, item);
               break;
            case JsonLabel.PROPERTIES:
               GetTypeElementProperties(property, item);
               break;
            case JsonLabel.REQUIRED:
               SetRequired(property, item);
               break;
            default:
               AddProperty(property, item);
               break;
         }
      }

      #endregion

      /// <summary>
      /// Set the data element format and if it matches with some tracked data
      /// elements then make those as the Element Type...
      /// </summary>
      /// <param name="formatName"></param>
      /// <param name="item"></param>
      private void SetFormat(string formatName, JsonItemInfo item)
      {
         formatName = formatName.Replace("-", string.Empty);
         var qn = new QualifiedNameInfo(
            item.Namespaces.GetDefaultPrefix(),
            formatName.ToLower());
         var r = JsonQualifiedNameInfo.CheckQualifiedName(qn, item.Namespaces);
         if (r != ElementType.undefined)
         {
            item.TypeQualifiedName = qn;
            item.QualifiedTypeNames.Clear();
            item.AddQualifiedTypeName(qn);
         }
      }

      /// <summary>
      /// Scan all required properties and set min occurs to 1...
      /// </summary>
      /// <param name="property">parent with value as JArray with required
      /// property names</param>
      /// <param name="item">parent whose children will be scanned</param>
      public void SetRequired(JProperty property, JsonItemInfo item)
      {
         foreach (string i in property.Value.Values<string>())
         {
            var v = item.Children.Find(
               (x) => x.ElementQualifiedName.OriginalName == i);
            if (v != null)
            {
               var o = new OccuranceInfo(v.Occurs)
               {
                  MinOccurance = 1
               };
               v.Occurs = o.Text;
            }
         }
      }

      /// <summary>
      /// Get Object represeted by the token
      /// </summary>
      /// <param name="token"></param>
      /// <param name="item"></param>
      /// <returns></returns>
      public AssetElementInfo<JToken> GetObject(
         JToken token, JsonItemInfo item)
      {
         foreach (JToken c in token.Children<JToken>())
         {
            switch (c.Type)
            {
               case JTokenType.Property:
                  GetProperty(c as JProperty, item);
                  break;
               case JTokenType.Object:
                  GetTypeElementProperties(c, item);
                  break;
            }
         }
         return item;
      }

      private static void SetTypeElementInfo(
         JsonItemInfo item, string typeName)
      {
         var qName = item.AddQualifiedTypeName(
            typeName, item.Namespaces.GetDefaultPrefix());
         var grp = JsonQualifiedNameInfo.CheckQualifiedName(
            qName, item.Namespaces);
         item.TypeQualifiedName = qName;
         item.ElementType = grp == ElementType.element ?
            grp : ElementType.type;
         item.SetFullPath(item.Instance.Path);
      }

      private static JsonItemInfo GetTypeElementProperties(
         JToken token, JsonItemInfo item)
      {
         foreach (var c in token.Children<JToken>())
         {
            switch (c.Type)
            {
               case JTokenType.Object:
                  GetTypeElementProperties(c, item);
                  break;
               case JTokenType.Property:
                  item.GetProperty(c as JProperty, item);
                  break;
               default:
                  break;
            }
         }
         return item;
      }

      /// <summary>
      /// Set Item as a reference...
      /// </summary>
      /// <param name="property">property</param>
      /// <param name="item">item</param>
      /// <returns>returnes the item</returns>
      private static JsonItemInfo SetReferenceElement(
         JProperty property, JsonItemInfo item)
      {
         string v = GetValue(property);
         var typeName = JsonQualifiedNameInfo.GetTypeName(v);

         if (item.ElementGroup == ElementGroup.NotApplicable)
         {
            item.TypeQualifiedName = new QualifiedNameInfo(
               item.Namespaces.GetDefaultPrefix(), typeName);
            item.AddQualifiedTypeName(item.TypeQualifiedName);
            item.SetFullPath(property.Path);
         }
         else if (item.IsGrouped)
         {
            item.AddProperty(property, item);
         }

         return item;
      }

      //private static JsonItemInfo GetReferenceElement(JProperty property)
      //{
      //   String v = GetValue(property);
      //   JsonItemInfo item = new JsonItemInfo(property);
      //   item.GroupType = AssetGroupItemType.reference;
      //   item.QualifiedName = new QualifiedNameInfo(v);
      //   item.SetFullPath(property.Path);
      //   return item;
      //}

   }

}
