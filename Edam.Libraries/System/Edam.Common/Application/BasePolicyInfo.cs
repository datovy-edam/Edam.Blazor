using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edam.Application
{

   /// <summary>
   /// Application Policy Info...
   /// </summary>
   public class BasePolicyInfo
   {

      public Int32  PolicyNo { get; set; }
      public String Description { get; set; }
      public String Value { get; set; }

      public PolicyType PolicyType
      {
         get { return (PolicyType)PolicyNo; }
         set { PolicyNo = (Int32)value; }
      }

      /// <summary>
      /// Clear Fields...
      /// </summary>
      public void ClearFields()
      {
         PolicyNo = 0;
         Description = String.Empty;
         Value = String.Empty;
      }

      public static BasePolicyInfo GetPolicy(PolicyType policy)
      {
         return new BasePolicyInfo
         {
            PolicyType = policy,
            Description = policy.ToString(),
            Value = "true"
         };
      }
   }

}
