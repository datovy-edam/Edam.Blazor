using System;
using System.Numerics;

namespace Edam.Data.AssetManagement
{
   public class OccuranceInfo
   {
      public const decimal MAX_NUMBER = 79228162514264337593543950335M;

      public decimal MinOccurance { get; set; }
      public decimal MaxOccurance { get; set; }
      public string Text
      {
         get
         {
            string max = (MaxOccurance >= Int32.MaxValue) ?
               "*" : MaxOccurance.ToString();
            return "(" + MinOccurance.ToString() + ":" + max + ")";
         }
         set
         {
            var o = Parse(value);
            MinOccurance = o.MinOccurance;
            MaxOccurance = o.MaxOccurance;
         }
      }
      public OccuranceInfo()
      {
         MinOccurance = 0;
         MaxOccurance = 1;
      }
      public static decimal GetInt32MaxOccurence(decimal occurrence)
      {
         return occurrence >= MAX_NUMBER ? Int32.MaxValue : occurrence;
      }
      public OccuranceInfo(string occuranceText)
      {
         Text = occuranceText;
      }
      public OccuranceInfo(decimal minOccurance, decimal maxOccurance)
      {
         MinOccurance = minOccurance;
         MaxOccurance = maxOccurance;
      }
      public OccuranceInfo(int? minOccurance, int? maxOccurance)
      {
         MinOccurance = minOccurance.HasValue ? minOccurance.Value : 0;
         MaxOccurance = maxOccurance.HasValue ? maxOccurance.Value : 0;
      }
      public static OccuranceInfo Parse(string occuranceText)
      {
         OccuranceInfo o = new OccuranceInfo();
         if (String.IsNullOrWhiteSpace(occuranceText))
            return o;
         var txt = occuranceText.Replace(" ", "").
            Replace("(","").Replace(")","");
         var l = txt.Split(":");
         if (l.Length != 2)
            return o;
         if (!(int.TryParse(l[0], out int mn) ||
            String.IsNullOrWhiteSpace(l[0])))
            mn = 0;

         int mx;
         if (l[1] == "*")
         {
            mx = int.MaxValue;
         }
         else if (!(int.TryParse(l[1], out mx) ||
            String.IsNullOrWhiteSpace(l[1])))
         {
            mx = 1;
         }

         o.MinOccurance = mn;
         o.MaxOccurance = mx;
         return o;
      }
   }
}
