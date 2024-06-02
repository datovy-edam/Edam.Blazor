using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

// -----------------------------------------------------------------------------
using Edam.DataObjects.Models;
using Edam.Serialization;
using System.Runtime.CompilerServices;
using Edam.Diagnostics;
using System.Dynamic;

namespace Edam.DataObjects.Dynamic
{

   public enum DynamicTableType
   {
      Unknown = 0,
      External = 1,
      Internal = 2
   }

   public class DynamicTableInfo
   {

      #region -- 1.00 - Fields and Properties definitions

      private DynamicTableType m_Type = DynamicTableType.Unknown;
      public DynamicTableType Type
      {
         get { return m_Type; }
      }

      private int m_MaxRowNumber = 0;
      public int MaxRowNumber
      {
         get { return m_MaxRowNumber; }
      }

      private ModelData? m_Model = null;
      public ModelData? Model
      {
         get { return m_Model; }
         set
         { 
            m_Model = value;
            Items = value == null ? null : m_Model.Data;
            SetTableType(value);
         }
      }

      private dynamic m_CurrentRow;
      public dynamic SelectedRow
      {
         get { return m_CurrentRow; }
         set
         { 
            m_CurrentRow = value; 
         }
      }

      public List<ModelColumnInfo> Columns
      {
         get { return m_Model.Columns; }
      }

      public bool HasModel
      {
         get
         {
            return m_Model != null &&
               m_Model.Columns != null ||
               m_Model.Columns.Count > 0; 
         }
      }

      private ObservableCollection<dynamic> m_Items = 
         new ObservableCollection<dynamic>();
      public ObservableCollection<dynamic> Items
      {
         get { return m_Items; }
         set
         {
            if (m_Items != value)
            {
               m_Items = value;
               SetMaxRowNumber(m_Items);
            }
         }
      }

      #endregion
      #region -- 4.00 - Support Methods

      /// <summary>
      /// Set Table Type based on given model and/or its absence...
      /// </summary>
      /// <param name="data"></param>
      private void SetTableType(ModelData data)
      {
         if (data == null || data.ParentNode == null)
         {
            m_Type = DynamicTableType.Unknown;
            return;
         }
         m_Type = data.ParentNode.Type == ResourceType.Dynamic ?
            DynamicTableType.Internal : DynamicTableType.External;
      }

      #endregion
      #region -- 4.00 - Manage Table Rows

      /// <summary>
      /// If a row don't exists ... prepare one...
      /// </summary>
      public void PrepareRow()
      {
         // is this a brand new record?
         if (m_CurrentRow == null)
         {
            m_CurrentRow = CreateNewRow();
         }
      }

      /// <summary>
      /// Set a row number for current row... and set Current Row to null.
      /// </summary>
      public void SetMaxRowNumber()
      {
         SetMaxRowNumber(m_CurrentRow);
      }

      /// <summary>
      /// Create new row.
      /// </summary>
      /// <returns></returns>
      public dynamic CreateNewRow()
      {
         dynamic eobject = ModelExpandoObject.New(m_Model.Columns);
         eobject.row_number = m_MaxRowNumber + 1;
         return eobject;
      }

      /// <summary>
      /// Call passing a list to identify the max row_number... if the 
      /// objects are not in order based on the row_number those will be
      /// forced to be.
      /// </summary>
      public void SetMaxRowNumber(ObservableCollection<dynamic> items)
      {
         if (items == null)
         {
            m_MaxRowNumber = 0;
            return;
         }

         int indx = 0;
         m_MaxRowNumber = 0;
         foreach (var i in items)
         {
            ModelExpandoObject rowData = new ModelExpandoObject(i);
            if (i.row_number > m_MaxRowNumber)
               m_MaxRowNumber = i.row_number;
            else if (i.row_number != indx)
               i.row_number = indx;
            indx++;
         }
      }

      /// <summary>
      /// Call with a newly made dataItem and if it has the same
      /// max row_number then increment the MaxRowNumber
      /// </summary>
      /// <param name="dataItem">newly created data item</param>
      public void SetMaxRowNumber(dynamic dataItem)
      {
         if (dataItem == null)
            return;
         if (dataItem.row_number == m_MaxRowNumber)
            m_MaxRowNumber++;
      }

