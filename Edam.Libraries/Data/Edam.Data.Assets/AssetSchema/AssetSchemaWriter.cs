using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Application;
using Edam.Data.AssetManagement;

namespace Edam.Data.AssetSchema
{

   /// <summary>
   /// Asset Schema Writer... provides support for Element (parent / child)
   /// instantiation, mappings and the output of schema definitions...
   /// </summary>
   public class AssetSchemaWriter: IResourceCallHelpers
   {

      #region -- Static Strings...

      protected const String DEFAULT_SIZE = "128";

      #endregion
      #region -- Properties and Declarations...

      private string m_DefaultBaseType;

      public NamespaceInfo Namespace { get; set; }

      public List<NamespaceInfo> Namespaces { get; } =
         new List<NamespaceInfo>();

      protected static int m_AssociationCounter = 0;
      protected DataTextMap m_Mapper = null;
      protected DataTextElementPropertyInfo m_MapDefaultProperty = null;

      public ElementTransform ElementTypeTransform { get; set; }

      #endregion
      #region -- Resolvers

      // delegates...
      public DataTextMapResolverDelegate DataTextMapResolver { get; set; }
      public DataTextTypeMapResolverDelegate DataTextTypeResolver { get; set; }
      public BaseTypeResolverDelegate DataBaseTypeResolver { get; set; }
      public NamespaceResolverDelegate DataNamespaceResolver { get; set; }
      public TypeResolverDelegate DataTypeResolver { get; set; }
      public DomainResolverDelegate DataDomainResolver { get; set; }
      public DataTransformResolverDelegate DataTransformResolver { get; set; }
      public ElementTypeResolverDelegate ElementTypeResolver { get; set; }

      // call backs...
      public Func<IDataElement, List<IDataElement>>
         DataFindKeyCandidates
      { get; set; }
      public Func<IDataElement, List<IDataElement>, String, List<IDataElement>>
         DataGetChildrenOfChildren
      { get; set; }

      #endregion
      #region -- Constructor, Destructor...

      public AssetSchemaWriter(string defaultBaseType)
      {
         m_DefaultBaseType = defaultBaseType;
      }

      #endregion
      #region -- Namespace Support

      public void AddNamespace(NamespaceInfo ns)
      {
         if (ns == null)
            return;
         NamespaceInfo i = Namespaces.Find((x) => x.Prefix == ns.Prefix);
         if (i == null)
            Namespaces.Add(ns);
      }

      public void Add(string prefix, string uri)
      {
         NamespaceInfo ns = new NamespaceInfo(prefix, uri);
         if (Namespace.Prefix != prefix)
            AddNamespace(ns);
      }

      #endregion
      #region -- Virtual Output Methods...

      protected virtual void OutputColumnDefinition(
         StringBuilder builder, IDataElement element,
         string typeName, DataTextElementInfo mapElement, string tail)
      {
      }

      protected virtual void OutputComment(StringBuilder builder, string comment)
      {
      }

      protected virtual void OutputColumnDefinition(StringBuilder builder,
         IDataElement element, String tail)
      {
      }

      protected virtual void OutputColumnDefinition(StringBuilder builder,
         IDataElement element, String tail, ConstraintType constraint)
      {
      }

      protected virtual void OutputConstraintDefinition(StringBuilder builder,
         string constraintName, string keyName, string foreignName,
         string foreignKeyName, string tail = ",")
      {
      }

      #endregion
      #region -- Types Discovery and Definition Support

      /// <summary>
      /// Get type declaration.
      /// </summary>
      /// <param name="element">data element</param>
      /// <param name="dataType">data type</param>
      /// <param name="dataSize">data size</param>
      /// <returns>type DataTextElementInfo instance is returned</returns>
      protected object GetTypeDeclaration(IDataElement element,
         String dataType, String dataSize)
      {
         DataTextTypeInfo type = new DataTextTypeInfo(
            m_DefaultBaseType, DEFAULT_SIZE);

         // first check parent...
         object delement = null;
         if (DataTextMapResolver != null)
         {
            delement = DataTextMapResolver(element.ElementName);
            var mapItem = delement as DataTextMapItem;
            if (delement == null || 
               (mapItem != null && mapItem.TargetText == null))
            {
               // map base type to target DDL type
               delement = DataTextMapResolver(dataType);
            }
         }

         return delement;
      }

      /// <summary>
      /// Get additional properties related to a resource name.
      /// </summary>
      /// <param name="resourceName"></param>
      /// <returns></returns>
      protected DataTextElementPropertyInfo GetAdditionalProperties(
         string resourceName)
      {
         DataTextElementPropertyInfo property =
            m_Mapper.FindElementProperty(resourceName);
         if (property == null)
         {
            property = m_MapDefaultProperty;
         }
         if (property == null)
         {
            return null;
         }
         return property;
      }

