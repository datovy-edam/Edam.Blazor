using Edam.DataObjects.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.B2b.Edi
{

   public class EdiSegmentList : List<EdiSegmentInfo>
   {

      /// <summary>
      /// Find Loop Segment based on its loop (segmentId) ID.
      /// </summary>
      /// <param name="segmentId">segment ID</param>
      /// <returns>segment is returned if any was found</returns>
      public EdiSegmentInfo? FindLoop(string loopId)
      {
         EdiSegmentInfo? segment =
            this.Find((x) => x.SegmentId == loopId && x.IsLoop);
         return segment;
      }

      /// <summary>
      /// Find segment with Tag and Code.
      /// </summary>
      /// <param name="parentTag">parent or preceeding Tag</param>
      /// <param name="tag">segment ID</param>
      /// <param name="entityId">entity ID</param>
      /// <returns>segment is returned if any was found</returns>
      public EdiSegmentInfo? FindTag(
         string parentTag, string tag, string entityId)
      {
         EdiSegmentInfo? segment = null;
         var list =
            this.FindAll((x) => x.SegmentId == tag && 
            (x.Codes.Count == 0 || x.Codes.Contains(entityId)));
         if (list != null && list.Count == 1)
         {
            segment = list[0];
         }
         else
         {
            foreach(var item in list)
            {
               var mt1 = parentTag.Substring(0, 2);
               var mt2 = item.SegmentParent.Substring(0, 2);
               if (String.Compare(mt2, mt1) >= 0)
               {
                  segment = item;
                  break;
               }
            }
         }
         return segment;
      }

      public string ToJson()
      {
         return JsonConvert.SerializeObject(this, Formatting.Indented);
      }

      /// <summary>
      /// 
      /// </summary>
      private class SegmentRecord
      {
         public EdiSegmentInfo Segment { get; set; }

         public string PathID { get; }

         public string EntityID { get; }
         public string EntityName { get; }
         public string EntityElementName { get; }
         public string EntityLink { get; }

         public List<string> Rows { get; } = new List<string>();

         public bool IsValid { get; }

         public SegmentRecord(EdiSegmentInfo segment)
         {
            Segment = segment;

            string [] lst = segment.ElementPath.Split('/');

            EntityID = lst.Length > 0 ? lst[0] : String.Empty;
            EntityName = lst.Length > 1 ? lst[1] : String.Empty;
            EntityElementName = lst.Length > 2 ? lst[2] : String.Empty;
            EntityLink = lst.Length > 3 ? lst[3] : String.Empty;

            PathID = String.IsNullOrWhiteSpace(EntityID) ||
               String.IsNullOrWhiteSpace(EntityName) ?
               String.Empty : EntityID + "/" + EntityName;

            IsValid = PathID.Length > 0;
         }

         public void AddUniqueIds(
            Text.JsonBuilder builder, EdiSegmentInfo segment)
         {
            builder.AddPropertyValue("Guid", segment.Guid);
            builder.AddPropertyValue("ParentGuid", segment.ParentGuid);
            builder.AddPropertyValue("LoopParentGuid", segment.LoopParentGuid);
         }

         /// <summary>
         /// Add Record for given segment instance.
         /// </summary>
         /// <returns>JSON text of record is returned</returns>
         public string AddItem(EdiSegmentInfo? segment)
         {
            Edam.Text.JsonBuilder builder = new Edam.Text.JsonBuilder();

            builder.StartBlock();
            AddUniqueIds(builder, segment);

            foreach (var child in segment.Children)
            {
               string[] lst = child.ElementPath.Split('/');
               if (lst.Length >= 2)
               {
                  builder.AddPropertyValue(lst[2], child.ValueText);
               }
               else
               {

               }
            }
            builder.EndBlock();

            string record = builder.ToString(addComma: false);
            Rows.Add(record);
            return record;
         }

         public string AddSchema(EdiSegmentInfo segment)
         {
            Edam.Text.JsonBuilder builder = new Edam.Text.JsonBuilder();

            builder.StartBlock();
            foreach (var child in segment.Children)
            {
               string[] lst = child.ElementPath.Split('/');
               if (lst.Length >= 2)
               {
                  builder.AddPropertyValue(lst[2], child.ValueText);
               }
            }
            builder.EndBlock();

            string record = builder.ToString();
            Rows.Add(record);
            return record;
         }

      }

      /// <summary>
      /// Using the "Element Path" create a JSON document with the structure 
      /// implied by this Path.  Given a segment the following JSON text record
      /// will be built and added to the Segment Main Paht (schema/table):
      /// {    
      ///    "Document": {
      ///       "Schema": [
      ///          {
      ///             "Table": [ 
      ///                {
      ///                   "property1": "value1",
      ///                   "property2": "value2",
      ///                ...
      ///                },
      ///                ...
      ///             ],
      ///             ...
      ///          },
      ///          ...
      ///       ]
      ///    }
      /// }
      /// </summary>
      /// <returns>JSON text document is returned</returns>
      public string ToJsonDocument(string rootElementName = null)
      {
         string? rootName = String.IsNullOrEmpty(rootElementName) ?
            null : rootElementName;

         Dictionary<string, SegmentRecord> tableItem = 
            new Dictionary<string, SegmentRecord>();
         Dictionary<string, SegmentRecord> schemaItem =
            new Dictionary<string, SegmentRecord>();

         // add rows/records for each table (schema/table)
         string[] lst = null;
         foreach(var item in this)
         {
            if (item.IsLoop)
            {
               continue;
            }

            SegmentRecord? ditem = new SegmentRecord(item);
            if (!ditem.IsValid)
            {
               continue;
            }

            // add row
            SegmentRecord? rowInstance = null;
            if (!tableItem.TryGetValue(ditem.PathID, out rowInstance))
            {
               tableItem.Add(ditem.PathID, ditem);
               rowInstance = ditem;
            }
            rowInstance.AddItem(ditem.Segment);

            // add schema
            SegmentRecord schemaInstance = null;
            if (!schemaItem.TryGetValue(ditem.EntityID, out schemaInstance))
            {
               schemaItem.Add(ditem.EntityID, schemaInstance);
            }
            else
            {
               schemaInstance = ditem;
            }
         }

         // add each table to schema and schema to root
         List<SegmentRecord> schemaRecords = tableItem.Values.ToList();

         Edam.Text.JsonBuilder builder = new Edam.Text.JsonBuilder();
         builder.StartDocument();

         // add root property if any was specified
         //if (rootName != null)
         //{
         //   builder.AddProperty(rootName);
         //}

         int count = 0;
         foreach (var schema in schemaItem.Keys)
         {
            if (count > 0)
            {
               builder.AppendComma();
            }

            builder.AddProperty(schema);
            var schemaItems = schemaRecords.FindAll(
               (x) => x.EntityID == schema);

            foreach(var sitem in schemaItems)
            {
               if (count != 0)
               {
                  builder.AppendComma();
               }
               int rcount = 0;

               builder.AddProperty(sitem.EntityName, isArray: true);
               foreach(var child in sitem.Rows)
               {
                  if (rcount != 0)
                  {
                     builder.AppendComma();
                  }
                  builder.AddText(child.ToString());
                  rcount++;
               }
               builder.EndProperty(isArray: true);
               count++;
            }

            builder.EndProperty();
            count++;
         }

         // close root if any was specified
         if (rootName != null)
         {
            builder.EndProperty();
         }

         builder.EndDocument();

         return builder.ToString();
      }

   }

}
