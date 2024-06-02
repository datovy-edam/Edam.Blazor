using Edam.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Application
{

   public class ApplicationHelper
   {
      public const string IDENTITY_CONNECTION_KEY = "IdentityConnectionKey";
      public const string IDENTITY_SCOPE = "IdentityScope";
      public const string IDENTITY_SCOPE_LOCAL = "Local";
      public const string IDENTITY_SCOPE_REMOTE = "Remote";

      public static string GetString(string key)
      {
         var rhelper = DependencyService.Get<IApplicationResource>();
         if (rhelper == null)
            return String.Empty;
         return rhelper.GetString(key);
      }

      public static string GetLocalString(string key)
      {
         var rhelper = DependencyService.Get<IApplicationResource>();
         if (rhelper == null)
            return String.Empty;
         return rhelper.GetLocalString(key);
      }

   }

}
