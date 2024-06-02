using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.Asset;
using Edam.Diagnostics;

namespace Edam.B2b.Edi
{

   public class EdiSegmentInfo
   {

      public string Guid { get; set; } = System.Guid.NewGuid().ToString();
      public string ParentGuid { get; set; }
      public string LoopParentGuid { get; set; }

      public int SequenceNo { get; set; }
      public int Index { get; set; }

      public string SegmentId { get; set; }
      public string Loop { get; set; }
      public string LoopParent { get; set; }
      public string Name { get; set; }

      /// <summary>
      /// true when the segment represent a trigger marking the need for a UID
      /// </summary>
      public bool IsTrigger { get; set; } = false;

      /// <summary>
      /// Correspond to ako database layout: schema/table/column/link, see 
      /// related AssetDataElement -> AlternateName that will hold the path.
      /// </summary>
      public string ElementPath { get; set; }

      public string DataType { get; set; }

      /// <summary>
      /// List of valid codes
      /// </summary>
      public List<string> Codes { get; set; } = new List<string>();

      public string ValueText { get; set; }

      /// <summary>
      /// True if skipped
      /// </summary>
      public bool Skipped { get; set; } = false;

      public int? MinLength { get; set; } = 0;
      public int? MaxLength { get; set; } = 0;

      public int? MinOccurrence { get; set; } = 0;
      public int? MaxOccurrence { get; set; } = 0;

      public QualifiedNameInfo QualifiedName { get; set; }
      public bool IsLoop { get; set; }

      public string SegmentParent { get; set; }

      public List<EdiSegmentInfo> Children { get; set; } = 
         new List<EdiSegmentInfo>();

      /// <summary>
      /// Seg Code Values.
      /// </summary>
      /// <param name="codes"></param>
      public void SetCodes(string codes)
      {
         if (String.IsNullOrWhiteSpace(codes))
         {
            return;
         }

         Codes.Clear();
         string[] l = codes.Split(" ");
         foreach(string c in l)
         {
            Codes.Add(c.Trim());
         }
      }

      /// <summary>
      /// Return true if given code is in the valid codes list.
      /// </summary>
      /// <param name="code">code to search</param>
      /// <returns></returns>
      public bool HasCode(string code)
      {
         return Codes.Contains(code);
      }

      #region -- 4.00 - Copy and Duplicate methods...

      /// <summary>
      /// Copy given segment into a segment of this instance.
      /// </summary>
      /// <param name="segment">segment to be copied</param>
      public void Copy(EdiSegmentInfo segment)
      {
         Guid = segment.Guid;
         ParentGuid = segment.ParentGuid;
         SequenceNo = segment.SequenceNo;
         Index = segment.Index;
         SegmentId = segment.SegmentId;
         Loop = segment.Loop;
         LoopParent = segment.LoopParent;
         Name = segment.Name;
         ElementPath = segment.ElementPath;
         ValueText = segment.ValueText;
         Skipped = segment.Skipped;
         MinLength = segment.MinLength;
         MaxLength = segment.MaxLength;
         MinOccurrence = segment.MinOccurrence;
         MaxOccurrence = segment.MaxOccurrence;
         SegmentParent = segment.SegmentParent;

         IsLoop = segment.IsLoop;
         IsTrigger = segment.IsTrigger;

         QualifiedName = new QualifiedNameInfo(
            segment.QualifiedName.Prefix, segment.QualifiedName.Name);
         Codes = new List<string>();
         foreach(var code in segment.Codes)
         {
            Codes.Add(code);
         }

         // copy children...
         Children.Clear();
         foreach(var child in segment.Children)
         {
            EdiSegmentInfo csegment = new EdiSegmentInfo();
            csegment.Copy(child);
            Children.Add(csegment);
         }
      }

      /// <summary>
      /// Duplicate Segment.
      /// </summary>
      /// <param name="segment">segment to duplicate</param>
      /// <returns></returns>
      public static EdiSegmentInfo Duplicate(EdiSegmentInfo segment)
      {
         EdiSegmentInfo duplicate = new EdiSegmentInfo();
         duplicate.Copy(segment);
         return duplicate;
      }

      #endregion
      #region -- 4.00 - Validate Segment and Set Values

      /// <summary>
      /// Validate that tokens match with those in the Segment.
      /// </summary>
      /// <param name="segment"></param>
      /// <param name="tokens"></param>
      /// <returns></returns>
      public static ResultLog Validate(EdiSegmentInfo segment, string[] tokens)
      {
         ResultLog results = new ResultLog();

         // validate the token counts
         var tokenCount = tokens.Length - 1;
         if (tokenCount != segment.Children.Count)
         {
            results.Failed(segment.SegmentId +
               "(" + segment.Children.Count.ToString() + ") <> " +
               tokenCount.ToString());
         }

         // validate tags and segment occurance, skip as needed
         if (segment.SegmentId != tokens[0])
         {
            if (segment.MinOccurrence > 0)
            {
               results.Failed(segment.SegmentId +
                  " expected but (" + tokens[0] + ") was found " +
                  "-- SKIPPED");
            }
            segment.Skipped = true;
         }

         results.Succeeded();
         return results;
      }

      /// <summary>
      /// Set Segment Children values using given tokens.
      /// </summary>
      /// <param name="segment">segment whose children values will be set
      /// </param>
      /// <param name="tokens">tokens values</param>
      public static void SetValues(EdiSegmentInfo segment, string[] tokens)
      {
         int idx = 1;
         foreach (var child in segment.Children)
         {
            if (idx < tokens.Length)
            {
               child.ValueText = tokens[idx];
            }
            else
            {
               break;
            }
            idx++;
         }
      }

      #endregion

   }

}
