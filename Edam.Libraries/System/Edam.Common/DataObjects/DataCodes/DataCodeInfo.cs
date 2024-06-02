using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

using Edam.Serialization;

namespace Edam.DataObjects.DataCodes
{

   public class DataCodeHelper
   {

      public static string ToJson(List<DataCodeInfo> items)
      {
         return JsonSerializer.Serialize<List<DataCodeInfo>>(items);
      }

      public static List<DataCodeInfo> FromJson(string jsonText)
      {
         if (String.IsNullOrWhiteSpace(jsonText))
         {
            return null;
         }
         return JsonSerializer.Deserialize<List<DataCodeInfo>>(jsonText);
      }

   }

   /// <summary>
   /// Helper to get commonly used lists.
   /// </summary>
   public class DataCodeInfo : IDataCode
   {
      public const string CODE_ID = "CodeId";
      public const string DESCRIPTION = "Description";

      public String GroupId { get; set; }
      public String CodeId { get; set; }
      public String Value { get; set; }
      public String Description
      {
         get { return Value; }
         set { Value = value; }
      }
      public String HelpText { get; set; }
      public Objects.ObjectStatus Status { get; set; }
      public Boolean Selected { get; set; }

      public Int16? CodeNo
      {
         get
         {
            if (Int16.TryParse(CodeId, out short result))
               return result;
            return null;
         }
         set { CodeId = value.ToString(); }
      }

      public DataCodeInfo()
      {
         ClearFields();
      }

      public void ClearFields()
      {
         GroupId = String.Empty;
         CodeId = String.Empty;
         HelpText = String.Empty;
         Status = Objects.ObjectStatus.Unknown;
         Selected = false;
      }

#if DATA_SUPPORT_

      /// <summary>
      /// Read Data.
      /// </summary>
      /// <param name="reader">data reader</param>
      /// <returns>a DataCodeValueInfo is returned</returns>
      public static DataCodeValueInfo ReadData(
         System.Data.Common.DbDataReader reader)
      {
         DataCodeValueInfo item = new DataCodeValueInfo();

         if (reader.FieldCount == 2)
         {
            item.GroupId = String.Empty;
            item.CodeId = DataField.GetString(reader[0]);
            item.Value = DataField.GetString(reader[1]);
         }
         else if (reader.FieldCount == 3)
         {
            String grpText = reader[0].ToString();
            item.GroupId = DataField.GetString(grpText);
            item.CodeId = DataField.GetString(reader[1]);
            item.Value = DataField.GetString(reader[2]);
         }
         else
         {
            String grpText = reader[0].ToString();
            item.GroupId = DataField.GetString(grpText);
            item.CodeId = DataField.GetString(reader[1]);
            item.Value = DataField.GetString(reader[2]);
            item.HelpText = DataField.GetString(reader[3]);
            item.Status = (Objects.ObjectStatus)DataField.GetInt16(reader[4]);
            item.Selected = DataField.GetBool(reader[5]);
         }
         return item;
      }

      /// <summary>
      /// Read code list.
      /// </summary>
      /// <param name="code">code type</param>
      /// <param name="reader">data reader</param>
      /// <returns>List of codes is returned</returns>
      public static List<IDataCodeValue> GetList(
         System.Data.Common.DbDataReader reader,
         DataCode code = DataCode.Uknown)
      {
         DataCodeValueInfo c;
         List<IDataCodeValue> l = new List<IDataCodeValue>();
         while(reader.Read())
         {
            c = ReadData(reader);
            l.Add(c);
         }
         return l;
      }

#endif

   }

}
