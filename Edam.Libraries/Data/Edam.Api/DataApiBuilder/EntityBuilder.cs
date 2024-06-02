using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Edam.Data.Asset;
using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Newtonsoft.Json;

namespace Edam.Api.DataApiBuilder
{

   public class EntityBuilder
   {

      /// <summary>
      /// Figure out parameter value based on element data type.
      /// </summary>
      /// <param name="element">element with info to evaluate</param>
      /// <returns>returns the base type enum</returns>
      private object GetParameterValue(AssetDataElement element)
      {
         object value;
         string ldType = element.OriginalDataType.ToLower();
         switch(ldType)
         {
            case "int":
            case "integer":
            case "bigint":
            case "smallint":
            case "money":
            case "float":
            case "double":
            case "decimal":
            case "numeric":
               value = 0; 
               break;
            case "bit":
               value = 0;
               break;
            default:
               value = String.Empty;
               break;
         }
         return value;
      }

      /// <summary>
      /// Prepare an instance of an Entity using given AssetDataElement 
      /// definition.
      /// </summary>
      /// <param name="item">asset data element item</param>
      /// <returns>instance of Entity_ is returned</returns>
      public Entity_? ElementToEntity(AssetDataItem item)
      {

         // Source can be substituted with a string that is the name of the 
         // backend object (i.e. table name) or an instance of EntityItem_
         EntityItem_ eitem = new EntityItem_();
         eitem.Object =
            item.Element.Domain + "." + item.Element.OriginalName;

         switch(item.Element.ElementType)
         {
            case ElementType.type:
               eitem.Type = EntityItemTypeEnum_.Table;
               break;
            case ElementType.view:
               eitem.Type = EntityItemTypeEnum_.View;
               break;
            case ElementType.procedure:
               eitem.Type = EntityItemTypeEnum_.StoredProcedure;
               break;
            default:
               return null;
         }

         // add all parameters (no-implemented-yet)
         if (eitem.Type == EntityItemTypeEnum_.StoredProcedure)
         {
            var p = new EntitySourceParameterMap_();
            foreach(var par in item.Children)
            {
               var pval = GetParameterValue(par);
               p.Add(par.ElementQualifiedName.OriginalName, pval);
            }
            eitem.Parameters = p;
         }

         // add all key fields
         var keys = new List<string>();
         foreach(var child in item.Children)
         {
            if (child.KeyType == Data.Asset.ConstraintType.key)
            {
               keys.Add(child.ElementQualifiedName.OriginalName);
            }
         }

         if (keys.Count > 0)
         {
            eitem.KeyFields = keys;
         }

         // prepare entity instance...
         Entity_ entity = new Entity_();
         entity.Source = eitem;

         if (item.Element.ElementType == ElementType.procedure)
         {
            var restProp = new RestProperties_();
            restProp.Methods = new MethodEmum_();
            restProp.Methods.Add(Anonymous2.Post);
            entity.Rest = restProp;

            var graphProp = new GraphqlProperties_();
            graphProp.Operation = OperationEnum_.Mutation;
            entity.Graphql = graphProp;
         }
         else
         {
            // rest - boolean or restProperties_ the last allows to define 
            // specific a path and specific methods to be supported
            entity.Rest = true;

            // graphql - boolean or graphqlProperties_ the last allowos to
            // define singular/plural names & operations (query/mutation) to be
            // supported
            entity.Graphql = true;
         }

         // specify mappings
         if (item.Element.ElementType != ElementType.procedure)
         {
            StringMap_ stringMap = new StringMap_();
            RelationshipsMap_ rmap = new RelationshipsMap_();
            foreach (var i in item.Children)
            {
               var cname = i.ElementQualifiedName.OriginalName;
               var camelName = Edam.Text.Convert.ToCamelCase(cname, true);
               stringMap.Add(cname, camelName);

               //if (i.OriginalName != i.EntityName)
               //{
               //   stringMap.Add(i.OriginalName, i.EntityName);
               //}

               // specify relationships - for graphql
               if (i.Constraints != null)
               {
                  foreach (var constraint in i.Constraints)
                  {
                     if (constraint.ContraintType ==
                        AssetElementContraintType.ForeignKey)
                     {
                        Relationships_ r = new Relationships_();
                        r.Cardinality = CardinalityEnum_.One;

                        string tname = constraint.ReferenceSchemaName +
                           constraint.ReferenceEntityName;
                        //var refEntityName = Edam.Text.Convert.ToCamelCase(
                        //   constraint.ReferenceSchemaName +
                        //   constraint.ReferenceEntityName, true);

                        r.TargetEntity = tname;
                        rmap.TryAdd(tname, r);
                        //rmap.Add("ref" + refEntityName, r);
                     }
                  }
               }
            }

            if (rmap.Count > 0)
            {
               entity.Relationships = rmap;
            }

            if (stringMap.Count > 0)
            {
               entity.Mappings = stringMap;
            }
         }

         // specify permissions
         var alist = new PermissionActionList_();
         alist.Add("*");

         var perms = new Permission_();
         perms.Role = "anonymous";
         perms.Actions = alist;

         entity.Permissions = new Permissions_();
         entity.Permissions.Add(perms);

         return entity;
      }

      /// <summary>
      /// Asset Data Element list to 
      /// </summary>
      /// <param name="arguments">Processed Arguments that contain the
      /// inteerting Asset Data Element list</param>
      /// <returns>instance of EntitiesMap_ is returned</returns>
      public EntitiesMap_ ElementToEntity(AssetConsoleArgumentsInfo arguments)
      {
         AssetDataElementList items = arguments.AssetDataItems[0].Items;
         EntitiesMap_ entitiesMap = new EntitiesMap_();
         var types = AssetDataElementList.GetTypes(items);
         foreach (AssetDataElement item in types)
         {

            if (item.InclusionType == DataElementInclusionType.Exclude)
            {
               continue;
            }
            if (item.OriginalName == arguments.RootElementName)
            {
               continue;
            }

            if (item.ElementType == ElementType.function)
            {
               continue;
            }

            var dataItem = AssetDataElementList.GetChildren(items, item);
            var oname = dataItem.Element.Domain + dataItem.Element.OriginalName;
            //var ename = Edam.Text.Convert.ToCamelCase(
            //   dataItem.Element.Domain + dataItem.Element.OriginalName, true);
            Entity_? entity_ = ElementToEntity(dataItem);
            if (entity_ != null)
            {
               entitiesMap.Add(oname, ElementToEntity(dataItem));
            }
         }

         return entitiesMap;
      }

      /// <summary>
      /// Get the JSON Text from given type.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public string ToJson(object item)
      {
         return JsonConvert.SerializeObject(
            item, Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
               NullValueHandling = NullValueHandling.Ignore
            });
      }

   }

}
