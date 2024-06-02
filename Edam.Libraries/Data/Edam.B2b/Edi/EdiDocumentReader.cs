using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Edam.Data.AssetConsole;
using Edam.Data.AssetSchema;
using Edam.Diagnostics;

namespace Edam.B2b.Edi
{

   /// <summary>
   /// An EDI defifnition is represented as an EDAM Asset data dictionary and
   /// given such Asset this class prepares the list of all segment loops and
   /// each loop related children.  The reader base is the EdiDocument that
   /// contains the EDI Segements information.
   /// </summary>
   public class EdiDocumentReader : EdiDocument
   {

      [JsonIgnore]
      protected int IndexCounter = 0;

      [JsonIgnore]
      protected int SequenceCounter = 0;

      [JsonIgnore]
      protected ResultLog results = new ResultLog();

      [JsonIgnore]
      public AssetDataItemList Elements { get; set; }

      private string m_CurrentSegment = String.Empty;
      private string m_CurrentParent = String.Empty;

      private Dictionary<string, EdiSegmentInfo> m_Segment =
         new Dictionary<string, EdiSegmentInfo>();

      #region -- 4.00 - Prepare Loops and Tags collections

      /// <summary>
      /// Return true if element is a LOOP.
      /// </summary>
      /// <param name="element">element to investigate</param>
      /// <returns>true is returned if element is a LOOP</returns>
      private bool IsLoop(AssetDataElement element)
      {
         return element.ElementName.Contains("LOOP_");
      }

      private string GetTypeName(string name)
      {
         return name + "_Type";
      }

      /// <summary>
      /// Add Segment.
      /// </summary>
      /// <param name="item">Item whose TAG needs registration</param>
      /// <returns>created Segment info is returned</returns>
      private EdiSegmentInfo AddSegment(AssetDataElement item)
      {
         EdiSegmentInfo segment = new EdiSegmentInfo
         {
            Index = IndexCounter,
            Name = item.RealName,
            ElementPath = item.AlternateName,
            QualifiedName = item.ElementQualifiedName,
            SegmentId = item.ElementQualifiedName.OriginalName,
            Loop = m_CurrentSegment,
            LoopParent = m_CurrentParent,
            DataType = item.TypeQualifiedName.OriginalName,

            MinLength = item.MinLength,
            MaxLength = item.MaxLength,

            MinOccurrence = item.MinOccurrence,
            MaxOccurrence = item.MaxOccurrence,

            IsLoop = IsLoop(item)
         };
         IndexCounter++;

         if (segment.IsLoop)
         {
            segment.SequenceNo = SequenceCounter;
         }

         // Setup Lopp info
         if (!String.IsNullOrWhiteSpace(item.Tags))
         {
            string[] l = item.Tags.Split("_");
            segment.Loop = l[1];
            segment.LoopParent = l[2];

            if (segment.Loop.IndexOf('-') >= 0)
            {
               segment.Loop = segment.Loop.Replace("-T", "");
               segment.IsTrigger = true;
            }
         }
         else if (IndexCounter != 1)
         {

         }

         // get parent GUID
         if (m_Segment.TryGetValue(segment.LoopParent, out EdiSegmentInfo parent))
         {
            segment.ParentGuid = parent.Guid;
         }

         // Setup other Stuff
         segment.SetCodes(item.SampleValue);

         if (segment.IsLoop)
         {
            //string[] l = segment.SegmentId.Split("_");
            //segment.SegmentId = l[1];
            //segment.Segment = l[1];
            //segment.Parent = l[2];

            segment.SegmentId = segment.Loop;
            m_CurrentParent = segment.LoopParent;
            m_CurrentSegment = segment.Loop;

            if (!m_Segment.TryGetValue(segment.Loop, out EdiSegmentInfo val))
            {
               m_Segment.Add(segment.Loop, segment);
            }
         }

         return segment;
      }

      /// <summary>
      /// Get the type segment children.
      /// </summary>
      /// <param name="typeName">type name</param>
      /// <param name="parentTag"> parent Tag</param>
      /// <param name="parentGuid">parent GUID</param>
      private List<EdiSegmentInfo> GetTypeSegmentChildren(
         string typeName, string parentTag, string parentGuid)
      {
         List<EdiSegmentInfo> children = new List<EdiSegmentInfo>();
         var items = Elements.Find((x) => x.Element.ElementName == typeName);
         if (items != null)
         {
            int sno = 0;
            foreach (var item in items.Children)
            {
               var tag = AddSegment(item);
               tag.SequenceNo = sno;
               tag.SegmentParent = parentTag;
               children.Add(tag);
               sno++;
            }
         }
         return children;
      }

      /// <summary>
      /// Process item by traversing through its structure.
      /// </summary>
      /// <param name="item">item to investigate</param>
      /// <param name="parent">parent TAG</param>
      private void ProcessItem(AssetDataItem item, EdiSegmentInfo parent)
      {
         var parentSegment = AddSegment(item.Element);
         Items.Add(parentSegment);
         parentSegment.SegmentParent = 
            parent == null ? String.Empty : 
               (parent.IsLoop ? parent.DataType : parent.SegmentId);

         foreach (var child in item.Children)
         {
            if (IsLoop(child))
            {
               var childItem = Elements.Find((x) =>
                  x.Element.ElementName == GetTypeName(child.ElementName));

               if (childItem == null)
               {
                  results.Failed(
                     "Expected (" + child.ElementName + ") but not found");
                  return;
               }

               ProcessItem(childItem, parentSegment);
            }
            else
            {
               var segment = AddSegment(child);
               Items.Add(segment);
               segment.IsLoop = false;
               segment.SegmentParent = parentSegment.SegmentId;

               segment.Children = 
                  GetTypeSegmentChildren(GetTypeName(
                     child.ElementName), child.ElementName, parentSegment.Guid);
            }
         }
      }

      /// <summary>
      /// Prepare EDI element look-up / tag catalog that will be converted into
      /// an XML or JSON file.
      /// </summary>
      /// <param name="arguments"></param>
      public ResultLog PrepareEdiInfo(AssetConsoleArgumentsInfo arguments)
      {
         if (arguments == null || arguments.AssetDataItems.Count == 0)
         {
            results.Failed(
               "Arguments expected but not found, or no Data Items found");
            return results;
         }

         Items = new EdiSegmentList();

         foreach (var a in arguments.AssetDataItems)
         {
            Elements = AssetDataItemList.ToDataItems(a.Items);
            var rootElement = Elements.Find((x) =>
               x.Element.ElementName == GetTypeName(arguments.RootElementName));
            if (rootElement == null)
            {
               results.Failed("No root element found!");
               return results;
            }

            ProcessItem(rootElement, null);
         }

         // finally assign sequence number to root entries
         foreach(var item in this.Items)
         {
            item.SequenceNo = SequenceCounter;
            SequenceCounter++;
         }

         results.ResultValueObject = this;
         results.Succeeded();
         return results;
      }

      /// <summary>
      /// Using the Asset Data Element List withing arguments prepare its 
      /// corresponding TAG - LOOP document.
      /// </summary>
      /// <param name="arguments">arguments needed to prepare EDI TAGs and
      /// Loops based Document</param>
      /// <returns>The </returns>
      public static ResultLog ToDocument(AssetConsoleArgumentsInfo arguments)
      {
         EdiDocumentReader item = new EdiDocumentReader();
         return item.PrepareEdiInfo(arguments);
      }

      #endregion

   }

}