      /// <summary>
      /// Add new Row.
      /// </summary>
      /// <param name="row">row to add.</param>
      /// <returns>true is returned if added.</returns>
      public int Add(dynamic row)
      {
         if (row == null)
         {
            return -1;
         }

         if (m_Items == null)
         {
            Items = new ObservableCollection<dynamic>();
         }

         int row_number = int.Parse(row.row_number.ToString());

         if (row_number >= m_Items.Count)
         {
            m_Items.Add(row);
         }

         m_Items[row_number] = ModelExpandoObject.Clone(m_CurrentRow);
         return row_number;
      }

      #endregion
      #region -- 4.00 - Clean-up Support

      /// <summary>
      /// Clear all Table Items...
      /// </summary>
      public void ClearData()
      {
         if (m_Items != null)
         {
            Items.Clear();
         }
         Items = null;
      }

      /// <summary>
      /// Clean-up resources...
      /// </summary>
      public void Dispose()
      {
         if (m_Model != null)
            Model.Dispose();
         m_MaxRowNumber = 0;
      }

      #endregion
      #region -- 4.00 - Provide row support, validate, change-log, other

      /// <summary>
      /// Make sure that the element original control values are remembered.
      /// </summary>
      public void SetColumnOriginalValues()
      {

         // there should be an exact match between the table columns and
         // expando object dictionary...

         int c = 0;
         ExpandoObject eobject = m_CurrentRow;
         IDictionary<string, object> expandoDict =
            eobject as IDictionary<string, object>;
         foreach (var i in expandoDict)
         {
            var col = m_Model.Columns[c];
            if (col.Element == null)
            {
               c++;
               continue;
            }

            col.Element.OriginalValue = i.Value;
            col.Element.ValueText = i.Value.ToString();
            col.Element.LastUpdateDate = DateTime.UtcNow;
            c++;
         }
      }

      /// <summary>
      /// Create a Change-Log set based on found differences between the
      /// original and current element control values.
      /// </summary>
      /// <param name="setOriginalValues">true if the original values should
      /// be reset to get ready for a new editing session</param>
      /// <returns>found change log is returned, returns null if no changes 
      /// were found</returns>
      public ResultsLog<ElementChangeLog> PrepareChangeLog(
         string moduleId = null, bool setOriginalValues = false)
      {
         ResultsLog<ElementChangeLog> elog = new ResultsLog<ElementChangeLog>();
         ElementChangeLog log = new ElementChangeLog();
         elog.Data = log;

         // there should be an exact match between the table columns and
         // expando object dictionary...

         int c = 0;
         ExpandoObject eobject = m_CurrentRow;
         IDictionary<string, object> expandoDict =
            eobject as IDictionary<string, object>;
         foreach (var i in expandoDict)
         {
            var col = m_Model.Columns[c];
            if (col.Element == null)
            {
               c++;
               continue;
            }

            if (col.Element.ChangedIndicator)
            {
               ElementChangeEntryInfo entry = new ElementChangeEntryInfo();
               entry.OriginalValue = col.Element.OriginalValue.ToString();
               entry.NewValue = col.Element.ValueText;
               entry.UpdateDate = DateTime.UtcNow;
               log.Add(entry);
            }

            if (setOriginalValues)
            {
               col.Element.OriginalValue = i.Value;
            }
            c++;
         }
         elog.Succeeded();
         
         return elog;
      }

