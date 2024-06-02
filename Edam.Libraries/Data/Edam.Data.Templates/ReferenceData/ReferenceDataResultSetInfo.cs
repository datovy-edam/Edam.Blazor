using System;
using System.Text;
using System.Collections.Generic;

// -----------------------------------------------------------------------------
//using System.Data.Common;
using Edam.DataObjects.Models;
using Edam.DataObjects.DataCodes;

namespace Edam.DataObjects.ReferenceData
{

   public class ReferenceDataResultSetInfo
   {

      private ElementNodeInfo m_Node;

      public List<ReferenceDataRowInfo> ResultSet { get; set; }

      public ReferenceDataResultSetInfo(ElementNodeInfo node)
      {
         m_Node = node;
         ResultSet = new List<ReferenceDataRowInfo>();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="name"></param>
      /// <param name="title"></param>
      /// <param name="length"></param>
      /// <param name="show"></param>
      /// <param name="codes">this is a list of possible values for columns that
      /// will be used as drop-down-box or lists</param>
      /// <returns></returns>
      private String GetColumnJson(String name, String title, Int32 length,
         Boolean show, MapInfo map, LinkInfo link)
      {
         // TODO: use JSOB Builder instead...
         StringBuilder sb = new StringBuilder();
         sb.AppendLine("{");
         sb.AppendLine("\"ColumnName\":\"" + name + "\",");
         sb.AppendLine("\"Title\":\"" + title + "\",");
         sb.AppendLine("\"Width\":\"" + length.ToString() + "\",");
         sb.AppendLine("\"Visible\":" + (show ? "true" : "false"));

         if (map != null && link != null && map.LinkCodes != null && 
            map.LinkCodes.Count > 0)
         {
            int c = 0;
            sb.AppendLine(",\"LinkName\":\"" +
               (String.IsNullOrWhiteSpace(link.LinkElementName) ? 
                  "" : link.LinkElementName) + "\"");
            sb.AppendLine(",\"Data\":[");
            foreach (var i in map.LinkCodes)
            {
               c++;
               sb.AppendLine("{"
                  + "\"GroupId\": \"" + (String.IsNullOrWhiteSpace(i.GroupId) ? 
                    "0" : i.GroupId) + "\","
                  + "\"CodeId\": \"" + i.CodeId + "\","
                  + "\"Value\": \"" + i.Value + "\""
                  + "}" + (c < map.LinkCodes.Count ? "," : ""));
            }
            sb.AppendLine("]");
         }

         sb.AppendLine("}");
         return sb.ToString();
      }

      /// <summary>
      /// Convert Result Set to JSON.
      /// </summary>
      /// <returns>a JSON string is returned</returns>
      public String ToJson()
      {
         String row;
         StringBuilder sb = new StringBuilder();
         Int32 cnt = 0;
         ElementItemInfo item;
         Boolean isVisible;
         sb.AppendLine("{\"Columns\":[");
         foreach(var i in ResultSet)
         {
            // add row-number / key column
            sb.AppendLine(
               GetColumnJson(ReferenceDataRowInfo.ROW_NUMBER_FIELD,
                  ReferenceDataRowInfo.ROW_NUMBER_TITLE, 20, false, null, null));
            cnt++;

            // add all other columns...
            MapInfo map;
            LinkInfo link;
            foreach (var c in i)
            {
               if (cnt > 0)
                  sb.Append(",");
               item = m_Node.Items.Find((x) => x.Name == c.ElementName);
               if (item == null)
                  continue;
               isVisible = item.Visibility != Objects.ObjectVisibility.Hiden &&
                  item.Visibility != Objects.ObjectVisibility.Unknown;
               map = null;
               link = null;
               if (m_Node.Maps != null)
               {
                  foreach(var m in m_Node.Maps)
                  {
                     var l = m.Link.Find((x) => x.ChildElementName == c.ElementName);
                     if (l != null)
                     {
                        map = m;
                        link = l;
                        break;
                     }
                  }
               }
               sb.AppendLine(
                  GetColumnJson(
                     c.ElementName, c.Title, c.MaxLength, isVisible, map, link));
               cnt++;
            }
            break;
         }
         sb.AppendLine("],");

         // TODO: use JSOB Builder instead...
         // setup the Data element...
         cnt = 0;
         sb.AppendLine("\"Data\":[");
         foreach (var i in ResultSet)
         {
            row = i.ToJson(cnt);
            if (!String.IsNullOrWhiteSpace(row))
            {
               if (cnt > 0)
                  sb.Append(",");
               sb.AppendLine(row);
               cnt++;
            }
         }
         sb.AppendLine("]}");
         String jsonText = sb.ToString();
         return jsonText;
      }

#if DATA_SUPPORT_
      /// <summary>
      /// Read list.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns>list of OrganizationInfo is returned</returns>
      public List<ReferenceDataRowInfo> GetList(
         ElementNodeInfo node, DbDataReader reader)
      {
         ResultSet = ResultSet ?? new List<ReferenceDataRowInfo>();

         ReferenceDataRowInfo o;
         while (reader.Read())
         {
            o = ReferenceDataRowInfo.ReadData(node, reader);
            ResultSet.Add(o);
         }
         return ResultSet;
      }
#endif

   }

}
