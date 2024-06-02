using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Application;
using Edam.Data.Asset;

// -----------------------------------------------------------------------------
using Edam.Data.AssetConsole;
using Edam.Diagnostics;
using Edam.InOut;

namespace Edam.Data.Asset
{

   public class CodeSet
   {
      public const string TAB_CODE_SET = "CodeSet";
      public const string TAB_ELEMENT_CODE_SET = "ElementCodeSet";

      private static List<string> Header = new List<string>()
      {
         "CodeSetUri",
         "OrganizationId",
         "VersionId",
         "CodeSetName",
         "CodeSetId",
         "CodeId",
         "AlternateId",
         "Description",
         "CategoryId",
         "DataOwnerId"
      };

      private static bool Same(List<string> items)
      {
         if (items == null || items.Count() != Header.Count())
         {
            return false;
         }

         bool same = true;
         for(int i = 0; i < items.Count; i++)
         {
            if (items[i] != Header[i])
            {
               same = false;
               break;
            }
         }
         return same;
      }

      private static CodeSetInfo GetCodeSet(List<string> row)
      {
         CodeSetInfo codeSet = new CodeSetInfo();
         codeSet.CodeSetNo = -1;
         codeSet.CodeSetUri = row[0];
         codeSet.OrganizationId = row[1];
         codeSet.VersionId = row[2];
         codeSet.CodeSetName = row[3];
         codeSet.CodeSetId = row[4];
         codeSet.DataOwnerId = row[9];
         codeSet.RecordStatusCode = "A";
         return codeSet;
      }

      private static CodeInfo GetCode(List<string> row)
      {
         CodeInfo code = new CodeInfo();
         code.IdNo = -1;
         code.CodeId = row[5];
         code.AlternateId = row[6];
         code.Description = row[5];
         code.CategoryId = row[8];
         return code;
      }

      public static ResultsLog<List<CodeSetInfo>> ToCodeSet(
         List<List<string>> items)
      {
         ResultsLog<List<CodeSetInfo>> results =
            new ResultsLog<List<CodeSetInfo>>();

         if (items == null || items.Count == 0)
         {
            results.Failed(EventCode.ListIsEmpty);
            return results;
         }

         if (!Same(items[0]))
         {
            results.Failed(EventCode.HeadersMismatch);
            return results;
         }

         int setCount = 0;
         int codeCount = 0;
         CodeSetInfo codeSet;
         CodeSetInfo set;
         CodeInfo code;

         List<CodeSetInfo> codes = new List<CodeSetInfo>();

         Dictionary<string, CodeSetInfo> visited = 
            new Dictionary<string, CodeSetInfo>();

         foreach (var row in items.Skip(1))
         {
            set = GetCodeSet(row);
            code = GetCode(row);

            if (String.IsNullOrWhiteSpace(set.CodeSetId))
            {
               continue;
            }

            if (!visited.TryGetValue(set.CodeSetId, out codeSet))
            {
               setCount++;
               set.CodeSetNo = setCount;
               visited.Add(set.CodeSetId, set);
               codes.Add(set);
               codeSet = set;
            }

            codeCount++;
            code.IdNo = codeCount;
            code.CodeSetNo = codeSet.CodeSetNo;
            code.VersionId = codeSet.VersionId;
            code.OrganizationId = codeSet.OrganizationId;
            code.DataOwnerId = codeSet.DataOwnerId;
            codeSet.Codes.Add(code);
         }

         results.Data = codes;
         results.Succeeded();
         return results;
      }

   }

}
