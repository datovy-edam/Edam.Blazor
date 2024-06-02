using System;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Data.AssetSchema;

namespace Edam.Data.AssetUseCases
{

   public class AssetUseCaseReconciliator
   {
      private const string CLASS_NAME = "AssetUseCaseReconciliator";
      private const string INVALID_SCHEMA = "Invalid Schema";

      private readonly Dictionary<string, AssetDataElement> m_PropertyVisited =
         new Dictionary<string, AssetDataElement>();
      //private readonly Dictionary<string, IAsset> m_TypeVisited =
      //   new Dictionary<string, IAsset>();

      private readonly List<AssetDataElement> m_Assets;
      private readonly List<AssetDataElement> m_Properties;

      public AssetUseCaseReconciliator(
         List<AssetDataElement> assets, List<AssetDataElement> properties)
      {
         m_Assets = assets;
         m_Properties = properties;
      }

      private IAssetElement GetElement(string entityQualifiedNameText,
         string elementQualifiedNameText)
      {
         var prop = GetProperty(entityQualifiedNameText);
         if (prop == null)
            return null;

         // try to find element declaration
         List<AssetDataElement> tlist = m_Assets.Where(
            s =>
               s.EntityQualifiedNameText == prop.DataType &&
               s.ElementQualifiedNameText == elementQualifiedNameText)
            .Select(s => s).ToList();

         // find asset... if found more than one something is wrong...
         if (tlist.Count() < 1)
            return null;

         IAssetElement type = tlist[0] as IAssetElement;

         return type;
      }

      private IAssetElement GetTypeElement(string entityQualifiedNameText,
         string elementQualifiedNameText)
      {
         List<AssetDataElement> tlist = null;

         // try to find element declaration
         tlist = m_Properties.Where(
            s => s.EntityQualifiedName != null &&
               s.EntityQualifiedNameText == entityQualifiedNameText &&
               s.ElementQualifiedNameText == elementQualifiedNameText)
            .Select(s => s).ToList();

         // find asset... if found more than one something is wrong...
         if (tlist.Count() < 1)
            return null;

         IAssetElement type = tlist[0] as IAssetElement;

         return type;
      }

      private AssetDataElement GetType(string entityQualifiedNameText,
         string elementQualifiedNameText)
      {
         List<AssetDataElement> tlist = null;

         // try to find element declaration
         tlist = m_Assets.Where(
            s => s.EntityQualifiedName != null &&
               s.EntityQualifiedNameText == entityQualifiedNameText &&
               s.ElementQualifiedNameText == elementQualifiedNameText)
            .Select(s => s).ToList();

         // find asset... if found more than one something is wrong...
         if (tlist.Count() < 1)
            return null;

         AssetDataElement type = tlist[0] as AssetDataElement;

         return type;
      }

      private AssetDataElement GetType(string elementQualifiedNameText)
      {
         List<AssetDataElement> tlist = null;

         // try to find element declaration
         tlist = m_Assets.Where(
            s => s.ElementQualifiedNameText == elementQualifiedNameText)
            .Select(s => s).ToList();

         // find asset... if found more than one something is wrong...
         if (tlist.Count() < 1)
            return null;

         AssetDataElement type = tlist[0] as AssetDataElement;

         return type;
      }

