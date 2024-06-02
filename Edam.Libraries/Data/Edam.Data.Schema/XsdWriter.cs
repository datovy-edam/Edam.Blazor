using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

using Edam.Data.Schema.SchemaObject;

namespace Edam.Xml.Xsd
{

   public class XsdWriter
   {

      public static String ToReferenceXsd(List<CatalogInfo> catalogs,
         string targetNamespace = null, string ns = null)
      {
         List<string> dic = new List<string>();

         StringBuilder rt = new StringBuilder();
         StringBuilder ct = new StringBuilder();
         StringBuilder sh = new StringBuilder();
         StringBuilder el = new StringBuilder();
         StringBuilder tb = new StringBuilder();
         StringBuilder cl = new StringBuilder();

         string ln;

         var tn = targetNamespace ?? "http://Edam.schema.xsd/v1r0";
         ns = ns ?? "ns1";

         rt.AppendLine(XsdHelper.GetSchemaStart(0, tn, ns));
         foreach (var i in catalogs)
         {
            ct.AppendLine(XsdHelper.GetComplexTypeStart(1, i.Name));
            foreach (var s in i.Schemas)
            {
               ct.AppendLine(XsdHelper.GetElementRef(2, s.Name, ns));
               sh.AppendLine(XsdHelper.GetComplexTypeStart(1, s.Name));
               foreach (var t in s.Items)
               {
                  sh.AppendLine(XsdHelper.GetElementRef(2, t.Name, ns));
                  tb.AppendLine(XsdHelper.GetComplexTypeStart(1, t.Name));
                  foreach (var c in t.Items)
                  {
                     tb.AppendLine(XsdHelper.GetElementRef(2, c.Name, ns));
                     ln = XsdHelper.GetElementStart(
                        1, dic, c.Name, c.DataType, ns);
                     if (ln != null)
                     {
                        el.AppendLine(ln);
                        el.AppendLine(XsdHelper.GetElementEnd(1));
                     }
                  }
                  ln = XsdHelper.GetElementStart(1, dic, t.Name, null, ns);
                  if (ln != null)
                  {
                     el.AppendLine(ln);
                     el.AppendLine(XsdHelper.GetElementEnd(1));
                  }
                  tb.AppendLine(XsdHelper.GetComplexTypeEnd(1));
               }
               sh.AppendLine(XsdHelper.GetComplexTypeEnd(1));
               ln = XsdHelper.GetElementStart(1, dic, s.Name, null, ns);
               if (ln != null)
               {
                  el.AppendLine(ln);
                  el.AppendLine(XsdHelper.GetElementEnd(1));
               }
            }
            ct.AppendLine(XsdHelper.GetComplexTypeEnd(1));
         }
         rt.AppendLine(ct.ToString());
         rt.AppendLine(sh.ToString());
         rt.AppendLine(tb.ToString());
         rt.AppendLine(el.ToString());
         rt.AppendLine(XsdHelper.GetSchemaEnd(0));
         return rt.ToString();
      }

      public static String ToElementXsd(List<CatalogInfo> catalogs,
         string targetNamespace = null, string ns = null)
      {
         StringBuilder ct = new StringBuilder();
         
         var tn = targetNamespace ?? "http://Edam.schema.xsd/v1r0";
         ns = ns ?? "ns1";

         ct.Append(XsdHelper.GetSchemaStart(0, tn, ns));
         ct.Append(XsdHelper.GetElementComplexTypeStart(
            1, "Repositories", false));
         foreach (var i in catalogs)
         {
            ct.Append(XsdHelper.GetElementComplexTypeStart(4, i.Name));
            foreach (var s in i.Schemas)
            {
               ct.Append(XsdHelper.GetElementComplexTypeStart(7, s.Name));
               foreach (var t in s.Items)
               {
                  ct.Append(XsdHelper.GetElementComplexTypeStart(10, t.Name));
                  foreach (var c in t.Items)
                  {
                     ct.AppendLine(XsdHelper.GetElementStart(
                        13, null, c.Name, c.DataType, ns, false, false, true));
                  }
                  ct.Append(XsdHelper.GetElementComplexTypeEnd(10));
               }
               ct.Append(XsdHelper.GetElementComplexTypeEnd(7));
            }
            ct.Append(XsdHelper.GetElementComplexTypeEnd(4));
         }
         ct.Append(XsdHelper.GetElementComplexTypeEnd(1));
         ct.AppendLine(XsdHelper.GetSchemaEnd(0));
         return ct.ToString();
      }

   }

}
