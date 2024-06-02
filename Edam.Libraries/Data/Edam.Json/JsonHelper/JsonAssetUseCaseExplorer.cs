using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// -----------------------------------------------------------------------------
using Edam.TextParse;
using Edam.Data.Asset;
using Edam.Data.AssetSchema;
using Edam.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Edam.Data.AssetConsole;
using Edam.Data.AssetUseCases;
using Edam.Json.JsonSchema;

namespace Edam.Json.JsonHelper
{

    public class JsonAssetUseCase
   {
      private const string CLASS_NAME = "JsonExplore";

      private JObject m_Document;
      private AssetConsoleArgumentsInfo m_Arguments;
      private readonly AssetUseCase m_UseCase;
      private readonly JsonAssetItemInfo m_Asset;
      private readonly ResultLog m_Results = new ResultLog();

      public bool Success
      {
         get { return m_Results.Success; }
      }

      public ResultLog Results
      {
         get { return m_Results; }
      }

      public AssetUseCase UseCase
      {
         get { return m_UseCase; }
      }

      public JsonAssetUseCase(
         AssetConsoleArgumentsInfo arguments, string jsonText)
      {
         m_Arguments = arguments;
         m_Asset = new JsonAssetItemInfo(
            arguments.Namespace, arguments.ProjectVersionId);
         m_Document = JObject.Parse(jsonText, new JsonLoadSettings() { 
            CommentHandling = CommentHandling.Load
         });
         ToDataElements();
         m_UseCase = new AssetUseCase(
            m_Arguments.Namespace, m_Arguments.Project.VersionId);
      }

      private List<Object> GetArrayData(JToken node)
      {
         List<Object> l = new List<object>();
         foreach(var i in node.Children())
         {
            l.Add(i.Value<object>().ToString());
         }
         return l;
      }

      /// <summary>
      /// Content is required to be provided as a list of properties in the
      /// form of "prefix": "URI".
      /// </summary>
      /// <param name="context">content property</param>
      private void ReadContext(JProperty context)
      {
         foreach (var i in context.Children().Children())
         {
            var p = i as JProperty;
            if (p == null)
               continue;
            var prefix = p.Name;
            var uri = p.First.Value<string>();
            m_Asset.Add(new NamespaceInfo(prefix, uri));
         }
      }

      /// <summary>
      /// Parse Processing Instructions....
      /// </summary>
      /// <param name="node">note containing instructions</param>
      /// <returns>AssetProcessInfo instance is returned</returns>
      private AssetProcessInfo ParseProcessingInstruction(JProperty node)
      {
         if (node == null)
         {
            m_UseCase.Instructions.CurrentInstruction = null;
            return null;
         }

         AssetProcessInfo i = new AssetProcessInfo();
         foreach (var c in node.Children().Children())
         {
            var p = c as JProperty;
            if (p == null)
               continue;

            var itm = new AssetProcessItem();
            itm.Value = p.First.Value<string>();
            itm.Column.Name = p.Name;

            switch(p.Name.ToLower())
            {
               case AssetProcess.TK_USECASE:
                  itm.Type = AssetProcessType.UseCaseDeclaration;
                  break;
               case AssetProcess.TK_TYPE:
                  itm.Type = AssetProcessType.Type;
                  break;
               case AssetProcess.TK_TO:
                  itm.Type = AssetProcessType.MapTo;
                  break;
               case AssetProcess.TK_NM_FUNCTION:
                  itm.Type = AssetProcessType.MapFunction;
                  break;
               case AssetProcess.TK_INSTRUCTIONS:
                  itm.Type = AssetProcessType.MapInstruction;
                  break;
               default:
                  itm.Type = AssetProcessType.Custom;
                  break;
            }

            if (itm.Type == AssetProcessType.Type)
            {
               if (itm.Value.ToLower() == AssetProcess.TK_MAP)
               {
                  i.Type = AssetProcessType.Map;
                  continue;
               }
            }
            else if (itm.Type == AssetProcessType.UseCaseDeclaration)
            {
               m_UseCase.Name = itm.Value;
               return null;
            }

            i.Items.Add(itm);
         }

         // check for comments instead of processing instruction
         AssetProcessType tag = AssetProcessType.Tag;
         string tagValue = node.Name;

         if (i.Items.Count == 0)
         {
            if (node.First == null)
            {
               return i;
            }

            // comment found... is not a processing instruction
            tag = AssetProcessType.Comment;
            tagValue = node.First.Value<string>();
         }

         i.UseCaseName = m_UseCase.Name;
         i.Tag = new AssetProcessItem
         {
            Type = tag,
            Value = tagValue
         };

         m_UseCase.Instructions.SetCurrentInstruction(i);
         return i;
      }

