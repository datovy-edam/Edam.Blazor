using asset = Edam.Data.Asset;
using Edam.Data.AssetReport;
using Edam.Data.AssetSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Text;
using Edam.Data.AssetManagement;

namespace Edam.Data.Assets.AssetReport
{

   public class ElementMapper
   {
      private const string TYPE_TYPE = "type";
      private const string TYPE_ROOT = "root";
      private const string ENUM = "enumerator";

      private const string TAG_PII = "PII";
      private const string TAG_PCI = "PCI";
      private const string TAG_SENSITIVE = "Sensitive";
      private const string TAG_CONFIDENTIAL = "Confidential";

      private IAssetElement _Element;
      private ITableReport _Report;
      private ITableRowHeader _RowHeader { get; set; }
      private DataTextMap _TextMap;

      private Type _Type;

      public Func<string> EntityNameFunc { get; set; }
      public Func<string> ElementNameFunc { get; set; }
      public Func<string> ElementDataTypeFunc { get; set; }

      public string Annotation
      {
         get { return AssetDataElement.GetAnnotattion(_Element); }
      }

      public string BusinessArea
      {
         get { return _Element.Domain; }
      }

      public string ItemStatus
      {
         get { return _Element.AssetStatus?.ToString(); }
      }

      public string Comments
      {
         get { return _Element.CommentText; }
      }

      public string DataType
      {
         get { return GetDataType(_Element.DataType ?? _Element.TypeName); }
      }

      public string Entity
      {
         get
         {
            return EntityNameFunc();
         }
      }

      public string EntityQualifiedNameText
      {
         get { return _Element.EntityQualifiedNameText; }
      }

      public string EntityName
      {
         get
         {
            return _Element.EntityQualifiedName == null ?
               String.Empty : _Element.EntityQualifiedName.OriginalName;
         }
      }

      public string Element
      {
         get
         {
            return ElementNameFunc();
         }
      }

      public string ElementQualifiedNameText
      {
         get
         {
            return (_Element.ElementType == asset.ElementType.attribute ?
            "@" : string.Empty) + _Element.ElementQualifiedNameText;
         }
      }

      public string ElementNo
      {
         get { return _Element.ElementNo.ToString(); }
      }

      public string ElementType
      {
         get { return _Element.ElementType.ToString(); }
      }

      public string ElementName
      {
         get
         {
            return _Element.ElementQualifiedName.OriginalName;
         }
      }

      public string FullPath
      {
         get { return _Element.GetFullPath(); }
      }

      public string MaxLength
      {
         get { return _Element.Length.HasValue ?
            _Element.Length.Value.ToString() : string.Empty; }
      }

      public string LastUpdateDate
      {
         get { return _Element.LastUpdateDate?.ToString(); }
      }

      public string Occurrence
      {
         get { return _Element.Occurs.Replace(" ", ""); }
      }

      public string Precision
      {
         get
         {
            return _Element.Precision.HasValue ?
               _Element.Precision.Value.ToString() : String.Empty;
         }
      }

      public string Scale
      {
         get
         {
            return _Element.Scale.HasValue ?
               _Element.Scale.Value.ToString() : String.Empty;
         }
      }

      public string TagConfidential
      {
         get
         {
            return _Element.Tags.Contains(TAG_CONFIDENTIAL) ?
               TAG_CONFIDENTIAL : String.Empty;
         }
      }

      public string TagSensitive
      {
         get
         {
            return _Element.Tags.Contains(TAG_SENSITIVE) ?
               TAG_SENSITIVE : String.Empty;
         }
      }

      public string TagPII
      {
         get
         {
            return _Element.Tags.Contains(TAG_PII) ? TAG_PII : String.Empty;
         }
      }

      public string TagPCI
      {
         get
         {
            return _Element.Tags.Contains(TAG_PCI) ? TAG_PCI : String.Empty;
         }
      }

