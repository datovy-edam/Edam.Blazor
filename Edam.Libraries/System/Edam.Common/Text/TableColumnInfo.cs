using System;
namespace Edam.Text
{

   public class TableColumnInfo : ITableHeaderItem
   {

      public int Index { get; set; } = -1;
      public string ItemName { get; set; }
      public string ElementName { get; set; }
      public string Name { get; set; } = string.Empty;
      public string Description { get; set; }
      public string Format { get; set; }
      public object Value { get; set; }
      public bool Hidden { get; set; } = false;
      public uint StyleNo { get; set; } = 0U;

      public TableColumnInfo()
      {

      }

      public TableColumnInfo(
         string elementName = null, string name = null,
         string description = null, string format = null)
      {
         ElementName = elementName;
         Name = name;
         Description = description;
         Format = format;
      }

      /// <summary>
      /// Validate and setup properties as needed.
      /// </summary>
      public void Validate()
      {
         if (String.IsNullOrWhiteSpace(ElementName))
         {
            ElementName = Name;
         }
         else if (String.IsNullOrWhiteSpace(Name))
         {
            Name = ElementName;
         }
         if (String.IsNullOrWhiteSpace(Description))
         {
            Description = Name;
         }
      }

   }

}
