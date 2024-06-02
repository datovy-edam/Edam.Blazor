using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Edam.Diagnostics;

namespace Edam.B2b.Edi
{

   /// <summary>
   /// Given an EDI instance file of a particular definition, read its content
   /// and 
   /// </summary>
   public class EdiInstanceReader
   {

      private StringBuilder m_Builder;
      private EdiDocument m_Document;
      private string m_Delimiter;
      private string m_EndOffLineChar;

      public string[] Lines { get; set; }

      public EdiInstance Instance { get; set; } = new EdiInstance();

      /// <summary>
      /// Manage Instance Uniqueness
      /// </summary>
      private class InstanceState
      {

         public class key_value
         {
            public string Key { get; set; }
            public string Value { get; set; }

            public key_value(string key, string value)
            {
               Key = key;
               Value = value;
            }
         }

         private EdiSegmentList m_CurrentInstance = null;
         private string m_Guid = System.Guid.NewGuid().ToString();

         /// <summary>
         /// Manage unique keys based on their segment loop-id's so every loop
         /// will have a unique ID
         /// </summary>
         private List<key_value> m_KeyMap = new List<key_value>();
         private List<key_value> m_LoopKeyMap = new List<key_value>();

         public List<key_value> LoopKeyMap
         {
            get { return m_LoopKeyMap; }
         }

         public string Guid
         {
            get { return m_Guid; }
         }

         public EdiSegmentList Instance
         {
            get { return m_CurrentInstance; }
         }

         public InstanceState()
         {
            m_CurrentInstance = new EdiSegmentList();
         }

         /// <summary>
         /// Get registered parent loop while managing revisiting trigger loops.
         /// </summary>
         /// <remarks>
         /// Trigger loops are those that if revisited the rest of the child 
         /// loops should be removed to allow the unique identification of a new
         /// set of loop children.
         /// </remarks>
         /// <param name="item">segment information</param>
         /// <returns>instance of key_value is returned</returns>
         private key_value? GetRegisteredParent(EdiSegmentInfo item)
         {
            // try to find items in the order they were inserted...
            for (int i = 0; i < m_KeyMap.Count; i++)
            {
               var kval = m_KeyMap[i];
               if (kval.Key == item.LoopParent)
               {
                  // if this is not a trigger item, just return
                  // the value of the key
                  if (!item.IsTrigger)
                  {
                     return kval;
                  }

                  // if we get here means that we are visiting a trigger item
                  // again and if so we need to create a new set of unique
                  // values for subsequent keys

                  // if we get here, re-key all subsequent entries that will
                  // efectively create a new sequence of unique-keys

                  var lkval = m_LoopKeyMap.Find((x) => x.Key == item.Loop);
                  lkval.Value = item.Guid;

                  m_KeyMap.RemoveRange(i, m_KeyMap.Count - i);
                  return kval;
               }
            }
            return null;
         }

         /// <summary>
         /// Fetch the Parent Loop Guid to uniquely identify the reference item
         /// </summary>
         /// <param name="item">segment information</param>
         /// <returns>GUID is returned</returns>
         private string GetParentLoopGuid(EdiSegmentInfo item)
         {
            // first register item loop as needed
            key_value lkval = m_LoopKeyMap.Find((x) => x.Key == item.Loop);
            if (lkval == null)
            {
               lkval = new key_value(item.Loop, item.Guid);
               m_KeyMap.Add(lkval);
            }
            else if (lkval.Value == null)
            {
               lkval.Value = item.Guid;
            }

            // manage parent loop
            key_value? kval = GetRegisteredParent(item);
            if (kval != null)
            {
               return kval.Value;
            }

            // find parent loop key
            var plkval = m_LoopKeyMap.Find(x => x.Key == item.LoopParent);
            //if (item.IsTrigger)
            //{
            //   plkval.Value = item.Guid;
            //}

            kval = new key_value(item.LoopParent, plkval.Value);
            m_KeyMap.Add(kval);

            return kval.Value;
         }

