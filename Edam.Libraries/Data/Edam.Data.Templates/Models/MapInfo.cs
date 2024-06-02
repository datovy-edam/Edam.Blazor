using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// LINKS
// -----------------------------------------------------------------------------
// Here a source Element depends on some value of a target...
// "Map": [
//    {
//       "ObjectNo": 38283,
//       "ParentNodeNo": 39029934,
//       "ChildNodeNo": 39920091,
//       "Name": "lk_MapStatusNumber",
//       "Link": [ { "ParentElement": "StatusNo", "ChildElement": "IdNo",
//                   "Title": "Status No", "SelectedValue": null, 
//                   "DefaultValue": 1, "DescriptionElement": "Description",
//                   "Name": "LinkStatusNo" } ],
//       "Description": "Define look-up or mapping"
//    }
// ]

// -----------------------------------------------------------------------------
//using DbField = Edam.Data.DataField;
using Edam.Data.Asset;
using Edam.DataObjects.DataCodes;
using Edam.Serialization;

namespace Edam.DataObjects.Models
{

   /// <summary>
   /// Given a Map - Link info schema as shown above (LINKS) it is read from 
   /// JSON and translated into this flat "MapInfo" one line per "Link".
   /// </summary>
   public class MapInfo : ObjectInfo, IObjectInfo
   {
      
      public ContextInfo Context { get; set; }

      [DataMember]
      public NamespaceInfo? ParentNamespace { get; set; }
      [DataMember]
      public Int64? ParentNodeNo { get; set; }
      [DataMember]
      public string? ParentNodeName { get; set; }

      [DataMember]
      public NamespaceInfo? ChildNamespace { get; set; }
      [DataMember]
      public Int64? ChildNodeNo { get; set; }
      [DataMember]
      public string? ChildNodeName { get; set; }

      /// <summary>
      /// A link identify the parent (source) and child (target) elements and 
      /// other details that helps to establish the relationship or association
      /// between those.
      /// </summary>
      [DataMember]
      public List<LinkInfo> Link { get; set; }

      /// <summary>
      /// A title identify how it is presented to other external interfaces or
      /// entities by detailing the elements that can be used to explain what is
      /// the link about.  For example, we may have a child status that can be
      /// linked to a parent valid list of status values.
      /// </summary>
      /// public List<TitleInfo> Title { get; set; }

      public List<DataCodeInfo> LinkCodes { get; set; }

      public MapInfo() : base()
      {
         Link = new List<LinkInfo>();
         ClearFields();
      }

      public new void ClearFields()
      {
         base.ClearFields();
         Type = ResourceType.MapKeyValue;
         if (Context == null)
            Context = new ContextInfo();
         Context.ClearFields();
         ParentNodeNo = null;
         ChildNodeNo = null;
         ParentNamespace = null;
         ChildNamespace = null;
         ParentNodeName = string.Empty;
         ChildNodeName = string.Empty;
         LinkCodes = new List<DataCodeInfo>();
      }

      public void AddLink(string parentElementName, string childElementName,
         string title, string descriptionElementName)
      {
         LinkInfo linkInfo = new LinkInfo
         {
            Title = title,
            ChildElementName = childElementName,
            ParentElementName = parentElementName,
            Description = descriptionElementName
         };
         Link.Add(linkInfo);
      }

      public static String ToJsonText(MapInfo map)
      {
         return JsonSerializer.Serialize<MapInfo>(map);
      }

      public static MapInfo FromJsonText(String jsonText)
      {
         return JsonSerializer.Deserialize<MapInfo>(jsonText);
      }

      public static String ListToJsonText(List<MapInfo> map)
      {
         return JsonSerializer.Serialize<List<MapInfo>>(map);
      }

      public static List<MapInfo> ListFromJsonText(String jsonText)
      {
         return JsonSerializer.Deserialize<List<MapInfo>>(jsonText);
      }

   }

}

