using System;
using System.Collections.Generic;

using Edam.Data.Asset;
using Edam.Data.Assets.Asset;
using txt = Edam.Text;

namespace Edam.Data.AssetManagement
{

   public enum DataTextElementType
   {
      Unknown = 0,
      BaseType = 1,
      Definition = 2,
      ID = 3,
      No = 4,
      IdValue = 5,
      CodeSet = 6,
      CodeValue = 7,
      ReferenceID = 8,
      ReferenceAndType = 9,
      XHTML = 10,
      SurrogateKey = 11
   }

   /// <summary>
   /// 
   /// </summary>
   /// <remarks>
   /// - As Association - Applicable to collections... when found use 
   ///   instructions in the ElementProperty - ElementTraversing...
   /// - As Element - only a reference id to the entity will be instantated
   /// - As Object - an element gets mapped to a full instantiation of the a
   ///   parent and element object.  For example, an Address element in a 
   ///   (parent) Person object gets instantiated as a full "Person_Address" 
   ///   object and the new object should point back to the parent (Person).
   /// </remarks>
   public enum DataTextElementTraverseAction
   {
      Unknown = 0,
      Ignore = 1,
      InstantiateAsAssociation = 2,
      InstantiateAsElement = 3,
      InstantiateAsObject = 4
   }

   public enum DataTextElementAction
   {
      Unknown = 0,
      Ignore = 1,
      Transform = 2
   }

   public enum DataTextElementTransformTo
   {
      Unknown = 0,
      Ignore = 1,
      ReferenceAndType = 2,
      Reference = 3
   }

   public class DataTextTypeInfo
   {
      public string NamespacePrefix { get; set; }
      public string TypeName { get; set; }
      public string DefaultSize { get; set; }
      public DataTextTypeInfo()
      {

      }
      public DataTextTypeInfo(string name, string size)
      {
         TypeName = name;
         DefaultSize = size;
      }
   }

   public class DataTextTypePatternInfo
   {
      public DataTextTypeInfo Type { get; set; }
      public string NamePattern { get; set; }
      public string TypePattern { get; set; }
   }

   public class DataTextTypeMap
   {
      public DataTextTypeInfo Type { get; set; }
      public List<DataTextTypePatternInfo> Items { get; set; }
      public DataTextTypeMap()
      {
         Type = new DataTextTypeInfo("unknown", "20");
         Items = new List<DataTextTypePatternInfo>();
      }

   }

   public class DataTextElementTransformInfo
   {
      public DataTextElementAction Action { get; set; }
      public DataTextElementTransformTo TransformTo { get; set; }
   }

   public class DataTextElementBaseTypeInfo
   {
      public string Name { get; set; }
      public string NamespacePrefix { get; set; }
      public string TypeName { get; set; }
      public string DataSize { get; set; }
      public string Default { get; set; }
      public string Description { get; set; }
      public DataElementKind Kind { get; set; }
      public bool Nullable { get; set; }
      public bool Identity { get; set; }
   }

   public class DataTextElementInfo
   {
      public string ElementName { get; set; }
      public string NamespacePrefix { get; set; }
      public string FormatText { get; set; }
      public DataTextElementType TypeName { get; set; }
      public DataTextElementTraverseAction TraverseAction { get; set; }
      public DataTextElementBaseTypeInfo BaseType { get; set; }

      public static DataTextElementInfo GetAssociationElementMap(
         IDataElement element)
      {
         DataTextElementInfo mapElement = new DataTextElementInfo
         {
            ElementName = element.ElementName,
            TraverseAction =
            DataTextElementTraverseAction.InstantiateAsAssociation,
            TypeName = DataTextElementType.ReferenceID
         };
         mapElement.BaseType = new DataTextElementBaseTypeInfo();
         mapElement.BaseType.TypeName = String.Empty;
         mapElement.BaseType.DataSize = String.Empty;
         return mapElement;
      }
   }

   public class DataTextElementTraversingInfo
   {
      public bool ArrayToID { get; set; }
      public bool ArrayToAssociation { get; set; }
      public bool Instantiate { get; set; }
      public DataTextElementTraversingInfo()
      {
         ArrayToID = false;
         ArrayToAssociation = false;
         Instantiate = true;
      }
   }

   public class DataTextElementInstanceInfo
   {
      public const string SEPARATOR = "--";
      public const string SLASH = "/";
      public const string UNDERSCORE = "_";
      public const string ASSOCIATION = "Association";

      private IDataElement m_Element = null;
      private string m_ElementName;
      private string m_EntityName;
      private string m_DataTypeName;
      private DataTextElementInfo m_MapElement;

      public DataTextElementInfo MapElement
      {
         get { return m_MapElement; }
      }
      public DataTextElementTraverseAction TraverseAction
      {
         get { return m_MapElement.TraverseAction; }
      }

      public IDataElement Element
      {
         get { return m_Element; }
      }
      public string ElementName
      {
         get { return m_ElementName; }
      }
      public string EntityName
      {
         get { return m_EntityName; }
      }
      public string DataTypeName
      {
         get { return m_DataTypeName; }
      }
      public string Name
      {
         get
         {
            return (
               txt.Convert.ToPascalCase(m_EntityName) + UNDERSCORE + 
               txt.Convert.ToTitleCase(m_ElementName) + UNDERSCORE +
               (TraverseAction == 
                DataTextElementTraverseAction.InstantiateAsAssociation
                  ? ASSOCIATION : m_DataTypeName)).Replace(" ","_");
         }
      }
      public DataTextElementInstanceInfo(IDataElement element,
         DataTextElementInfo mapElement)
      {
         m_Element = element;
         m_ElementName = 
            DataElement.GetUnqualifyText(element.ElementName);
         m_EntityName = 
            DataElement.GetUnqualifyText(element.EntityName);
         m_DataTypeName = 
            DataElement.GetUnqualifyText(element.ElementDataType).
               Replace(" ", "_");
         m_MapElement = mapElement;
      }
   }

   public class DataTextElementPropertyInfo
   {
      public string ElementName { get; set; }
      public DataTextElementInfo DefaultSurragateReference { get; set; }
      public DataTextElementTraversingInfo ElementTraversing { get; set; }
      public List<DataTextElementBaseTypeInfo> PrimaryKey { get; set; }
      public List<DataTextElementBaseTypeInfo> RecordTrackingItem { get; set; }
      public DataTextElementPropertyInfo()
      {
         PrimaryKey = new List<DataTextElementBaseTypeInfo>();
         RecordTrackingItem = new List<DataTextElementBaseTypeInfo>();
         ElementTraversing = new DataTextElementTraversingInfo();
      }
   }

}