         /// <summary>
         /// Set Unique ID's...
         /// </summary>
         /// <param name="item">item to set its ID's</param>
         public void SetUniqueIds(EdiSegmentInfo item)
         {
            // make the segment unique...
            //item.Guid = System.Guid.NewGuid().ToString();
            item.ParentGuid = GetParentLoopGuid(item);

            var kval = m_LoopKeyMap.Find(x => x.Key == item.Loop);
            item.LoopParentGuid = kval.Value;
         }

         /// <summary>
         /// Add given item to the current instance...
         /// </summary>
         /// <param name="item">instance item to add</param>
         public void AddInstance(EdiSegmentInfo item)
         {
            SetUniqueIds(item);
            m_CurrentInstance.Add(item);

            // update ids of children...
            foreach(var child in item.Children)
            {
               child.ParentGuid = item.Guid;
            }
         }

      }

      /// <summary>
      /// Manage Reader State
      /// </summary>
      private class ReaderState
      {
         public ResultLog Results = new ResultLog();

         public InstanceState? CurrentInstance { get; set; } = null;

         private int m_CurrentTagIndex = -1;
         private EdiSegmentInfo? m_CurrentTag = null;
         private string? m_CurrentSegmentId = null;
         private EdiInstance? m_Instance = null;

         public EdiSegmentList? Items { get; set; } = null;

         public ReaderState(EdiDocument document, EdiInstance? instance)
         {
            EdiSegmentList? itms = null;
            Items = document.Items;
            m_Instance = instance;
         }

         /// <summary>
         /// Set the given TAG as current.
         /// </summary>
         /// <param name="tag">tag to set</param>
         private void SetCurrent(EdiSegmentInfo? tag)
         {
            m_CurrentTag = tag;
            m_CurrentSegmentId = tag.SegmentParent;
            m_CurrentTagIndex = tag.SequenceNo + 1;
         }

         /// <summary>
         /// Get Next Tag in Segment.
         /// </summary>
         /// <param name="segmentId">Segment ID being searched based on
         /// SegmentParent</param>
         /// <param name="tokens">current instance segment</param>
         /// <returns>found segment is returned else null</returns>
         private EdiSegmentInfo? GetTagInSegment(
            string segmentId, string[] tokens)
         {
            while (m_CurrentTagIndex < Items.Count)
            {
               var tag = Items[m_CurrentTagIndex];

               if (tag.SegmentParent == segmentId)
               {
                  if (tag.SegmentId == tokens[0])
                  {
                     if (tag.Codes.Count > 0)
                     {
                        if (tag.Codes.Contains(tokens[1]))
                        {
                           SetCurrent(tag);
                           return m_CurrentTag;
                        }
                     }
                     else
                     {
                        SetCurrent(tag);
                        return m_CurrentTag;
                     }
                  }
               }
               else
               {
                  var seg = Items.FindTag(segmentId, tokens[0], tokens[1]);
                  if (seg != null)
                  {
                     SetCurrent(seg);
                     return m_CurrentTag;
                  }
               }

               m_CurrentTagIndex++;
            }

            // maybe we are starting a new submission...
            if (tokens[0] == "ISA")
            {
               // find ISA tag... to start a new submission...
               var seg = Items.FindTag(String.Empty, tokens[0], tokens[1]);
               if (seg != null)
               {
                  SetCurrent(seg);
                  return m_CurrentTag;
               }
            }

            return null;
         }

         /// <summary>
         /// Get Next Tag...
         /// </summary>
         /// <returns>next tag is returned</returns>
         public EdiSegmentInfo? GetNextTag(string[] tokens)
         {
            if (m_CurrentTagIndex == -1)
            {
               var seg = Items.FindTag(
                  String.Empty, tokens[0], tokens[1]);
               if (seg != null)
               {
                  SetCurrent(seg);
                  return seg;
               }
               else
               {

               }
            }

            if (m_CurrentTagIndex < Items.Count)
            {
               GetTagInSegment(m_CurrentSegmentId, tokens);
               return m_CurrentTag;
            }

            Results.Failed("End of Document found");
            return null;
         }