      /// <summary>
      /// If any processing instruction is available (preceeds element
      /// declaration) review it and add acordingly based on its Tag Type.
      /// </summary>
      /// <param name="element">element to apply instruction to</param>
      /// <param name="name">name of element</param>
      private void ApplyProcessingInstruction(
         AssetDataElement element, string name)
      {
         // a previous nameless instruction gets assigned to next Element
         if (m_UseCase != null && 
            m_UseCase.Instructions.CurrentInstruction != null &&
            element != null)
         {
            switch (m_UseCase.Instructions.CurrentInstruction.Tag.Type)
            {
               case AssetProcessType.Tag:
                  if (String.IsNullOrWhiteSpace(
                     m_UseCase.Instructions.CurrentInstruction.Tag.Value))
                  {
                     m_UseCase.Instructions.CurrentInstruction.Tag.Value = name;
                  }

                  element.ProcessInstructionsBag =
                     m_UseCase.Instructions.CurrentInstruction;
                  m_UseCase.Instructions.CurrentInstruction.Parent = element;
                  break;
               case AssetProcessType.Comment:
                  element.AddAnnotation(
                     m_UseCase.Instructions.CurrentInstruction.Tag.Value);
                  break;
            }

            m_UseCase.Instructions.CurrentInstruction = null;
         }
      }

      /// <summary>
      /// Go through all nodes recursively...
      /// </summary>
      /// <param name="node">node to visit</param>
      /// <param name="parent">parent node</param>
      /// <returns>added data element</returns>
      private AssetDataElement ReadAllNodes(
         JToken node, AssetDataElement parent)
      {
         AssetDataElement element = null;
         string propertyName = null;
         if (node.Type == JTokenType.Property)
         {
            JProperty prop = node as JProperty;
            propertyName = prop.Name;
            if (prop.Name.StartsWith("@"))
            {
               if (prop.Name.StartsWith("@am:comment"))
               {
                  ParseProcessingInstruction(prop);
               }
               else if (prop.Name.StartsWith("@context"))
               {
                  ReadContext(prop);
               }
               return element;
            }
            element = m_Asset.ToDataElement(prop, parent);
         }

         ApplyProcessingInstruction(element, propertyName);

         var children = node.Children();
         foreach(var c in children)
         {
            switch(c.Type)
            {
               case JTokenType.Array:
                  GetArrayData(c);
                  continue;
               case JTokenType.String:
                  element.DataType = "string";
                  element.ElementPath = c.Path;
                  element.SampleValue = c.Value<object>().ToString();
                  break;
               case JTokenType.Object:
                  
                  break;
               case JTokenType.Property:
                  break;
               default:
                  break;
            }
            ReadAllNodes(c, element);
         }
         return element;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public List<AssetDataElement> ToDataElements()
      {
         string FUNC = CLASS_NAME + "::" + "ToDataElements";

         AssetDataElement element = null;
         JToken t = m_Document.First as JToken;

         try
         {
            do
            {
               element = ReadAllNodes(t, element);
               t = t.Next;
            } while (t != null);
            m_Results.Succeeded();
         }
         catch(Exception ex)
         {
            m_Results.Failed(FUNC, ex);
         }

         if (!m_Results.Success)
            return null;

         return m_Asset.Items;
      }

   }

}