      private AssetDataElement GetProperty(string qualifyNameText)
      {
         List<AssetDataElement> proplist = null;
         if (string.IsNullOrWhiteSpace(qualifyNameText))
         {
            return null;
         }

         AssetDataElement entry = null;
         var qname = new QualifiedNameInfo(qualifyNameText);
         if (!m_PropertyVisited.TryGetValue(
            qualifyNameText, out entry))
         {
            // try to find element declaration
            proplist = m_Properties.Where(
               s => s.ElementQualifiedName.Name == qualifyNameText)
               .Select(s => s).ToList();

            // find property... if found more than one something is wrong...
            if (proplist.Count() >= 1)
            {
               entry = proplist[0] as AssetDataElement;
               m_PropertyVisited.Add(entry.ElementQualifiedName.Name, entry);
               return entry;
            }
         }

         if (entry != null)
            return entry;

         if (!m_PropertyVisited.TryGetValue(
            qname.OriginalName, out entry))
         {
            // try to find element declaration
            proplist = m_Properties.Where(
               s => s.ElementQualifiedName.Name == qualifyNameText)
               .Select(s => s).ToList();

            // find property... if found more than one something is wrong...
            if (proplist.Count() < 1)
               return null;

            foreach (var e in proplist)
            {
               var p = e as AssetDataElement;
               if (p != null)
               {
                  if (p.ElementQualifiedName.Name == qualifyNameText)
                  {
                     entry = p;
                     m_PropertyVisited.Add(p.ElementQualifiedName.Name, p);
                     break;
                  }
               }
            }
            return entry;
         }

         return entry;
      }

      private string GetEntityElement(string entityQualifiedNameText)
      {
         IAssetElement property = GetProperty(entityQualifiedNameText);
         return property == null ? entityQualifiedNameText :
            property.DataType;
      }

      /// <summary>
      /// Get type information specified in the element definition within a
      /// complex type...
      /// </summary>
      /// <param name="entityQualifiedNameText"></param>
      /// <returns></returns>
      private string GetTypeName(string entityQualifiedNameText)
      {
         string typeName;
         // fetch ... register visited property
         IAssetElement property = GetProperty(entityQualifiedNameText);
         if (property == null)
         {
            typeName =
               GetEntityElement(entityQualifiedNameText);
         }
         else
         {
            typeName = property.DataType;
         }
         return typeName;
      }

      private void ProcessType(AssetDataElement element)
      {
         // fetch ... register visited property
         IAssetElement property = GetProperty(element.ElementQualifiedName.Name);

         // if not found try finding a Entity - Element declaration...
         if (property == null)
         {
            var entityQNameText =
               GetEntityElement(element.EntityQualifiedNameText);
            property = GetType(
               entityQNameText, element.ElementQualifiedNameText);
         }

         if (property != null)
         {
            element.DataType = property.DataType;
            element.SetOccurance(property.Occurs);
         }
         //else
         //{
         //   //return;
         //   throw new Exception(CLASS_NAME + ": (" +
         //      element.EntityQualifiedNameText + " - " +
         //      element.ElementQualifiedNameText + ") " + INVALID_SCHEMA);
         //}
      }

      private void ProcessElement(AssetDataElement element)
      {
         //if (element.IsAttribute)
         //   return;

         IAssetElement aelement = GetElement(element.EntityQualifiedNameText,
            element.ElementQualifiedNameText);
         if (aelement == null)
         {
            var etype = GetProperty(element.EntityQualifiedNameText);
            if (etype != null)
               aelement = GetTypeElement(
                  etype.DataType, element.ElementQualifiedNameText);
         }
         if (aelement == null)
         {
            QualifiedNameInfo qn =
               ElementBaseTypeInfo.GetBaseType(element.DataType);
            if (qn != null && qn.OriginalName == element.DataType)
            {
               element.DataType = qn.Name;
            }
            return;
            //throw new Exception(CLASS_NAME + ": (" +
            //   element.EntityQualifiedNameText + " - " +
            //   element.ElementQualifiedNameText + ") " + INVALID_SCHEMA);
         }
         element.DataType = aelement.DataType;
         element.SetOccurance(aelement.Occurs);
      }

      /// <summary>
      /// Reconcile by going through all Use Cases and 
      /// </summary>
      /// <param name="useCases">list of Use Cases to reconciliate</param>
      public void Reconcile(List<AssetUseCase> useCases)
      {
         if (useCases == null)
         {
            useCases = new List<AssetUseCase>();
         }

         foreach (var uc in useCases)
         {
            foreach (var item in uc.Items)
            {
               if (item.IsType)
               {
                  ProcessType(item);
               }
               else
               {
                  ProcessElement(item);
               }

            }
         }
      }

   }

}