         /// <summary>
         /// Prepare New Submission since a ISA was found.
         /// </summary>
         /// <param name="tokens">ISA data items values</param>
         /// <returns>if ISA was found the segment instance is returend else
         /// null</returns>
         public EdiSegmentInfo? PrepareNewSubmission(string[] tokens)
         {
            if (CurrentInstance != null)
            {
               m_Instance.Instances.Add(CurrentInstance.Instance);
            }

            // get ready for new Instance
            CurrentInstance = new InstanceState();

            // register all loops...
            var groupByLoop =
               from item in Items
               group item by item.Loop into newGroup
               orderby newGroup.Key
               select newGroup;

            foreach (var item in groupByLoop)
            {
               var kval = new InstanceState.key_value(
                  item.Key, null);
               CurrentInstance.LoopKeyMap.Add(kval);
            }

            // find ISA tag... to start a new submission...
            var seg = Items.FindTag(String.Empty, tokens[0], tokens[1]);
            if (seg != null)
            {
               SetCurrent(seg);
               return m_CurrentTag;
            }

            return null;
         }
      }

      private ReaderState m_State;

      public EdiInstanceReader(EdiDocument document)
      {
         m_Builder = new StringBuilder();
         m_Document = document;
         m_State = new ReaderState(document, Instance);
      }

      /// <summary>
      /// Process instance lines using given document dictionary.
      /// </summary>
      /// <param name="document">target dictionary</param>
      /// <returns></returns>
      public string ToXml()
      {
         foreach(var items in m_Document.Items)
         {

         }
         return m_Builder.ToString();
      }

      /// <summary>
      /// Parse instance lines.
      /// </summary>
      /// <param name="lines">instance lines</param>
      private void Parse(string[] lines)
      {
         Lines = lines;
         if (Lines == null || Lines.Length <= 2)
         {
            return;
         }

         // get all values for segment items...
         m_Delimiter = new string(Lines[2][2], 1);

         int lineCount = 0;
         EdiSegmentInfo? segment = null;
         foreach (var item in Lines)
         {
            lineCount++;
            string[] tokens = item.Split(m_Delimiter);

            if (tokens != null && String.IsNullOrWhiteSpace(tokens[0]))
            {
               continue;
            }

            // is this a new ISA instance? if so, start a new instance
            if (tokens[0] == "ISA")
            {
               m_State.PrepareNewSubmission(tokens);
            }

            segment = m_State.GetNextTag(tokens);

            if (segment != null)
            {
               var sitem = EdiSegmentInfo.Duplicate(segment);
               sitem.Guid = System.Guid.NewGuid().ToString();

               EdiSegmentInfo.SetValues(sitem, tokens);
               m_State.CurrentInstance.AddInstance(sitem);
            }
            else
            {

            }
         }

         // add last instance...
         Instance.Instances.Add(m_State.CurrentInstance.Instance);

         if (lineCount != Lines.Length)
         {

         }

      }

      #region -- 4.00 - Manage Document Instance

      /// <summary>
      /// Read EDI document instance lines from a file.
      /// </summary>
      /// <param name="filePath">document instance file path</param>
      /// <returns>if success lines are returned</returns>
      public static ResultsLog<EdiInstance> FromFile(
         EdiDocumentReader document, string filePath)
      {
         ResultsLog<EdiInstance> results = 
            new ResultsLog<EdiInstance>();
         EdiInstanceReader reader = new EdiInstanceReader(document);
         try
         {
            reader.Parse(System.IO.File.ReadAllLines(filePath));
            results.Data = reader.Instance;
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      #endregion

   }

}
