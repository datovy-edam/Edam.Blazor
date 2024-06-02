using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

// -----------------------------------------------------------------------------
using Edam.TextParse;
using ixone = Edam.Serialization;
using Edam.Text;

namespace Edam.Data.AssetSchema
{

   public enum AssetProcessType
   {
      Unknown = 0,
      UseCaseDeclaration = 1,
      Map = 2,
      Tag = 3,
      MapTo = 4,
      MapInstruction = 5,
      MapFunction = 6,
      Type = 7,
      Comment = 8,
      Custom = 99
   }

   public class AssetProcess
   {
      public const string AM_MAP = "ammap";
      public const string TK_TAG = "tag";
      public const string TK_TO = "to";
      public const string TK_MAP = "map";
      public const string TK_INSTRUCTIONS = "instructions";
      public const string NM_INSTRUCTIONS = "Instructions";
      public const string TK_FUNCTION = "func";
      public const string NM_FUNCTION = "Function";
      public const string TK_NM_FUNCTION = "function";

      public const string TK_TYPE = "type";
      public const string TK_MAP_TO = "mapto";
      public const string NM_MAP_TO = "MapTo";
      public const string TK_USECASE = "usecase";

      private TableRowHeaderInfo m_Columns { get; set; }

      public AssetProcessInfo CurrentInstruction { get; set; }
      public List<AssetProcessInfo> Items { get; set; }

      public TableRowHeaderInfo Columns
      {
         get => m_Columns;
      }

      public AssetProcess()
      {
         m_Columns = new TableRowHeaderInfo();
         Items = new List<AssetProcessInfo>();
      }

      public AssetProcessInfo ParseProcessingInstruction(
         string nodeName, string value)
      {
         AssetProcessInfo i = new AssetProcessInfo();

         var dic = KeyValueParser.TextToKeyValue(value, "\" ", '=');
         if (nodeName.ToLower() == AM_MAP)
         {
            i.Type = AssetProcessType.Map;
            foreach (var t in dic)
            {
               AssetProcessItem p = new AssetProcessItem();
               var key = t.Key.ToLower();
               if (key == TK_TO)
               {
                  p.Column.Name = NM_MAP_TO;
                  p.Type = AssetProcessType.MapTo;
                  p.Value = t.Value;
               }
               else if (key == TK_FUNCTION)
               {
                  p.Column.Name = NM_FUNCTION;
                  p.Type = AssetProcessType.MapFunction;
                  p.Value = t.Value;
               }
               else if (key == TK_INSTRUCTIONS)
               {
                  p.Column.Name = NM_INSTRUCTIONS;
                  p.Type = AssetProcessType.MapInstruction;
                  p.Value = t.Value;
               }
               else
               {
                  p.Column.Name = key;
                  p.Type = AssetProcessType.Custom;
                  p.Value = t.Value;
               }
               i.Items.Add(p);
            }
         }
         SetCurrentInstruction(i);
         return i;
      }

      public void SetCurrentInstruction(AssetProcessInfo process)
      {
         if (process == null || process.Items.Count == 0)
         {
            CurrentInstruction = null;
            return;
         }
         foreach(var i in process.Items)
         {
            m_Columns.Add(i.Column.Name);
         }
         Items.Add(process);
         CurrentInstruction = process;
      }
   }

   public class AssetProcessItem
   {
      public TableColumnInfo Column { get; set; }
      public AssetProcessType Type { get; set; }
      public string Value { get; set; }

      public AssetProcessItem()
      {
         Column = new TableColumnInfo();
      }
   }

   public class AssetProcessInfo
   {
      public List<AssetProcessItem> Items { get; set; }

      [JsonIgnore]
      public object Parent { get; set; }

      public string UseCaseName { get; set; }
      public AssetProcessType Type { get; set; }

      public AssetProcessItem Tag { get; set; }

      public int SequenceNo { get; set; }

      public AssetProcessInfo()
      {
         Parent = null;
         Items = new List<AssetProcessItem>();
         SequenceNo = 0;
      }

      public string ToJson()
      {
         return ixone.JsonSerializer.Serialize<AssetProcessInfo>(this);
      }

      public static AssetProcessInfo FromJson(string jsonText, object parent)
      {
         if (String.IsNullOrWhiteSpace(jsonText))
         {
            var itm = new AssetProcessInfo();
            itm.Parent = parent;
            return itm;
         }
         return ixone.JsonSerializer.Deserialize<AssetProcessInfo>(jsonText);
      }

      public bool FromJsonText(string jsonText, object parent)
      {
         var itm = ixone.JsonSerializer.Deserialize<AssetProcessInfo>(jsonText);
         if (itm != null)
         {
            Parent = parent;
            SequenceNo = itm.SequenceNo;
            UseCaseName = itm.UseCaseName;
            Type = itm.Type;
            Tag = itm.Tag;
            return true;
         }
         return false;
      }

   }

}
