using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Json.JsonSchema
{

   public class JsonPropertyInfo : JsonPropertyItemInfo
   {

      public JsonPropertyInfo(
         string propertyName, JSchema item, JsonSchemaInfo schema,
         NamespaceList namespaces) :
         base(propertyName, item, schema, namespaces)
      {
         EntityName = ElementQualifiedName.Name;
         // check children for definitions and choices (options/groups)...
         ExamineChildren();
      }

      public bool CheckDefinition(
         JsonPropertyItemInfo item1, JsonPropertyItemInfo item2)
      {
         if (item1.IsDefinition && item2.IsDefinition)
         {
            return item1.ItemNameType == item2.ItemNameType;
         }
         return false;
      }

      public bool ExamineDefinition(JsonPropertyItemInfo item)
      {
         bool done = false;
         if (item.IsDefinition)
         {
            AddQualifiedTypeName(item.ElementQualifiedName.Name);
            ElementQualifiedName = item.ElementQualifiedName;
            done = true;
         }
         return done;
      }

      public bool ExamineArray(
         JsonPropertyItemInfo item, string occuranceText)
      {
         bool done = false;
         if (item.IsArray)
         {
            if (item.Children.Count == 1 &&
                item.Children[0].IsDefinition)
            {
               if (item.Children[0].ElementQualifiedName.Name ==
                  ElementQualifiedName.Name)
               {
                  if (IsOneOf)
                  {
                     Occurs = occuranceText;
                     done = true;
                  }
               }
            }
         }
         return done;
      }

      /// <summary>
      /// Examine Children and setup OneOf Definition or Array if that 
      /// structure is found, else return false
      /// </summary>
      /// <returns>true is returned if it is the same definition for a OneOf
      /// for a type declaration or the same definition as an array,
      /// else false</returns>
      public bool ExamineDefinitionAndArray()
      {
         bool done = false;
         bool gotDefinition = false;
         if (Children.Count >= 1)
         {
            gotDefinition = ExamineDefinition(Children[0]);
         }
         if (Children.Count == 2 && gotDefinition)
         {
            done = ExamineArray(Children[1], OCCURS_1TO_UNBOUNDED);
         }
         return gotDefinition || done;
      }

      /// <summary>
      /// The same definition for an item and an item array for OneOf and AnyOf
      /// children...
      /// </summary>
      /// <remarks>
      /// Children are added based on their hierarchy backwards... so:
      ///    OneOf - AnyOf
      /// will be found as:
      ///    AnyOf - OneOf
      /// </remarks>
      /// <returns>true is returned if OneOf and AnyOf with the same definition/
      /// type is found, else false</returns>
      public bool ExamineDefinitionArrayOneOfAnyOf()
      {
         bool done = false;
         string typeName = null;
         if (Children.Count >= 2 && Children[0].Children.Count >= 0 &&
            Children[0].Children[0].Children.Count >= 1 &&
            Children[0].Children[0].Children[0].IsOneOf)
         {
            if (Children[0].IsAnyOf &&
               Children[0].Children[0].Children[0].IsDefinition)
            {
               typeName = Children[0].Children[0].Children[0].TypeName;
            }
         }
         if (typeName == null)
            return false;
         string arrayTypeName = null;
         if (Children[1].IsArray && Children[1].Children.Count >= 1 &&
            Children[1].Children[0].Children.Count >= 1)
         {
            if (Children[1].Children[0].IsAnyOf)
            {
               if (Children[1].Children[0].Children[0].IsOneOf)
               {
                  arrayTypeName = Children[1].Children[0].Children[0].TypeName;
               }
            }
         }
         if (arrayTypeName == null)
            return false;
         if (typeName == arrayTypeName && TypeName == JsonLabel.OBJECT)
         {
            ElementQualifiedName = new QualifiedNameInfo(typeName);
            AddQualifiedTypeName(typeName);
            Occurs = OCCURS_ZTO_UNBOUNDED;
            done = true;
         }
         return done;
      }

      /// <summary>
      /// Examine Extension looking for a reference...
      /// </summary>
      /// <param name="token"></param>
      public string ExamineExtensionFindReference(JToken token)
      {
         string refText = null;
         foreach (var child in token)
         {
            if (child.Type == JTokenType.Property)
            {
               JProperty property = (JProperty) child;
               if (property.Name == JsonLabel.REF)
               {
                  refText = property.Value.ToString();
                  break;
               }
            }
         }
         return refText;
      }

      /// <summary>
      /// Examine the item as a Reference.
      /// </summary>
      /// <returns>true if it is a reference</returns>
      public bool ExamineReferenceAs()
      {
         bool done = false;
         string description = Instance.Description;
         if (Instance.ExtensionData != null)
         {
            foreach (var e in Instance.ExtensionData)
            {
               foreach(var c in e.Value)
               {
                  var refText = ExamineExtensionFindReference(c);
                  if (refText != null)
                  {
                     string[] l = refText.Split("/");
                     TypeQualifiedName = new QualifiedNameInfo(
                        ElementQualifiedName.Prefix, l[l.Length-1]);
                     return true;
                  }
               }
            }
         }
         return done;
      }

      /// <summary>
      /// Try to fine a Type and Occurance definition examinine it's children,
      /// if so then IsDefined will be set to true.
      /// </summary>
      /// <remarks>
      /// If IsDefined is true then there is no need to examine its children and
      /// this entry will have all needed for a full element definition.
      /// </remarks>
      /// <returns>true is returned of both the type and the item occurance  was
      /// assigned, else false</returns>
      public bool ExamineChildren()
      {
         m_IsDefined = false;
         bool isDefinitionAndArray = ExamineDefinitionAndArray();
         if (isDefinitionAndArray)
         {
            m_IsDefined = true;
            return m_IsDefined;
         }
         bool isDefinitionOneOfAnyOf = ExamineDefinitionArrayOneOfAnyOf();
         if (isDefinitionOneOfAnyOf)
         {
            m_IsDefined = true;
            return m_IsDefined;
         }
         bool isReference = ExamineReferenceAs();
         if (isReference)
         {
            m_IsDefined = true;
         }
         return m_IsDefined;
      }

   }

}
