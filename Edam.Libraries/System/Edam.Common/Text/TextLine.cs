using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Text
{

   public enum TextTokenType
   {
      Uknown = 0,
      Spaces = 1,
      Word = 2,
      Punctuation = 3
   }

   public enum TextTokenOption
   {
      Unknown = 0,
      LowerCase = 1,
      UpperCase = 2
   }

   public class TextToken
   {
      public string Text { get; set; }
      public TextTokenType Type { get; set; }
   }

   public class TextLine
   {
      public const char WHITE_SPACE = ' ';

      public string OriginalLine { get; set; }
      public List<TextToken> Tokens { get; set; }
      public TextLine(string line)
      {
         Tokens = new List<TextToken>();
         OriginalLine = line;
      }

      public void Clear(char[] list)
      {
         for (var i = 0; i < list.Length; i++)
         {
            list[i] = (char)0;
         }
      }

      public void Tokenize()
      {
         Tokens.Clear();
         char[] chars = OriginalLine.ToCharArray();
         char[] token = new char[chars.Length];
         int c = 0;
         int t = 0;
         do
         {
            // eat all spaces...
            t = 0;
            Clear(token);
            while (c < chars.Length && chars[c] == WHITE_SPACE)
            {
               token[t] = chars[c];
               c++;
               if (c == chars.Length || chars[c] != WHITE_SPACE)
               {
                  TextToken item = new TextToken();
                  item.Type = TextTokenType.Spaces;
                  item.Text = new string(token.Take(t + 1).ToArray());
                  Tokens.Add(item);
                  break;
               }
               t++;
            }

            // scan for a word
            t = 0;
            Clear(token);
            while (c < chars.Length && chars[c] != WHITE_SPACE)
            {
               token[t] = chars[c];
               c++;
               if (c == chars.Length || chars[c] == WHITE_SPACE ||
                  Char.IsPunctuation(token[t]))
               {
                  TextToken item = new TextToken();
                  item.Type = (t == 0 && Char.IsPunctuation(token[t])) ?
                     TextTokenType.Punctuation : TextTokenType.Word;
                  item.Text = new string(token.Take(t + 1).ToArray());
                  Tokens.Add(item);
                  break;
               }
               t++;
            }
         }
         while (c < chars.Length);
      }

      public string MixCaseToUnderscoredWords(
         TextTokenOption option = TextTokenOption.Unknown)
      {
         System.Text.StringBuilder sb = new StringBuilder();
         string word;
         foreach (TextToken token in Tokens)
         {
            word = token.Text;
            if (token.Type == TextTokenType.Word)
            {
               string titleCase = Text.Convert.ToTitleCase(word);
               if (titleCase.Length > word.Length)
               {
                  word = titleCase.Replace(' ', '_');
                  switch(option)
                  {
                     case TextTokenOption.LowerCase:
                        word = word.ToLower();
                        break;
                     case TextTokenOption.UpperCase:
                        word = word.ToUpper();
                        break;
                     default:
                        break;
                  }
               }
            }
            sb.Append(word);
         }
         return sb.ToString();
      }

      public string ToLine()
      {
         System.Text.StringBuilder sb = new StringBuilder();
         foreach (TextToken token in Tokens)
         {
            sb.Append(token.Text);
         }
         return sb.ToString();
      }
   }

}