      public uint RowStyle
      {
         get
         {
            uint styleNo;
            string etype = ElementType.ToLower();
            if (etype == TYPE_TYPE || etype == TYPE_ROOT)
            {
               styleNo = (uint)TableRowStyle.Fill4Border1Font12;
            }
            else if (etype.ToLower() == ENUM)
            {
               styleNo = (uint)TableRowStyle.Fill1Border1Font12;
            }
            else
               styleNo = (uint)TableRowStyle.Fill1Font12;
            return styleNo;
         }
      }

      public string SampleValue
      {
         get { return _Element.SampleValue; }
      }

      public string URI
      {
         get { return _Element.Namespace; }
      }

      public string Value
      {
         get { return string.IsNullOrWhiteSpace(_Element.DefaultValue) ?
            string.Empty : _Element.DefaultValue;
         }
      }

      /// <summary>
      /// Initialize Mapper
      /// </summary>
      /// <param name="item"></param>
      /// <param name="report"></param>
      /// <param name="textMapping"></param>
      public ElementMapper(
         IAssetElement item, ITableReport report, DataTextMap textMapping)
      {
         _Element = item;
         _Report = report ?? new ReportInfo();
         _RowHeader = report.RowHeader;
         _Type = this.GetType();
         _TextMap = textMapping;

         if (report.Options.ShowFullyQualifiedNames)
         {
            EntityNameFunc = () => { return EntityQualifiedNameText; };
            ElementNameFunc = () => { return ElementQualifiedNameText; };
         }
         else if (_Element.IsType)
         {
            if (_Report.Options.SetElementTypeAsEntity)
            {
               EntityNameFunc = () => { return TrimLast(ElementName, "_"); };
               ElementNameFunc = () => { return String.Empty; };
            }
            else
            {
               EntityNameFunc = () => { return EntityName; };
               ElementNameFunc = () => { return TrimLast(ElementName, "_"); };
            }
         }
         else
         {
            EntityNameFunc = () => { return TrimLast(EntityName, "_"); };
            ElementNameFunc = () => { return ElementName; };
         }
      }

      /// <summary>
      /// Trim last character if simmilar to given text to trim.
      /// </summary>
      /// <param name="text"></param>
      /// <param name="textToTrimLast"></param>
      /// <returns></returns>
      private string TrimLast(string text, string textToTrimLast)
      {
         string outText = text;
         if (text.EndsWith(textToTrimLast))
         {
            outText = text.Remove(text.Length - textToTrimLast.Length);
         }
         return outText;
      }

      /// <summary>
      /// Get Data Type.
      /// </summary>
      /// <param name="typeName"></param>
      /// <returns>return data type text</returns>
      private string GetDataType(string typeName)
      {
         if (_TextMap == null)
         {
            return typeName;
         }
         var val = _TextMap.MapText(typeName, DataTextMapDirection.To);
         if (val == null || string.IsNullOrWhiteSpace(val))
         {
            if (_Element.QualifiedTypeNames.Count > 0)
            {
               val = _Element.QualifiedTypeNames[0].OriginalName;
            }
         }
         return val;
      }

      /// <summary>
      /// Get Text Value with given property name.
      /// </summary>
      /// <param name="propertyName">property name</param>
      /// <returns>text value is returned</returns>
      public string GetValueText(string propertyName)
      {
         var prop = _Type.GetProperty(propertyName);
         if (prop == null)
         {
            return String.Empty;
         }
         var value = prop.GetValue(this, null);
         if (value == null)
         {
            return String.Empty;
         }
         return value.ToString();
      }

      /// <summary>
      /// Get Values.
      /// </summary>
      /// <remarks>if a value is specified in the header and none is found on
      /// the _Element then the header value is returned and assumed to be the
      /// default value</remarks>
      /// <returns>number of found values are returned</returns>
      public int GetValues()
      {
         int cnt = 0;
         foreach(var item in _RowHeader.Items)
         {
            item.Value = String.Empty;
            var value = GetValueText(item.ItemName);
            if (String.IsNullOrWhiteSpace(value))
            {
               value = String.IsNullOrWhiteSpace(item.Value == null ?
                  String.Empty : item.Value.ToString()) ?
                  String.Empty : item.Value.ToString();
            }
            item.Value = value;
            cnt++;
         }
         return cnt;
      }

   }

}
