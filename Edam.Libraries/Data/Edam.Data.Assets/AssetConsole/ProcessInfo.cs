using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Edam.Data.AssetConsole
{

   public class ProcessInfo
   {
      public String RecordId { get; set; }
      public String Name { get; set; }

      public String OrganizationId { get; set; }
      public String OrganizationDomainUri { get; set; }

      // procedure to execute
      public String ProcedureName { get; set; }
      public String ProcedureTag { get; set; }
      public SchemaType? SchemaType { get; set; }
      public bool ScanFilesFolder { get; set; } = false;

      [IgnoreDataMember]
      public AssetConsoleProcedure Procedure
      {
         get { return ToProcedure(ProcedureName); }
         set { ProcedureName = value.ToString(); }
      }

      public List<DataMapItemInfo> MapItem { get; set; }

      public string NextProcess { get; set; }
      public List<string> NextProcedure { get; set; }

      public ProcessInfo()
      {
         NextProcedure = new List<string>();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="procedureName"></param>
      /// <returns></returns>
      public static AssetConsoleProcedure ToProcedure(String procedureName)
      {
         return String.IsNullOrWhiteSpace(procedureName) ?
               AssetConsoleProcedure.Unknown : (AssetConsoleProcedure)
               Enum.Parse(typeof(AssetConsoleProcedure), procedureName, true);
      }

   }

}
