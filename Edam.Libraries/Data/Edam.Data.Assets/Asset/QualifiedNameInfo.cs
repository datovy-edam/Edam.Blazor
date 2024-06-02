using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Data.Asset
{

   public class QualifiedNameInfo
   {
      private String m_Name;

      public String Prefix { get; set; }
      public String OriginalName { get; set; }
      public String Name
      {
         get => GetName();
         set => Parse(value);
      }

      public Boolean IsBaseType
      {
         get { return isBaseType();  }
      }

      public QualifiedNameInfo(String qname)
      {
         Parse(qname);
      }

      public QualifiedNameInfo()
      {

      }

      public QualifiedNameInfo(string prefix, string name)
      {
         Name = name;
         Prefix = prefix;
      }

      public string Parse(string qname)
      {
         if (String.IsNullOrWhiteSpace(qname))
         {
            Prefix = String.Empty;
            OriginalName = String.Empty;
            return String.Empty;
         }
         var l = GetPrefix(qname);
         if (l.Count == 2)
         {
            Prefix = l[0];
            OriginalName = l[1];
         }
         else if (l.Count == 1)
         {
            Prefix = String.Empty;
            OriginalName = l[0];
         }
         else
         {
            Prefix = String.Empty;
            OriginalName = Prefix;
         }
         GetName();
         if (String.IsNullOrWhiteSpace(Prefix))
         {
            Prefix = ElementBaseTypeInfo.XS;
         }
         return m_Name;
      }

      public string GetName()
      {
         m_Name = (Prefix == null ? String.Empty : Prefix + ":") +
              (OriginalName ?? String.Empty);
         return m_Name;
      }

      public static QualifiedNameInfo GetQualifiedName(String qname)
      {
         return new QualifiedNameInfo(qname);
      }

      public static Boolean ContainsName(
         QualifiedNameInfo qname, List<QualifiedNameInfo> items)
      {
         QualifiedNameInfo foundItem = items.Find((x) => x.Name == qname.Name);
         return foundItem != null;
      }

      private Boolean isBaseType()
      {
         return ElementBaseTypeInfo.IsBase(OriginalName);
      }

      /// <summary>
      /// Get Prefix ... and remaining URI.
      /// </summary>
      /// <param name="uri"></param>
      /// <returns></returns>
      public static List<String> GetPrefix(String uri)
      {
         List<string> l = new List<string>();

         if (uri == null)
            return l;
         var ndx = uri.IndexOf(':');
         if (ndx == -1)
         {
            l.Add(uri);
            return l;
         }
         l.Add(uri.Substring(0, ndx));
         l.Add(uri.Substring(ndx+1, uri.Length - ndx - 1));
         return l;
      }

   }

}
