using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// -----------------------------------------------------------------------------
using Edam.DataObjects.DataCodes;
using Edam.DataObjects.Models;
using Edam.DataObjects.Requests;
using Edam.DataObjects.ReferenceData;
using Edam.Diagnostics;

namespace Edam.DataObjects.ReferenceData
{

   /// <summary>
   /// Support Reference Data Edit Templates.
   /// </summary>
   public class ReferenceDataEditTemplateHelper
   {

      #region -- 1.0 - Fields and Properties declaration

      public static readonly String CLASS_NAME = 
         "ReferenceDataEditTemplateHelper";
      public static readonly Int16 TEMPLATE_GROUP_ALL = Int16.MaxValue;

      #endregion
      #region -- 1.1 - Initialization and Setup Support 

      /// <summary>
      /// Setup Groups for given templates.
      /// </summary>
      /// <param name="data">templates codes set</param>
      public static void SetupGroups(ReferenceDataEditTemplateCodesInfo data)
      {
         if (data.Groups == null || data.Templates == null)
            return;
         foreach (var t in data.Templates)
         {
            t.Group = data.Groups.Find((x) => x.CodeId == t.GroupNo.ToString());
         }
      }

      #endregion
      #region -- 4.1 - Template to and from JSON

      /// <summary>
      /// Given a Reference Data Edit Template instance return corresponding
      /// ElementNodeInfo JSON text representation.
      /// </summary>
      /// <param name="item">instance of ReferenceDataEditTemplateInfo</param>
      /// <returns>JSON Text based on ElementNodeInfo</returns>
      public static String ToJsonText(ReferenceDataEditTemplateInfo item)
      {
         // TODO: use JSON Builder instead...
         return "{ \"Title\": \"" + item.Title + "\","
            + "\"Description\": \"" + item.Title + "\","
            + "\"Name\": \"" + item.ResourceName + "\","
            + "\"TemplateNo\": " + item.TemplateNo.ToString() + ","
            + "\"Type\": \"" + item.TemplateTypeNo.ToString() + "\","
            + "\"Items\": " + item.TemplateData + "}";
      }

      /// <summary>
      /// Get Templates as a JSON string.
      /// </summary>
      /// <returns>JSON String</returns>
      public static String ToJsonText(ReferenceDataEditTemplateCodesInfo data)
      {
         SetupGroups(data);

         Int32 i = 0;
         Int32 grpCount = 0, itmCount;
         Int64 groupNo;
         IDataCode grpCode = new DataCodes.DataCodeInfo();
         ReferenceDataEditTemplateInfo itm;

         StringBuilder sb = new StringBuilder();
         StringBuilder grp = new StringBuilder();

         var l = data.Templates.OrderBy((x) => x.GroupNo);

         // TODO: use JSON Builder instead...
         sb.AppendLine("{ \"Title\":\"\", \"Items\": [");
         while (i < l.Count())
         {
            itm = l.ElementAt(i);

            groupNo = l.ElementAt(i).GroupNo;
            grpCode.GroupId = groupNo.ToString();
            grpCode.Value = itm.Group == null ?
               "GROUP " + groupNo.ToString() : itm.Group.Value;

            grp.Clear();
            if (grpCount > 0)
               grp.Append(",");
            grp.AppendLine("{ \"Title\": \"" + grpCode.Value + "\"," +
               "\"Description\": \"" + grpCode.Value + "\"," +
               "\"Items\": [");
            itmCount = 0;

            while (true)
            {
               if (itm.GroupNo != groupNo)
                  break;

               if (itmCount > 0)
                  grp.Append(",");
               grp.AppendLine(ToJsonText(itm));

               itmCount++;
               i++;
               if (i >= l.Count())
                  break;

               itm = l.ElementAt(i);
            }

            grp.AppendLine("]}");
            grpCount++;

            sb.AppendLine(grp.ToString());
         }
         sb.AppendLine("]}");

         return sb.ToString();
      }

      /// <summary>
      /// Validate given JSON text by trying to get corresponding
      /// ElementGroupInfo model.
      /// </summary>
      /// <param name="jsonText">JSON Text to convert</param>
      /// <returns>ResultsLog instance is returned, if all was OK
      /// Success will be true.</returns>
      public static ResultLog IsValid(String jsonText)
      {
         ResultLog results = new ResultLog();
         var r = ToElementGroup(jsonText);
         results.Copy(r);
         return results;
      }

      #endregion
      #region -- 4.1 - Element Group and Node Support

