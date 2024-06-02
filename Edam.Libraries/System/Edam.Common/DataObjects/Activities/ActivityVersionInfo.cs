using System;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.DataObjects.References;

namespace Edam.DataObjects.Activities
{

   public enum ActivityVersionOption
   {
      Unknown = 0,
      ReferenceOnly = 1,
      All = 99
   }

   public class ActivityVersionInfo
   {

      public ReferenceVersionInfo Version { get; set; }
      public List<ActivityContentVersionInfo> Content { get; set; }

      static public List<ActivityVersionInfo> ToActivityVersion(
         List<ReferenceVersionInfo> versions, 
         List<ActivityContentVersionInfo> content)
      {
         List<ActivityVersionInfo> l = new List<ActivityVersionInfo>();
         ActivityVersionInfo a;
         foreach(var v in versions)
         {
            a = new ActivityVersionInfo();
            a.Version = v;
            a.Content = content.FindAll(x => x.VersionNo == v.VersionNo);
            l.Add(a);
         }
         return l;
      }

   }

}