      /// <summary>
      /// Validate row according to schema.
      /// </summary>
      /// <returns>results log instance with found issues is returned, else
      /// the log will be empty and the log Success will be set to true
      /// </returns>
      public ResultsLog<MessageLogEntry> ValidateRow()
      {
         ResultsLog<MessageLogEntry> log = new ResultsLog<MessageLogEntry>();

         // there should be an exact match between the table columns and
         // expando object dictionary...

         int c = 0;
         ExpandoObject eobject = m_CurrentRow;
         IDictionary<string, object> expandoDict =
            eobject as IDictionary<string, object>;
         foreach(var i in expandoDict)
         {
            var col = m_Model.Columns[c];
            if (col.Element == null)
            {
               c++;
               continue;
            }

            var v = string.IsNullOrWhiteSpace(i.Value.ToString()) ?
               string.Empty : i.Value.ToString();

            if (col.Element.KeyType == KeyType.Key &&
               string.IsNullOrWhiteSpace(v))
            {
               log.Add(MessageLogEntry.GetEntry(
                  i.Key + ": key value must be provided"));
            }

            if (col.Element.Required == Objects.ObjectRequirable.Required &&
               string.IsNullOrWhiteSpace(v))
            {
               log.Add(MessageLogEntry.GetEntry(
                  i.Key + ": value is required (" +
                     col.Element.MinLength.ToString() + ")"));
            }

            if (col.Element.MinLength != 0 && v.Length < col.Element.MinLength)
            {
               log.Add(MessageLogEntry.GetEntry(
                  i.Key + ": minimum lengths is (" +
                     col.Element.MinLength.ToString() + ")"));
            }

            if (col.Element.MaxLength != 0 && v.Length > col.Element.MaxLength)
            {
               log.Add(MessageLogEntry.GetEntry(
                  i.Key + ": maximum lengths is (" +
                     col.Element.MinLength.ToString() + ")"));
            }

            c++;
         }
         if (log.Count == 0)
         {
            log.Succeeded();
         }
         else
         {
            log.Failed(EventCode.Failed);
         }
         return log;
      }

      #endregion
      #region -- 4.00 - Binary, Text and Json Serialization Support

      /// <summary>
      /// Items to JSON.
      /// </summary>
      /// <param name="name"></param>
      /// <param name="items"></param>
      /// <returns></returns>
      public static string ToJson(string name, List<dynamic> items)
      {
         System.Text.StringBuilder sb = new StringBuilder();
         sb.AppendLine("{ \"" + name + "\": [");
         int iCount = items.Count;
         int c = 1;
         foreach (var i in items)
         {
            sb.AppendLine(ModelExpandoObject.ToJson(i)
               + (c != iCount ? "," : String.Empty));
            c++;
         }
         sb.AppendLine("] }");
         return sb.ToString();
      }

      /// <summary>
      /// Get dynamic ObservableCollection from JSON text.
      /// </summary>
      /// <param name="jsonText">JSON text to parse</param>
      /// <returns>instance of ObervableCollection</returns>
      public static ObservableCollection<dynamic> FromJson(string jsonText)
      {
         JObject jobj = JsonSerializer.ToDynamic(jsonText);
         if (jobj.Type != JTokenType.Object)
         {
            return null;
         }
         JProperty prop = jobj.First as JProperty;
         if (prop.Type != JTokenType.Property)
         {
            return null;
         }
         JArray arr = prop.First as JArray;
         if (arr.Type != JTokenType.Array)
         {
            return null;
         }

         ObservableCollection<dynamic> items =
            new ObservableCollection<dynamic>();
         JObject itm = arr.First as JObject;
         while (itm != null)
         {
            ModelExpandoObject eobject = new ModelExpandoObject();
            prop = itm.First as JProperty;
            while (prop != null)
            {
               if (prop.Type != JTokenType.Property)
               {
                  continue;
               }
               object val;
               switch (prop.Value.Type)
               {
                  case JTokenType.Integer:
                     val = prop.Value.Value<int>();
                     break;
                  case JTokenType.Float:
                     val = prop.Value.Value<float>();
                     break;
                  case JTokenType.Date:
                     val = prop.Value.Value<DateTime>();
                     break;
                  case JTokenType.TimeSpan:
                     val = prop.Value.Value<TimeSpan>();
                     break;
                  case JTokenType.Boolean:
                     val = prop.Value.Value<bool>();
                     break;
                  default:
                     val = prop.Value.ToString();
                     break;
               }
               eobject.AddProperty(prop.Name, val);
               prop = prop.Next as JProperty;
            }
            items.Add(eobject.Instance);
            itm = itm.Next as JObject;
         }
         return items;
      }

      #endregion

   }

}
