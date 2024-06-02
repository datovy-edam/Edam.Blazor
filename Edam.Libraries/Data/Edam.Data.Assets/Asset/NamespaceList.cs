using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.Asset
{

   public class NamespaceList : List<NamespaceInfo>
   {
      public const string DEFAULT_PREFIX = "unk";

      public NamespaceInfo DefaultNamespace { get; set; }
      public NamespaceList() : base()
      {

      }
      public NamespaceList(List<NamespaceInfo> list)
      {
         Add(list);
      }

      /// <summary>
      /// Get Default Prefix.  If no Default Namespace has been defined "unk"
      /// (or the value of DEFAULT_PREFIX) will be returned...
      /// </summary>
      /// <returns></returns>
      public string GetDefaultPrefix(string name = null)
      {
         if (name != null)
         {
            var l = name.Split(':');
            if (l.Length == 2)
            {
               return l[0];
            }
         }
         if (DefaultNamespace == null)
         {
            SetDefaultNamespace();
            if (DefaultNamespace == null)
               return DEFAULT_PREFIX;
         }
         return DefaultNamespace.Prefix;
      }

      /// <summary>
      /// Set Default Namespace.
      /// </summary>
      /// <param name="prefix"></param>
      /// <param name="uriText"></param>
      public void SetDefaultNamespace(string prefix, string uriText)
      {
         var i = Find((x) => (x.Uri.OriginalString == uriText));
         if (i != null)
         {
            i = new NamespaceInfo(prefix, uriText);
            Add(i);
         }
         DefaultNamespace = i;
      }

      public void SetDefaultNamespace()
      {
         if (this.Count == 0)
            return;
         DefaultNamespace = this[0];
      }

      public void Add(string prefix, string uri)
      {
         NamespaceInfo ns = new NamespaceInfo(prefix, uri);
         var fns = this.Find((x) => x.Prefix == prefix);
         if (fns == null)
            Add(ns);
      }

      public new void Add(NamespaceInfo ns)
      {
         var i = Find((x) => (x.Uri.OriginalString == ns.Uri.OriginalString));
         if (i != null)
            return;
         base.Add(ns);
         SetDefaultNamespace();
      }
      public void Add(List<NamespaceInfo> list)
      {
         foreach(var i in list)
         {
            Add(i);
         }
      }
   }

}
