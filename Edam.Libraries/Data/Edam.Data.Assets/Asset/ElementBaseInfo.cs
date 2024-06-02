using System;
using System.Collections.Generic;
using System.Text;

// -----------------------------------------------------------------------------

namespace Edam.Data.Asset
{

   public enum ElementResolutionResult
   {
      Unknown = 0,
      Found = 1,
      Added = 2,
      Duplicate = 3,
      NotFound = 4
   }

   /// <summary>
   /// Provides base information about an element.
   /// </summary>
   public class ElementBaseInfo
   {

      protected BaseGrammarType m_GrammarType = BaseGrammarType.Unknown;
      protected IDataElement m_BaseElement;
      protected ElementBaseTypeInfo m_ElementTypeInfo;
      protected readonly String m_TypeNameSuffix;
      
      public DataTypeInfo DataType { get; set; }
      public Int16 ResolvedCount { get; set; }
      public NamespaceInfo Namespace { get; set; }
      public IDataElement BaseElement
      {
         get { return m_BaseElement; }
      }
      public String DataTypeName
      {
         get { return GetTypeName(BaseElement); }
      }
      public QualifiedNameInfo DataTypeQualifiedName
      {
         get { return GetTypeQualifiedName(BaseElement); }
      }
      public ElementBaseType ElementType
      {
         get { return m_ElementTypeInfo.Type; }
      }

      public bool IsBase
      {
         get { return m_ElementTypeInfo.Type == ElementBaseType.Base; }
      }

      public bool IsElement
      {
         get { return m_ElementTypeInfo.Type == ElementBaseType.Element;  }
      }

      public bool IsType
      {
         get { return m_ElementTypeInfo.Type == ElementBaseType.Type; }
      }

      public bool IsObject
      {
         get { return m_ElementTypeInfo.Type == ElementBaseType.Object; }
      }

      public bool IsAttribute
      {
         get { return m_ElementTypeInfo.Type == ElementBaseType.Attribute; }
      }

      public bool IsKey
      {
         get { return m_ElementTypeInfo.Constraint == ConstraintType.key; }
      }

      public ElementResolutionResult ResolutionResult { get; set; }

      /// <summary>
      /// Initialize information about an Element type.
      /// </summary>
      /// <param name="baseElement">base element</param>
      /// <param name="ns">namespace information</param>
      /// <param name="typeNameSuffix"></param>
      public ElementBaseInfo(
         IDataElement baseElement, NamespaceInfo ns = null,
         String typeNameSuffix = null)
      {
         m_BaseElement = baseElement;
         Namespace = ns;
         SetElementType(baseElement.ElementType);
         m_TypeNameSuffix = typeNameSuffix ?? "";
         ResolutionResult = ElementResolutionResult.Unknown;
         ResolvedCount = 0;
      }

      public ElementBaseInfo(QualifiedNameInfo name, ElementBaseType type)
      {
         m_BaseElement = null;
         Namespace = new NamespaceInfo(name.Prefix, NamespaceInfo.W3C_URI);
         m_ElementTypeInfo = new ElementBaseTypeInfo(m_GrammarType);
         m_ElementTypeInfo.Type = type;
         m_ElementTypeInfo.TypeName = name.OriginalName;
         m_TypeNameSuffix = String.Empty;
         ResolutionResult = ElementResolutionResult.Unknown;
         ResolvedCount = 0;
      }

      public void SetElementType(String elementTypeText)
      {
         var i = String.IsNullOrWhiteSpace(elementTypeText) ?
            new ElementBaseTypeInfo(m_GrammarType) : 
               ElementBaseTypeInfo.GetElementTypeInfo(
                  elementTypeText, m_GrammarType);
         m_ElementTypeInfo = i;
      }

      public void SetElementType(ElementType type)
      {
         var i = ElementBaseTypeInfo.GetElementTypeInfo(type, m_GrammarType);
         m_ElementTypeInfo = i;
      }

      public static Boolean IsBaseElementType(IDataElement element)
      {
         if (element == null)
         {
            return false;
         }
         return (ElementBaseTypeInfo.IsBase(element.TypeName));
      }

      public static Boolean IsBaseElementType(
         IDataElement element, BaseGrammarType grammarType)
      {
         if (element == null)
         {
            return false;
         }
         return (ElementBaseTypeInfo.IsBase(
            element.ElementDataType, grammarType));
      }

      public static Boolean IsBaseObjectType(
         IDataElement element, BaseGrammarType grammarType)
      {
         if (element == null)
         {
            return false;
         }
         return (ElementBaseTypeInfo.IsObject(
            element.ElementDataType, grammarType));
      }

      public static Boolean IsOptionOne(IDataElement element)
      {
         return (element.ElementGroup == ElementGroup.OptionOne);
      }

      public String GetTypeName(IDataElement element)
      {
         if (element == null)
         {
            return m_ElementTypeInfo.TypeName;
         }

         if (m_ElementTypeInfo.Type == ElementBaseType.Base)
            return element.Type;
         if (m_ElementTypeInfo.Type == ElementBaseType.Unknown)
            return null;
         return element.ElementName;
      }

      public QualifiedNameInfo GetTypeQualifiedName(IDataElement element)
      {
         if (element == null)
         {
            return new QualifiedNameInfo(
               String.Empty, m_ElementTypeInfo.TypeName);
         }

         if (m_ElementTypeInfo.Type == ElementBaseType.Base)
            return element.TypeQualifiedName;
         if (m_ElementTypeInfo.Type == ElementBaseType.Unknown)
            return null;
         return element.ElementQualifiedName;
      }

      /// <summary>
      /// Given a data-element find out if it is a key element....
      /// </summary>
      /// <param name="element"></param>
      /// <returns></returns>
      public static Boolean IsKeyElement(IDataElement element)
      {
         return element.KeyType == ConstraintType.key;
      }

   }

}