      /// <summary>
      /// Search for Map Element information for the element and related type 
      /// and output definitions based on element details.
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="element">element to consider</param>
      /// <param name="constraints">when instanced as an element the element
      /// is added to the constraints list that need consideration</param>
      /// <param name="tail">last element... if so output a blank else a comma
      /// </param>
      /// <returns></returns>
      protected bool CheckMapElement(StringBuilder builder,
         IDataElement element, List<DataTextElementInstanceInfo> constraints,
         List<DataTextElementInstanceInfo> objects, string tail)
      {
         // map the Element by name
         DataTextElementInfo mapElement =
            m_Mapper.FindElement(element.ElementName);
         if (mapElement != null && mapElement.TraverseAction ==
            DataTextElementTraverseAction.Ignore)
         {
            OutputColumnDefinition(builder, element,
               element.ElementDataType, mapElement, tail);
            return true;
         }

         // map the Element Data Type using its name
         DataTextElementInfo mapType =
            m_Mapper.FindElement(element.ElementDataType);
         if (mapType != null)
         {
            // ignore is the same as TraverseAction AsElement (see below)
            if (mapType.TraverseAction == DataTextElementTraverseAction.Ignore)
            {
               OutputColumnDefinition(builder, element,
                  element.ElementDataType, mapType, tail);
               return true;
            }

            // AsElement will instantate an element id reference and adds it
            // to the constraints list (like an FK)
            else if (mapType.TraverseAction ==
               DataTextElementTraverseAction.InstantiateAsElement)
            {
               constraints.Add(
                  new DataTextElementInstanceInfo(element, mapType));
               OutputColumnDefinition(builder, element,
                  element.ElementDataType, mapType, tail);
               return true;
            }

            // AsObject will add the element to the Objects list to later 
            // instantiate a full object pointing back to the parent
            else if (mapType.TraverseAction ==
               DataTextElementTraverseAction.InstantiateAsObject)
            {
               OutputComment(builder, "Element (" + element.ElementName +
                  ") of Type (" + element.ElementDataType + ")");
               objects.Add(
                  new DataTextElementInstanceInfo(element, mapType));
               return true;
            }
         }

         return false;
      }

      #endregion
      #region -- Define State Management...

      /// <summary>
      /// Runtime environment to help in the type system walkthrough and ouput
      /// generation.
      /// </summary>
      protected class OutputState
      {
         /// <summary>
         /// Name of the driving resource that is being examined or instantiated
         /// </summary>
         public string ResourceName { get; set; }

         /// <summary>
         /// Object related to a Parent Object are those that reference back to
         /// its parent.  If the relationship don't exist the instancing method
         /// should create such reference.
         /// </summary>
         public IDataElement ParentObject { get; set; }

         /// <summary>
         /// Builder is used to buffer the output as is being prepared.
         /// </summary>
         public StringBuilder Builder { get; set; }

         /// <summary>
         /// Log or console output to trace the execution of the process...
         /// </summary>
         public ApplicationLog Console { get; set; }

         /// <summary>
         /// Resource/asset or children...
         /// </summary>
         public AssetDataElementList Items { get; set; }

         /// <summary>
         /// Primary keys or collection (list) of id's / references that as a
         /// collection establishes the uniqueness of the object.
         /// </summary>
         public List<IDataElement> Keys { get; set; }

         /// <summary>
         /// List of additional properties that should be instantiated for
         /// every object/entity that is outputed.
         /// </summary>
         public DataTextElementPropertyInfo AdditionalProperties { get; set; }

         /// <summary>
         /// List of needed constraints that may or not require a related 
         /// association object/entity.
         /// </summary>
         public List<DataTextElementInstanceInfo> Constraints { get; set; }

         /// <summary>
         /// Foreign key constraints from tables
         /// </summary>
         public AssetElementConstraintList ConstraintItems { get; set; } = 
            new AssetElementConstraintList();

         /// <summary>
         /// An object is such entity that requires the (child) object to 
         /// reference back to its parent (see ParentObject).
         /// </summary>
         public List<DataTextElementInstanceInfo> Objects { get; set; }

         public bool HasContraints
         {
            get { return Constraints.Count > 0; }
         }

         /// <summary>
         /// State constructor...
         /// </summary>
         /// <param name="resourceName">name of the resource in consideration
         /// </param>
         /// <param name="properties">collection of additional properties to 
         /// instantiate</param>
         /// <param name="assets">collection of resource children</param>
         public OutputState(string resourceName,
            DataTextElementPropertyInfo properties,
            AssetDataElementList assets)
         {
            ResourceName = resourceName;
            ParentObject = null;
            Items = assets;

            // get additional properties...
            AdditionalProperties = AdditionalProperties;

            // prepare constraints/objects resources...
            Constraints = new List<DataTextElementInstanceInfo>();
            Objects = new List<DataTextElementInstanceInfo>();

            // prepare resource to gather keys...
            Keys = new List<IDataElement>();
         }
      }

      #endregion
      #region -- Assets Output Helpers...

