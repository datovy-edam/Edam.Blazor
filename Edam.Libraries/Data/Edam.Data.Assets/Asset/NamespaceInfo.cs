using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Edam.Data.Asset
{

   [DataContract, Serializable]
   public class NamespacePath
   {
      public const String DEFAULT_VERSION_ID = "v1r0";

      public String Root { get; set; }
      public String Domain { get; set; }
      public String Schema { get; set; }

      private string m_VersionId;
      public String VersionId
      {
         get { return m_VersionId == null ? DEFAULT_VERSION_ID : m_VersionId; }
         set { m_VersionId = value; }
      }

      public String DomainUri { get; set; }

      public String FullName
      {
         get 
         { 
            return (Root + "_" + Domain).Replace("/","_") + 
               (String.IsNullOrWhiteSpace(Schema) ? 
                  String.Empty : "_" + Schema); 
         }
      }

      public void SetUri(Uri uri)
      {
         if (uri == null)
         {
            return;
         }

         var p = uri.AbsolutePath.Substring(1, uri.AbsolutePath.Length - 1);
         var i = p.Split('/');
         if (i != null && i.Length > 1)
         {
            Root = i[0];

            if (i.Length >= 2)
            {
               Domain = i[1];
            }
            else
            {
               Domain = String.Empty;
            }

            if (i.Length >= 3)
            {
               Schema = i[2];
            }
            else
            {
               Schema = Domain;
            }

            // setup version
            var vid = i[i.Length-1]; // [^1]
            VersionId = ParseVersion(vid);
            var vtext = "/" + VersionId;
            if (!String.IsNullOrEmpty(VersionId))
               Domain = Domain.Replace(vtext, String.Empty);
            DomainUri = uri.OriginalString.Replace(vtext, String.Empty);
         }
         else
         {
            Root = p;
            Domain = p;
            Schema = String.Empty;
            VersionId = DEFAULT_VERSION_ID;
            DomainUri = uri.OriginalString;
         }
      }

      private static String ParseVersion(String version)
      {
         if (String.IsNullOrWhiteSpace(version))
            return null;

         var v = version.ToLower();
         if (v[0] == 'v')
         {
            var i = v.Substring(1, v.Length - 1).Split('r');
            if (i.Length == 2)
               return "v" + i[0] + "r" + i[1];
         }

         return null;
      }

   }

   [DataContract, Serializable]
   public class NamespaceInfo
   {
      public const String HTTP_ROOT = "http://";
      public const String EDD_ORG_DM_ID = "edd.schema.org";
      public const String DEFAULT_UNKNOWN_URI = "http://unknown";
      public const String W3C_HOST = "www.w3.org";
      public const String W3C_PREFIX = "xs";
      public const String W3C_PREFIX_XSD = "xsd";
      public const String W3C_URI = "http://www.w3.org/2001/XMLSchema";
      public const String URN = "urn";

      private Uri m_Uri;

      public String OrganizationDomainId { get; set; }
      public String Prefix { get; set; }
      public Uri Uri
      {
         get { return m_Uri; }
         set
         {
            m_Uri = value;
            NamePath = new NamespacePath();
            NamePath.SetUri(value);
         }
      }
      public string UriText
      {
         get
         {
            if (m_Uri == null)
            {
               return String.Empty;
            }
            return m_Uri.OriginalString;
         }
      }
      public String Extension { get; set; }

      public String RootElementName { get; set; }

      public NamespacePath NamePath { get; set; }

      public Boolean IsW3CSchema 
      {
         get { return m_Uri.Host == W3C_HOST; }
      }

      public Boolean IsWellFormedUriString
      {
         get
         {
            return Uri.IsWellFormedUriString(
               m_Uri.AbsoluteUri, UriKind.Absolute);
         }
      }

      public void Initialize(
         String prefix, Uri uri, String organizationDomainId, String extension)
      {
         Prefix = prefix;
         Uri = uri;
         OrganizationDomainId = organizationDomainId ?? EDD_ORG_DM_ID;
         Extension = extension ?? String.Empty;
         RootElementName = String.Empty;
      }

      public NamespaceInfo(
         String prefix, Uri uri, String organizationDomainId = null,
         String extension = null)
      {
         Initialize(prefix, uri, organizationDomainId, extension);
      }
      public NamespaceInfo(
         String prefix, String uri, String organizationDomainId = null,
         String uriExtension = null)
      {
         Uri u = String.IsNullOrWhiteSpace(uri) ? null : new Uri(uri);
         Initialize(prefix, u, organizationDomainId, uriExtension);
      }
      public NamespaceInfo(
         String prefix, String uriText, NamespaceInfo defaultNamespace)
      {
         if (String.IsNullOrWhiteSpace(uriText) || 
            uriText.StartsWith("http:///"))
         {
            uriText = defaultNamespace.Uri.OriginalString;
         }
         Uri u = String.IsNullOrWhiteSpace(uriText) ? null : new Uri(uriText);
         Initialize(prefix, u, null, null);
      }
      public NamespaceInfo(NamespaceInfo ns)
      {
         Copy(ns);
      }

      public NamespaceInfo()
      {
         Uri = new Uri(DEFAULT_UNKNOWN_URI);
      }

      public void Copy(NamespaceInfo ns)
      {
         Initialize(ns.Prefix, ns.Uri, ns.OrganizationDomainId, ns.Extension);
      }

      public String ToReferenceUriText(String extension = null)
      {
         String ex = extension ?? Extension;
         ex = ex ?? String.Empty;
         return HTTP_ROOT + OrganizationDomainId + "/" +
            this.Uri.OriginalString.Replace(HTTP_ROOT, "").Replace('/', '_') +
            (String.IsNullOrWhiteSpace(ex) ? String.Empty : "." + ex);
      }

      public static bool IsUrn(string uriText)
      {
         return uriText == null ? false : uriText.StartsWith(URN);
      }
      public static bool IsW3CUri(string uriText)
      {
         return uriText == W3C_URI;
      }

      public static String UriToFileName(String uriText)
      {
         // TODO: remove hardcoded label...
         if (String.IsNullOrWhiteSpace(uriText))
            return "NO_URI_PROVIDED";
         return uriText.Replace(HTTP_ROOT, "").Replace('/', '_');
      }

      public String ToFileName()
      {
         return Uri.OriginalString.Replace(HTTP_ROOT, "").Replace('/', '_')
            .Replace("www.", "").Replace('.', '_');
      }

      public static NamespaceInfo GetW3CNamespace()
      {
         return new NamespaceInfo(W3C_PREFIX, W3C_URI);
      }

      public string MapItem(string item)
      {
         return (ElementBaseTypeInfo.IsBase(item)) ? "xs:" + item :
            Prefix + ":" + item;
      }

      public static void Merge(
         List<NamespaceInfo> destination, List<NamespaceInfo> source)
      {
         foreach(var i in source)
         {
            var itm = destination.Find((x) => x.Uri == i.Uri);
            if (itm != null)
               continue;
            destination.Add(i);
         }
      }

   }

}
