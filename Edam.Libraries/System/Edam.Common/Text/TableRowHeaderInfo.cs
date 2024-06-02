using Edam.Serialization;
using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Edam.Text
{

   /// <summary>
   /// This is defines the headers list to be used, basically is a list of 
   /// column header names.
   /// </summary>
   public class TableRowHeaderInfo : ITableRowHeader
   {

      public const string DEFAULT_MAIN_HEADER =
         "AssetNo,AssetStatus,LastUpdateDate," +
         "Entity,Element,Value,Type,ElementType,Length,Occurs,Comment," +
         "Annotation,Sample,Namespace,FullPath";

      public List<TableColumnInfo> Items { get; set; } = 
         new List<TableColumnInfo>();

      public TableRowHeaderInfo()
      {
      }

      /// <summary>
      /// Create a header given a comma delimited string of headers.
      /// </summary>
      /// <param name="commaDelimitedText">comma delimited string of headers. 
      /// if text is empty or null then the DEFAULT_MAIN_HEADER will be used
      /// instead</param>
      /// <returns>instance of list is returned</returns>
      public static List<TableColumnInfo> GetItems(
         string commaDelimitedText = null)
      {
         if (String.IsNullOrWhiteSpace(commaDelimitedText))
         {
            commaDelimitedText = DEFAULT_MAIN_HEADER;
         }

         var l = commaDelimitedText.Split(',');
         var items = new List<TableColumnInfo>();
         foreach (var item in l)
         {
            items.Add(new TableColumnInfo(name: item));
         }
         return items;
      }

      /// <summary>
      /// Get the headers as a comma delimited text string.
      /// </summary>
      /// <returns>text string is returned</returns>
      public string GetCommaDelimitedHeaders()
      {
         string csv = String.Empty;
         foreach (var item in Items)
         {
            csv += csv == String.Empty ?
               item.ElementName : ',' + item.ElementName;
         }
         return csv;
      }

      /// <summary>
      /// Get header items list from a JSON file.
      /// </summary>
      /// <param name="filePath">JSON file path</param>
      /// <returns>list of headers is returned</returns>
      public static TableRowHeaderInfo FromJson(string filePath)
      {
         string jsonText = System.IO.File.ReadAllText(filePath);
         var results =
            JsonSerializer.TryDeserialize<TableRowHeaderInfo>(jsonText);
         if (results.Success)
         {
            int index = 0;
            foreach (var item in results.Data.Items)
            {
               item.Index = index;
               item.Validate();
               index++;
            }
         }
         return results.Success ? results.Data : null;
      }

      /// <summary>
      /// Find a column header name in existing list.
      /// </summary>
      /// <param name="name">name to find</param>
      /// <returns>instance of an Asset Column Item is returned</returns>
      public TableColumnInfo Find(string name)
      {
         return Items.Find((x) => x.Name == name);
      }

      /// <summary>
      /// Add a column header name.
      /// </summary>
      /// <param name="name">name to add</param>
      /// <returns>instance of column-item is returned</returns>
      public TableColumnInfo Add(string name)
      {
         TableColumnInfo itm = Find(name);
         if (itm == null)
         {
            itm = new TableColumnInfo
            {
               Name = name,
               Index = Items.Count
            };
            Items.Add(itm);
         }
         return itm;
      }

      /// <summary>
      /// Add header column...
      /// </summary>
      /// <param name="index">column index</param>
      /// <param name="name">column name</param>
      /// <param name="hidden">true if hidden</param>
      /// <param name="styleNo">stype No</param>
      /// <returns></returns>
      public TableColumnInfo Add(
         int index, string name, bool hidden, uint styleNo)
      {
         TableColumnInfo itm = Find(name);
         if (itm == null)
         {
            itm = new TableColumnInfo
            {
               Name = name,
               Index = index,
               Hidden = hidden,
               StyleNo = styleNo
            };
            Items.Add(itm);
         }
         return itm;
      }


      /// <summary>
      /// Add all given Column Items as column headers.
      /// </summary>
      /// <param name="items">list of column-items to add</param>
      public void Add(List<TableColumnInfo> items)
      {
         foreach (var i in items)
            Add(i.Name);
      }

      /// <summary>
      /// Given a comma delimited header in the form header1, header2, ...
      /// parse it and add it
      /// </summary>
      /// <param name="header"></param>
      public List<TableColumnInfo> AddCommaDelimitedHeader(
         string header, bool hidden = false, uint styleNo = 0U)
      {
         List<TableColumnInfo> items = new List<TableColumnInfo>();
         string[] list = header.Split(",");
         int indx = 0;
         foreach (var item in list)
         {
            items.Add(new TableColumnInfo() 
            { 
               Index = indx, Name = item, Hidden = hidden, StyleNo = styleNo
            });
            indx++;
         }

         Items.AddRange(items);

         return items;
      }

      /// <summary>
      /// Get headers as list of strings...
      /// </summary>
      /// <returns></returns>
      public List<string> ToList()
      {
         var list = new List<string>();
         foreach(var i in Items)
         {
            list.Add(i.Name);
         }
         return list;
      }

   }

}