      /// <summary>
      /// Go through the Assets list and output corresponding definitions 
      /// according to the element map directives.
      /// </summary>
      /// <param name="state">runtime variables</param>
      protected void OutputAsset(OutputState state)
      {
         // scan each item and output column definitions based on the type of
         // required transformation (i.e. based on element-key, or anscetry (
         // children of children)...
         int c = 0;
         int b = 0;
         int ccnt = state.Items.Count - 1;
         int bcnt;
         string tail = String.Empty;
         string btail = String.Empty;
         List<IDataElement> assetChildren = null;
         int tot = state.Items.Count;

         // output all columns
         bool isKey;
         int keyCount = 0;
         int totalCnt = state.Items.Count;
         string etail = String.Empty;
         foreach (var asset in state.Items)
         {
            state.ParentObject = asset;
            isKey = false;
            // add to key if it is so...
            if (asset.KeyType == ConstraintType.key)
            {
               state.Keys.Add(asset);
               isKey = true;
               keyCount++;
            }

            // setup tail...
            tail = (state.AdditionalProperties != null ||
               (state.Keys == null || state.Keys.Count == 0) ||
               c < ccnt || state.Keys.Count > 0) ? "," : String.Empty;
            c++;

            // get Element Map instance or default...
            DataTextElementPropertyInfo mapProperties =
               m_Mapper.FindElementProperty(asset.ElementName);
            if (mapProperties == null)
            {
               mapProperties = m_MapDefaultProperty;
            }

            // check cardinality, if more than one register fk and output now
            if (mapProperties != null && 
               !mapProperties.ElementTraversing.Instantiate &&
               asset.MaxOccurrence > 1)
            {
               DataTextElementInfo mapElement =
                  DataTextElementInfo.GetAssociationElementMap(asset);
               state.Constraints.Add(new DataTextElementInstanceInfo(asset,
                  mapElement));
               OutputColumnDefinition(state.Builder, asset,
                  asset.ElementDataType,
                  mapProperties.DefaultSurragateReference, tail);
               continue;
            }

            // is there is any Map Element resource available?
            if (CheckMapElement(
               state.Builder, asset, state.Constraints, state.Objects, tail))
            {
               continue;
            }

            // base on the element-type-transform decide to insert data-type
            // keys or inherited children...
            bool isInstaced = false;

            switch (ElementTypeTransform)
            {
               case ElementTransform.InstanceKeys:
                  if (DataFindKeyCandidates != null)
                  {
                     assetChildren = DataFindKeyCandidates(asset);
                  }
                  break;
               case ElementTransform.InstanceChildren:
                  if (DataGetChildrenOfChildren != null)
                  {
                     assetChildren =
                        DataGetChildrenOfChildren(asset, null, null);
                     isInstaced = true;
                  }
                  break;
               default:
                  assetChildren = null;
                  break;
            }

            // output column definition for each asset child element...
            if (assetChildren != null && assetChildren.Count != 0)
            {
               b = 0;
               bcnt = assetChildren.Count - 1;

               foreach (var i in assetChildren)
               {
                  // prepare tail
                  btail = (state.AdditionalProperties != null || b < bcnt) ?
                     "," : tail;
                  b++;

                  // is there is any Map Element resource available?
                  if (CheckMapElement(
                     state.Builder, i, state.Constraints, state.Objects, btail))
                  {
                     continue;
                  }

                  // prepare column
                  if (isInstaced && i.KeyType == ConstraintType.key &&
                     i.AutoGenerateType == ConstraintType.autoGenerate)
                  {
                     OutputColumnDefinition(state.Builder, i, btail, 
                        ConstraintType.autoGenerate);
                  }
                  else if (isInstaced && i.KeyType == ConstraintType.key)
                  {
                     OutputColumnDefinition(state.Builder, i, btail);
                  }
                  else
                  {
                     OutputColumnDefinition(
                        state.Builder, i, btail, ConstraintType.nonkey);
                  }
               }
               continue;
            }

            // is a base-type so, create a type column
            if (isKey)
               OutputColumnDefinition(state.Builder, asset, tail,
                  asset.AutoGenerateType);
            else
               OutputColumnDefinition(state.Builder, asset, tail);

            // add constraints
            foreach(var cons in asset.Constraints)
            {
               cons.ParentName = state.ResourceName;
               cons.ElementName = asset.OriginalName;
            }
            state.ConstraintItems.AddRange(asset.Constraints);
         }
#if DEBUG_
         state.Console.WriteLine(
            "Added: " + state.ResourceName + " (" + c.ToString() + ")");
#endif
      }

      #endregion
      #region -- Resource Output Helpers...

      /// <summary>
      /// Check if the resource needs to be ignored...
      /// </summary>
      /// <param name="resourceName">resource name</param>
      /// <returns>true if it should be ignored</returns>
      protected bool IgnoreResourceElement(string resourceName)
      {
         // is there is any Map Element resource available?
         DataTextElementInfo resourceElement =
            m_Mapper.FindElement(resourceName);
         return (resourceElement != null && resourceElement.TraverseAction ==
            DataTextElementTraverseAction.Ignore);
      }

      #endregion

   }

}