      /// <summary>
      /// Given a JSON text convert it into corresponding ElementNodeInfo model.
      /// </summary>
      /// <param name="jsonText">JSON Text to convert</param>
      /// <returns>ResultsLog instance is returned, model instance will be found
      /// in Data if Sucess is true, else null and Sucess is false</returns>
      public static ResultsLog<ElementNodeInfo> ToElementNode(
         String jsonText)
      {
         ResultsLog<ElementNodeInfo> results =
            new ResultsLog<ElementNodeInfo>();
         if (String.IsNullOrWhiteSpace(jsonText))
         {
            results.Failed(CLASS_NAME + "::ToElementNodeModel",
               "JSON Text was Expected but Not Provided.");
            return results;
         }

         try
         {
            results.Data = Edam.Serialization.JsonSerializer.
               Deserialize<ElementNodeInfo>(jsonText);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      /// <summary>
      /// Get Element Node for given template.
      /// </summary>
      /// <param name="template"></param>
      /// <returns></returns>
      public static ResultsLog<ElementNodeInfo> ToElementNode(
         ReferenceDataEditTemplateInfo template)
      {
         String json = ToJsonText(template);
         var results = ToElementNode(json);
         return results;
      }

      /// <summary>
      /// Given some template data set including the node, reference data codes,
      /// and other details return the corresponding ElementNodeInfo...
      /// </summary>
      /// <remarks>
      /// The set is checked to have only one template and if not null will be
      /// returned, if true then the template and expected mapped linked codes
      /// are returned if those are available.
      /// </remarks>
      /// <param name="templateDataSet">template and codes data set</param>
      /// <returns>the corresponding instance of the set is returned as an
      /// Element-Node</returns>
      public static ResultsLog<ElementNodeInfo> ToElementNode(
         ReferenceDataEditTemplateCodesInfo templateDataSet)
      {
         if (templateDataSet == null)
         {
            ResultsLog<ElementNodeInfo> l = new ResultsLog<ElementNodeInfo>();
            l.Failed(EventCode.NullInstanceFound);
            return l;
         }

         var t = templateDataSet.Templates.Count == 1 ?
            templateDataSet.Templates[0] : null;
         String json = ToJsonText(t);
         var results = ToElementNode(json);

         // if all is OK, then check if we have mapped - code values and bind
         // those to the resulted node items...
         if (results.Success && templateDataSet.Maps != null)
         {
            foreach(var m in templateDataSet.Maps)
            {
               foreach(var l in m.Link)
               {
                  var i = results.Data.Items.Find(
                     (x) => x.Name == l.ChildElementName);
                  if (i == null)
                     continue;
                  i.LinkCodes = m.LinkCodes;
               }
            }
         }
         return results;
      }

      /// <summary>
      /// Get Element Node for given template-no.
      /// </summary>
      /// <param name="sessionId"></param>
      /// <param name="organizationId"></param>
      /// <param name="itemNo"></param>
      /// <param name="optionNo"></param>
      /// <returns></returns>
      public static ResultsLog<ElementNodeInfo> ToElementNode(
         RequestResponseInfo<ReferenceDataEditTemplateCodesInfo> response)
      {
         ReferenceDataEditTemplateInfo tinfo = (response.ResponseData == null ||
            response.ResponseData.Templates == null ||
            response.ResponseData.Templates.Count != 1) ? null :
            response.ResponseData.Templates[0];

         if (!response.Success || tinfo == null)
         {
            ResultsLog<ElementNodeInfo> rlog =
               new ResultsLog<ElementNodeInfo>();
            rlog.Copy(response.Results);
            return rlog;
         }

         tinfo.OrganizationId = Edam.Application.Session.OrganizationId;
         var results = ToElementNode(tinfo);
         results.Data.Maps = response.ResponseData.Maps;
         return results;
      }

      /// <summary>
      /// Given a JSON text convert it into corresponding 
      /// ElementGroupInfo model.
      /// </summary>
      /// <param name="jsonText">JSON Text to convert</param>
      /// <returns>ResultsLog instance is returned, model instance will be found
      /// in Data if Sucess is true, else null and Sucess is false</returns>
      public static ResultsLog<ElementGroupInfo> ToElementGroup(String jsonText)
      {
         ResultsLog<ElementGroupInfo> results =
         new ResultsLog<ElementGroupInfo>();
         if (String.IsNullOrWhiteSpace(jsonText))
         {
            results.Failed(CLASS_NAME + "::ToModel",
               "JSON Text was Expected but Not Provided.");
            return results;
         }

         try
         {
            results.Data = Edam.Serialization.JsonSerializer.
               Deserialize<ElementGroupInfo>(jsonText);
            results.Succeeded();
         }
         catch (Exception ex)
         {
            results.Failed(ex);
         }
         return results;
      }

      /// <summary>
      /// Assign Values from source to destination.
      /// </summary>
      /// <remarks>
      /// It is assumed that caller did send to nodes that have the same items
      /// meaning that represent the same template and therefore we don't check
      /// for those here.  Use the dictionary to force values as needed, for 
      /// example, given an OrganizationId you don't trust the sender and then
      /// here you force the value regardless of the value provided by sender.
      /// </remarks>
      /// <param name="destination">destination</param>
      /// <param name="source">source</param>
      /// <param name="keywords">optional keywords (key, value) for constants...
      /// </param>
      public static void AssignValues(
         ElementNodeInfo destination, ElementNodeInfo source, 
         Dictionary<string, string> keywords = null)
      {
         // if we don't have a source or destination we don't do nothing...
         if (source == null || source.Items == null)
            return;
         if (destination == null || destination.Items == null)
            return;

         // assign values now...
         ElementItemInfo e = null;
         foreach(var i in source.Items)
         {
            e = destination.Items.Find((x) => x.SerialNo == i.SerialNo);
            if (e == null)
               continue;

            // if name,value is found in given dictionary then force its value
            // and continue...
            if (keywords != null)
            {
               if (keywords.TryGetValue(i.Name.ToLower(), out string value))
               {
                  e.ValueText = value;
                  continue;
               }
            }
            e.ValueText = i.ValueText;
         }
      }

      #endregion
      #region -- 4.1 - Template Data Support
      
      /// <summary>
      /// Given a JSON text convert it into corresponding MapInfo model.
      /// </summary>
      /// <param name="jsonText">JSON Text to convert</param>
      /// <returns>ResultsLog instance is returned, model instance will be found
      /// in Data if Sucess is true, else null and Sucess is false</returns>
      public static ResultsLog<List<MapInfo>> ToMapData(String jsonText)
      {
         ResultsLog<List<MapInfo>> results =
            new ResultsLog<List<MapInfo>>();
         if (String.IsNullOrWhiteSpace(jsonText))
         {
            results.Failed(CLASS_NAME + "::ToMapData",
               "JSON Text was Expected but Not Provided.");
            return results;
         }

         try
         {
            results.Data = Edam.Serialization.JsonSerializer.
               Deserialize<List<MapInfo>>(jsonText);
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
