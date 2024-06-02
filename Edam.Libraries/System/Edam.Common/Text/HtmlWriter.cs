using System;
using System.Collections.Generic;
using System.Text;

namespace Edam.Text
{

   public enum HtmlTextWriterTag {
      H2,
      H3,
      P
   }

   public class HtmlWriter
   {
      public static readonly string TAG_H2 = "h2";
      public static readonly string TAG_H3 = "h3";
      public static readonly string TAG_P = "P";

      private readonly List<string> m_Tags = new List<string>();
      private readonly StringBuilder m_Builder = new StringBuilder();

      public void RenderBeginTag(string tag)
      {
         m_Tags.Add(tag);
         m_Builder.Append("<" + tag + ">");
      }
      public void RenderText(string text)
      {
         m_Builder.Append(text);
      }
      public void RenderEndTag(string tag)
      {
         m_Builder.Append("</" + tag + ">");
      }
      public void RenderEndTag()
      {
         if (m_Tags.Count == 0)
            return;
         m_Tags.RemoveAt(m_Tags.Count - 1);
      }
      public string GetTagText(HtmlTextWriterTag tag)
      {
         string text;
         switch (tag)
         {
            case HtmlTextWriterTag.H2:
               text = TAG_H2;
               break;
            case HtmlTextWriterTag.H3:
               text = TAG_H3;
               break;
            case HtmlTextWriterTag.P:
            default:
               text = TAG_P;
               break;
         }
         return text;
      }
      public void RenderBeginTag(HtmlTextWriterTag tag)
      {
         string text = GetTagText(tag);
         RenderBeginTag(text);
      }
      public void RenderEndTag(HtmlTextWriterTag tag)
      {
         string text = GetTagText(tag);
         RenderEndTag(text);
      }
      public void WriteEncodedText(string text)
      {
         RenderText(text);
      }
   }

}
