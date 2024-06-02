using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.Assets.AssetSchema
{

   public enum ElementPropertyType
   {
      Unknown = 0,
      Description = 1,
      Privacy = 2,
      ExternalReference = 3
   }

   public class ElementPropertyInfo
   {
      public ElementPropertyType Type { get; set; } = 
         ElementPropertyType.Unknown;
      public string SchemaName { get; set; }
      public string EntityName { get; set; }
      public string ElementName { get; set; }
      public string PropertyName { get; set; }
      public string PropertyValue { get; set; }
   }

   public class AssetProperties
   {
      private List<ElementPropertyInfo> m_Items;

      public AssetProperties(List<ElementPropertyInfo> elements)
      {
         m_Items = elements;
      }

      /// <summary>
      /// Find item matching given schema, entity, element and type...
      /// </summary>
      /// <param name="schemaName">schema name</param>
      /// <param name="entityName">entity name</param>
      /// <param name="elementName">element name</param>
      /// <param name="type">type</param>
      /// <returns>returns an instance of ElementPropertyInfo</returns>
      public ElementPropertyInfo Find(string schemaName,
         string entityName, string elementName, ElementPropertyType type)
      {
         ElementPropertyInfo entry = m_Items.Find((x) => 
            x.Type == type &&
            x.SchemaName == schemaName && 
            (String.IsNullOrWhiteSpace(elementName) && 
             String.IsNullOrWhiteSpace(x.ElementName) ? true :
             x.ElementName == elementName) && 
            x.EntityName == entityName);
         return entry;
      }

      /// <summary>
      /// Prepare documentation items...
      /// </summary>
      /// <param name="list">list of a list of strings</param>
      /// <returns>instance of AssetDocumentation</returns>
      public static AssetProperties GetInstance(List<List<string>> list)
      {
         if (list == null || list.Count <= 1)
         {
            return null;
         }

         List<ElementPropertyInfo> items =
            new List<ElementPropertyInfo>();
         foreach (var item in list)
         {
            if (item.Count != 5)
            {
               continue;
            }
            ElementPropertyInfo entry = 
               new ElementPropertyInfo();

            entry.SchemaName = item[0];
            entry.EntityName = item[1];
            entry.ElementName = item[2];
            entry.PropertyName = item[3];
            entry.PropertyValue= item[4];

            switch (entry.PropertyName)
            {
               case "Description":
                  entry.Type = ElementPropertyType.Description;
                  break;
               case "ExternalReference":
                  entry.Type = ElementPropertyType.ExternalReference;
                  break;
               case "Privacy":
                  entry.Type = ElementPropertyType.Privacy;
                  break;
            }

            items.Add(entry);
         }

         return new AssetProperties(items);
      }

   }

}
