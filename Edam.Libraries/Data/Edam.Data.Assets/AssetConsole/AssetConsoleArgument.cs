using System;
using System.Globalization;

// -----------------------------------------------------------------------------

namespace Edam.Data.AssetConsole
{
   
   public class AssetConsoleArgument
   {
      public String ArgumentName { get; set; }
      public String Value { get; set; }
      public Boolean IsArgument { get; set; }
      public int Index { get; set; }
      public AssetConsoleArgument()
      {
         IsArgument = false;
         ArgumentName = String.Empty;
         Value = String.Empty;
         Index = 0;
      }

      public static bool IsArgumentName(string name)
      {
         return name.StartsWith("/") || name.StartsWith("-");
      }

      /// <summary>
      /// Parse argument-value pair...
      /// </summary>
      /// <param name="args"></param>
      /// <param name="index"></param>
      /// <returns></returns>
      public static AssetConsoleArgument Parse(string[] args, ref int index)
      {
         if (index >= args.Length)
            return null;

         AssetConsoleArgument a = new AssetConsoleArgument();

         string argumentText = args[index];
         string[] largs = argumentText.Split(":");

         string firstToken = largs[0];
         string secondToken = argumentText.Substring(firstToken.Length+1);

         // argument and value is all in one token (-arg:value)
         if (largs.Length > 1 && IsArgumentName(firstToken) &&
            !String.IsNullOrWhiteSpace(secondToken))
         {
            a.ArgumentName = 
               largs[0].Substring(1, firstToken.Length - 1).ToLower().Trim();

            a.Value = secondToken.Trim();
            a.IsArgument = true;
            a.Index = index;

            index++;
            return a;
         }

         // argument and value are separated by a space (-arg: value)
         if (IsArgumentName(argumentText))
         {
            index++;
            if (index >= args.Length)
               return null;

            a.IsArgument = true;
            int colonPos = argumentText.IndexOf(":");
            if (colonPos != -1)
            {
               a.Index = index;
               a.Value = args[index];
               a.ArgumentName = argumentText.Substring(1, colonPos - 1);
            }
            index++;
         }
         a.ArgumentName.ToLower(CultureInfo.InvariantCulture);
         return a;
      }
   }

}
